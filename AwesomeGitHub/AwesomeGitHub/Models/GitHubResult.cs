namespace AwesomeGitHub.Models
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class GitHubResult
    {
        [JsonProperty("total_count")]
        public int Count { get; set; }

        [JsonProperty("items")]
        public List<GitHubRepository> Items { get; set; } = new List<GitHubRepository>();
    }
}