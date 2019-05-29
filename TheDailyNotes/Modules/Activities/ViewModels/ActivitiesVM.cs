using Framework.Classes;
using Framework.Enum;
using Framework.Messages;
using Framework.Service;

using Modules.Activities.Classes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Template10.Mvvm;
using TheDailyNotes.Classes;
using TheDailyNotes.Modules.Activities.Commands;
using TheDailyNotes.Modules.Activities.Components;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.StartScreen;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TheDailyNotes.Modules.Activities.ViewModels
{
    public class ActivitiesVM : TDNVMBase<Activity>
    {
        private bool enableSlideControls;
        public bool EnableSlideControls
        {
            get { return enableSlideControls; }
            set
            {
                enableSlideControls = value;
                RaisePropertyChanged(nameof(EnableSlideControls));
            }
        }

        public DelegateCommand<Activity> CompleteCommand => new DelegateCommand<Activity>((Activity act) =>
        {
            ((ActivitiesManager)App.ManaLoc.GetManager(act.ManagerID)).CompleteTask(act);
        });

        /// <summary>
        /// Init and message registration
        /// </summary>
        /// <param name="messenger">Message</param>
        public ActivitiesVM(Messenger messenger) : base(messenger, new SecondaryTile("ActivitySecTile", "Activities", "Activities", new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png"), TileSize.Square150x150), ItemTypeEnum.Activity)
        {
            var actiMan = new ActivitiesManager();
            App.ManaLoc.AddManager(actiMan, actiMan.ID);

            DeleteItems = new DeleteItemsCommand(messenger);

            Messenger.Register<ItemCompletedMsg>(ActivityCompleted);
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
                "I've shared with you my activities.";

            var shareFile = await ApplicationData.Current.LocalFolder.GetFileAsync(ShareFileName);

            var shareFileList = new List<StorageFile>
            {
                shareFile
            };

            DataRequest dReq = args.Request;
            dReq.Data.Properties.Title = $"Collection of activities";
            shareMessage = "Activities has been shared";

            dReq.Data.SetStorageItems(shareFileList);

            base.DaTranManaItems_DataRequestedAsync(sender, args);
        }

        /// <summary>
        /// Send message about completed activity
        /// </summary>
        /// <param name="obj">Completed activity</param>
        private void ActivityCompleted(ItemCompletedMsg obj)
        {
            if (obj.ItemType != ItemTypeEnum.Activity)
                return;

            var act = ((ActivitiesManager)App.ManaLoc.GetManager(obj.ManagerID)).GetItem(obj.ID);

            Messenger.Send(new SelectedDetailItemMsg()
            {
                ItemType = ItemTypeEnum.Activity,
                ID = obj.ID,
                Edit = true,
                ManagerID = obj.ManagerID
            });
        }

        /// <summary>
        /// New activity added message recieved
        /// </summary>
        /// <param name="obj">Message</param>
        protected override void NewItemAdded(ItemAddSavedMsg obj)
        {
            if (obj.ItemType != ItemTypeEnum.Activity)
                return;

            var xxx = UpdateSourceAsync(true);

            string NotifyText = "Activity added";

            if (obj.ID != -1)
                NotifyText = "Activity edited";

            if (obj.MoreItemsAdded)
                NotifyText = "Activities added";

            if (obj.ID != -1 && !obj.MoreItemsAdded)
                Messenger.Send(new SelectedDetailItemMsg()
                {
                    ItemType = ItemTypeEnum.Activity,
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
                //((ActivityAdd)PivotPanes.Where(x => x.Name == AddPivotItemName)?.FirstOrDefault().Content).DataContext = null;
                CloseAddPane(null);
            }
        }

        protected override async Task UpdateSourceAsync(bool secureChanged = false)
        {
            Source = await ((ActivitiesManager)App.ManaLoc.GetManagersByType(typeof(ActivitiesManager)).FirstOrDefault())?.GetItemsAsync(secureChanged);

            if (Source == null)
                return;

            foreach (var item in Source)
            {
                item.Remove = new DeleteButtonEvent(App.ManaLoc);
            }

            RaisePropertyChanged(nameof(Source));
        }

        /// <summary>
        /// Load items on Load
        /// </summary>
        /// <param name="SecureChanged">Reload data, because security was changed</param>
        /// <returns></returns>
        public override async Task OnLoadAsync(bool SecureChanged = false)
        {
            await UpdateSourceAsync(SecureChanged);

            EnableSlideControls = SettingsService.Instance.UseSlidableItems;
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

                    content = new ActivityAdd();

                    if (msg.GetType() == typeof(ItemAddNewMsg))
                    {
                        header = "Add Activity";
                    }
                    else
                    {
                        header = "Edit Activity";
                    }

                    break;

                case detailPivotItemName:

                    content = new ActivityDetail();

                    header = "Activity Detail";
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
