namespace AwesomeGitHub.Services
{
    using System.Reactive.Subjects;
    using Xamarin.Essentials;

    public class InternetService : IInternetService
    {
        private bool _internetConnection;

        public Subject<bool> InternetConnection { get; }
        public bool GetInternetConnection => _internetConnection;

        public InternetService()
        {
            InternetConnection = new Subject<bool>();
            _internetConnection = Connectivity.NetworkAccess == NetworkAccess.Internet;

            InternetConnection.OnNext(_internetConnection);

            Connectivity.ConnectivityChanged += ConnectivityOnConnectivityChanged;
        }

        private void ConnectivityOnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            _internetConnection = e.NetworkAccess == NetworkAccess.Internet;
            InternetConnection.OnNext(_internetConnection);
        }
    }
}