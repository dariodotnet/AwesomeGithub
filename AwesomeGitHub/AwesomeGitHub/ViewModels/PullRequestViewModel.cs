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

    public class PullRequestViewModel : ViewModelBase
    {
        private readonly ICacheService _cacheService;

        private SourceList<GitHubPullRequest> _pullRequestData;

        private ReadOnlyObservableCollection<GitHubPullRequest> _pullRequests;
        public ReadOnlyObservableCollection<GitHubPullRequest> PullRequests => _pullRequests;

        [Reactive] public string OpenCount { get; set; }
        [Reactive] public string CloseCount { get; set; }
        [Reactive] public bool Complete { get; set; }

        public GitHubRepository Repository { [ObservableAsProperty]get; }
        public bool Loading { [ObservableAsProperty]get; }
        public bool Adding { [ObservableAsProperty]get; }

        public ReactiveCommand<Unit, GitHubRepository> LoadCommand { get; private set; }
        public ReactiveCommand<Unit, IEnumerable<GitHubPullRequest>> AddCommand { get; private set; }
        private ReactiveCommand<Unit, IEnumerable<GitHubPullRequest>> LoadCacheCommand { get; set; }

        public PullRequestViewModel()
        {
            _cacheService = Locator.Current.GetService<ICacheService>();

            _pullRequestData = new SourceList<GitHubPullRequest>();

            OpenCount = "0 opened";
            CloseCount = "0 closed";

            _pullRequestData.Connect()
                .Sort(SortExpressionComparer<GitHubPullRequest>.Descending(x => x.PullRequestDate))
                .Do(x => Count())
                .Bind(out _pullRequests)
                .DisposeMany().Subscribe();

            ConfigureLoadCommand();
            ConfigureLoadCacheCommand();
            ConfigureAddCommand();

            this.WhenAnyValue(x => x.Repository)
                .Where(x => x != null)
                .Select(x => Unit.Default)
                .InvokeCommand(LoadCacheCommand);
        }

        private void ConfigureLoadCommand()
        {
            LoadCommand = ReactiveCommand.CreateFromObservable(_cacheService.GetCurrentRepository);
            LoadCommand.ToPropertyEx(this, x => x.Repository);
        }

        private void ConfigureLoadCacheCommand()
        {
            LoadCacheCommand = ReactiveCommand.CreateFromObservable(_cacheService.GetPullRequests);
            LoadCacheCommand.IsExecuting.ToPropertyEx(this, x => x.Loading);
            LoadCacheCommand.ThrownExceptions.Subscribe(ex =>
            {
                //TODO Handle cache exceptions
            });
            LoadCacheCommand.Subscribe(x =>
            {
                if (x.Any())
                    _pullRequestData.AddRange(x);
                else
                    Observable.Return(Unit.Default).InvokeCommand(AddCommand);

            });
        }

        private void ConfigureAddCommand()
        {
            var canAdd = this.WhenAny(x => x.Adding, x => x.Complete, (a, c) => !a.Value && !c.Value);

            AddCommand = ReactiveCommand.CreateFromObservable(_cacheService.LoadNextPullRequests, canAdd);
            AddCommand.IsExecuting.ToPropertyEx(this, x => x.Adding);
            AddCommand.ThrownExceptions.SelectMany(ex => ExceptionInteraction.Handle(ex)).Subscribe();
            AddCommand.Subscribe(x =>
            {
                //Check if there are more pull request
                if (x.Any())
                    _pullRequestData.AddRange(x);

                //Check if response has result per page configured on IGitHubApi
                if (!x.Any() || x.Count() < 50)
                    Complete = true;

            });
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