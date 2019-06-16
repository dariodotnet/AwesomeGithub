namespace AwesomeGitHub.Services
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;

    public class ApiService : IApiService
    {
        private readonly IGitHubApi _api;

        public ApiService()
        {
            _api = Refit.RestService.For<IGitHubApi>(KeyValues.ApiBaseUrl);
        }

        public IObservable<IEnumerable<GitHubRepository>> GetRepositories(string language, int page) =>
            _api.Search(language, page).Select(x => x.Items);

        public IObservable<IEnumerable<GitHubPullRequest>> GetPullRequests(string userName, string repositoryName, int page) =>
            _api.RequestPullRequest(userName, repositoryName, page);

    }
}