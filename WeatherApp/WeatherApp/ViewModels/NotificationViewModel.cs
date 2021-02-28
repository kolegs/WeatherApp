using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using WeatherApp.Models;
using WeatherApp.Resources;
using Xamarin.Forms;

namespace WeatherApp.ViewModels
{
    public class NotificationViewModel : BaseViewModel
    {
        public NotificationViewModel()
        {
            Refresh();
        }

        public void Refresh()
        {
            Items.Clear();
            foreach (var place in Settings.Settings.Instance.Places)
            {
                var group = new NotificationGroups();
                group.PlaceName = place.Add.PlaceName;
                {
                    var item = new NotificationItem();
                    item.Latitude = place.Add.Latitude;
                    item.Longitude = place.Add.Longitude;
                    item.NotificationName = AppTranslations.NotificationStorm;
                    item.IsEnabled = place.Notification.Storm;
                    item.NotificationType = NotificationPlace.NotificationType.Storm;
                    group.Add(item);
                }
                {
                    var item = new NotificationItem();
                    item.Latitude = place.Add.Latitude;
                    item.Longitude = place.Add.Longitude;
                    item.NotificationName = AppTranslations.NotificationRain;
                    item.IsEnabled = place.Notification.Rain;
                    item.NotificationType = NotificationPlace.NotificationType.Rain;
                    group.Add(item);
                }
                {
                    var item = new NotificationItem();
                    item.Latitude = place.Add.Latitude;
                    item.Longitude = place.Add.Longitude;
                    item.NotificationName = AppTranslations.NotificationSnow;
                    item.IsEnabled = place.Notification.Snow;
                    item.NotificationType = NotificationPlace.NotificationType.Snow;
                    group.Add(item);
                }
                {
                    var item = new NotificationItem();
                    item.Latitude = place.Add.Latitude;
                    item.Longitude = place.Add.Longitude;
                    item.NotificationName = AppTranslations.NotificationFrost;
                    item.IsEnabled = place.Notification.Frost;
                    item.NotificationType = NotificationPlace.NotificationType.Frost;
                    group.Add(item);
                }
                Items.Add(group);
            }
            DateText = DateTime.Now.ToString("d");
            Time = Settings.Settings.Instance.NotificationTime;
        }
        
        private ObservableCollection<NotificationGroups> _items = new ObservableCollection<NotificationGroups>();

        public ObservableCollection<NotificationGroups> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        private TimeSpan _time;

        public TimeSpan Time
        {
            get => _time;
            set
            {
                SetProperty(ref _time, value);
                Settings.Settings.Instance.SetNotificationTime(Time);
            }
        }

        private string _dateText;
        public string DateText
        {
            get => _dateText;
            set => SetProperty(ref _dateText, value);
        }

    }
}
