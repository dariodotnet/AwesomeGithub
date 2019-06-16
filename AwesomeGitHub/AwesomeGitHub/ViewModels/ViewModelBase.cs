namespace AwesomeGitHub.ViewModels
{
    using ReactiveUI;
    using System;
    using System.Reactive;

    public class ViewModelBase : ReactiveObject
    {
        public Interaction<Exception, Unit> ExceptionInteraction { get; }

        protected ViewModelBase()
        {
            ExceptionInteraction = new Interaction<Exception, Unit>(RxApp.MainThreadScheduler);
        }
    }
}