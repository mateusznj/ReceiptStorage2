using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using Hawaii.Services.Client;
using Hawaii.Services.Client.Ocr;
using Microsoft.Phone;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using ReceiptStorage.Extensions;
using ReceiptStorage.Model;
using ReceiptStorage.DataModel.Enums;
using ReceiptStorage.Utilities;

namespace ReceiptStorage.View
{
    public partial class Add : PhoneApplicationPage
    {
        CameraCaptureTask _cameraCapture;

        public Add()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
            App.ViewModel.LoadShopsFromDatabase();
            lpkOperationType.ItemsSource = Enum<OperationType>.GetNames();
            lpkShopsCategory.ItemsSource = Enum<ShopsCategory>.GetNames();
            //Create new instance of CameraCaptureClass
            _cameraCapture = new CameraCaptureTask();
            //Create new event handler for capturing a photo
            _cameraCapture.Completed += new EventHandler<PhotoResult>(CameraCapture_Completed );
            //Foto_Click.Click += new RoutedEventHandler(Photo_Click);
        }

        private void Add_Click(object sender, EventArgs e)
        {
            MessageBoxResult msResult = MessageBoxResult.OK;
            if (tbKwota.Text.Length > 0)
            {
                var shop = lpkShop.SelectedItem;
                var operaitonType = (OperationType)Enum.Parse(typeof (OperationType), lpkOperationType.SelectedItem.ToString(), true);
                var shopsCategory =(ShopsCategory)Enum.Parse(typeof (ShopsCategory), lpkShopsCategory.SelectedItem.ToString(), true);

                byte[] pictureByte = new byte[] {};
                if (PhoneApplicationService.Current.State.ContainsKey("ActivePicture"))
                {
                    pictureByte = (byte[]) PhoneApplicationService.Current.State["ActivePicture"];
                    PhoneApplicationService.Current.State.Remove("ActivePicture");
                }
                else
                {
                    msResult = MessageBox.Show("Czy napewno chcesz doać informację o paragonie bez jego zdjęcia?", "",MessageBoxButton.OKCancel);
                    if (msResult == MessageBoxResult.OK)
                    {
                        pictureByte = ImageConverter.ConvertToBytes("Background.png");
                    }
                }

                if (msResult == MessageBoxResult.OK)
                {
                    var fotoName = ((Shops) (shop)).ShopName + String.Format("{0:_yyyy_MM_dd_HH_mm}", DateTime.Now);


                    Foto newFoto = new Foto
                                       {
                                           FotoPath = fotoName,
                                           FotoImage = pictureByte
                                       };

                    Receipt newReceiptItem = new Receipt
                                                 {
                                                     ReceiptMoney = double.Parse(tbKwota.Text, CultureInfo.InvariantCulture),
                                                     ReceiptCurrency = Currency.PLN,
                                                     ReceiptCreate = DateTime.Now,
                                                     ReceiptDate = (DateTime) dpDataParagonu.Value,
                                                     ReceiptOperationType = operaitonType,
                                                     ReceiptShopsCategory = shopsCategory,
                                                     ShopsId = (Shops) shop
                                                 };

                    newReceiptItem.ReceiptFoto.Add(newFoto);

                    // Add the item to the ViewModel.
                    App.ViewModel.AddReceiptItem(newReceiptItem);

                    // Return to the main page.
                    if (NavigationService.CanGoBack)
                    {
                        NavigationService.GoBack();
                    }
                }

            }else
            {
                MessageBox.Show("Proszę wpisać kwotę zakupów.");
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

        private void Foto_Click(object sender, EventArgs e)
        {
           
            //Show the camera.
            _cameraCapture.Show();

            //Set progress bar to visible to show time between user snapshot and decoding
            //of image into a writeable bitmap object.
            progressBar1.Visibility = Visibility.Visible;
        }

        private void btAddShop_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/AddShop.xaml", UriKind.Relative));
        }

        
        /// <summary>
        /// Event handler for retrieving the JPEG photo stream.
        /// Also to for decoding JPEG stream into a writeable bitmap and displaying.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CameraCapture_Completed(object sender, PhotoResult e)
        {

            if (e.TaskResult == TaskResult.OK && e.ChosenPhoto != null)
            {
                //Test
                var pictureByte = ImageConverter.ConvertToBytes("ExampleData/Foto/WP_20120616_1.jpg");
                
                progressBar1.Visibility = Visibility.Collapsed;

                byte[] data;
                using (var br = new BinaryReader(e.ChosenPhoto))
                {
                    data = br.ReadBytes((Int32)e.ChosenPhoto.Length);
                }
                //PhoneApplicationService.Current.State["ActivePicture"] = data;
                PhoneApplicationService.Current.State["ActivePicture"] = pictureByte;

                //ReceiptImage.Source = ImageConverter.ConvertToImage(data);
                ReceiptImage.Source = ImageConverter.ConvertToImage(pictureByte);
                
                this.StartOcrConversion(pictureByte); // launch OCR service
            }
        }



        private void StartOcrConversion(byte[] photoStream)
        {
            byte[] photoBuffer = photoStream;

            OcrService.RecognizeImageAsync(
                SettingsApi.HawaiiGuid,
                photoBuffer,
                (output) =>
                {
                    this.Dispatcher.BeginInvoke(() => OnOcrCompleted(output));
                });
        }



        private void OnOcrCompleted(OcrServiceResult result)
        {
            if (result.Status == Status.Success)
            {
                int wordCount = 0;
                StringBuilder sb = new StringBuilder();
                foreach (OcrText item in result.OcrResult.OcrTexts)
                {
                    wordCount += item.Words.Count;
                    sb.Append(item.Text);
                    sb.Append("\n");
                }

                if (wordCount > 0)
                {
                    MessageBox.Show(sb.ToString());
                }
            }
        }





    }
}