namespace AwesomeGitHub.Views
{
    using ReactiveUI;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using ViewModels;
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
                this.WhenAnyValue(v => v.ViewModel.LoadCommand)
                    .Where(x => x != null)
                    .Select(x => Unit.Default)
                    .InvokeCommand(ViewModel.LoadCommand);

                this.OneWayBind(ViewModel, vm => vm.OpenCount, v => v.Open.Text).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.CloseCount, v => v.Closed.Text).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.PullRequests, v => v.PullRequests.ItemsSource).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Loading, v => v.GridLoader.IsVisible).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.Repository.RepositoryName, v => v.Title,
                    e => $"{e[0].ToString().ToUpper()}{e.Substring(1)}").DisposeWith(d);

                ViewModel.ExceptionInteraction.RegisterHandler(async interaction =>
                {
                    await DisplayAlert("Error", interaction.Input.Message, "Ok");
                    interaction.SetOutput(Unit.Default);
                });
            });

            base.OnAppearing();
        }
    }
}