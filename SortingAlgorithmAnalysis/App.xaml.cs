using Prism;
using Prism.Ioc;
using Prism.Unity;
using SortingAlgorithmAnalysis.ViewModels;
using SortingAlgorithmAnalysis.Pages;
using Xamarin.Forms;

namespace SortingAlgorithmAnalysis
{
    public partial class App : PrismApplication
    {
        public App(IPlatformInitializer initializer = null) : base(initializer) { }

        protected async override void OnInitialized()
        {
            InitializeComponent();

            await NavigationService.NavigateAsync($"{nameof(NavigationPage)}/{nameof(HomePage)}");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<HomePage, HomePageViewModel>();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
