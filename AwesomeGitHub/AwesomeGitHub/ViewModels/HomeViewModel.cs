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
    using System.Reactive;
    using System.Reactive.Linq;

    public class HomeViewModel : ViewModelBase
    {
        private readonly ICacheService _cacheService;

        private SourceList<GitHubRepository> _repositoriesData;

        private ReadOnlyObservableCollection<GitHubRepository> _repositories;
        public ReadOnlyObservableCollection<GitHubRepository> Repositories => _repositories;

        public bool Loading { [ObservableAsProperty] get; }

        public ReactiveCommand<Unit, IEnumerable<GitHubRepository>> LoadRepositoriesCommand { get; }

        public HomeViewModel()
        {
            _cacheService = Locator.Current.GetService<ICacheService>();
            _cacheService.ClearCache();

            _repositoriesData = new SourceList<GitHubRepository>();

            LoadRepositoriesCommand = ReactiveCommand.CreateFromObservable(_cacheService.GetRepositories);
            LoadRepositoriesCommand.IsExecuting.ToPropertyEx(this, x => x.Loading);
            LoadRepositoriesCommand.ThrownExceptions.Subscribe(x =>
            {
                //TODO handle exceptions
            });

            LoadRepositoriesCommand.Subscribe(_repositoriesData.AddRange);

            _repositoriesData.Connect()
                .Sort(SortExpressionComparer<GitHubRepository>.Ascending(x => x.StarsCount))
                .Bind(out _repositories)
                .DisposeMany().Subscribe();
        }
    }
}