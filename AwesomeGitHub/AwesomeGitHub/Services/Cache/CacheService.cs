namespace AwesomeGitHub.Services
{
    using Models;
    using Splat;
    using SQLite;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Xamarin.Essentials;

    public class CacheService : ICacheService
    {
        public event EventHandler LanguageChanged;
        private readonly SQLiteConnection _db;
        private readonly IApiService _apiService;
        private LocalRepository _current;
        private int _repositoriesCount;
        private int _pullRequestCount;

        public string Language { get; set; }

        public CacheService()
        {
            _apiService = Locator.Current.GetService<IApiService>();
            Language = Preferences.Get(KeyValues.DefaultLanguage, "");

            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "awesomegithub.db3");

            _db = new SQLiteConnection(databasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex, false);
            _db.CreateTable<LocalRepository>();
            _db.CreateTable<LocalPullRequest>();
        }

        public void SetCurrentRepository(LocalRepository repository) => _current = repository;

        public LocalRepository GetCurrentRepository() => _current;

        public void ChangeLanguage(string language)
        {
            if (language.Equals(Language.ToLower()))
                return;

            Preferences.Set(KeyValues.DefaultLanguage, language.ToLower());
            Language = language;
            _repositoriesCount = 0;
            _pullRequestCount = 0;
            OnLanguageChanged();
        }

        public IEnumerable<LocalRepository> LoadCachedRepositories()
        {
            var cached = _db.Table<LocalRepository>().Where(x => x.Language.Equals(Language)).ToList();
            if (cached is null)
                return new List<LocalRepository>();

            _repositoriesCount = cached.Count;
            return cached.OrderByDescending(x => x.StarsCount);
        }

        public async Task<IEnumerable<LocalRepository>> LoadNextRepositories()
        {
            var page = _repositoriesCount > 0 ? _repositoriesCount / 50 : 0;
            var github = await _apiService.GetRepositories(Language, page);
            var local = github.Select(x => x.ToLocal(Language)).ToList();
            if (local.Count + _repositoriesCount > 999)
            {
                local = local.Take(999 - _repositoriesCount).ToList();
            }
            _repositoriesCount += local.Count;
            local.ForEach(x => _db.InsertOrReplace(x, typeof(LocalRepository)));
            return local.OrderByDescending(x => x.StarsCount);
        }

        public IEnumerable<LocalPullRequest> LoadCachedPullRequests()
        {
            var cache = _db.Table<LocalPullRequest>().Where(x => x.RepositoryId.Equals(_current.Id)).ToList();
            _pullRequestCount = cache.Count;
            return cache;
        }

        public async Task<IEnumerable<LocalPullRequest>> LoadNextPullRequests()
        {
            var page = CalculatePage(_pullRequestCount);
            var github = await _apiService.GetPullRequests(_current.Owner, _current.Name, page);
            var local = github.Select(x => x.ToLocal(_current.Id)).ToList();
            _pullRequestCount += local.Count;
            local.ForEach(x => _db.InsertOrReplace(x, typeof(LocalPullRequest)));
            return local;
        }

        public int GetRepositoriesCount() => _repositoriesCount;
        public int GetPullRequestCount() => _pullRequestCount;

        private int CalculatePage(int total)
        {
            return total > 0 ? total / 50 : 0;
        }

        protected virtual void OnLanguageChanged() => LanguageChanged?.Invoke(this, EventArgs.Empty);
    }
}