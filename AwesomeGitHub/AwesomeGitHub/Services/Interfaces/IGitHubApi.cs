namespace AwesomeGitHub.Services
{
    using AwesomeGitHub.Models;
    using Refit;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IGitHubApi
    {
        [Headers("User-Agent: Awesome App")]
        [Get("/search/repositories?q=language:{language}&sort=stars&per_page=50&page={page}")]
        Task<GitHubResult> Search(string language, int page);

        [Headers("User-Agent: Awesome Octocat App")]
        [Get("/repos/{name}/{repo}/pulls?per_page=50&page={page}&state=all")]
        Task<IEnumerable<GitHubPullRequest>> RequestPullRequest(string name, string repo, int page);
    }
}