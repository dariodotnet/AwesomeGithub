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
    public partial class HomeView
    {
        public HomeView()
        {
            InitializeComponent();

            ViewModel = new HomeViewModel();
        }

        protected override void OnAppearing()
        {
            this.WhenActivated(d =>
            {
                this.WhenAnyValue(v => v.ViewModel.LoadRepositoriesCommand)
                    .Where(x => x != null)
                    .Select(x => Unit.Default)
                    .InvokeCommand(ViewModel.LoadRepositoriesCommand);

                this.Bind(ViewModel, vm => vm.Search, v => v.SearchBox.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Repositories, v => v.Repositories.ItemsSource).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Loading, v => v.GridLoader.IsVisible).DisposeWith(d);

                this.WhenAnyValue(v => v.ViewModel.Adding)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(visible =>
                    {
                        GridAdder.TranslateTo(0, visible ? 0 : 60, 500);
                    });

                this.WhenAnyValue(v => v.Repositories.SelectedItem)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Where(x => x != null)
                    .Subscribe(selected =>
                    {
                        ViewModel.SetCurrent((GitHubRepository)selected)
                            .Subscribe(x => Navigation.PushAsync(new PullRequestView()));
                    })
                    .DisposeWith(d);

                Observable.FromEventPattern<EventHandler<ItemVisibilityEventArgs>, ItemVisibilityEventArgs>(
                        x => Repositories.ItemAppearing += x,
                        x => Repositories.ItemAppearing -= x)
                    .Where(x => x != null)
                    .Select(x => x.EventArgs.ItemIndex)
                    .Select(NeedLoad)
                    .Where(x => x)
                    .Select(x => Unit.Default)
                    .InvokeCommand(ViewModel.AddRepositoriesCommand).DisposeWith(d);
            });
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            Repositories.SelectedItem = null;
            base.OnDisappearing();
        }

        private bool NeedLoad(int index)
        {
            var items = Repositories.ItemsSource.Cast<GitHubRepository>().Count();
            if (items < 10 || items >= KeyValues.MaxRepositories)
                return false;

            return index >= items - 5;
        }
    }
}