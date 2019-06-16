namespace AwesomeGitHub.Services
{
    using Models;
    using System;
    using System.Collections.Generic;

    public interface IApiService
    {
        IObservable<IEnumerable<GitHubRepository>> GetRepositories(string language, int page);
        IObservable<IEnumerable<GitHubPullRequest>> GetPullRequests(string userName, string repositoryName, int page);
    }
}