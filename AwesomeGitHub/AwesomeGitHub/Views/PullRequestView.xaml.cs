namespace AwesomeGitHub.Views
{
    using Models;
    using ReactiveUI;
    using System;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using ViewModels;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PullRequestView
    {
        public PullRequestView()
        {
            InitializeComponent();

            ViewModel = new PullRequestViewModel();
        }

        protected override void OnAppearing()
        {
            this.WhenActivated(d =>
            {
                this.WhenAnyValue(v => v.ViewModel.LoadCurrentRepository)
                    .Where(x => x != null)
                    .Select(x => Unit.Default)
                    .InvokeCommand(ViewModel.LoadCurrentRepository);

                this.OneWayBind(ViewModel, vm => vm.OpenCount, v => v.Open.Text).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.CloseCount, v => v.Closed.Text).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.PullRequests, v => v.PullRequests.ItemsSource).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Loading, v => v.GridLoader.IsVisible).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.Repository.Name, v => v.Title,
                    e => $"{e[0].ToString().ToUpper()}{e.Substring(1)}").DisposeWith(d);

                this.WhenAnyValue(v => v.ViewModel.Adding)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(visible =>
                    {
                        GridAdder.TranslateTo(0, visible ? 0 : 60, 500);
                    });

                Observable.FromEventPattern<EventHandler<ItemVisibilityEventArgs>, ItemVisibilityEventArgs>(
                        x => PullRequests.ItemAppearing += x,
                        x => PullRequests.ItemAppearing -= x)
                    .Where(x => x != null)
                    .Select(x => x.EventArgs.ItemIndex)
                    .Select(NeedLoad)
                    .Where(x => x)
                    .Select(x => Unit.Default)
                    .InvokeCommand(ViewModel.AddCommand).DisposeWith(d);

                ViewModel.ExceptionInteraction.RegisterHandler(async interaction =>
                {
                    await DisplayAlert("Error", interaction.Input.Message, "Ok");
                    interaction.SetOutput(Unit.Default);
                });
            });

            base.OnAppearing();
        }

        private bool NeedLoad(int index)
        {
            var items = PullRequests.ItemsSource.Cast<GitHubPullRequest>().Count();
            if (items < 10 || items >= KeyValues.MaxRepositories)
                return false;

            return index >= items - 5;
        }
    }
}