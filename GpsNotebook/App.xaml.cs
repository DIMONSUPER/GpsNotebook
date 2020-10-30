using Acr.UserDialogs;
using GpsNotebook.Helpers;
using GpsNotebook.Services.Authorization;
using GpsNotebook.Services.Location;
using GpsNotebook.Services.Permissions;
using GpsNotebook.Services.Pin;
using GpsNotebook.Services.Repository;
using GpsNotebook.ViewModels;
using GpsNotebook.Views;
using Plugin.Settings;
using Prism;
using Prism.Ioc;
using Prism.Unity;
using Xamarin.Forms;

namespace GpsNotebook
{
    public partial class App : PrismApplication
    {
        public App() : this(null) { }
        public App(IPlatformInitializer initializer = null) : base(initializer) { }
        protected async override void OnInitialized()
        {
            InitializeComponent();
            if (string.IsNullOrEmpty(Settings.RememberedEmail))
            {
                await NavigationService.NavigateAsync($"{nameof(NavigationPage)}/{nameof(SignInPage)}");
            }
            else
            {
                await NavigationService.NavigateAsync($"{nameof(NavigationPage)}/{nameof(MainTabbedPage)}");
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance(UserDialogs.Instance);
            containerRegistry.RegisterInstance(CrossSettings.Current);

            containerRegistry.RegisterInstance<IRepositoryService>(Container.Resolve<RepositoryService>());
            containerRegistry.RegisterInstance<IPinService>(Container.Resolve<PinService>());
            containerRegistry.RegisterInstance<IAuthorizationService>(Container.Resolve<AuthorizationService>());
            containerRegistry.RegisterInstance<IPermissionsService>(Container.Resolve<PermissionsService>());
            containerRegistry.RegisterInstance<ILocationService>(Container.Resolve<LocationService>());

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainTabbedPage>();
            containerRegistry.RegisterForNavigation<SignInPage, SignInPageViewModel>();
            containerRegistry.RegisterForNavigation<SignUpPage, SignUpPageViewModel>();
            containerRegistry.RegisterForNavigation<MainMapPage, MainMapPageViewModel>();
            containerRegistry.RegisterForNavigation<AllPinsPage, AllPinsPageViewModel>();   
            containerRegistry.RegisterForNavigation<AddPinPage, AddPinPageViewModel>();
        }
    }
}
