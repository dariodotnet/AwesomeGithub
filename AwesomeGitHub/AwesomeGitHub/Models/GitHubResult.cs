namespace AwesomeGitHub.Services
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class GitHubResult
    {
        [JsonProperty("total_count")]
        public int Count { get; set; }

        [JsonProperty("items")]
        public List<GitHubRepository> Items { get; set; } = new List<GitHubRepository>();
    }
}