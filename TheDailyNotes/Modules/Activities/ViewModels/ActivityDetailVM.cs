using Framework.Classes;
using Framework.Enum;
using Framework.Messages;
using Framework.ViewModel;
using Modules.Activities.Classes;

namespace TheDailyNotes.Modules.Activities.ViewModels
{
    public class ActivityDetailVM : ModuleDetailVMBase<Activity>
    {
        public ActivityDetailVM() : base()
        {
        }

        /// <summary>
        /// Init and message registration
        /// </summary>
        /// <param name="messenger">Messenger</param>
        public ActivityDetailVM(Messenger messenger) : base(messenger, ItemTypeEnum.Activity)
        {
        }

        /// <summary>
        /// Message recieve for load recieved message
        /// </summary>
        /// <param name="obj">Message</param>
        protected override void DetailOpen(SelectedDetailItemMsg obj)
        {
            if (obj.ItemType == ItemTypeEnum.Activity)
                if (!obj.Edit || obj.ID == DetailItem?.ID)
                {
                    DetailItem = ((ActivitiesManager)App.ManaLoc.GetManager(obj.ManagerID))?.GetItem(obj.ID);
                }
        }
    }
}
