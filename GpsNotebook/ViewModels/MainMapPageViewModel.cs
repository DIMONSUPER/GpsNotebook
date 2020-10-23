using GpsNotebook.Services;
using Prism.Navigation;
using Prism.Services;

namespace GpsNotebook.ViewModels
{
    public class MainMapPageViewModel : ViewModelBase
    {
        private IRepositoryService RepositoryService { get; }
        private IPageDialogService PageDialogService { get; }

        public MainMapPageViewModel(
            INavigationService navigationService,
            IRepositoryService repositoryService,
            IPageDialogService pageDialogService)
            : base(navigationService)
        {
            RepositoryService = repositoryService;
            PageDialogService = pageDialogService;
        }
    }
}

