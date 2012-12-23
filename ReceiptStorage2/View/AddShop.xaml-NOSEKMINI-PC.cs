using System;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ReceiptStorage.DataModel.Enums;
using ReceiptStorage.Extensions;
using ReceiptStorage.Model;

namespace ReceiptStorage.View
{
    public partial class AddShop : PhoneApplicationPage
    {
        private PlaceHelper place;
        public AddShop()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
            lpkCountry.ItemsSource = Enum<Country>.GetNames();


        }

        private void Close_Click(object sender, EventArgs e)
        {
            // Return to the main page.
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }


        private void Add_Click(object sender, EventArgs e)
        {
            if (tbName.Text.Length > 0)
            {
               
                // Create a new to-do item.
                Shops newShopsItem = new Shops
                {
                    ShopName = tbName.Text,
                    ShopAdres = tbAdress.Text,
                    ShopCity = tbCity.Text,
                    ShopCountry = (Country)Enum.Parse(typeof(Country), lpkCountry.SelectedItem.ToString(),true) ,
                    ShopGpsLocalization = (place.position.ToString() ?? String.Empty)
                };
                
                // Add the item to the ViewModel.
                App.ViewModel.AddShopsItem(newShopsItem);

                // Return to the main page.
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();

                }
            }else
            {
                MessageBox.Show("Proszę wpisać nazwę sklepu!");
            }
        }

        private void Location_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/Location.xaml", UriKind.Relative));
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey("place"))
            {

                place = (PlaceHelper) PhoneApplicationService.Current.State["place"];
                tbName.Text = place.title;
                tbAdress.Text = place.vicinity.Split('\n')[0];
                tbCity.Text = place.vicinity.Split('\n')[1];
                
            }

        }

    }
}