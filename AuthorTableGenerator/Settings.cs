using System.Text.Json.Serialization;

namespace AuthorTableGenerator;

[Serializable]
class Settings
{
    [JsonPropertyName("columns")]
    public int Columns { get; set; }

    [JsonPropertyName("pathToRepositoryRoot")]
    public string? PathToRepository { get; set; }

    [JsonPropertyName("repositoryOrganizationOrOwner")]
    public string? RepositoryOrganization { get; set; }

    [JsonPropertyName("repositoryName")]
    public string? RepositoryName { get; set; }

    [JsonPropertyName("fileName")]
    public string? FileName { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("footer")]
    public string? Footer { get; set; }

    [JsonPropertyName("showCommits")]
    public bool ListCommits { get; set; }
}