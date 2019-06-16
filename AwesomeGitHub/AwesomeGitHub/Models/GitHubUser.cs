namespace AwesomeGitHub.Models
{
    using Newtonsoft.Json;

    public class GitHubUser
    {
        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("avatar_url")]
        public string Avatar { get; set; }
    }
}