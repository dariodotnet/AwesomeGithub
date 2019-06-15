namespace AwesomeGitHub.Views
{
    using ReactiveUI;
    using System.Reactive;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using ViewModels;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomeView
    {
        public HomeView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.WhenAnyValue(v => v.ViewModel.LoadRepositoriesCommand)
                    .Where(x => x != null)
                    .Select(x => Unit.Default)
                    .InvokeCommand(ViewModel.LoadRepositoriesCommand);

                this.OneWayBind(ViewModel, vm => vm.Repositories, v => v.Repositories.ItemsSource).DisposeWith(d);
            });

            ViewModel = new HomeViewModel();
        }
    }
}