using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using Microsoft.Live;
using ReceiptStorage.DataModel.Enums;
using ReceiptStorage.Model;

namespace ReceiptStorage.Utilities
{
    public class SkyDrive
    {
        public static LiveConnectClient client;
        private static string skyDriveFolderID = string.Empty;
        private static IsolatedStorageFileStream readStream;

        #region SetUpSkyDriveFolder

        public static void SetUpSkyDriveFolder()
        {
            if (string.IsNullOrEmpty(skyDriveFolderID))
            {
                try
                {
                    LiveConnectClient clientGetFolderList = new LiveConnectClient(App.Session);
                    clientGetFolderList.GetCompleted += new EventHandler<LiveOperationCompletedEventArgs>(ClientGetFolderList_GetCompleted);
                    clientGetFolderList.GetAsync("me/skydrive/files");
                }
                catch (Exception eSetup)
                {
                    Debug.WriteLine("eSetup error: " + eSetup.Message);
                }
            }
        }

        static void ClientGetFolderList_GetCompleted(object sender, LiveOperationCompletedEventArgs e)
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
                            createFolderClient.PostAsync("/me/skydrive", body, state);
                        }
                        catch (Exception eCreateFolder)
                        {
                            MessageBox.Show("Nie można utworzyć folderu na SkyDrive: " + eCreateFolder.Message);
                        }
                    }

                }
                else
                {
                    MessageBox.Show("Problem z dostępem do SkyDrive: " + e.Error.Message);
                }
            }
            catch (Exception eFolder)
            {
                Debug.WriteLine("Błąd konfiguracji folderu: " + eFolder.Message);
            }

        }

        //finished creating the IsolatedStorageData folder
        private static void CreateFolder_Completed(object sender, LiveOperationCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                Dictionary<string, object> folder = (Dictionary<string, object>)e.Result;
                skyDriveFolderID = folder["id"].ToString(); //grab the folder ID
            }
            else
            {
                MessageBox.Show(e.Error.Message);
            }
        }
        #endregion

        #region Upload to SkyDrive
        public static void UploadFile()
        {
            if (skyDriveFolderID != string.Empty) //the folder must exist, it should have already been created
            {
                client.UploadCompleted
                    += new EventHandler<LiveOperationCompletedEventArgs>(ISFile_UploadCompleted);

                try
                {
                    
                    var fotoToSync = App.ViewModel.GetAllFotoToSync();

                    foreach (Foto ft in fotoToSync)
                    {
                        
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
                }

                catch
                {
                    MessageBox.Show("Błąd dostępu IsolatedStorage. Proszę zamknąć aplikację i otworzyć ją ponownie, a następnie spróbuj ponownie stworzenie kopii zapasowyej!", "Backup Failed", MessageBoxButton.OK);

                }
            }
        }



        private static void ISFile_UploadCompleted(object sender, LiveOperationCompletedEventArgs args)
        {
            if (args.Error == null)
            {
                readStream.Dispose(); //stop using the readStream so that the user can click Backup again without it crashing
                App.ViewModel.UpdateFotoSyncStatus(Int32.Parse(args.UserState.ToString()), StatusEnum.Yes);

            }
            else
            {
                App.ViewModel.UpdateFotoSyncStatus(Int32.Parse(args.UserState.ToString()), StatusEnum.No);
            }
        }
        #endregion
    }
}
