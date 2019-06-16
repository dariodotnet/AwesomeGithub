namespace AwesomeGitHub.Services
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using Xamarin.Essentials;

    public class ApiService : IApiService
    {
        private readonly IGitHubApi _api;
        private bool _hasInternet;

        public ApiService()
        {
            _api = Refit.RestService.For<IGitHubApi>(KeyValues.ApiBaseUrl);

            _hasInternet = Connectivity.NetworkAccess == NetworkAccess.Internet;
            Connectivity.ConnectivityChanged += ConnectivityOnConnectivityChanged;
        }

        private void ConnectivityOnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            _hasInternet = e.NetworkAccess == NetworkAccess.Internet;
        }

        public IObservable<IEnumerable<GitHubRepository>> GetRepositories(string language, int page)
        {
            if (!_hasInternet)
                throw new ConnectivityException();

            return _api.Search(language, page).Select(x => x.Items);
        }

        public IObservable<IEnumerable<GitHubPullRequest>> GetPullRequests(string userName, string repositoryName, int page)
        {
            if (!_hasInternet)
                throw new ConnectivityException();

            return _api.RequestPullRequest(userName, repositoryName, page);
        }

    }
}