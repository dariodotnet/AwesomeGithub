namespace AwesomeGitHub.ViewModels
{
    using DynamicData;
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

    public class PullRequestViewModel : ViewModelBase
    {
        private readonly ICacheService _cacheService;

        private SourceList<GitHubPullRequest> _pullRequestData;

        private ReadOnlyObservableCollection<GitHubPullRequest> _pullRequests;
        public ReadOnlyObservableCollection<GitHubPullRequest> PullRequests => _pullRequests;

        [Reactive] public string OpenCount { get; set; }
        [Reactive] public string CloseCount { get; set; }

        public GitHubRepository Repository { [ObservableAsProperty]get; }
        public bool Loading { [ObservableAsProperty]get; }
        public bool Adding { [ObservableAsProperty]get; }

        public ReactiveCommand<Unit, GitHubRepository> LoadCommand { get; }
        private ReactiveCommand<Unit, IEnumerable<GitHubPullRequest>> LoadCacheCommand { get; }

        public PullRequestViewModel()
        {
            _cacheService = Locator.Current.GetService<ICacheService>();

            _pullRequestData = new SourceList<GitHubPullRequest>();

            OpenCount = "0 opened";
            CloseCount = "0 closed";

            _pullRequestData.Connect()
                .Do(x => Count())
                .Bind(out _pullRequests)
                .DisposeMany().Subscribe();

            LoadCommand = ReactiveCommand.CreateFromObservable(_cacheService.GetCurrentRepository);
            LoadCommand.ToPropertyEx(this, x => x.Repository);

            LoadCacheCommand = ReactiveCommand.CreateFromObservable(_cacheService.GetPullRequests);
            LoadCacheCommand.IsExecuting.ToPropertyEx(this, x => x.Loading);
            LoadCacheCommand.ThrownExceptions.SelectMany(ex => ExceptionInteraction.Handle(ex)).Subscribe();
            LoadCacheCommand.Subscribe(_pullRequestData.AddRange);

            this.WhenAnyValue(x => x.Repository)
                .Where(x => x != null)
                .Select(x => Unit.Default)
                .InvokeCommand(LoadCacheCommand);
        }

        private void Count()
        {
            OpenCount = $"{_pullRequestData.Items.Count(x => x.State.Equals("open"))} opened";
            CloseCount = $"{_pullRequestData.Items.Count(x => x.State.Equals("closed"))} closed";
        }

        private IObservable<IEnumerable<GitHubPullRequest>> LoadPullRequests() =>
            Observable.Return(new List<GitHubPullRequest>());
    }
}