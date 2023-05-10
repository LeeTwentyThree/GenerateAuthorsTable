using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace AuthorTableGenerator;

public static class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Please provide a working directory with a valid settings.json and get-contributors.bat file: ");
        string? directory = Console.ReadLine();
        if (directory is null || !Directory.Exists(directory))
        {
            Error("Invalid directory, ya dingus!");
            return;
        }
        string settingsFile = Path.Combine(directory, "settings.json");

        if (!File.Exists(settingsFile))
        {
            Error("No settings.json file found in working directory! See the project README for more info.");
            return;
        }

        AuthorTableSettings? settings = JsonSerializer.Deserialize<AuthorTableSettings>(File.ReadAllText(settingsFile));

        if (settings is null)
        {
            Error("Failed to load setting.json file!");
            return;
        }
        else if (settings.Columns == 0)
        {
            Error("Attempting to create 0 columns; divide by zero error! Honestly, you should've known better, I'm disappointed.");
            return;
        }

        Console.WriteLine("\nThe following names will be excluded:");
        if (settings.ExcludedNames != null && settings.ExcludedNames.Count > 0)
        {
            foreach (string name in settings.ExcludedNames)
            {
                Console.WriteLine(name);
            }
        }
        else
        {
            Console.WriteLine("[NONE]");
        }

        if (settings.LocalRepositoryDirectory is null || !Directory.Exists(settings.LocalRepositoryDirectory))
        {
            Error("Failed to load local repository directory!");
            return;
        }

        string gitResults = GetCommitInfoFromGit(settings.LocalRepositoryDirectory);

        string[] gitResultsSplit = gitResults.Split('\n');

        List<string> contributors = new List<string>();

        for (int i = 2; i < gitResultsSplit.Length; i++)
        {
            int startIndex = 7;
            if (startIndex > gitResultsSplit[i].Length) continue;
            string contributorName = gitResultsSplit[i].Substring(startIndex);
            if (settings.ExcludedNames.Contains(contributorName)) continue;
            contributors.Add(contributorName);
        }

        Console.WriteLine("\nThe following contributors were found:");

        foreach (var contributor in contributors)
        {
            Console.WriteLine(contributor);
        }

        int rows = contributors.Count / settings.Columns;
        bool remainderRow = contributors.Count % settings.Columns > 0;
        if (remainderRow) rows++;

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < settings.Columns; i++)
        {
            sb.Append("| Name | Picture ");
        }
        sb.AppendLine("|");

        for (int i = 0; i < settings.Columns; i++)
        {
            sb.Append("| ---- | --------------- ");
        }
        sb.AppendLine("|");

        int j = 0;
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < settings.Columns; c++)
            {
                sb.Append($"| { contributors[j] } | <img src=\"https://github.com/{ contributors[j] }.png\" width=\"50\"> ");
                j++;
                if (j >= contributors.Count) break;
            }
            sb.AppendLine("|");
        }

        File.WriteAllText(Path.Combine(Assembly.GetExecutingAssembly().Location, "../", "authors-table.md"), sb.ToString());

        Console.WriteLine("\nSaved results to output authors-table.md output file.");

        Console.ReadLine();
    }

    private static string GetCommitInfoFromGit(string repoDir)
    {
        Process p = new Process();
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.WorkingDirectory = repoDir;
        p.StartInfo.FileName = Path.Combine(Assembly.GetExecutingAssembly().Location, "../", "get-contributors.bat");
        p.Start();
        string output = p.StandardOutput.ReadToEnd();
        p.WaitForExit();
        return output;
    }

    private static void Error(string message)
    {
        Console.WriteLine("ERROR!!! Fucking shouldn't have been stupid! : " + message);
    }
}
