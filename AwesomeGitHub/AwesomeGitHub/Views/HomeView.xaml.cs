namespace AwesomeGitHub.Views
{
    using ReactiveUI;
    using Services;
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
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

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
            });

            ViewModel = new HomeViewModel();
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