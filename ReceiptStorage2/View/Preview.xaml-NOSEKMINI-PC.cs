using System;
using System.Windows;
using Microsoft.Phone.Controls;
using ReceiptStorage.DataModel.Simplified;
using ReceiptStorage.Model;

namespace ReceiptStorage.View
{
    public partial class Preview : PhoneApplicationPage
    {
        public Preview()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string receiptId;

            if (NavigationContext.QueryString.TryGetValue("id", out receiptId))
            {
                var receipt = App.ViewModel.GetReceipt(Convert.ToInt32(receiptId));
                ReceiptDetail.DataContext = receipt;
            }
        }

        private void Close_Click(object sender, EventArgs e)
        {
            // Return to the main page.
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void Remove_Click(object sender, EventArgs e)
        {
            MessageBoxResult msResult = MessageBox.Show("Czy napewno chcesz usunąć dany paragon?","",MessageBoxButton.OKCancel);
            if (msResult == MessageBoxResult.OK)
            {
                var _receiptDetail = (ReceiptSimplified) ReceiptDetail.DataContext;

                App.ViewModel.DeleteReceiptItem(_receiptDetail.ReceiptId);

                // Return to the main page.
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
            }
        }
    }
}