namespace AwesomeGitHub.Views
{
    using System;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RepositoryViewCell
    {
        public RepositoryViewCell()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (ViewModel is null)
                return;

            RepositoryName.Text = $"{ViewModel.Name[0].ToString().ToUpper()}{ViewModel.Name.Substring(1)}";
            Description.Text = ViewModel.Description;
            ForksCount.Text = ViewModel.ForksCount.ToString();
            StarsCount.Text = ViewModel.StarsCount.ToString();
            OwnerLogin.Text = ViewModel.Owner;
            FullName.Text = ViewModel.FullName;

            UserImage.Source = ImageSource.FromUri(new Uri(ViewModel.Image));
        }
    }
}