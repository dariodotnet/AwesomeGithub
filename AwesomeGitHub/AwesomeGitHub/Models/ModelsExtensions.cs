namespace AwesomeGitHub.Models
{
    using System;

    public static class ModelsExtensions
    {
        public static LocalPullRequest ToLocal(this GitHubPullRequest pullRequest) =>
            new LocalPullRequest
            {
                Id = (int)pullRequest.Id,
                Title = pullRequest.Title,
                Description = pullRequest.Description,
                UserLogin = pullRequest.User.Login,
                Date = pullRequest.PullRequestDate,
                Image = pullRequest.User.Avatar,
                Url = pullRequest.Url,
                IsClosed = pullRequest.State.Equals("closed"),
                ExpireAt = DateTimeOffset.Now.AddHours(12)
            };

        public static LocalRepository ToLocal(this GitHubRepository repository, string language) =>
            new LocalRepository
            {
                Id = (int)repository.Id,
                Name = repository.RepositoryName,
                Description = repository.Description,
                ForksCount = repository.ForksCount,
                StarsCount = repository.StarsCount,
                FullName = repository.FullName,
                Owner = repository.Owner.Login,
                Image = repository.Owner.Avatar,
                Language = language,
                ExpireAt = DateTimeOffset.Now.AddHours(12)
            };
    }
}