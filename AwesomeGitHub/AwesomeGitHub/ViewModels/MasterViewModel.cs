using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive.Linq;

namespace AwesomeGitHub.ViewModels
{
    public class MasterViewModel : ViewModelBase
    {
        [Reactive] public string Language { get; set; }

        public MasterViewModel()
        {
            this.WhenAnyValue(x => x.Language)
                .Where(x => !string.IsNullOrEmpty(x) && x.Length > 2)
                .Throttle(TimeSpan.FromSeconds(2))
                .Subscribe(x =>
                {
                    Console.WriteLine(x);
                });
        }
    }
}
