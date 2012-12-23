using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using Microsoft.Live;
using Microsoft.Live.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ReceiptStorage.DataModel.Enums;
using ReceiptStorage.Model;
using ReceiptStorage.Utilities;

namespace ReceiptStorage.View
{
    public partial class Settings : PhoneApplicationPage
    {
        // a good way to communicate what application doing with SkyDrive is the ProgressIndicator
        ProgressIndicator _progressIndicator;

        private LiveConnectClient client;
        private string skyDriveFolderID = string.Empty;
        private IsolatedStorageFileStream readStream;


        public Settings()
        {
            InitializeComponent();

            _progressIndicator = new ProgressIndicator();
            _progressIndicator.IsVisible = false;
            _progressIndicator.IsIndeterminate = true;
            SystemTray.SetProgressIndicator(this, _progressIndicator);

            btnSignin.ClientId =SettingsApi.SkyDriveClientId;
        }


        private void Close_Click(object sender, EventArgs e)
        {
            // Return to the main page.
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void btBackup_Click(object sender, RoutedEventArgs e)
        {
            if (client == null || client.Session == null)
            {
                MessageBox.Show("Musisz być zalogowany.");
            }
            else
            {
                if (MessageBox.Show("Czy na pewno chcesz utworzyć kopię zapasową? Spowoduje to zastąpienie starego pliku kopii zapasowej!", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    UploadFile();
                }
            }

        }

        #region SignIn
        private void SignInButton_SessionChanged(object sender, LiveConnectSessionChangedEventArgs e)
        {
            if (e.Error == null)
            {
                if (e.Status == LiveConnectSessionStatus.Connected)
                {
                    client = new LiveConnectClient(e.Session);
                    App.Session = e.Session;
                    this.txtLoginResult.Text = "Zalogowany.";
                    client.GetCompleted += new EventHandler<LiveOperationCompletedEventArgs>(OnGetCompleted);
                    client.GetAsync("me");

                }
                else
                {
                    App.Session = null;
                    client = null;
                    btBackup.IsEnabled = false;
                }
            }
            else
            {
                MessageBox.Show("SkyDrive problem: " + e.Error.Message);
            }
        }

        void OnGetCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if (e.Result.ContainsKey("first_name") && e.Result.ContainsKey("last_name"))
                {
                    if (e.Result["first_name"] != null && e.Result["last_name"] != null)
                    {
                        this.tblLoginText.Text = "Witaj, " + e.Result["first_name"].ToString() + " " + e.Result["last_name"].ToString() + "!";
                    }
                }
                else
                {
                    tblLoginText.Text = "Witaj, signed-in user!";
                }

                SetUpSkyDriveFolder();
                btBackup.IsEnabled = true;
            }
            else
            {
                tblLoginText.Text = "Error calling API: " + e.Error.ToString();
            }



        }
        #endregion

        #region SetUpSkyDriveFolder
        private void SetUpSkyDriveFolder()
        {
            if (string.IsNullOrEmpty(skyDriveFolderID))
            {
                try
                {
                    LiveConnectClient clientGetFolderList = new LiveConnectClient(App.Session);
                    clientGetFolderList.GetCompleted += new EventHandler<LiveOperationCompletedEventArgs>(ClientGetFolderList_GetCompleted);
                    _progressIndicator.Text = "Ładowanie folderu...";
                    _progressIndicator.IsVisible = false;
                    clientGetFolderList.GetAsync("me/skydrive/files");
                }
                catch (Exception eSetup)
                {
                    Debug.WriteLine("eSetup error: " + eSetup.Message);
                }
            }
            else
            {
                //tblInfo.Text = "Gotowy do baskup'u.";
                //tblDate.Text = "";
                btBackup.IsEnabled = true;

                //get the file ID's if they exist
                //client = new LiveConnectClient(App.Session);
                //client.GetCompleted += new EventHandler<LiveOperationCompletedEventArgs>(GetFiles_GetCompleted);
                //client.GetAsync(skyDriveFolderID + "/files"); //check through the files in the folder
            }

        }

        void ClientGetFolderList_GetCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null)
                {
                    bool dataFolderExists = false;
                    List<object> data = (List<object>)e.Result["data"];
                    foreach (IDictionary<string, object> dictionary in data)
                    {
                        if (string.IsNullOrEmpty(skyDriveFolderID))
                        {
                            if (dictionary.ContainsKey("name") && (string)dictionary["name"] == SettingsApi.SkyDriveFolderName &&
                                dictionary.ContainsKey("type") && (string)dictionary["type"] == "folder")
                            {
                                if (dictionary.ContainsKey("id"))
                                {
                                    skyDriveFolderID = (string)dictionary["id"];
                                    dataFolderExists = true;
                                    btBackup.IsEnabled = true;
                                    //tblInfo.Text = "Gotowy do baskup'u.";
                                    //tblDate.Text = "";
                                    //btBackup.IsEnabled = true;

                                    ////get the file ID's if they exist
                                    //client = new LiveConnectClient(App.Session);
                                    //client.GetCompleted += new EventHandler<LiveOperationCompletedEventArgs>(GetFiles_GetCompleted);
                                    //client.GetAsync(skyDriveFolderID + "/files"); //check through the files in the folder
                                    //break;
                                }
                            }
                        }
                    }

                    if (!dataFolderExists)
                    {
                        // create SkyDrive data folder
                        Dictionary<string, object> body = new Dictionary<string, object>();
                        body.Add("name", SettingsApi.SkyDriveFolderName);
                        object[] state = new object[2];
                        state[0] = "create folder";
                        state[1] = body["name"];

                        try
                        {
                            LiveConnectClient createFolderClient = new LiveConnectClient(App.Session);
                            createFolderClient.PostCompleted += new EventHandler<LiveOperationCompletedEventArgs>(CreateFolder_Completed);
                            _progressIndicator.IsVisible = true;
                            _progressIndicator.Text = "Tworzenie folderu...";
                            createFolderClient.PostAsync("/me/skydrive", body, state);
                        }
                        catch (Exception eCreateFolder)
                        {
                            _progressIndicator.IsVisible = false;
                            MessageBox.Show("Nie można utworzyć folderu na SkyDrive: " + eCreateFolder.Message);
                        }
                    }

                }
                else
                {
                    _progressIndicator.IsVisible = false;
                    MessageBox.Show("Problem z dostępem do SkyDrive: " + e.Error.Message);
                }
            }
            catch (Exception eFolder)
            {
                Debug.WriteLine("Błąd konfiguracji folderu: " + eFolder.Message);
            }

        }

        //finished creating the IsolatedStorageData folder
        private void CreateFolder_Completed(object sender, LiveOperationCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                tblInfo.Text = "Gotowy do wykonania kopi zapasowej.";
                //tblDate.Text = "Brak poprzedniej kopii zapasowej.";
                Dictionary<string, object> folder = (Dictionary<string, object>)e.Result;
                skyDriveFolderID = folder["id"].ToString(); //grab the folder ID
                btBackup.IsEnabled = true;
                _progressIndicator.IsVisible = false;
            }
            else
            {
                _progressIndicator.IsVisible = false;
                MessageBox.Show(e.Error.Message);
            }
        }
        #endregion

        #region Upload to SkyDrive
        public void UploadFile()
        {
            if (skyDriveFolderID != string.Empty) //the folder must exist, it should have already been created
            {
                this.client.UploadCompleted
                    += new EventHandler<LiveOperationCompletedEventArgs>(ISFile_UploadCompleted);

                _progressIndicator.IsVisible = true;
                _progressIndicator.Text = "Przesyłanie kopii zapasowej...";
                // tblDate.Text = "";

                try
                {
                    int count = 0;
                    var fotoToSync = App.ViewModel.GetAllFotoToSync();

                    foreach (Foto ft in fotoToSync)
                    {
                        count += 1;
                        _progressIndicator.Text = "Przesyłanie pliku " + count + " z " + fotoToSync.Count;
                        object[] state = new object[1];
                        state[0] = ft.FotoId;


                        using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                        {
                            using (IsolatedStorageFileStream stream = myIsolatedStorage.OpenFile(ft.FotoPath, System.IO.FileMode.Create))
                            {
                                stream.Write(ft.FotoImage, 0, ft.FotoImage.Length);
                            }

                            readStream = myIsolatedStorage.OpenFile(ft.FotoPath, FileMode.Open);
                            client.UploadAsync(skyDriveFolderID, ft.FotoPath, true, readStream, ft.FotoId);
                        }
                    }

                    if (!fotoToSync.Any())
                    {
                        tblInfo.Text = "Brak plików do przesłania.";
                    }
                }

                catch
                {
                    _progressIndicator.IsVisible = false;
                    MessageBox.Show("Błąd dostępu IsolatedStorage. Proszę zamknąć aplikację i otworzyć ją ponownie, a następnie spróbuj ponownie stworzenie kopii zapasowyej!", "Backup Failed", MessageBoxButton.OK);
                    tblInfo.Text = "Błąd. Zamknij aplikację i uruchom ponownie.";
                    //tblDate.Text = "";
                }
                finally
                {
                    _progressIndicator.IsVisible = false;
                }
            }
        }



        private void ISFile_UploadCompleted(object sender, LiveOperationCompletedEventArgs args)
        {
            if (args.Error == null)
            {
                _progressIndicator.IsVisible = false;
                //tblInfo.Text = args.ToString();
                readStream.Dispose(); //stop using the readStream so that the user can click Backup again without it crashing
                App.ViewModel.UpdateFotoSyncStatus(Int32.Parse(args.UserState.ToString()), StatusEnum.Yes);
                //tblInfo.Text = "Sprawdzanie nowej kopii zapasowej...";
                ////get the newly created fileID's (it will update the time too, and enable restoring)
                //client = new LiveConnectClient(App.Session);
                //client.GetCompleted += new EventHandler<LiveOperationCompletedEventArgs>(GetFiles_GetCompleted);
                //client.GetAsync(skyDriveFolderID + "/files");
            }
            else
            {
                _progressIndicator.IsVisible = false;
                this.tblInfo.Text = "Błąd przesyłania plików: " + args.Error.ToString();
                App.ViewModel.UpdateFotoSyncStatus(Int32.Parse(args.UserState.ToString()), StatusEnum.No);
            }
        }
        #endregion

    }
}