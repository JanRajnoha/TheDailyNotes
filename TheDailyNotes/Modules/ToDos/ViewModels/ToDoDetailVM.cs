using Framework.Classes;
using Framework.Enum;
using Framework.Messages;
using Framework.ViewModel;
using Modules.ToDos.Classes;

namespace TheDailyNotes.Modules.ToDos.ViewModels
{
    public class ToDoDetailVM : ModuleDetailVMBase<ToDo>
    {
        public ToDoDetailVM() : base()
        {
        }

        /// <summary>
        /// Init and message registration
        /// </summary>
        /// <param name="messenger">Messenger</param>
        public ToDoDetailVM(Messenger messenger) : base(messenger, ItemTypeEnum.Activity)
        {
        }

        /// <summary>
        /// Message recieve for load recieved message
        /// </summary>
        /// <param name="obj">Message</param>
        protected override void DetailOpen(SelectedDetailItemMsg obj)
        {
            if (obj.ItemType == ItemTypeEnum.ToDo)
                if (!obj.Edit || obj.ID == DetailItem?.ID)
                {
                    DetailItem = ((ToDosManager)App.ManaLoc.GetManager(obj.ManagerID))?.GetItem(obj.ID);
                }
        }
    }
}
