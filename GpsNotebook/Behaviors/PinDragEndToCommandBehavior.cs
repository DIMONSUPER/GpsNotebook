using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.GoogleMaps;

namespace GpsNotebook.Behaviors
{
    [Preserve(AllMembers = true)]
    public sealed class PinDragEndToCommandBehavior : EventToCommandBehaviorBase
    {
        protected override void OnAttachedTo(Map bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.PinDragEnd += OnPinDragEnd;
        }

        protected override void OnDetachingFrom(Map bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.PinDragEnd -= OnPinDragEnd;
        }

        private void OnPinDragEnd(object sender, PinDragEventArgs args)
        {
            Command?.Execute(args);
        }
    }
}
