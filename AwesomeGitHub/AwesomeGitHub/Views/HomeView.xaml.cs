namespace AwesomeGitHub.Views
{
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
                this.WhenAnyValue(v => v.ViewModel.LoadCache)
                    .Where(x => x != null)
                    .Select(x => Unit.Default)
                    .InvokeCommand(ViewModel.LoadCache);

                this.Bind(ViewModel, vm => vm.Search, v => v.SearchBox.Text).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.Repositories, v => v.Repositories.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Selected, v => v.Repositories.SelectedItem);
                this.OneWayBind(ViewModel, vm => vm.ItemTreshold, v => v.Repositories.RemainingItemsThreshold).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.LoadNext, v => v.Repositories, nameof(Repositories.RemainingItemsThresholdReached)).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.Loading, v => v.GridLoader.IsVisible).DisposeWith(d);

                this.WhenAnyValue(v => v.ViewModel.Adding)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(visible =>
                    {
                        GridAdder.TranslateTo(0, visible ? 0 : 60, 500);
                    });

                this.WhenAnyValue(v => v.ViewModel.Selected)
                    .Where(x => x != null)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Do(async selected => await Navigation.PushAsync(new PullRequestView()))
                    .Do(x => ViewModel.Selected = null)
                    .Subscribe().DisposeWith(d);

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