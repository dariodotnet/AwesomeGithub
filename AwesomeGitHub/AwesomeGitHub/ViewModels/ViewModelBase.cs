namespace AwesomeGitHub.ViewModels
{
    using ReactiveUI;
    using ReactiveUI.Fody.Helpers;
    using Services;
    using Splat;
    using System.Reactive.Linq;

    public class ViewModelBase : ReactiveObject
    {
        protected IInternetService InternetService;

        public bool CanLoad { [ObservableAsProperty]get; }

        protected ViewModelBase()
        {
            InternetService = Locator.Current.GetService<IInternetService>();

            Observable.Return(InternetService.GetInternetConnection).ToPropertyEx(this, x => x.CanLoad);

            InternetService.InternetConnection.ToPropertyEx(this, x => x.CanLoad);
        }
    }
}