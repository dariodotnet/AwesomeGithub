namespace AwesomeGitHub.Views
{
    using Models;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PullRequestView
    {
        public PullRequestView(GitHubRepository repository)
        {
            InitializeComponent();
        }
    }
}