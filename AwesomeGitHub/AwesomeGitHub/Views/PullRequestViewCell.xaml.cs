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
        private TapGestureRecognizer _tap = new TapGestureRecognizer();

        public PullRequestViewCell()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Title, v => v.Title.Text,
                    e => $"{e[0].ToString().ToUpper()}{e.Substring(1)}").DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Description, v => v.Description.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.User.Login, v => v.UserLogin.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.PullRequestDate, v => v.PullRequestDate.Text,
                    e => e.ToShortDateString()).DisposeWith(d);
            });

            View.GestureRecognizers.Add(_tap);

            _tap.Tapped += async (sender, args) =>
            {
                await Browser.OpenAsync(new Uri(ViewModel.Url), new BrowserLaunchOptions
                {
                    LaunchMode = BrowserLaunchMode.SystemPreferred,
                    TitleMode = BrowserTitleMode.Show,
                    PreferredToolbarColor = (Color)Application.Current.Resources["ApplicationStatus"]
                });
            };
        }
    }
}