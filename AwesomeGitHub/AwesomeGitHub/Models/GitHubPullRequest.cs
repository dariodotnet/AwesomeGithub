namespace AwesomeGitHub.Models
{
    using Newtonsoft.Json;
    using System;

    public class GitHubPullRequest
    {
        [JsonProperty("id")]
        public long Id { get; set; }


        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("body")]
        public string Description { get; set; }

        [JsonProperty("user")]
        public GitHubUser User { get; set; }

        [JsonProperty("created_at")]
        public DateTime PullRequestDate { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("html_url")]
        public string Url { get; set; }
    }
}