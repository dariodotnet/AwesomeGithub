namespace AwesomeGitHub.Services
{
    using Akavache;
    using Models;
    using Splat;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;

    public class CacheService : ICacheService
    {
        public event EventHandler LanguageChanged;

        private readonly IBlobCache _blob;
        private readonly IApiService _apiService;
        private int _currentPage = 1;
        private string _language = KeyValues.DefaultLanguage;

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
           _blob.GetObject<IEnumerable<GitHubRepository>>(nameof(GitHubRepository));

        public IObservable<IEnumerable<GitHubRepository>> LoadNext()
        {
            return _blob.GetObject<IEnumerable<GitHubRepository>>(nameof(GitHubRepository))
                .Select(x =>
                {
                    _currentPage = x.Any() ? x.Count() / 50 : 0;
                    _currentPage++;
                    return _apiService.GetRepositories(_language, _currentPage)
                        .Select(response =>
                        {
                            var copy = new List<GitHubRepository>(x);
                            if (copy.Count + response.Count() < KeyValues.MaxRepositories)
                                copy.AddRange(response);
                            else
                            {
                                var rest = KeyValues.MaxRepositories - copy.Count;
                                var complete = new List<GitHubRepository>(response.Take(rest));
                                copy.AddRange(complete);
                                response = new List<GitHubRepository>(complete);
                            }

                            _blob.InsertObject(nameof(GitHubRepository), copy, DateTimeOffset.UtcNow.AddDays(1));
                            return response;
                        }).Wait();
                });
        }

        public IObservable<IEnumerable<GitHubPullRequest>> GetPullRequests(long id) =>
            Observable.Return(new List<GitHubPullRequest>());

        public IObservable<Unit> ClearCache() =>
            _blob.InvalidateAll().Select(x =>
                {
                    _blob.InsertObject(nameof(GitHubRepository), new List<GitHubRepository>());
                    return x;
                });

        public IObservable<Unit> Initialize() =>
            _blob.GetAllKeys()
                .Select(keys =>
                {
                    if (keys is null || !keys.Any())
                    {
                        return _blob.InsertObject(nameof(GitHubRepository), new List<GitHubRepository>());
                    }

                    return Observable.Return(Unit.Default);
                }).Wait();

        public void ChangeLanguage(string language)
        {
            if (language.Equals(_language))
                return;

            ClearCache().Subscribe(x =>
            {
                _language = language;
                _currentPage = 1;
                OnLanguageChanged();
            });
        }

        protected virtual void OnLanguageChanged() => LanguageChanged?.Invoke(this, EventArgs.Empty);
    }
}