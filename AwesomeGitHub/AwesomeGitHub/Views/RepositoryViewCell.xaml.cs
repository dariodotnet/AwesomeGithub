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

                    this.OneWayBind(ViewModel, vm => vm.Description, v => v.Description.Text).DisposeWith(d);
                    this.OneWayBind(ViewModel, vm => vm.ForksCount, v => v.ForksCount.Text).DisposeWith(d);
                    this.OneWayBind(ViewModel, vm => vm.StarsCount, v => v.StarsCount.Text).DisposeWith(d);
                    this.OneWayBind(ViewModel, vm => vm.Owner.Login, v => v.OwnerLogin.Text).DisposeWith(d);
                    this.OneWayBind(ViewModel, vm => vm.FullName, v => v.FullName.Text).DisposeWith(d);
                });
        }
    }
}