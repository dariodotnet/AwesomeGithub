namespace AwesomeGitHub.Models
{
    using SQLite;
    using System;

    public class LocalPullRequest
    {
        [PrimaryKey]
        public int Id { get; set; }

        [Indexed]
        public int RepositoryId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string UserLogin { get; set; }
        public DateTime Date { get; set; }
        public string Url { get; set; }
        public string Image { get; set; }
        public bool IsClosed { get; set; }
        public DateTimeOffset ExpireAt { get; set; }
    }
}