namespace AwesomeGitHub.Models
{
    using SQLite;
    using System;

    public class LocalRepository
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ForksCount { get; set; }
        public int StarsCount { get; set; }
        public string FullName { get; set; }
        public string Owner { get; set; }
        public string Image { get; set; }
        public DateTimeOffset ExpireAt { get; set; }

        [Indexed]
        public string Language { get; set; }
    }
}