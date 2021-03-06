using Framework.Classes;
using Framework.Enum;
using Framework.Security;
using Framework.Storage;
using Framework.Template;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace Framework.Commands
{
    public class ShareItemsCommand<T> : Command where T : BaseItem
    {
        private Messenger messenger;

        public ShareItemsCommand(Messenger messenger)
        {
            this.messenger = messenger;
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        //Insp -> vlastni typ pripony
        public override void Execute(object parameter)
        {
            if (parameter is ListViewBase itemList)
            {
                if (itemList.Items.Count == 0)
                    return;

                string itemTypeName = itemList.Items[0].GetType().Name;
                var typeOfItemList = System.Enum.GetValues(typeof(ItemTypeEnum)).Cast<ItemTypeEnum>().ToList();

                var sharedData = new ItemStorage<T>
                {
                    TypeOfItem = typeOfItemList[typeOfItemList.IndexOf(typeOfItemList.FirstOrDefault(x => x.ToString() == itemTypeName))]
                };
                
                foreach (var item in new ObservableCollection<T>(itemList.SelectedItems.Cast<T>().Select(x => (T)x.Clone()).ToList()))
                {
                    T itemToShare = item;

                    if (itemToShare.Secured)
                    {
                        itemToShare.Name = Crypting.Encrypt(itemToShare.Name);
                        itemToShare.Description = Crypting.Encrypt(itemToShare.Description);
                    }

                    sharedData.Items.Add(item);
                }

                try
                {
                    XmlSerializer Serializ = new XmlSerializer(typeof(ItemStorage<T>));

                    using (Stream XmlStream = ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync("Share.tdn", CreationCollisionOption.ReplaceExisting).GetAwaiter().GetResult())
                    {
                        Serializ.Serialize(XmlStream, sharedData);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }

                DataTransferManager.ShowShareUI();
            }
        }
    }
}
