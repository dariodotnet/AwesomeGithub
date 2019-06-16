﻿using AwesomeGitHub.Views;
using Xamarin.Forms;

namespace AwesomeGitHub
{
    using Services;
    using Splat;

    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            RegisterServices();

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
            Locator.CurrentMutable.RegisterLazySingleton(() => new InternetService(), typeof(IInternetService));
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
