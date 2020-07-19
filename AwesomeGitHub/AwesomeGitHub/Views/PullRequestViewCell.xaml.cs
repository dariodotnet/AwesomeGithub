namespace AwesomeGitHub.Views
{
    using ReactiveUI;
    using System;
    using System.Reactive.Disposables;
    using Xamarin.Essentials;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PullRequestViewCell
    {
        private readonly TapGestureRecognizer _tap = new TapGestureRecognizer();

        public PullRequestViewCell()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.IsClosed, v => v.Closed.IsVisible).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Title, v => v.Title.Text,
                    e => $"{e[0].ToString().ToUpper()}{e.Substring(1)}").DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Description, v => v.Description.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.UserLogin, v => v.UserLogin.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Date, v => v.PullRequestDate.Text,
                    e => e.ToShortDateString()).DisposeWith(d);
            });

            _tap.Tapped += async (sender, args) =>
            {
                await Browser.OpenAsync(new Uri(ViewModel.Url), new BrowserLaunchOptions
                {
                    LaunchMode = BrowserLaunchMode.SystemPreferred,
                    TitleMode = BrowserTitleMode.Show,
                    PreferredToolbarColor = (Color)Application.Current.Resources["ApplicationStatus"]
                });
            };

            GestureRecognizers.Add(_tap);
        }
    }
}