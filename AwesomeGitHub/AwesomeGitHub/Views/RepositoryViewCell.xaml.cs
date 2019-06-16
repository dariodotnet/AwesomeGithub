namespace AwesomeGitHub.Views
{
    using ReactiveUI;
    using System.Reactive.Disposables;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RepositoryViewCell
    {
        public RepositoryViewCell()
        {
            InitializeComponent();

            this.WhenActivated(d =>
                {
                    this.OneWayBind(ViewModel, vm => vm.RepositoryName, v => v.RepositoryName.Text,
                        e => $"{e[0].ToString().ToUpper()}{e.Substring(1)}").DisposeWith(d);
                });
        }
    }
}