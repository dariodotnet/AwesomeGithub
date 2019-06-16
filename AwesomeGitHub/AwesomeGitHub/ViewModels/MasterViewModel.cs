namespace AwesomeGitHub.ViewModels
{
    using ReactiveUI;
    using ReactiveUI.Fody.Helpers;
    using Services;
    using Splat;
    using System.Reactive;

    public class MasterViewModel : ViewModelBase
    {
        private readonly ICacheService _cacheService;

        [Reactive] public string Language { get; set; }

        public ReactiveCommand<Unit, Unit> ChangeCommand { get; }

        public MasterViewModel()
        {
            _cacheService = Locator.Current.GetService<ICacheService>();

            ChangeCommand = ReactiveCommand.Create(ChangeLanguage);

        }

        private void ChangeLanguage() => _cacheService.ChangeLanguage(Language);
    }
}
