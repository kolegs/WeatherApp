using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using WeatherApp.Models;
using WeatherApp.REST;
using Xamarin.Forms;

namespace WeatherApp.ViewModels
{
    public class SearchViewModel : BaseViewModel
    {
        public SearchViewModel(string searchText)
        {
            SearchCommand = new Command(
                execute: async (object address) =>
                {
                    IsLoading = true;
                    var search = await Nominatim.GetAddress(address as string);
                    Update(search);
                },
                canExecute: (object address) => true);

            BackCommand = new Command(
                execute: async () =>
                {
                    await Application.Current.MainPage.Navigation.PopModalAsync();
                },
                canExecute: () => true);

            SearchText = searchText;
        }
        
        public void Update(List<Nominatim.Addresses> addresses)
        {
            IsLoading = false;
            Addresses.Clear();
            foreach (var address in addresses)
            {
                var a = new Address()
                {
                    DisplayName = address.DisplayName,
                    Latitude = address.Latitude,
                    Longitude = address.Longitude,
                };
                if (!string.IsNullOrEmpty(address.Address.Administrative))
                    a.PlaceName = address.Address.Administrative;
                else if (!string.IsNullOrEmpty(address.Address.City))
                    a.PlaceName = address.Address.City;
                else if (!string.IsNullOrEmpty(address.Address.Village))
                    a.PlaceName = address.Address.Village;
                else if (!string.IsNullOrEmpty(address.Address.Country))
                    a.PlaceName = address.Address.Country;
                else
                    a.PlaceName = SearchText;
                Addresses.Add(a);
            }
        }

        private bool _isLoading = true;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private string _searchText;

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public ICommand BackCommand { private set; get; }
        public ICommand SearchCommand { private set; get; }
        

        private ObservableCollection<Address> _addresses = new ObservableCollection<Address>();
        public ObservableCollection<Address> Addresses
        {
            get => _addresses;
            set => SetProperty(ref _addresses, value);
        }

        private Address _addressSelected = null;

        public Address AddressSelected
        {
            get => _addressSelected;
            set
            {
                if (_addressSelected == value)
                    return;
                _addressSelected = value;
                OnPropertyChanged();
                ChangePlace();
            }
        }

        public async void ChangePlace()
        {
            Settings.Settings.Instance.AddPlace(AddressSelected);
            WeatherViewModel.Instance.SetLocation(AddressSelected.PlaceName, AddressSelected.Latitude, AddressSelected.Longitude);
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }
    }
}
