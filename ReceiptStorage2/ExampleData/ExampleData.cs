using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ReceiptStorage.DataModel.Enums;
using ReceiptStorage.Extensions;
using ReceiptStorage.ViewModel;
using ReceiptStorage.Model;

namespace ReceiptStorage.Model
{
    public class ExampleData
    {
        public static void AddToDatabase()
        {
           Foto newFoto = new Foto
                                 {
                                     FotoPath = "RC_2012_06_20_22_40_13_894.jpg",
                                     FotoImage = ImageConverter.ConvertToBytes("ExampleData/Foto/WP_20120614_5.jpg"),
                                     FotoSyncStatus = StatusEnum.No
                                 };

            Receipt newReceiptItem = new Receipt
                                            {
                                                ReceiptMoney = 14.90,
                                                ReceiptCurrency = Currency.PLN,
                                                ReceiptCreate = DateTime.Now,
                                                ReceiptDate = (DateTime)DateTime.Now.AddDays(-40),
                                                ReceiptOperationType = OperationType.gotówka,
                                                ReceiptShopsCategory = ShopsCategory.restauracja,
                                                ReceiptShopsId = 10
                                            };

            newReceiptItem.ReceiptFoto.Add(newFoto);

            // Add the item to the ViewModel.
            App.ViewModel.AddReceiptItem(newReceiptItem);

            newFoto = new Foto
            {
                FotoPath = "RC_2012_06_14_22_40_13_894.jpg",
                FotoImage = ImageConverter.ConvertToBytes("ExampleData/Foto/WP_20120614_4.jpg"),
                FotoSyncStatus = StatusEnum.No
            };

            newReceiptItem = new Receipt
            {
                ReceiptMoney = 14.90,
                ReceiptCurrency = Currency.PLN,
                ReceiptCreate = DateTime.Now,
                ReceiptDate = (DateTime)DateTime.Now.AddDays(-50),
                ReceiptOperationType = OperationType.gotówka,
                ReceiptShopsCategory = ShopsCategory.restauracja,
                ReceiptShopsId = 6
            };

            newReceiptItem.ReceiptFoto.Add(newFoto);

            // Add the item to the ViewModel.
            App.ViewModel.AddReceiptItem(newReceiptItem);
        }
    }
}
