namespace AwesomeGitHub.Services
{
    using System;
    using System.Collections.Generic;

    public interface IApiService
    {
        IObservable<IEnumerable<GitHubRepository>> GetRepositories(string language, int page);
    }
}