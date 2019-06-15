namespace AwesomeGitHub.Services
{
    using Akavache;
    using Splat;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;

    public class CacheService : ICacheService
    {
        private readonly IBlobCache _blob;
        private readonly IApiService _apiService;
        private int _currentPage = 1;

        public CacheService()
        {
            BlobCache.ApplicationName = KeyValues.AppName;
            BlobCache.EnsureInitialized();
            BlobCache.ForcedDateTimeKind = DateTimeKind.Utc;

            _blob = BlobCache.LocalMachine;
            _apiService = Locator.Current.GetService<IApiService>();

            _blob.GetAllKeys().Subscribe(keys =>
            {
                if (keys is null || !keys.Any())
                {
                    _blob.InsertObject(nameof(GitHubRepository), new List<GitHubRepository>());
                }
            });
        }

        public IObservable<IEnumerable<GitHubRepository>> GetRepositories() =>
            _blob.GetAndFetchLatest(nameof(GitHubRepository), () =>
                    _apiService.GetRepositories("JavaScript", _currentPage),
                offset => UpdatePast(offset, seconds: 1));

        public IObservable<IEnumerable<GitHubPullRequest>> GetPullRequests(long id) =>
            Observable.Return(new List<GitHubPullRequest>());

        public IObservable<Unit> ClearCache() => _blob.InvalidateAll();

        private bool UpdatePast(DateTimeOffset offset, int hours = 0, int minutes = 0, int seconds = 0)
        {
            var elapsed = DateTimeOffset.Now - offset;
            return elapsed > new TimeSpan(hours, minutes, seconds);
        }
    }
}