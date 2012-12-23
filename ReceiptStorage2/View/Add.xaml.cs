using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
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
        private IList<string> _ocrResultList = new List<string>();
        ProgressIndicator _progressIndicator;

        public Add()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
            App.ViewModel.LoadShopsFromDatabase();

            if(App.ViewModel.AllShopsItems.Count == 0 )
            {
                MessageBox.Show("Zanim wprowadzisz paragon należy dodać sklep do bazy.");
            }

            lpkOperationType.ItemsSource = Enum<OperationType>.GetNames();
            lpkShopsCategory.ItemsSource = Enum<ShopsCategory>.GetNames();
            //Create new instance of CameraCaptureClass
            _cameraCapture = new CameraCaptureTask();
            //Create new event handler for capturing a photo
            _cameraCapture.Completed += new EventHandler<PhotoResult>(CameraCapture_Completed );
            //Progres Indicator
            _progressIndicator = new ProgressIndicator();
            _progressIndicator.IsVisible = false;
            _progressIndicator.IsIndeterminate = true;
            SystemTray.SetProgressIndicator(this, _progressIndicator);

        }

        private void Add_Click(object sender, EventArgs e)
        {
            MessageBoxResult msResult = MessageBoxResult.OK;
            if( App.ViewModel.AllShopsItems.Count == 0)
            {
                MessageBox.Show("Proszę dodać sklep do bazy danych.");
                return;
            }

            if (tbKwota.Text.Length == 0 && lpkOcrResult.Visibility == Visibility.Collapsed)
            {
                MessageBox.Show("Proszę wpisać kwotę zakupów.");
                return;
            }

            if (tbKwota.Text.Length == 0 && (lpkOcrResult.Visibility != Visibility.Collapsed ? lpkOcrResult.SelectedItem.ToString().Length == 0:false))
            {
                MessageBox.Show("Proszę wpisać/wybrać kwotę zakupów.");
                return;
            }


            if (double.Parse((lpkOcrResult.Visibility != Visibility.Collapsed ? lpkOcrResult.SelectedItem.ToString() : tbKwota.Text), CultureInfo.InvariantCulture) > 10000)
            {
                MessageBox.Show("Kwota zakupów nie może być siększa niż 9999 zł.");
                return;
            }
            
                
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
                var fotoName = "RC_" + String.Format("{0:yyyy_MM_dd_HH_mm_ss_FFFFFFF}", DateTime.Now) + ".jpg";


                Foto newFoto = new Foto
                                    {
                                        FotoPath = fotoName,
                                        FotoImage = pictureByte,
                                        FotoSyncStatus = StatusEnum.No
                                    };

                Receipt newReceiptItem = new Receipt
                                                {
                                                    ReceiptMoney = double.Parse((lpkOcrResult.Visibility != Visibility.Collapsed ? lpkOcrResult.SelectedItem.ToString() : tbKwota.Text), CultureInfo.InvariantCulture),
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
            progressBar.Visibility = Visibility.Visible;
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
                //Test - OCR
                var pictureByte = ImageConverter.ConvertToBytes("ExampleData/Foto/mayaReceipt.jpg");
                PhoneApplicationService.Current.State["ActivePicture"] = pictureByte;
                ReceiptImage.Source = ImageConverter.ConvertToImage(pictureByte);
                this.StartOcrConversion(pictureByte);

                //progressBar.Visibility = Visibility.Collapsed;

                //byte[] data;
                //using (var br = new BinaryReader(e.ChosenPhoto))
                //{
                //    data = br.ReadBytes((Int32)e.ChosenPhoto.Length);
                //}
                //PhoneApplicationService.Current.State["ActivePicture"] = data;

                //ReceiptImage.Source = ImageConverter.ConvertToImage(data);

                //this.StartOcrConversion(data); // launch OCR service
            }
        }



        private void StartOcrConversion(byte[] photoStream)
        {
            _progressIndicator.IsVisible = true;
            _progressIndicator.Text = "Trwa konwersja OCR...";
            
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
            _progressIndicator.IsVisible = true;
            _progressIndicator.Text = "Trwa przeszukiwanie kwot...";
            _ocrResultList.Clear();
            if (result.Status == Status.Success)
            {
                //int wordCount = 0;

                string pattern = @"[1-9][0-9]{0,2}(?:,?[0-9]{3}){0,3}\.[0-9]{2}";
                Regex r = new Regex(pattern);

                foreach (OcrText item in result.OcrResult.OcrTexts)
                {
                    //wordCount += item.Words.Count;

                     _ocrResultList.Add(String.Empty);
                    MatchCollection mc = r.Matches(item.Text);
                    foreach (var ocrText in mc)
                    {
                        _ocrResultList.Add(ocrText.ToString());
                    }
                }
                _progressIndicator.IsVisible = false;
                if (_ocrResultList.Count > 1)
                {
                    lpkOcrResult.Visibility = Visibility.Visible;
                    lpkOcrResult.ItemsSource = _ocrResultList;
                    
                }
                else
                {
                    lpkOcrResult.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                _progressIndicator.IsVisible = false;
                MessageBox.Show("Nie przetworzono obrazu - wystąpił błąd OCR", "", MessageBoxButton.OK);
            }
            
        }

        private void tbKwota_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (lpkOcrResult.Items.Count > 0)
            {
                lpkOcrResult.SelectedIndex = 0;
            }
        }

        private void lpkOcrResult_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            tbKwota.Text = String.Empty;
        }





    }
}