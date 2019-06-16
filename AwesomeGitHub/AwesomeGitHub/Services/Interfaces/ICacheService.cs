namespace AwesomeGitHub.Services
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Reactive;

    public interface ICacheService
    {
        event EventHandler LanguageChanged;

        IObservable<Unit> Initialize();
        IObservable<Unit> ClearCache();
        IObservable<IEnumerable<GitHubRepository>> GetRepositories();
        IObservable<IEnumerable<GitHubRepository>> LoadNextRepositories();
        IObservable<GitHubRepository> SetCurrentRepository(GitHubRepository repository);
        IObservable<GitHubRepository> GetCurrentRepository();
        IObservable<IEnumerable<GitHubPullRequest>> GetPullRequests();
        IObservable<IEnumerable<GitHubPullRequest>> LoadNextPullRequests();

        void ChangeLanguage(string language);
    }
}