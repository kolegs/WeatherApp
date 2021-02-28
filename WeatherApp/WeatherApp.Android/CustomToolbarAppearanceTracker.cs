using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace WeatherApp.Droid
{

    // Toolbar appearance
    public class CustomToolbarAppearanceTracker : IShellToolbarAppearanceTracker
    {
        public void Dispose()
        {

        }

        public void SetAppearance(Toolbar toolbar, IShellToolbarTracker toolbarTracker, ShellAppearance appearance)
        {
            //toolbar.SetBackgroundResource(Resource.Drawable.custom_gradient);
        }

        public void ResetAppearance(Toolbar toolbar, IShellToolbarTracker toolbarTracker)
        {
        }
    }
}