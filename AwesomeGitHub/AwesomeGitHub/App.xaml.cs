namespace AwesomeGitHub
{
    using AwesomeGitHub.Views;
    using Services;
    using Splat;
    using System;
    using System.Reactive.Linq;
    using ReactiveUI;
    using Xamarin.Forms;

    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            RegisterServices();

            var cache = Locator.Current.GetService<ICacheService>();

            cache.Initialize()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x =>
                {
                    cache.ChangeLanguage("JavaScript");
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
                });
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
