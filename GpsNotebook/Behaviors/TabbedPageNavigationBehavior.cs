using Prism.Common;
using Prism.Navigation;
using System;
using Xamarin.Forms;

namespace GpsNotebook.Behaviors
{
    public class TabbedPageNavigationBehavior : BehaviorBase<TabbedPage>
    {
        private Page CurrentPage { get; set; }

        protected override void OnAttachedTo(TabbedPage bindable)
        {
            bindable.CurrentPageChanged += OnCurrentPageChanged;
            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(TabbedPage bindable)
        {
            bindable.CurrentPageChanged -= OnCurrentPageChanged;
            base.OnDetachingFrom(bindable);
        }

        private void OnCurrentPageChanged(object sender, EventArgs e)
        {
            Page newPage = AssociatedObject.CurrentPage;

            if (CurrentPage != null)
            {
                NavigationParameters parameters = new NavigationParameters();
                PageUtilities.OnNavigatedFrom(CurrentPage, parameters);
                PageUtilities.OnNavigatedTo(newPage, parameters);
            }

            CurrentPage = newPage;
        }
    }
}
