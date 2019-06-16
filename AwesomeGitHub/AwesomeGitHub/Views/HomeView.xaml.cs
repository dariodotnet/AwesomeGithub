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

                Observable.FromEventPattern<EventHandler<ItemVisibilityEventArgs>, ItemVisibilityEventArgs>(
                        x => Repositories.ItemAppearing += x,
                        x => Repositories.ItemAppearing -= x)
                    .Where(x => x != null)
                    .Select(x => x.EventArgs.ItemIndex)
                    .Select(NeedLoad)
                    .Where(x => x)
                    .Select(x => Unit.Default)
                    .InvokeCommand(ViewModel.AddRepositoriesCommand).DisposeWith(d);

                Observable.FromEventPattern<EventHandler<SelectedItemChangedEventArgs>, SelectedItemChangedEventArgs>(
                        h => Repositories.ItemSelected += h,
                        h => Repositories.ItemSelected -= h)
                    .Where(x => x != null && x.EventArgs.SelectedItem != null)
                    .Subscribe(selection =>
                    {
                        var repository = (GitHubRepository)selection.EventArgs.SelectedItem;
                        Navigation.PushAsync(new PullRequestView(repository));

                        (selection.Sender as ListView).SelectedItem = null;
                    });
            });
            base.OnAppearing();
        }

        private bool NeedLoad(int index)
        {
            var items = Repositories.ItemsSource.Cast<GitHubRepository>().Count();
            Console.WriteLine($"TOTAL REPOSITORIES: {items}");
            if (items < 10 || items >= KeyValues.MaxRepositories)
                return false;

            Console.WriteLine($"COMPROBANDO INDICE: {index} : {items}");
            return index >= items - 5;
        }
    }
}