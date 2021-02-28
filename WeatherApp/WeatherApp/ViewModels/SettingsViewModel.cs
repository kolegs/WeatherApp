using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using WeatherApp.Resources;
using Xamarin.Forms;

namespace WeatherApp.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public SettingsViewModel()
        {
            DateText = DateTime.Now.ToString("d");
            
            TemperatureType =
                Settings.Settings.Instance.Units.Temperature == Settings.Settings.UnitSettings.TemperatureType.Metric
                    ? "°C"
                    : "°F";
            
            WindSpeedType =
                Settings.Settings.Instance.Units.WindSpeed == Settings.Settings.UnitSettings.WindSpeedType.Meters
                    ? "m/s"
                    : "km/h";

            TemperatureClicked = new Command(
                execute: async () =>
                {
                    var selected = await Application.Current.MainPage.DisplayActionSheet(
                        AppTranslations.SettingsSetTemperatureTitle,
                        AppTranslations.SettingsSetTemperatureClose, null,
                        "°C", "°F");
                    switch (selected)
                    {
                        case "Anuluj":
                            return;
                        case "°C":
                            TemperatureType = "°C";
                            Settings.Settings.Instance.SetTemperatureType(Settings.Settings.UnitSettings.TemperatureType.Metric);
                            return;
                        case "°F":
                            TemperatureType = "°F";
                            Settings.Settings.Instance.SetTemperatureType(Settings.Settings.UnitSettings.TemperatureType.Imperial);
                            return;
                    }
                    
                },
                canExecute: () => true);
            
            WindSpeedClicked = new Command(
                execute: async () =>
                {
                    var selected = await Application.Current.MainPage.DisplayActionSheet(
                        AppTranslations.SettingsSetWindSpeedTitle,
                        AppTranslations.SettingsSetWindSpeedClose, null,
                        "m/s", "km/h");
                    switch (selected)
                    {
                        case "Anuluj":
                            return;
                        case "m/s":
                            WindSpeedType = "m/s";
                            Settings.Settings.Instance.SetWindSpeedType(Settings.Settings.UnitSettings.WindSpeedType.Meters);
                            return;
                        case "km/h":
                            WindSpeedType = "km/h";
                            Settings.Settings.Instance.SetWindSpeedType(Settings.Settings.UnitSettings.WindSpeedType.Kilometers);
                            return;
                    }
                },
                canExecute: () => true);
        }
        public void Refresh()
        {
            DateText = DateTime.Now.ToString("d");
        }

        private string _dateText;
        public string DateText
        {
            get => _dateText;
            set => SetProperty(ref _dateText, value);
        }

        private string _temperatureType;
        public string TemperatureType
        {
            get => _temperatureType;
            set => SetProperty(ref _temperatureType, value);
        }

        private string _windSpeedType;
        public string WindSpeedType
        {
            get => _windSpeedType;
            set => SetProperty(ref _windSpeedType, value);
        }

        public ICommand TemperatureClicked { private set; get; }
        public ICommand WindSpeedClicked { private set; get; }


    }
}
