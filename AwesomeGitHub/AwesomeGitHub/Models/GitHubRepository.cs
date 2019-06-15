namespace AwesomeGitHub.Services
{
    using Newtonsoft.Json;

    public class GitHubRepository
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string RepositoryName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("full_name")]
        public string FullName { get; set; }

        [JsonProperty("forks_count")]
        public int ForksCount { get; set; }

        [JsonProperty("stargazers_count")]
        public int StarsCount { get; set; }

        [JsonProperty("owner")]
        public GitHubUser Owner { get; set; }
    }
}