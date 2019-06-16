namespace AwesomeGitHub.Services
{
    using System;
    using System.Collections.Generic;
    using System.Reactive;

    public interface ICacheService
    {
        event EventHandler LanguageChanged;

        IObservable<Unit> ClearCache();
        IObservable<IEnumerable<GitHubRepository>> GetRepositories();
        IObservable<IEnumerable<GitHubRepository>> LoadNext();
        IObservable<IEnumerable<GitHubPullRequest>> GetPullRequests(long id);

        void ChangeLanguage(string language);
    }
}