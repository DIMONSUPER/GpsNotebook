using Acr.UserDialogs;
using GpsNotebook.Helpers;
using GpsNotebook.Services;
using GpsNotebook.Services.LocationService;
using GpsNotebook.ViewModels;
using GpsNotebook.Views;
using Plugin.Settings;
using Prism;
using Prism.Ioc;
using Prism.Navigation;
using Prism.Unity;
using Xamarin.Forms;

namespace GpsNotebook
{
    public partial class App : PrismApplication
    {
        public App():this(null) { }
        public App(IPlatformInitializer initializer = null) : base(initializer) { }
        protected async override void OnInitialized()
        {
            InitializeComponent();
            if (string.IsNullOrEmpty(Settings.RememberedEmail))
            {
                await NavigationService.NavigateAsync($"/{nameof(NavigationPage)}/{nameof(SignInPage)}");
            }
            else
            {
                await NavigationService.NavigateAsync(
                    $"/{nameof(NavigationPage)}" +
                    $"/{nameof(MainTabbedPage)}" +
                    $"?{KnownNavigationParameters.SelectedTab}" +
                    $"={nameof(MainMapPage)}");
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance<IRepositoryService>(Container.Resolve<RepositoryService>());
            containerRegistry.RegisterInstance<ILocationService>(Container.Resolve<LocationService>());

            containerRegistry.RegisterInstance(UserDialogs.Instance);
            containerRegistry.RegisterInstance(CrossSettings.Current);

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainTabbedPage>();
            containerRegistry.RegisterForNavigation<SignInPage, SignInPageViewModel>();
            containerRegistry.RegisterForNavigation<SignUpPage, SignUpPageViewModel>();
            containerRegistry.RegisterForNavigation<MainMapPage, MainMapPageViewModel>();
            containerRegistry.RegisterForNavigation<AllPinsPage, AllPinsPageViewModel>();
            containerRegistry.RegisterForNavigation<PinInfoPage, PinInfoPageViewModel>();
            containerRegistry.RegisterForNavigation<AddPinPage, AddPinPageViewModel>();
        }
    }
}
