using CustomSettingsDLL;
using Framework.Classes;
using Framework.Controls;
using Framework.Enum;
using Framework.Messages;
using Framework.Security;
using Framework.Template;
using Framework.ViewModel;
using Modules.Activities.Classes;
using Modules.Notes.Classes;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TheDailyNotes.Modules.Activities.Components;
using TheDailyNotes.Modules.Notes.Components;
using Windows.Storage;
using Windows.UI.StartScreen;
using Windows.UI.Xaml.Controls;

namespace TheDailyNotes.Classes
{
    public class TDNVMBase<T> : ModuleVMBase<T> where T : BaseItem
    {
        public TDNVMBase() : base()
        {

        }

        public TDNVMBase(Messenger messenger, SecondaryTile secondaryTile, ItemTypeEnum itemType) : base(messenger, secondaryTile, itemType)
        {

        }

        public override Task OnLoadAsync(bool SecureChanged = false)
        {
            throw new NotImplementedException("Method OnLoadASync is not implemented");
        }

        protected override void AddPane<TMessage>(string paneName, TMessage msg)
        {
            throw new NotImplementedException("Method AddPane is not implemented");
        }

        protected override void NewItemAdded(ItemAddSavedMsg obj)
        {
            throw new NotImplementedException("Method NewItemAdded is not implemented");
        }

        protected override async void ShowModal(ShowModalActivationMsg obj)
        {
            var file = obj.Files[0];

            try
            {
                string fileXmlString = await FileIO.ReadTextAsync((IStorageFile)file);
                ItemTypeEnum? itemType = CategoryDecode(fileXmlString);

                MessageDialog importMoreItemsMessage = new MessageDialog("You are trying to import more than one item.\n\n" +
                    "Import all items?", "Importing more items", MessageDialogButtonsEnum.YesNo);

                TextBlock itemSecuredMessage = new TextBlock()
                {
                    Text =
                    "One or more items are secured.\n\n" +
                    "Please, log in and repeat action.",
                    FontSize = 30
                };

                TextBlock importingMessage = new TextBlock()
                {
                    Text =
                    "Importing items\n\n" +
                    "Please wait.",
                    FontSize = 30
                };

                if (itemType == null)
                    return;

                switch (itemType)
                {
                    case ItemTypeEnum.Activity:
                        var activityFile = DecodeItemFile<Activity>(fileXmlString);

                        ActivitiesManager actiMan = new ActivitiesManager();
                        await actiMan.GetItemsAsync();
                        App.ManaLoc.AddManager(actiMan, actiMan.ID);

                        if (activityFile != null)
                            foreach (var item in activityFile.Items)
                            {
                                item.ID = -1;

                                if (item.Secured)
                                {
                                    item.Name = Crypting.Decrypt(item.Name);
                                    item.Description = Crypting.Decrypt(item.Description);
                                }
                            }
                        else
                            return;

                        if (activityFile.Items.Count(x => x.Secured) != 0 && !CustomSettings.IsUserLogged)
                            ModalWindow.SetVisibility(true, itemSecuredMessage);
                        else if (activityFile.Items.Count > 1)
                        {
                            if (await importMoreItemsMessage.ShowAsync() == MessageDialogResultEnum.Yes)
                            {
                                ModalWindow.SetVisibility(true, importingMessage, false);

                                actiMan = (ActivitiesManager)App.ManaLoc.GetManager(actiMan.ID);

                                await actiMan.AddItemRange(activityFile.Items.ToList()).ContinueWith((res) =>
                                {
                                    Messenger.Send(new ItemAddSavedMsg()
                                    {
                                        ItemType = ItemTypeEnum.Activity,
                                        MoreItemsAdded = true
                                    });
                                });

                                ModalWindow.SetVisibility(false, importingMessage, false);
                            }
                        }
                        else
                            ModalWindow.SetVisibility(true, new ActivityAdd(true, activityFile.Items[0]), false);
                        break;

                    case ItemTypeEnum.Note:
                        var noteFile = DecodeItemFile<Note>(fileXmlString);

                        NotesManager noMan = new NotesManager();
                        await noMan.GetItemsAsync();
                        App.ManaLoc.AddManager(noMan, noMan.ID);

                        if (noteFile != null)
                            foreach (var item in noteFile.Items)
                            {
                                item.ID = -1;

                                if (item.Secured)
                                {
                                    item.Name = Crypting.Decrypt(item.Name);
                                    item.Description = Crypting.Decrypt(item.Description);
                                }
                            }
                        else
                            return;

                        if (noteFile.Items.Count(x => x.Secured) != 0 && !CustomSettings.IsUserLogged)
                            ModalWindow.SetVisibility(true, itemSecuredMessage);
                        else if (noteFile.Items.Count > 1)
                        {
                            if (await importMoreItemsMessage.ShowAsync() == MessageDialogResultEnum.Yes)
                            {
                                ModalWindow.SetVisibility(true, importingMessage, false);

                                noMan = (NotesManager)App.ManaLoc.GetManager(noMan.ID);

                                await noMan.AddItemRange(noteFile.Items.ToList()).ContinueWith((res) =>
                                {
                                    Messenger.Send(new ItemAddSavedMsg()
                                    {
                                        ItemType = ItemTypeEnum.Note,
                                        MoreItemsAdded = true
                                    });
                                });

                                ModalWindow.SetVisibility(false, importingMessage, false);
                            }
                        }
                        else
                            ModalWindow.SetVisibility(true, new NoteAdd(true, noteFile.Items[0]), false);
                        break;

                    default:
                        return;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }


        }

        protected override Task UpdateSourceAsync(bool secureChanged = false)
        {
            throw new NotImplementedException("Method UpdateSourceAsync is not implemented");
        }
    }
}
