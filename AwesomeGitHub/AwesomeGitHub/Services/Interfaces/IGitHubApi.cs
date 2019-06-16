namespace AwesomeGitHub.Services
{
    using AwesomeGitHub.Models;
    using Refit;
    using System;
    using System.Collections.Generic;

    public interface IGitHubApi
    {
        [Headers("User-Agent: Awesome App")]
        [Get("/search/repositories?q=language:{language}&sort=stars&per_page=50&page={page}")]
        IObservable<GitHubResult> Search(string language, int page);

        [Headers("User-Agent: Awesome Octocat App")]
        [Get("/repos/{name}/{repo}/pulls?page={page}&state=all")]
        IObservable<IEnumerable<GitHubPullRequest>> RequestPullRequest(string name, string repo, int page);
    }
}