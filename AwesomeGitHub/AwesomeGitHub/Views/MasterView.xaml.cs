namespace AwesomeGitHub.Views
{
    using ReactiveUI;
    using System;
    using System.Reactive;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using ViewModels;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterView
    {
        public MasterView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.Bind(ViewModel, vm => vm.Language, v => v.Searcher.Text).DisposeWith(d);

                Observable.FromEventPattern<FocusEventArgs>(
                        x => Searcher.Unfocused += x,
                        x => Searcher.Unfocused += x)
                    .Subscribe(x =>
                    {
                        Observable.Return(Unit.Default).InvokeCommand(ViewModel.ChangeCommand);
                        ((MasterDetailPage)Application.Current.MainPage).IsPresented = false;
                    });
            });

            BindingContext = new MasterViewModel();
        }
    }
}