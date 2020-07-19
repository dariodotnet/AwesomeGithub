namespace AwesomeGitHub.Services
{
    using Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;
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

        public async Task<IEnumerable<GitHubRepository>> GetRepositories(string language, int page)
        {
            if (!_hasInternet)
                throw new ConnectivityException();

            var search = await _api.Search(language, page);
            return search.Items;
        }

        public Task<IEnumerable<GitHubPullRequest>> GetPullRequests(string userName, string repositoryName, int page)
        {
            if (!_hasInternet)
                throw new ConnectivityException();

            return _api.RequestPullRequest(userName, repositoryName, page);
        }

    }
}