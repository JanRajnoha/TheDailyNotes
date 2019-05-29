using Framework.Classes;
using Framework.Enum;
using Framework.Messages;
using Framework.ViewModel;
using Modules.Activities.Classes;
using System.Windows.Input;
using Template10.Mvvm;
using Windows.ApplicationModel.DataTransfer;

namespace TheDailyNotes.Modules.Activities.ViewModels
{
    public class ActivityProgressVM : ModuleItemVMBase<Activity>
    {
        private bool enableTextSelect;
        public bool EnableTextSelect
        {
            get { return enableTextSelect; }
            set
            {
                enableTextSelect = value;
                RaisePropertyChanged(nameof(enableTextSelect));
            }
        }


        public ICommand CompleteCommand { get; set; }

        public ActivityProgressVM() : base()
        {

        }

        /// <summary>
        /// Init and command registration
        /// </summary>
        /// <param name="messenger">Messenger</param>
        public ActivityProgressVM(Messenger messenger) : base(messenger, ItemTypeEnum.Activity)
        {
            CompleteCommand = new DelegateCommand<Activity>(CompleteActivity);

            if (CurrentItem != null)
            {
                CurrentItem.Remove = new DeleteButtonEvent(App.ManaLoc);
                CurrentItem.Complete = new DeleteButtonEvent(App.ManaLoc); 
            }
        }

        /// <summary>
        /// Prepare package for share
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void DaTranManaItem_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            shareHtml =
                "Hi<br><br>" +
                "I've shared with you my activity.";

            DataRequest dReq = args.Request;
            dReq.Data.Properties.Title = $"Activity: {CurrentItem.Name}";

            shareMessage = "Your activity has benn shared";

            dReq.Data.SetStorageItems(shareFileList);

            base.DaTranManaItem_DataRequested(sender, args);
        }

        /// <summary>
        /// Send message about completed activity
        /// </summary>
        /// <param name="act">Completed activity</param>
        private void CompleteActivity(Activity act)
        {
            ((ActivitiesManager)App.ManaLoc?.GetManager(act.ManagerID))?.CompleteTask(act);

            messenger.Send(new ItemCompletedMsg()
            {
                ItemType = ItemTypeEnum.Activity,
                ID = act.ID,
                ManagerID = act.ManagerID
            });
        }
    }
}
