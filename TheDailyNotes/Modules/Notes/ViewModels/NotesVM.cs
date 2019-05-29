using Framework.Classes;
using Framework.Enum;
using Framework.Messages;
using Modules.Notes.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheDailyNotes.Classes;
using TheDailyNotes.Modules.Notes.Commands;
using TheDailyNotes.Modules.Notes.Components;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.StartScreen;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TheDailyNotes.Modules.Notes.ViewModels
{
    public class NotesVM : TDNVMBase<Note>
    {
        /// <summary>
        /// Init and message registration
        /// </summary>
        /// <param name="messenger">Message</param>
        public NotesVM(Messenger messenger) : base(messenger, new SecondaryTile("NoteSecTile", "Notes", "Notes", new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png"), TileSize.Square150x150), ItemTypeEnum.Note)
        {
            var noMan = new NotesManager();
            App.ManaLoc.AddManager(noMan, noMan.ID);

            DeleteItems = new DeleteItemsCommand(messenger);
        }

        /// <summary>
        /// Prepare package for share
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override async void DaTranManaItems_DataRequestedAsync(DataTransferManager sender, DataRequestedEventArgs args)
        {
            shareHtml =
                "Hi<br><br>" +
                "I've shared with you my notes.";

            var shareFile = await ApplicationData.Current.LocalFolder.GetFileAsync("Share.tdn");

            var shareFileList = new List<StorageFile>
            {
                shareFile
            };

            DataRequest dReq = args.Request;
            dReq.Data.Properties.Title = $"Collection of notes";
            shareMessage = "Notes has been shared";

            dReq.Data.SetStorageItems(shareFileList);

            base.DaTranManaItems_DataRequestedAsync(sender, args);
        }

        /// <summary>
        /// Add new note message recieved
        /// </summary>
        /// <param name="obj">Message</param>
        protected override void NewItemAdded(ItemAddSavedMsg obj)
        {
            if (obj.ItemType != ItemTypeEnum.Note)
                return;

            // Insp
            var xxx = UpdateSourceAsync(true);

            string NotifyText = "Note added";

            if (obj.ID != -1)
                NotifyText = "Note edited";

            if (obj.MoreItemsAdded)
                NotifyText = "Notes added";

            if (obj.ID != -1)
                Messenger.Send(new SelectedDetailItemMsg()
                {
                    ItemType = ItemTypeEnum.Note,
                    ID = obj.ID,
                    Edit = true,
                    ManagerID = obj.ManagerID
                });

            Messenger.Send(new ShowNotificationMsg()
            {
                Text = NotifyText
            });

            if (obj.ClosePane)
            {
                //((NoteAdd)PivotPanes.Where(x => x.Name == AddPivotItemName)?.FirstOrDefault().Content).DataContext = new NoteAddVM();
                CloseAddPane(null);
            }
        }

        protected override async Task UpdateSourceAsync(bool secureChanged = false)
        {
            Source = await ((NotesManager)App.ManaLoc.GetManagersByType(typeof(NotesManager)).FirstOrDefault())?.GetItemsAsync(secureChanged);

            RaisePropertyChanged(nameof(Source));
        }

        /// <summary>
        /// Load items on Load
        /// </summary>
        /// <param name="secureChanged">Reload data, because security was changed</param>
        /// <returns></returns>
        public override async Task OnLoadAsync(bool secureChanged = false)
        {
            await UpdateSourceAsync(secureChanged);
        }

        /// <summary>
        /// Open pane based on pane name
        /// </summary>
        /// <typeparam name="TMessage">Message</typeparam>
        /// <param name="paneName">Pane name</param>
        /// <param name="msg">Message</param>
        protected override void AddPane<TMessage>(string paneName, TMessage msg)
        {
            string header = "";
            object content;

            switch (paneName)
            {
                case addPivotItemName:

                    content = new NoteAdd();

                    if (msg.GetType() == typeof(ItemAddNewMsg))
                    {
                        header = "Add Note";
                    }
                    else
                    {
                        header = "Edit Note";
                    }

                    break;

                case detailPivotItemName:

                    content = new NoteDetail();

                    header = "Note Detail";
                    break;

                default:
                    throw new NotImplementedException();
            }

            try
            {
                if (PivotPanes.FirstOrDefault(x => x.Name == paneName) == null)
                    PivotPanes.Insert(0, new PivotItem()
                    {
                        Name = paneName,
                        Header = header,
                        Margin = new Thickness(0),
                        Content = content
                    });
                else
                {
                    PivotPanes.FirstOrDefault(x => x.Name == paneName).Header = header;
                    PivotPanes.FirstOrDefault(x => x.Name == paneName).Content = content;
                }
            }
            catch (Exception)
            {
                PivotPanes.Clear();
                PivotPanes.Insert(0, new PivotItem()
                {
                    Name = paneName,
                    Header = header,
                    Content = content
                });
            }
            finally
            {
                if (PivotPanes.FirstOrDefault(x => x.Name == paneName) != null)
                {
                    //pivotPanes = PivotPanes;
                    RaisePropertyChanged(nameof(PivotPanes));

                    PaneVisibility = true;

                    double MinNormalWidth = (double)Application.Current.Resources["NormalMinWidth"];
                    if (MinNormalWidth > ApplicationView.GetForCurrentView().VisibleBounds.Width)
                    {
                        MasterFrame = new GridLength(0);
                    }

                    Messenger.Send(msg);
                }
            }
        }
    }
}
