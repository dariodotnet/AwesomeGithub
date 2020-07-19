namespace AwesomeGitHub
{
    using AwesomeGitHub.Views;
    using Services;
    using Splat;
    using Xamarin.Essentials;
    using Xamarin.Forms;

    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            RegisterServices();

            if (string.IsNullOrEmpty(Preferences.Get(KeyValues.DefaultLanguage, "")))
                Preferences.Set(KeyValues.DefaultLanguage, "javascript");

            var detail = new NavigationPage(new HomeView())
            {
                BarBackgroundColor = (Color)App.Current.Resources["ApplicationColor"]
            };

            MainPage = new MasterDetailPage
            {
                Master = new MasterView(),
                Detail = detail,
                IsGestureEnabled = true,
                IconImageSource = ImageSource.FromResource("todo")
            };
        }

        private void RegisterServices()
        {
            Locator.CurrentMutable.RegisterLazySingleton(() => new CacheService(), typeof(ICacheService));
            Locator.CurrentMutable.RegisterLazySingleton(() => new ApiService(), typeof(IApiService));
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
