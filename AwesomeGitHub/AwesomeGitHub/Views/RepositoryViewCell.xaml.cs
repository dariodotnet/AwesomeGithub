using Xamarin.Forms.Xaml;

namespace AwesomeGitHub.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RepositoryViewCell
    {
        public RepositoryViewCell()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            //UserImage.Source = null;

            //if (ViewModel is null)
            //    return;

            //UserImage.Source = ViewModel.Owner.Avatar;
            base.OnBindingContextChanged();
        }
    }
}