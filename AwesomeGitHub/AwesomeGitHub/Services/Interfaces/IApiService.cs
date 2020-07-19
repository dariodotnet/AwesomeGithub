namespace AwesomeGitHub.Services
{
    using Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IApiService
    {
        Task<IEnumerable<GitHubRepository>> GetRepositories(string language, int page);
        Task<IEnumerable<GitHubPullRequest>> GetPullRequests(string userName, string repositoryName, int page);
    }
}