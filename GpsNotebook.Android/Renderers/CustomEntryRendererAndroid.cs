using Android.Graphics.Drawables;
using GpsNotebook.Controls;
using GpsNotebook.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRendererAndroid))]
namespace GpsNotebook.Droid.Renderers
{
    public class CustomEntryRendererAndroid : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                var gradiendDrawable = new GradientDrawable();
                gradiendDrawable.SetCornerRadius(30);
                gradiendDrawable.SetStroke(5, Android.Graphics.Color.DeepPink);
                Control.SetBackground(gradiendDrawable);

                Control.SetPadding(50, Control.PaddingTop, Control.PaddingRight, Control.PaddingBottom);
            }
        }
    }
}
