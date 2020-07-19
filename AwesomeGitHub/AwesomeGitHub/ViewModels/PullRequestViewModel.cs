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

    public class PullRequestViewModel : ViewModelBase
    {
        private readonly ICacheService _cacheService;

        private SourceList<LocalPullRequest> _pullRequestData;

        private ReadOnlyObservableCollection<LocalPullRequest> _pullRequests;
        public ReadOnlyObservableCollection<LocalPullRequest> PullRequests => _pullRequests;

        [Reactive] public string OpenCount { get; set; }
        [Reactive] public string CloseCount { get; set; }
        [Reactive] public bool Complete { get; set; }

        public LocalRepository Repository { [ObservableAsProperty]get; }
        public bool Loading { [ObservableAsProperty]get; }
        public bool Adding { [ObservableAsProperty]get; }

        public ReactiveCommand<Unit, LocalRepository> LoadCurrentRepository { get; private set; }
        public ReactiveCommand<Unit, IEnumerable<LocalPullRequest>> AddCommand { get; private set; }
        private ReactiveCommand<Unit, IEnumerable<LocalPullRequest>> LoadCacheCommand { get; set; }

        public PullRequestViewModel()
        {
            _cacheService = Locator.Current.GetService<ICacheService>();

            _pullRequestData = new SourceList<LocalPullRequest>();

            OpenCount = "0 opened";
            CloseCount = "0 closed";

            _pullRequestData.Connect()
                .Sort(SortExpressionComparer<LocalPullRequest>.Descending(x => x.Date))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(x => Count())
                .Bind(out _pullRequests)
                .DisposeMany().Subscribe();

            ConfigureAddCommand();
            ConfigureLoadCommand();
            ConfigureLoadCacheCommand();

            this.WhenAnyValue(x => x.Repository)
                .Where(x => x != null)
                .Select(x => Unit.Default)
                .InvokeCommand(LoadCacheCommand);
        }

        private void ConfigureLoadCommand()
        {
            LoadCurrentRepository = ReactiveCommand.Create(_cacheService.GetCurrentRepository);
            LoadCurrentRepository.ToPropertyEx(this, x => x.Repository);
        }

        private void ConfigureLoadCacheCommand()
        {
            LoadCacheCommand = ReactiveCommand.Create(_cacheService.LoadCachedPullRequests);
            LoadCacheCommand.IsExecuting.ToPropertyEx(this, x => x.Loading);
            LoadCacheCommand.ThrownExceptions.Subscribe(ex =>
            {
                //TODO Handle cache exceptions
            });

            LoadCacheCommand.Where(x => x != null && x.Any())
                .Do(async cache => await AddPullRequest(cache))
                .Subscribe();
            LoadCacheCommand.Where(x => x == null || !x.Any())
                .Select(x => Unit.Default)
                .InvokeCommand(AddCommand);
        }

        private void ConfigureAddCommand()
        {
            var canAdd = this.WhenAny(x => x.Adding, x => x.Complete, (a, c) => !a.Value && !c.Value);

            AddCommand = ReactiveCommand.CreateFromTask(_cacheService.LoadNextPullRequests, canAdd);
            AddCommand.IsExecuting.ToPropertyEx(this, x => x.Adding);
            AddCommand.ThrownExceptions.SelectMany(ex => ExceptionInteraction.Handle(ex)).Subscribe();
            AddCommand.Where(x => x != null && x.Any())
                .Do(async api => await AddPullRequest(api))
                .Subscribe();
        }

        private void Count()
        {
            OpenCount = $"{_pullRequestData.Items.Count(x => !x.IsClosed)} opened";
            CloseCount = $"{_pullRequestData.Items.Count(x => x.IsClosed)} closed";
        }

        private async Task AddPullRequest(IEnumerable<LocalPullRequest> pullRequests)
        {
            foreach (var pullRequest in pullRequests)
            {
                _pullRequestData.Add(pullRequest);
                await Task.Delay(10);
            }
        }
    }
}