using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.FirebasePushNotification;
using Xamarin.Forms;

namespace WeatherApp.REST
{
    public static class SyncData
    {
        public static async Task<bool> SyncSettings(List<Settings.Settings.Place> places, TimeSpan notificationTime)
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                //iOS stuff
                return false;
            }
            try
            {
                CrossFirebasePushNotification.Current.RegisterForPushNotifications();
                var token = await CrossFirebasePushNotification.Current.GetTokenAsync();
                if (token == "")
                    return false;

                var sync = new SyncStruct();
                sync.Token = token;
                sync.SyncTime = notificationTime;
                foreach (var place in places)
                {
                    sync.Places.Add(new SyncPlace()
                    {
                        Latitude = place.Add.Latitude,
                        Longitude = place.Add.Longitude,
                        Frost = place.Notification.Frost,
                        Rain = place.Notification.Rain,
                        Snow = place.Notification.Snow,
                        Storm = place.Notification.Storm,
                    });
                }

                var culture = System.Globalization.CultureInfo.CurrentUICulture;
                var language = culture.Name.Split('-').FirstOrDefault();

                sync.Language = language;

                var json = JsonConvert.SerializeObject(sync);

                // FIXME: We should add some kind of authentication, maybe Bearer token or sth
                var address =
                    $"http://192.168.20.132:5000/register";
                var sContentType = "application/json";
                using (var client = new HttpClient())
                {
                    var response =
                        await client.PostAsync(address, new StringContent(json, Encoding.UTF8, sContentType));
                    if (response.IsSuccessStatusCode == false)
                        return false;
                    var responseString = await response.Content.ReadAsStringAsync();
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public class SyncStruct
        {
            [JsonProperty("token")]
            public string Token { get; set; }
            [JsonProperty("sync_time")]
            public TimeSpan SyncTime { get; set; }
            [JsonProperty("language")]
            public string Language { get; set; }

            [JsonProperty("places")] public List<SyncPlace> Places { get; set; } = new List<SyncPlace>();
        }

        public class SyncPlace
        {
            [JsonProperty("latitude")]
            public double Latitude { get; set; }
            [JsonProperty("longitude")]
            public double Longitude { get; set; }
            [JsonProperty("snow")]
            public bool Snow { get; set; }
            [JsonProperty("storm")]
            public bool Storm { get; set; }
            [JsonProperty("rain")]
            public bool Rain { get; set; }
            [JsonProperty("frost")]
            public bool Frost { get; set; }
        }
    }
}
