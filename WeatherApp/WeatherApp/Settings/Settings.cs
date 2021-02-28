using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using Newtonsoft.Json;
using Plugin.FirebasePushNotification;
using WeatherApp.Models;
using WeatherApp.REST;
using Xamarin.Essentials;

namespace WeatherApp.Settings
{
    public class Settings
    {
        private static string PLACES_KEY = "places";
        private static string SETTINGS_KEY = "settings";
        private static string NOTIFICATION_TIME_KEY = "notification_time";
        private static string IS_SYNCED_KEY = "synced_key";
        private static string NOTIFICATION_SET = "notification_set";
        private static Settings _instance;
        private Settings()
        {
            LoadPlaces();
            LoadUnits();
            LoadNotificationTime();
            LoadIsSynced();
            LoadNotificationSet();

            CrossFirebasePushNotification.Current.OnTokenRefresh += Current_OnTokenRefresh;
        }

        private void Current_OnTokenRefresh(object source, FirebasePushNotificationTokenEventArgs e)
        {
            SetIsSynced(false);
        }

        public static Settings Instance
        {
            get
            {
                if (_instance is null)
                    _instance = new Settings();
                return _instance;
            }
        }

        public UnitSettings Units { get; private set; } = new UnitSettings();
        private void LoadUnits()
        {
            try
            {
                Units = JsonConvert.DeserializeObject<UnitSettings>(Preferences.Get(SETTINGS_KEY, ""));
                if (Units is null)
                    Units = new UnitSettings();
            }
            catch (Exception)
            {
                Units = new UnitSettings();
            }
        }
        private void SaveUnits()
        {
            var json = JsonConvert.SerializeObject(Units);
            Preferences.Set(SETTINGS_KEY, json);
        }

        public void SetTemperatureType(UnitSettings.TemperatureType temperatureType)
        {
            Units.Temperature = temperatureType;
            SaveUnits();
        }
        
        public void SetWindSpeedType(UnitSettings.WindSpeedType windSpeedType)
        {
            Units.WindSpeed = windSpeedType;
            SaveUnits();
        }

        private bool NotificationSet { get; set; } = false;
        private void LoadNotificationSet()
        {
            try
            {
                NotificationSet = JsonConvert.DeserializeObject<bool>(Preferences.Get(NOTIFICATION_SET, ""));
            }
            catch (Exception)
            {
                NotificationSet = false;
            }
        }

        private void NotificationEnable()
        {
            NotificationSet = true;
            var json = JsonConvert.SerializeObject(NotificationSet);
            Preferences.Set(NOTIFICATION_SET, json);
        }

        public List<Place> Places { get; private set; } = new List<Place>();

        private void SavePlaces()
        {
            var json = JsonConvert.SerializeObject(Places);
            Preferences.Set(PLACES_KEY, json);

            SetIsSynced(false);
        }

        private void LoadPlaces()
        {
            try
            {
                Places = JsonConvert.DeserializeObject<List<Place>>(Preferences.Get(PLACES_KEY, ""));
                if (Places is null)
                    Places = new List<Place>();
            }
            catch (Exception)
            {
                Places = new List<Place>();
            }
        }

        public void UpdatePlace(double latitude, double longitude, OpenWeatherOneCall weather)
        {
            foreach (var place in Places)
            {
                if (place.Add.Latitude != latitude)
                    continue;
                if (place.Add.Longitude != longitude)
                    continue;

                place.Weather = weather;
                place.LastTimeUpdate = DateTime.Now;
                SavePlaces();
            }
        }

        public void UpdateNotification(double latitude, double longitude, bool enabled,
            NotificationPlace.NotificationType notificationType)
        {
            if(enabled && !NotificationSet)
                NotificationEnable();
            
            foreach (var place in Places)
            {
                if (place.Add.Latitude != latitude)
                    continue;
                if (place.Add.Longitude != longitude)
                    continue;

                switch (notificationType)
                {
                    case NotificationPlace.NotificationType.Storm:
                        place.Notification.Storm = enabled;
                        break;
                    case NotificationPlace.NotificationType.Rain:
                        place.Notification.Rain = enabled;
                        break;
                    case NotificationPlace.NotificationType.Snow:
                        place.Notification.Snow = enabled;
                        break;
                    case NotificationPlace.NotificationType.Frost:
                        place.Notification.Frost = enabled;
                        break;
                }
                
                SavePlaces();
            }
        }
        
        public void RemovePlace(Address address)
        {
            for (var i = 0; i < Places.Count; i++)
            {
                var place = Places.ElementAt(i);
                if(place.Add != address)
                    continue;

                Places.RemoveAt(i);
                SavePlaces();
            }
        }

        public void AddPlace(Address add)
        {
            foreach (var place in Places)
            {
                if (place.Add.Latitude != add.Latitude)
                    continue;
                if (place.Add.Longitude != add.Longitude)
                    continue;
                return;
            }
            Places.Add(new Place()
            {
                Add = add
            });
        }
        
        public OpenWeatherOneCall GetWeather(double latitude, double longitude)
        {
            foreach (var place in Places)
            {
                if (place.Add.Latitude != latitude)
                    continue;
                if (place.Add.Longitude != longitude)
                    continue;
                if (DateTime.Now < place.LastTimeUpdate)
                    return null;
                if (DateTime.Now - place.LastTimeUpdate <= TimeSpan.FromHours(1))
                    return place.Weather;
                return null;
            }

            return null;
        }
        public OpenWeatherOneCall GetWeatherFull(double latitude, double longitude)
        {
            foreach (var place in Places)
            {
                if (place.Add.Latitude != latitude)
                    continue;
                if (place.Add.Longitude != longitude)
                    continue;
                return place.Weather;
            }

            return null;
        }

        public DateTime? GetWeatherRefreshTime(double latitude, double longitude)
        {
            foreach (var place in Places)
            {
                if (place.Add.Latitude != latitude)
                    continue;
                if (place.Add.Longitude != longitude)
                    continue;
                return place.LastTimeUpdate;
            }
            
            return null;
        }

        public class Place
        {
            public Address Add { get; set; }
            public DateTime LastTimeUpdate { get; set; }
            public OpenWeatherOneCall Weather { get; set; }
            public NotificationPlace Notification { get; set; } = new NotificationPlace();
        }

        public class UnitSettings
        {
            public TemperatureType Temperature { get; set; } = TemperatureType.Metric;
            public WindSpeedType WindSpeed { get; set; } = WindSpeedType.Meters;
            public enum TemperatureType
            {
                Metric = 0,
                Imperial
            }
            public enum WindSpeedType
            {
                Meters = 0,
                Kilometers
            }
        }

        public TimeSpan NotificationTime { get; private set; } = new TimeSpan(0, 8, 0, 0);

        private void LoadNotificationTime()
        {
            try
            {
                NotificationTime = JsonConvert.DeserializeObject<TimeSpan>(Preferences.Get(NOTIFICATION_TIME_KEY, ""));
            }
            catch (Exception)
            {
                NotificationTime = new TimeSpan(0, 8, 0, 0);
            }
        }
        public void SetNotificationTime(TimeSpan time)
        {
            NotificationTime = time;
            var json = JsonConvert.SerializeObject(NotificationTime);
            Preferences.Set(NOTIFICATION_TIME_KEY, json);

            SetIsSynced(false);
        }

        public bool IsSynced { get; private set; } = false;

        private void LoadIsSynced()
        {
            try
            {
                IsSynced = JsonConvert.DeserializeObject<bool>(Preferences.Get(IS_SYNCED_KEY, ""));
            }
            catch (Exception)
            {
                IsSynced = false;
            }
        }

        public async void SetIsSynced(bool synced)
        {
            IsSynced = synced;
            var json = JsonConvert.SerializeObject(IsSynced);
            Preferences.Set(IS_SYNCED_KEY, json);
            
            if (synced == false && NotificationSet)
            {
                if (await SyncData.SyncSettings(Places, NotificationTime))
                    SetIsSynced(true);
            }
        }
    }
}
