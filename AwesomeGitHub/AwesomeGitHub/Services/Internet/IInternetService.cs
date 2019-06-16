namespace AwesomeGitHub.Services
{
    using System.Reactive.Subjects;

    public interface IInternetService
    {
        Subject<bool> InternetConnection { get; }
        bool GetInternetConnection { get; }
    }
}