namespace AwesomeGitHub.Views
{
    using ReactiveUI;
    using System.Reactive.Disposables;
    using ViewModels;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterView
    {
        public MasterView()
        {
            InitializeComponent();

            this.WhenActivated(d => { this.Bind(ViewModel, vm => vm.Language, v => v.Searcher.Text).DisposeWith(d); });

            BindingContext = new MasterViewModel();
        }
    }
}