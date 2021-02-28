using System;
using Plugin.FirebasePushNotification;
using WeatherApp.Models;
using WeatherApp.Resources;
using WeatherApp.REST;
using WeatherApp.Views;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WeatherApp
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();

            Xamarin.Essentials.Connectivity.ConnectivityChanged += ConnectivityOnConnectivityChanged;

            CrossFirebasePushNotification.Current.OnNotificationReceived += CurrentOnOnNotificationReceived;
        }

        private void CurrentOnOnNotificationReceived(object source, FirebasePushNotificationDataEventArgs e)
        {
            try
            {
                var title = e.Data["title"].ToString();
                var message = e.Data["body"].ToString();

                if (title == "")
                    return;
                if (message == "")
                    return;
                Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert(title, message,
                        AppTranslations.Close);
                });

            }
            catch (Exception)
            {
            }
        }

        private void ConnectivityOnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (Settings.Settings.Instance.IsSynced == false)
                Settings.Settings.Instance.SetIsSynced(false);
        }

        protected override void OnStart()
        {
        }
        

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
            if (Settings.Settings.Instance.IsSynced == false)
                Settings.Settings.Instance.SetIsSynced(false);
        }
    }
}
