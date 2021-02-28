using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using WeatherApp.Annotations;
using WeatherApp.ViewModels;
using Xamarin.Forms;

namespace WeatherApp.Models
{
    public class FavoritePlace : INotifyPropertyChanged
    {
        public FavoritePlace()
        {
            ClickCommand = new Command(
                execute: () =>
                {
                    if (Place is null)
                        return;
                    WeatherViewModel.Instance.SetLocation(Place.PlaceName, Place.Latitude, Place.Longitude);
                },
                canExecute: () => true);
            LongClickCommand = new Command(
                execute: async () =>
                {
                    if (Place is null)
                        return;
                    var answer = await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Usuwanie", $"Jesteś pewien, że chcesz usunąć {Place.PlaceName} z ulubionych?", "Tak", "Nie");
                    if (!answer) return;
                    
                    Settings.Settings.Instance.RemovePlace(Place);
                    WeatherViewModel.Instance.RefreshFavoritePlaces();
                },
                canExecute: () => true);
        }
        
        private string _positionText;

        public string PositionText
        {
            get => _positionText;
            set
            {
                if (value == _positionText)
                    return;
                _positionText = value;
                OnPropertyChanged();
            }
        }

        public ICommand ClickCommand { private set; get; }
        public ICommand LongClickCommand { private set; get; }
        public Address Place { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
