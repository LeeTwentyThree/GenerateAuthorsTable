using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AuthorTableGenerator;

public static class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("This program expects a valid table-generation-settings.json file in the same folder. Press enter to continue.");

        Console.ReadLine();

        string myFolder = Path.Combine(Assembly.GetExecutingAssembly().Location, "..");

        string settingsFile = Path.Combine(myFolder, "table-generation-settings.json");

        if (!File.Exists(settingsFile))
        {
            Error("No settings.json file found in working directory! See the project README for more info.");
            return;
        }

        Settings? settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText(settingsFile));

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

        string repositoryFolder = Path.Combine(myFolder, settings.PathToRepository);

        if (repositoryFolder is null || !Directory.Exists(repositoryFolder))
        {
            Error("Failed to load local repository directory! Just like when your mom failed to... uh... idk?");
            return;
        }

        var contributors = GetContributorInfoFromGithub(settings, repositoryFolder); 

        Console.WriteLine("\nThe following contributors were found:");

        foreach (var contributor in contributors)
        {
            Console.WriteLine(contributor.Login);
        }

        int rows = contributors.Length / settings.Columns;
        bool remainderRow = contributors.Length % settings.Columns > 0;
        if (remainderRow) rows++;
        int columns = settings.Columns;
        if (rows == 1)
        {
            columns = contributors.Length;
        }

        StringBuilder sb = new StringBuilder();

        if (settings.Title != null)
        {
            sb.AppendLine($"# {settings.Title}");
        }

        if (settings.Description != null)
        {
            sb.AppendLine($"{settings.Description}\n");
        }

        for (int i = 0; i < columns; i++)
        {
            sb.Append("| Name | Picture ");
            if (settings.ListCommits)
            {
                sb.Append("| Commits ");
            }
        }
        sb.AppendLine("|");

        for (int i = 0; i < columns; i++)
        {
            sb.Append("| ---- | --------------- ");
            if (settings.ListCommits)
            {
                sb.Append("| ------ ");
            }
        }
        sb.AppendLine("|");

        int j = 0;
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                sb.Append($"| [{contributors[j].Login}]({contributors[j].HtmlUrl}) | <img src=\"{contributors[j].AvatarURL}\" width=\"50\"> ");
                if (settings.ListCommits)
                {
                    sb.Append($"| {contributors[j].Contributions} ");
                }
                j++;
                if (j >= contributors.Length)
                {
                    break;
                }
            }
            sb.AppendLine("|");
        }

        if (settings.Footer != null)
        {
            sb.AppendLine($"\n{settings.Footer}");
        }

        File.WriteAllText(Path.Combine(repositoryFolder, settings.FileName), sb.ToString());

        Console.WriteLine($"\nSaved results to output {settings.FileName} output file in the repository root.");

        Console.ReadLine();
    }

    private static GithubContributor[] GetContributorInfoFromGithub(Settings settings, string repoDir)
    {
        string urlAddress = $"https://api.github.com/repos/{settings.RepositoryOrganization}/{settings.RepositoryName}/contributors";
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
        request.Headers.Add("User-Agent", "request");
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        string output = null;
        if (response.StatusCode == HttpStatusCode.OK)
        {
            Stream receiveStream = response.GetResponseStream();
            StreamReader readStream = null;
            if (response.CharacterSet == null)
                readStream = new StreamReader(receiveStream);
            else
                readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
            output = readStream.ReadToEnd();
            response.Close();
            readStream.Close();
        }

        if (output == null)
        {
            Error("Invalid API request data!");
            return new GithubContributor[0];
        }
        GithubContributor[]? contributors = JsonSerializer.Deserialize<GithubContributor[]>(output);
        return contributors;
    }

    private static void Error(string message)
    {
        Console.WriteLine("ERROR!!! Just shouldn't have been stupid, maybe you wouldn't have run into this error : " + message);
        Console.ReadLine();
    }
}
