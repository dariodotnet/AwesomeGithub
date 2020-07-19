namespace AwesomeGitHub.Services
{
    using Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICacheService
    {
        string Language { get; }

        void SetCurrentRepository(LocalRepository repository);
        LocalRepository GetCurrentRepository();
        void ChangeLanguage(string language);
        IEnumerable<LocalRepository> LoadCachedRepositories();
        Task<IEnumerable<LocalRepository>> LoadNextRepositories();
        IEnumerable<LocalPullRequest> LoadCachedPullRequests();
        Task<IEnumerable<LocalPullRequest>> LoadNextPullRequests();
        int GetRepositoriesCount();
        int GetPullRequestCount();
    }
}