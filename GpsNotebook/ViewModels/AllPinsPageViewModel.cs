using GpsNotebook.Services;
using Prism.Navigation;
using Prism.Services;

namespace GpsNotebook.ViewModels
{
    public class AllPinsPageViewModel : ViewModelBase
    {
        private IRepositoryService RepositoryService { get; }
        private IPageDialogService PageDialogService { get; }

        public AllPinsPageViewModel(
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

