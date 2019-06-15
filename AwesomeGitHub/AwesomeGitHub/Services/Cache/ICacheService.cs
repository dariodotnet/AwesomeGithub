namespace AwesomeGitHub.Services
{
    using System;
    using System.Collections.Generic;
    using System.Reactive;

    public interface ICacheService
    {
        IObservable<Unit> ClearCache();
        IObservable<IEnumerable<GitHubRepository>> GetRepositories();
        IObservable<IEnumerable<GitHubPullRequest>> GetPullRequests(long id);
    }
}