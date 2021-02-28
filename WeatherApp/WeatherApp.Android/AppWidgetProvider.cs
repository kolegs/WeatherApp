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
using System.Threading.Tasks;
using Android.Appwidget;
using WeatherApp.Converters;
using WeatherApp.Extensions;
using WeatherApp.REST;
using Xamarin.Essentials;
using Xamarin.Forms.Platform.Android;
using UnitConverters = WeatherApp.Converters.UnitConverters;

namespace WeatherApp.Droid
{
    [BroadcastReceiver(Label = "Weather widget")]
    [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
    [MetaData("android.appwidget.provider", Resource = "@xml/appwidgetprovider")]
    public class AppWidget : AppWidgetProvider
    {
        public override async void OnUpdate(Context context,
            AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            var me = new ComponentName(context, Java.Lang.Class.FromType(typeof(AppWidget)).Name);
            appWidgetManager.UpdateAppWidget(me, await BuildRemoteViews(context, appWidgetIds));
        }
        private async Task<RemoteViews> BuildRemoteViews(Context context, int[] appWidgetIds)
        {
            var widgetView = new RemoteViews(context.PackageName, Resource.Layout.Widget);
            RegisterClicks(context, appWidgetIds, widgetView);

            try
            {
                var location = await Geolocation.GetLocationAsync();
                if (location != null)
                {
                    var placeName = "";
                    var address = await Nominatim.GetAddress(location.Latitude, location.Longitude);
                    if (!string.IsNullOrEmpty(address.Address.Administrative))
                        placeName = address.Address.Administrative;
                    else if (!string.IsNullOrEmpty(address.Address.City))
                        placeName = address.Address.City;
                    else if (!string.IsNullOrEmpty(address.Address.Village))
                        placeName = address.Address.Village;

                    widgetView.SetTextViewText(Resource.Id.PlaceName, placeName);
                    await SetLocation(placeName, location.Latitude, location.Longitude, widgetView);

                    ShowError(widgetView, ErrorType.None);
                }
                else
                {
                    ShowError(widgetView, ErrorType.LocalizationError);
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                ShowError(widgetView, ErrorType.LocalizationError);
            }
            catch (FeatureNotEnabledException fneEx)
            {
                ShowError(widgetView, ErrorType.LocalizationError);
            }
            catch (PermissionException pEx)
            {
                ShowError(widgetView, ErrorType.LocalizationError);
            }
            catch (Exception)
            {
                ShowError(widgetView, ErrorType.InternetConnectionError);
            }

            return widgetView;
        }

        private void ShowError(RemoteViews widgetView, ErrorType error)
        {
            switch (error)
            {
                case ErrorType.None:
                    widgetView.SetViewVisibility(Resource.Id.MainLayout, ViewStates.Visible);
                    widgetView.SetViewVisibility(Resource.Id.PlaceName, ViewStates.Visible);
                    widgetView.SetViewVisibility(Resource.Id.Description, ViewStates.Visible);
                    widgetView.SetViewVisibility(Resource.Id.ErrorConnection, ViewStates.Gone);
                    break;
                case ErrorType.LocalizationError:
                    widgetView.SetViewVisibility(Resource.Id.MainLayout, ViewStates.Gone);
                    widgetView.SetViewVisibility(Resource.Id.PlaceName, ViewStates.Gone);
                    widgetView.SetViewVisibility(Resource.Id.Description, ViewStates.Gone);
                    widgetView.SetTextViewText(Resource.Id.ErrorConnection, Resources.Translations.GetMissingLocalizationPermissionsText());
                    widgetView.SetViewVisibility(Resource.Id.ErrorConnection, ViewStates.Visible);
                    break;
                case ErrorType.InternetConnectionError:
                    widgetView.SetViewVisibility(Resource.Id.MainLayout, ViewStates.Gone);
                    widgetView.SetViewVisibility(Resource.Id.PlaceName, ViewStates.Gone);
                    widgetView.SetViewVisibility(Resource.Id.Description, ViewStates.Gone);
                    widgetView.SetTextViewText(Resource.Id.ErrorConnection, Resources.Translations.GetNoInternetConnectionText());
                    widgetView.SetViewVisibility(Resource.Id.ErrorConnection, ViewStates.Visible);
                    break;
            }
        }

        public enum ErrorType
        {
            None,
            LocalizationError,
            InternetConnectionError
        }

        public override void OnReceive(Context context, Intent intent)
        {
            base.OnReceive(context, intent);
        }

        public async Task SetLocation(string location, double latitude, double longitude, RemoteViews widgetView)
        {
            var weather = Settings.Settings.Instance.GetWeather(latitude, longitude);

            if (weather is null)
            {
                try
                {
                    weather = await OpenWeather.GetWeather(latitude, longitude, OpenWeather.Units.Metric);
                    Settings.Settings.Instance.UpdatePlace(latitude, longitude, weather);
                }
                catch (Exception)
                {
                    weather = Settings.Settings.Instance.GetWeatherFull(latitude, longitude);
                    if (weather is null)
                    {
                        return;
                    }
                }
            }
            widgetView.SetTextViewText(Resource.Id.Temperature, FormatTemperature(weather.Current.Temperature));
            widgetView.SetTextViewText(Resource.Id.Sunrise, string.Format(Resources.Translations.GetSunriseText(),
                UnixTimeStampToDateTimeConverter.Convert(weather.Current.Sunrise).ToString("t")));
            widgetView.SetTextViewText(Resource.Id.Sunset, string.Format(Resources.Translations.GetSunsetText(),
                UnixTimeStampToDateTimeConverter.Convert(weather.Current.Sunset).ToString("t")));
            widgetView.SetTextViewText(Resource.Id.Pressure, weather.Current.Pressure.ToString("F0") + "hPa");
            widgetView.SetTextViewText(Resource.Id.WindSpeed, FormatWindSpeed(weather.Current.WindSpeed));

            if (weather.Current.WeatherList.Count > 0)
            {
                SetWeatherIcon(widgetView, weather.Current.WeatherList[0].Icon);
                widgetView.SetTextViewText(Resource.Id.Description, weather.Current.WeatherList[0].Description.FirstCharToUpper());
            }

        }

        private void SetWeatherIcon(RemoteViews widgetView, string icon)
        {
            switch (icon)

            {
                case "01d":
                    widgetView.SetImageViewResource(Resource.Id.WeatherIcon, Resource.Drawable.weather_fc_clear_day);
                    break;
                case "01n":
                    widgetView.SetImageViewResource(Resource.Id.WeatherIcon, Resource.Drawable.weather_fc_clear_night);
                    break;

                case "02d":
                    widgetView.SetImageViewResource(Resource.Id.WeatherIcon, Resource.Drawable.weather_fc_partly_cloudy_day);
                    break;
                case "02n":
                    widgetView.SetImageViewResource(Resource.Id.WeatherIcon, Resource.Drawable.weather_fc_partly_cloudy_night);
                    break;

                case "03d":
                    widgetView.SetImageViewResource(Resource.Id.WeatherIcon, Resource.Drawable.weather_fc_cloudy);
                    break;
                case "03n":
                    widgetView.SetImageViewResource(Resource.Id.WeatherIcon, Resource.Drawable.weather_fc_cloudy);
                    break;

                case "04d":
                    widgetView.SetImageViewResource(Resource.Id.WeatherIcon, Resource.Drawable.weather_fc_cloudy);
                    break;
                case "04n":
                    widgetView.SetImageViewResource(Resource.Id.WeatherIcon, Resource.Drawable.weather_fc_cloudy);
                    break;

                case "09d":
                    widgetView.SetImageViewResource(Resource.Id.WeatherIcon, Resource.Drawable.weather_fc_rain);
                    break;
                case "09n":
                    widgetView.SetImageViewResource(Resource.Id.WeatherIcon, Resource.Drawable.weather_fc_rain);
                    break;

                case "10d":
                    widgetView.SetImageViewResource(Resource.Id.WeatherIcon, Resource.Drawable.weather_fc_10d);
                    break;
                case "10n":
                    widgetView.SetImageViewResource(Resource.Id.WeatherIcon, Resource.Drawable.weather_fc_10n);
                    break;

                case "11d":
                    widgetView.SetImageViewResource(Resource.Id.WeatherIcon, Resource.Drawable.weather_fc_11d);
                    break;
                case "11n":
                    widgetView.SetImageViewResource(Resource.Id.WeatherIcon, Resource.Drawable.weather_fc_11n);
                    break;

                case "13d":
                    widgetView.SetImageViewResource(Resource.Id.WeatherIcon, Resource.Drawable.weather_fc_snow);
                    break;
                case "13n":
                    widgetView.SetImageViewResource(Resource.Id.WeatherIcon, Resource.Drawable.weather_fc_13n);
                    break;

                case "50d":
                    widgetView.SetImageViewResource(Resource.Id.WeatherIcon, Resource.Drawable.weather_fc_fog);
                    break;
                case "50n":
                    widgetView.SetImageViewResource(Resource.Id.WeatherIcon, Resource.Drawable.weather_fc_fog);
                    break;

                default:
                    widgetView.SetImageViewResource(Resource.Id.WeatherIcon, Resource.Drawable.weather_fc_clear_day);
                    break;
            }

        }

        public string FormatWindSpeed(double windSpeed)
        {
            if (Settings.Settings.Instance.Units.WindSpeed == Settings.Settings.UnitSettings.WindSpeedType.Meters)
                return windSpeed.ToString("F0") + "m/s";
            return Converters.UnitConverters.MetersPerSecondsToKilometersPerHours(windSpeed).ToString("F0") + "km/h";
        }

        public string FormatTemperature(double temperature)
        {
            if (Settings.Settings.Instance.Units.Temperature == Settings.Settings.UnitSettings.TemperatureType.Metric)
                return temperature.ToString("F0") + "°C";
            return UnitConverters.CelsiusToFahrenheit(temperature).ToString("F0") + "°F";
        }
        private void RegisterClicks(Context context, int[] appWidgetIds, RemoteViews widgetView)
        {
            var intent = new Intent(context, typeof(AppWidget));
            intent.SetAction(AppWidgetManager.ActionAppwidgetUpdate);
            intent.PutExtra(AppWidgetManager.ExtraAppwidgetIds, appWidgetIds);

            // Register click event for the Background
            var piBackground = PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.UpdateCurrent);
            widgetView.SetOnClickPendingIntent(Resource.Id.WidgetBackground, piBackground);
        }
    }
}