using CustomSettingsDLL;
using Extensions;
using Framework.Security;
using Framework.Service;
using Framework.Template;
using Framework.Update.UpdateHistory;
using Modules.Activities.Classes;
using Modules.Notes.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Diagnostics;
using Windows.Storage;

namespace AppUpdater
{
    /// <summary>
    /// Background task for update data after app is updated
    /// </summary>
    public sealed class AppUpdater : IBackgroundTask
    {
        BackgroundTaskDeferral deferral;

        /// <summary>
        /// Vustom history list
        /// </summary>
        List<UpdateHistoryItem<UpdateHistoryKeys>> updateHistoryList = new List<UpdateHistoryItem<UpdateHistoryKeys>>()
        {
            new UpdateHistoryItem<UpdateHistoryKeys>() { Key = UpdateHistoryKeys.Decryption, UpdateVersion = new Version(1, 2, 46)},
            new UpdateHistoryItem<UpdateHistoryKeys>() { Key = UpdateHistoryKeys.FileRename, UpdateVersion = new Version(1, 3, 17)},
            new UpdateHistoryItem<UpdateHistoryKeys>() { Key = UpdateHistoryKeys.ItemStorageChange, UpdateVersion = new Version(1, 3, 30)}
        };

        /// <summary>
        /// Default function for background task
        /// </summary>
        /// <param name="taskInstance"></param>
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            deferral = taskInstance.GetDeferral();
            
            UpdateHistory<UpdateHistoryKeys> upHis = new UpdateHistory<UpdateHistoryKeys>(updateHistoryList);

            foreach (var item in await upHis.GetUpdateHistoryListAsync())
            {
                switch (item.Key)
                {
                    case UpdateHistoryKeys.Decryption:
                        if (!await upHis.CheckKeyAsync(item.Key.ToString()))
                            await ItemsEncryption();
                        break;

                    case UpdateHistoryKeys.FileRename:
                        if (!await upHis.CheckKeyAsync(item.Key.ToString()))
                            await FilesRename();
                        break;

                    case UpdateHistoryKeys.ItemStorageChange:
                        if (!await upHis.CheckKeyAsync(item.Key.ToString()))
                            await ItemStorageChangeAsync();
                        break;

                    default:
                        break;
                }
            }

            deferral.Complete();
        }

        /// <summary>
        /// Update task: Change structure of stored items
        /// Verion: 1.0
        /// </summary>
        /// <returns></returns>
        private static async Task ItemStorageChangeAsync()
        {
            var files = await ApplicationData.Current.LocalFolder.GetFilesAsync();

            bool currentLogStatus = CustomSettings.IsUserLogged;
            CustomSettings.IsUserLogged = true;

            foreach (var file in files.Where(x => x.FileType == ".tdn"))
            {
                switch (file.Name.Replace(".tdn", "").ToLower())
                {
                    case "notes":
                        {
                            var data = await OpenAndReadFileAsync<Note>(file);

                            NotesManager noteMan = new NotesManager();

                            await noteMan.AddItemRange(data.ToList(), CheckItems: false);

                            await noteMan.SaveChangesAsync();
                        }                        
                        break;

                    case "activities":
                        {
                            var data = OpenAndReadFileAsync<Activity>(file).Result;
                            
                            ActivitiesManager actiMan = new ActivitiesManager();

                            await actiMan.AddItemRange(data.ToList(), CheckItems: false);

                            await actiMan.SaveChangesAsync();
                        }
                        break;

                    default:
                        break;
                }
            }

            CustomSettings.IsUserLogged = currentLogStatus;
        }

        /// <summary>
        /// Update task: Files rename
        /// Verion: 1.0
        /// </summary>
        /// <returns></returns>
        private static async Task FilesRename()
        {
            var files = await ApplicationData.Current.LocalFolder.GetFilesAsync();

            foreach (var file in files.Where(x => x.FileType == ".xml" && x.Name != "UpdateHistory.xml"))
            {
                await file.RenameAsync(file.DisplayName + ".tdn");
            }
        }

        /// <summary>
        /// Update task: Item encryption
        /// Version: 1.0
        /// </summary>
        /// <returns></returns>
        private static async Task ItemsEncryption()
        {
            bool currentLogStatus = CustomSettings.IsUserLogged;
            CustomSettings.IsUserLogged = true;
            
            var actiCollection = await OpenAndReadFileAsync<Activity>(await ApplicationData.Current.LocalFolder.GetFileAsync("Activities.xml"));

            foreach (var item in actiCollection)
            {
                if (!IsEncrypted(item))
                    item.Encrypted = false;
            }

            await SaveFileAsync(await ApplicationData.Current.LocalFolder.GetFileAsync("Activities.xml"), actiCollection);
            
            var noCollection = await OpenAndReadFileAsync<Activity>(await ApplicationData.Current.LocalFolder.GetFileAsync("Notes.xml"));

            foreach (var item in noCollection)
            {
                if (!IsEncrypted(item) && item.Secured)
                    item.Encrypted = false;
            }

            await SaveFileAsync(await ApplicationData.Current.LocalFolder.GetFileAsync("Notes.xml"), actiCollection);

            CustomSettings.IsUserLogged = currentLogStatus;
        }

        /// <summary>
        /// Check encryption of item
        /// </summary>
        /// <param name="item">Checked item</param>
        /// <returns>True, if item is encrypted</returns>
        private static bool IsEncrypted(BaseItem item)
        {
            return IsEncrypted(item.Name) || IsEncrypted(item.Description);
        }

        /// <summary>
        /// Check encryption of string
        /// </summary>
        /// <param name="str">Checked string</param>
        /// <returns>True, if string is encrypted</returns>
        private static bool IsEncrypted(string str)
        {
            List<int> ordValues = new List<int>();

            if (str == null)
                return false;

            foreach (char c in str)
            {
                ordValues.Add(c);
            }

            if (ordValues.Count == 0)
                return false;

            int range = ordValues.Max() - ordValues.Min();

            if (range <= 10)
            {
                if (!ordValues.Contains(' ') || ordValues.Where(x => (x >= 'A' && x <= 'Z') || (x >= 'a' && x <= 'z')).ToList().Count == 0)
                {
                    if (ordValues.Where(x => x >= 170 && x <= 750).ToList().Count != 0)
                    {
                        if (ordValues[0] == (ordValues[ordValues.Count - 1] - 10))
                        {
                            return true;
                        }
                        else
                            return false;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// Old read function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file"></param>
        /// <param name="attempts"></param>
        /// <returns></returns>
        private static async Task<ObservableCollection<T>> OpenAndReadFileAsync<T>(StorageFile file, int attempts = 0) where T : BaseItem
        {
            object readedObjects = null;

            try
            {
                XmlSerializer Serializ = new XmlSerializer(typeof(ObservableCollection<T>));
                Stream XmlStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(file.Name);

                using (XmlStream)
                {
                    readedObjects = (ObservableCollection<T>)Serializ.Deserialize(XmlStream);
                }

                XmlStream.Dispose();

                if (readedObjects != null)
                {
                    return new ObservableCollection<T>(((ObservableCollection<T>)readedObjects));
                }
                else
                    return new ObservableCollection<T>();
            }

            // When is file unavailable - 10 attempts is enough
            catch (Exception e) when ((e.Message.Contains("denied")) && (attempts < 10))
            {
                return await OpenAndReadFileAsync<T>(file, attempts + 1);
            }

            catch /*(Exception e)*/
            {
                return new ObservableCollection<T>();
            }
        }

        /// <summary>
        /// Old save function
        /// </summary>
        /// <param name="Attempts"></param>
        /// <returns></returns>
        private static async Task<bool> SaveFileAsync<T>(StorageFile file, ObservableCollection<T> DeliveredData, int Attempts = 0) where T : BaseItem
        {
            try
            {
                foreach (var item in DeliveredData.Where(x => x.Secured && !x.Encrypted))
                {
                    item.Name = Crypting.Encrypt(item.Name);
                    item.Description = Crypting.Encrypt(item.Description);
                    item.Encrypted = true;
                }

                XmlSerializer Serializ = new XmlSerializer(typeof(ObservableCollection<T>));

                using (Stream XmlStream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(file.Name, CreationCollisionOption.ReplaceExisting))
                {
                    Serializ.Serialize(XmlStream, DeliveredData);
                }

                return true;
            }

            // When file is unavailable
            catch (Exception e) when ((e.Message.Contains("denied") || e.Message.Contains("is in use")) && (Attempts < 10))
            {
                return await SaveFileAsync(file, DeliveredData, Attempts + 1);
            }

            catch /*(Exception e)*/
            {
                return false;
            }
        }
    }
}
