using SQLite;
using Xamarin.Forms.GoogleMaps;

namespace GpsNotebook.Behaviors
{
    [Preserve(AllMembers = true)]
    public sealed class MapClickedToCommandBehavior : EventToCommandBehaviorBase
    {
        protected override void OnAttachedTo(Map bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.MapClicked += OnMapClicked;
        }

        protected override void OnDetachingFrom(Map bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.MapClicked -= OnMapClicked;
        }

        private void OnMapClicked(object sender, MapClickedEventArgs mapClickedEventArgs)
        {
            Command?.Execute(mapClickedEventArgs);
        }
    }
}
