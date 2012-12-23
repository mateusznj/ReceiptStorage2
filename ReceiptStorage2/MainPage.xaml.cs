using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Coding4Fun.Phone.Controls;
using Microsoft.Live;
using Microsoft.Phone.Controls;
using ReceiptStorage.DataModel.Enums;
using ReceiptStorage.Extensions;
using ReceiptStorage.Model;
using ReceiptStorage.DataModel.Simplified;
using ReceiptStorage.Utilities;
using ReceiptStorage.View;

namespace ReceiptStorage
{
    public partial class MainPage : PhoneApplicationPage
    {
        private string skyDriveFolderID = string.Empty;
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
            
        }

        
        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Set the page DataContext property to the ViewModel.
            this.DataContext = App.ViewModel;

            //Dashboard
            lbReceipt.ItemsSource = App.ViewModel.GetAllReceiptWithShops();
            tbFilter.Visibility = Visibility.Collapsed;
            tbFilter.Text = String.Empty;
            //Wykres tygodnia
            receiptWeeksChart.DataSource = App.ViewModel.GetReceiptExpensesChartsPerWeek();
            //Wykres miesiąca
            receiptMonthChart.DataSource = App.ViewModel.GetReceiptExpensesChartsPerMonth();
            tblLastMonth.Text = "Wykres wydatków z miesiąca " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Month) + ":";
            //Wykres roku
            receiptYearChart.DataSource = App.ViewModel.GetReceiptExpensesChartsPerYear();
            tblLastYear.Text = "Wykres wydatków z roku " + DateTime.Now.Year + ":";
            
            ExpensesData(CalendarType.miesiąc);
            

        }

        private void Add_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/Add.xaml", UriKind.Relative));
            //ExampleData.AddToDatabase();
        }

        private void Filter_Click(object sender, EventArgs e)
        {
            if (tbFilter.Visibility == Visibility.Collapsed)
            {
                tbFilter.Visibility = Visibility.Visible;

            }
            else
            {
                tbFilter.Visibility = Visibility.Collapsed;
                tbFilter.Text = String.Empty;
                lbReceipt.ItemsSource = App.ViewModel.GetAllReceiptWithShops();
                
            }
        }

        private void Setting_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/Settings.xaml", UriKind.Relative));
        }

        private void Receipt_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var selectedItem = (sender as ListBox).SelectedItem as ReceiptSimplified;
            if (selectedItem != null)
            {
                int id = selectedItem.ReceiptId;

                NavigationService.Navigate(new Uri("/View/Preview.xaml?id=" + id, UriKind.RelativeOrAbsolute)); 

            }
        }

        private void ExpenseWeek_Click(object sender, EventArgs e)
        {
            ExpensesData(CalendarType.tydzień);
        }

        private void ExpenseMonth_Click(object sender, EventArgs e)
        {
            ExpensesData(CalendarType.miesiąc);
        }

        private void ExpenseYear_Click(object sender, EventArgs e)
        {
            ExpensesData(CalendarType.rok);
        }

        private void ExpensesData(CalendarType calType)
        {
            var expenses = App.ViewModel.GetReceiptExpensesPer(calType);
            tbExplensesSum.Text = expenses["ReceiptSum"].ToString() + " zł";//String.Format(" {0}",Currency.PLN);
            tbExpensesAvg.Text = expenses["ReceiptAvg"].ToString() + " zł";
            receiptShopsCategoryChart.DataSource = App.ViewModel.GetReceiptShopsCategory(calType);

            lbExpensesSum.Text = "Suma wydatków ("+ String.Format("{0}", calType) +"):";
            lbExpensesAvg.Text = "Średnia wydatków (" + String.Format("{0}", calType) + "):";
        }

        private void About_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/About.xaml", UriKind.Relative));
        }

        private void Search_ActionIconTapped(object sender, EventArgs e)
        {
            lbReceipt.ItemsSource = App.ViewModel.GetAllReceiptWithShops(tbFilter.Text);
        }

    }
}