namespace AwesomeGitHub.ViewModels
{
    using DynamicData;
    using DynamicData.Binding;
    using Models;
    using ReactiveUI;
    using ReactiveUI.Fody.Helpers;
    using Services;
    using Splat;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Threading.Tasks;

    public class HomeViewModel : ViewModelBase
    {
        private readonly ICacheService _cacheService;

        private SourceList<LocalRepository> _repositoriesData;

        private ReadOnlyObservableCollection<LocalRepository> _repositories;
        public ReadOnlyObservableCollection<LocalRepository> Repositories => _repositories;

        public bool Loading { [ObservableAsProperty] get; }
        public bool Adding { [ObservableAsProperty] get; }

        [Reactive] public string Search { get; set; }
        [Reactive] public LocalRepository Selected { get; set; }

        public ReactiveCommand<Unit, IEnumerable<LocalRepository>> LoadCache { get; private set; }
        public ReactiveCommand<Unit, IEnumerable<LocalRepository>> LoadNext { get; private set; }

        public HomeViewModel(ICacheService cacheService = null)
        {
            _cacheService = cacheService ?? Locator.Current.GetService<ICacheService>();

            _repositoriesData = new SourceList<LocalRepository>();

            var filter = this.WhenAnyValue(x => x.Search)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .Select(BuildFilter);

            _repositoriesData.Connect()
                .Filter(filter)
                .Sort(SortExpressionComparer<LocalRepository>.Descending(x => x.StarsCount))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _repositories)
                .DisposeMany().Subscribe();

            this.WhenAnyValue(x => x.Selected)
                .Where(x => x != null)
                .Do(x => _cacheService.SetCurrentRepository(x))
                .Subscribe();

            ConfigureAddCommand();
            ConfigureLoadCommand();
        }

        private void ConfigureLoadCommand()
        {
            LoadCache = ReactiveCommand.Create(_cacheService.LoadCachedRepositories);
            LoadCache.IsExecuting.ToPropertyEx(this, x => x.Loading);
            LoadCache.ThrownExceptions.Subscribe(x =>
            {
                //TODO handle exceptions
            });

            LoadCache.Where(x => x != null && x.Any())
                .Do(async cache => await AddRepositories(cache)).Subscribe();

            LoadCache.Where(x => x != null && !x.Any())
                .Select(x => Unit.Default)
                .InvokeCommand(LoadNext);
        }

        private void ConfigureAddCommand()
        {
            var canLoad = this.WhenAny(x => x.Adding, (a) => !a.Value);

            LoadNext = ReactiveCommand.CreateFromTask(_cacheService.LoadNextRepositories, canLoad);

            LoadNext.IsExecuting.ToPropertyEx(this, x => x.Adding);
            LoadNext.ThrownExceptions.SelectMany(ex => ExceptionInteraction.Handle(ex)).Subscribe();
            LoadNext.Where(x => x != null && x.Any())
                .Do(async cache => await AddRepositories(cache)).Subscribe();
        }

        private static Func<LocalRepository, bool> BuildFilter(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
                return repository => true;

            return repo => repo.Name.Contains(searchText) || repo.Owner.Contains(searchText);
        }

        private async Task AddRepositories(IEnumerable<LocalRepository> repositories)
        {
            foreach (var repository in repositories)
            {
                if (!_repositoriesData.Items.Any(x => x.Id.Equals(repository.Id)))
                {
                    _repositoriesData.Add(repository);
                    await Task.Delay(10);
                }
            }
        }
    }
}