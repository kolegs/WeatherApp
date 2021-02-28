using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using WeatherApp.Converters;
using WeatherApp.Extensions;
using WeatherApp.Models;
using WeatherApp.Resources;
using WeatherApp.REST;
using WeatherApp.Views;
using Xamarin.Essentials;
using Xamarin.Forms;
using UnitConverters = WeatherApp.Converters.UnitConverters;

namespace WeatherApp.ViewModels
{
    public class WeatherViewModel : BaseViewModel
    {
        private static WeatherViewModel _instance;
        public static WeatherViewModel Instance
        {
            get
            {
                if (_instance is null)
                    _instance = new WeatherViewModel();
                return _instance;
            }
        }
        private WeatherViewModel()
        {
            SearchCommand = new Command(
                execute: async(object address) =>
                {
                    var page = new SearchPage();
                    var model = new SearchViewModel(address as string);
                    page.BindingContext = model;
                    await Application.Current.MainPage.Navigation.PushModalAsync(page);
                    SearchText = "";
                    var search = await Nominatim.GetAddress(address as string);
                    model.Update(search);
                },
                canExecute: (object address) => true);

            CurrentLocationCommand = new Command(
                execute: async () =>
                {
                    try
                    {
                        DateText = DateTime.Now.ToString("d");
                        RefreshFavoritePlaces();
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

                            SetLocation(placeName, location.Latitude, location.Longitude);
                        }
                        else
                        {
                            if (Settings.Settings.Instance.Places.Count == 0)
                                await Application.Current.MainPage.DisplayAlert(Resources.AppTranslations.MissingLocalizationTitle,
                                    Resources.AppTranslations.MissingLocalizationText,
                                    Resources.AppTranslations.MissingLocalizationClose);
                            else
                                SetLocation(Settings.Settings.Instance.Places.ElementAt(0).Add.PlaceName,
                                    Settings.Settings.Instance.Places.ElementAt(0).Add.Latitude,
                                    Settings.Settings.Instance.Places.ElementAt(0).Add.Longitude);
                        }
                    }
                    catch (FeatureNotSupportedException fnsEx)
                    {
                        await Application.Current.MainPage.DisplayAlert(Resources.AppTranslations.MissingLocalizationTitle,
                            Resources.AppTranslations.MissingLocalizationText,
                            Resources.AppTranslations.MissingLocalizationClose);
                    }
                    catch (FeatureNotEnabledException fneEx)
                    {
                        await Application.Current.MainPage.DisplayAlert(Resources.AppTranslations.MissingLocalizationTitle,
                            Resources.AppTranslations.MissingLocalizationText,
                            Resources.AppTranslations.MissingLocalizationClose);
                    }
                    catch (PermissionException pEx)
                    {
                        await Application.Current.MainPage.DisplayAlert(Resources.AppTranslations.MissingLocalizationTitle,
                            Resources.AppTranslations.MissingLocalizationText,
                            Resources.AppTranslations.MissingLocalizationClose);
                    }
                    catch (Exception ex)
                    {
                        if (Settings.Settings.Instance.Places.Count == 0)
                            await Application.Current.MainPage.DisplayAlert(Resources.AppTranslations.NoInternetTitle,
                                Resources.AppTranslations.NoInternetText,
                                Resources.AppTranslations.NoInternetClose);
                        else
                            SetLocation(Settings.Settings.Instance.Places.ElementAt(0).Add.PlaceName,
                                Settings.Settings.Instance.Places.ElementAt(0).Add.Latitude,
                                Settings.Settings.Instance.Places.ElementAt(0).Add.Longitude);
                    }
                    IsLoading = false;
                },
                canExecute: () => true);

            Connectivity.ConnectivityChanged += (sender, args) =>
            {
                Refresh();
            };
        }

        private bool onlyOnce = false;
        public async void Refresh()
        {
            RefreshFavoritePlaces();
            if (string.IsNullOrEmpty(CurrentLocation))
            {
                if (onlyOnce == false)
                {
                    onlyOnce = true;
                    CurrentLocationCommand.Execute(null);
                }
            }
            else
            {
                SetLocation(CurrentLocation, Latitude, Longitude);
            }
        }
        
        private double Latitude;
        private double Longitude;

        public async void SetLocation(string location, double latitude, double longitude)
        {
            IsLoading = true;
            CurrentLocation = location;
            Latitude = latitude;
            Longitude = longitude;
            OlderDataVisible = false;
            
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
                    if(weather is null)
                    {
                        IsLoading = false;
                        return;
                    }

                    OlderDataVisible = true;
                    OlderDataText = string.Format(Resources.AppTranslations.OldDataFrom,
                        Settings.Settings.Instance.GetWeatherRefreshTime(latitude, longitude)?.ToString("g"));
                }
            }


            DateText = DateTime.Now.ToString("d");
            if (weather.Current.WeatherList.Count > 0)
            {
                WeatherIcon = StringToIconConverter.Convert(weather.Current.WeatherList[0].Icon);
                WeatherDescription = weather.Current.WeatherList[0].Description.FirstCharToUpper();
            }

            WeatherTemperature = FormatTemperature(weather.Current.Temperature);
            if (weather.Current.Rain != null)
            {
                var max = Math.Max(weather.Current.Rain.ThreeHours, weather.Current.Rain.OneHour);
                WeatherRain = max.ToString("F0") + "mm";
            }
            if (weather.Current.Snow != null)
            {
                var max = Math.Max(weather.Current.Snow.ThreeHours, weather.Current.Snow.OneHour);
                WeatherRain = max.ToString("F0") + "mm";
            }

            WeatherWind = FormatWindSpeed(weather.Current.WindSpeed);

            WeatherPressure = weather.Current.Pressure.ToString("F0") + "hPa";
            WeatherSunrise = string.Format(AppTranslations.Sunrise,
                UnixTimeStampToDateTimeConverter.Convert(weather.Current.Sunrise).ToString("t"));
            WeatherSunset = string.Format(AppTranslations.Sunset,
                UnixTimeStampToDateTimeConverter.Convert(weather.Current.Sunset).ToString("t"));

            HoursForecastItems.Clear();
            foreach (var hourlyWeather in weather.Hourly)
            {
                var forecast = new HourForecast()
                {
                    Time = UnixTimeStampToDateTimeConverter.Convert(hourlyWeather.Datetime)
                        .ToString("t"),
                    Temperature = FormatTemperature(hourlyWeather.Temperature)
                };
                if (hourlyWeather.WeatherList.Count > 0)
                {
                    forecast.IconSource = StringToIconConverter.Convert(hourlyWeather.WeatherList[0].Icon);
                }

                HoursForecastItems.Add(forecast);
                if (HoursForecastItems.Count >= 24)
                    break;
            }
            
            DaysForecastItems.Clear();
            foreach (var dailyWeather in weather.Daily)
            {
                var forecast = new DayForecast()
                {
                    Temperature = FormatTemperature(dailyWeather.Temperature.Day),
                    Time = UnixTimeStampToDateTimeConverter.Convert(dailyWeather.Datetime + weather.TimezoneOffset)
                        .ToString("ddd"),
                    Rain = (dailyWeather.Rain + dailyWeather.Snow).ToString("F0") + "mm",
                };
                if (dailyWeather.WeatherList.Count > 0)
                {
                    forecast.IconSource = StringToIconConverter.Convert(dailyWeather.WeatherList[0].Icon);
                }

                DaysForecastItems.Add(forecast);
                
                if (DaysForecastItems.Count >= 7)
                    break;
            }

            RefreshFavoritePlaces();
            IsLoading = false;
        }

        public string FormatWindSpeed(double windSpeed)
        {
            if (Settings.Settings.Instance.Units.WindSpeed == Settings.Settings.UnitSettings.WindSpeedType.Meters)
                return windSpeed.ToString("F0") + "m/s";
            return UnitConverters.MetersPerSecondsToKilometersPerHours(windSpeed).ToString("F0") + "km/h";
        }

        public string FormatTemperature(double temperature)
        {
            if (Settings.Settings.Instance.Units.Temperature == Settings.Settings.UnitSettings.TemperatureType.Metric)
                return temperature.ToString("F0") + "°C";
            return UnitConverters.CelsiusToFahrenheit(temperature).ToString("F0") + "°F";
        }

        public void RefreshFavoritePlaces()
        {
            FavoritePlaces.Clear();
            foreach (var place in Settings.Settings.Instance.Places)
            {
                FavoritePlaces.Add(new FavoritePlace()
                {
                    PositionText = place.Add.PlaceName,
                    Place = place.Add
                });
            }
        }



        private string _dateText;
        public string DateText
        {
            get => _dateText;
            set => SetProperty(ref _dateText, value);
        }

        private string _currentLocation;
        public string CurrentLocation
        {
            get => _currentLocation;
            set => SetProperty(ref _currentLocation, value);
        }

        private string _olderDataText;
        public string OlderDataText
        {
            get => _olderDataText;
            set => SetProperty(ref _olderDataText, value);
        }

        private bool _olderDataVisible = false;
        public bool OlderDataVisible
        {
            get => _olderDataVisible;
            set => SetProperty(ref _olderDataVisible, value);
        }

        private ObservableCollection<FavoritePlace> _favoritePlaces = new ObservableCollection<FavoritePlace>();
        public ObservableCollection<FavoritePlace> FavoritePlaces
        {
            get => _favoritePlaces;
            set => SetProperty(ref _favoritePlaces, value);
        }

        private string _weatherIcon;

        public string WeatherIcon
        {
            get => _weatherIcon;
            set => SetProperty(ref _weatherIcon, value);
        }

        private string _weatherTemperature;
        public string WeatherTemperature
        {
            get => _weatherTemperature;
            set => SetProperty(ref _weatherTemperature, value);
        }

        private string _weatherPressure;
        public string WeatherPressure
        {
            get => _weatherPressure;
            set => SetProperty(ref _weatherPressure, value);
        }
        private string _weatherRain;
        public string WeatherRain
        {
            get => _weatherRain;
            set => SetProperty(ref _weatherRain, value);
        }
        private string _weatherWind;
        public string WeatherWind
        {
            get => _weatherWind;
            set => SetProperty(ref _weatherWind, value);
        }

        private string _weatherDescription;
        public string WeatherDescription
        {
            get => _weatherDescription;
            set => SetProperty(ref _weatherDescription, value);
        }

        private string _weatherSunrise;
        public string WeatherSunrise
        {
            get => _weatherSunrise;
            set => SetProperty(ref _weatherSunrise, value);
        }

        private string _weatherSunset;
        public string WeatherSunset
        {
            get => _weatherSunset;
            set => SetProperty(ref _weatherSunset, value);
        }

        private ObservableCollection<HourForecast> _hoursForecastItems = new ObservableCollection<HourForecast>();

        public ObservableCollection<HourForecast> HoursForecastItems
        {
            get => _hoursForecastItems;
            set => SetProperty(ref _hoursForecastItems, value);
        }

        private ObservableCollection<DayForecast> _daysForecastItems = new ObservableCollection<DayForecast>();

        public ObservableCollection<DayForecast> DaysForecastItems
        {
            get => _daysForecastItems;
            set => SetProperty(ref _daysForecastItems, value);
        }

        public ICommand SearchCommand { private set; get; }
        
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }
        
        public ICommand CurrentLocationCommand { private set; get; }

        private bool _isLoading = true;

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }
        
        
    }
}
