namespace AwesomeGitHub.ViewModels
{
    using DynamicData;
    using DynamicData.Binding;
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

    public class HomeViewModel : ViewModelBase
    {
        private readonly ICacheService _cacheService;

        private SourceList<GitHubRepository> _repositoriesData;

        private ReadOnlyObservableCollection<GitHubRepository> _repositories;
        public ReadOnlyObservableCollection<GitHubRepository> Repositories => _repositories;

        public bool Loading { [ObservableAsProperty] get; }
        public bool Adding { [ObservableAsProperty] get; }

        [Reactive] public string Search { get; set; }

        public ReactiveCommand<Unit, IEnumerable<GitHubRepository>> LoadRepositoriesCommand { get; private set; }
        public ReactiveCommand<Unit, IEnumerable<GitHubRepository>> AddRepositoriesCommand { get; private set; }

        public HomeViewModel()
        {
            _cacheService = Locator.Current.GetService<ICacheService>();

            _repositoriesData = new SourceList<GitHubRepository>();

            var filter = this.WhenAnyValue(x => x.Search)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .Select(BuildFilter);

            _repositoriesData.Connect()
                .Filter(filter)
                .Sort(SortExpressionComparer<GitHubRepository>.Descending(x => x.StarsCount))
                .Bind(out _repositories)
                .DisposeMany().Subscribe();

            ConfigureLoadCommand();
            ConfigureAddCommand();

            Observable.FromEventPattern(
                    x => _cacheService.LanguageChanged += x,
                    x => _cacheService.LanguageChanged -= x)
                .Do(x => _repositoriesData.Clear())
                .Select(x => Unit.Default)
                .InvokeCommand(AddRepositoriesCommand);
        }

        private void ConfigureLoadCommand()
        {
            LoadRepositoriesCommand = ReactiveCommand.CreateFromObservable(_cacheService.GetRepositories);
            LoadRepositoriesCommand.IsExecuting.ToPropertyEx(this, x => x.Loading);
            LoadRepositoriesCommand.ThrownExceptions.Subscribe(x =>
            {
                //TODO handle exceptions
            });

            LoadRepositoriesCommand.Subscribe(x =>
            {
                if (x.Any())
                    _repositoriesData.AddRange(x);
                else
                    Observable.Return(Unit.Default).InvokeCommand(AddRepositoriesCommand);
            });
        }

        private void ConfigureAddCommand()
        {
            AddRepositoriesCommand = ReactiveCommand.CreateFromObservable(_cacheService.LoadNext,
                this.WhenAny(x => x.Adding, a => !a.Value));

            AddRepositoriesCommand.IsExecuting.ToPropertyEx(this, x => x.Adding);
            AddRepositoriesCommand.ThrownExceptions.Subscribe(x =>
            {
                //TODO handle exceptions
            });

            AddRepositoriesCommand.Subscribe(_repositoriesData.AddRange);
        }

        private static Func<GitHubRepository, bool> BuildFilter(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
                return repository => true;

            return repo => repo.RepositoryName.Contains(searchText) || repo.Owner.Login.Contains(searchText);
        }
    }
}