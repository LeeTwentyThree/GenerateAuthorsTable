using System.Text.Json.Serialization;

namespace AuthorTableGenerator;

[Serializable]
class AuthorTableSettings
{
    [JsonPropertyName("columns")]
    public int Columns { get; set; }

    [JsonPropertyName("excludedNames")]
    public List<string> ExcludedNames { get; set; }

    [JsonPropertyName("localRepositoryDirectory")]
    public string? LocalRepositoryDirectory { get; set; }
}