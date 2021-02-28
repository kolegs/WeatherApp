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
using WeatherApp;
using WeatherApp.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Resource = WeatherApp.Droid.Resource;

[assembly: ExportRenderer(typeof(AppShell), typeof(CustomShellRenderer))]
namespace WeatherApp.Droid
{
    public class CustomShellRenderer : ShellRenderer
    {
        public CustomShellRenderer(Context context) : base(context)
        {
        }

        protected override IShellToolbarAppearanceTracker CreateToolbarAppearanceTracker()
        {
            return new CustomToolbarAppearanceTracker();
        }
    }
}