using System.Text.Json.Serialization;

namespace AuthorTableGenerator;

[Serializable]
class GithubContributor
{
    [JsonPropertyName("login")]
    public string? Login { get; set; }

    [JsonPropertyName("avatar_url")]
    public string? AvatarURL { get; set; }

    [JsonPropertyName("html_url")]
    public string? HtmlUrl { get; set; }

    [JsonPropertyName("contributions")]
    public int Contributions { get; set; }
}