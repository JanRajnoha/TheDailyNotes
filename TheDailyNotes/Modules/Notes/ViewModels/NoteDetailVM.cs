using Framework.Classes;
using Framework.Enum;
using Framework.Messages;
using Framework.ViewModel;
using Modules.Notes.Classes;

namespace TheDailyNotes.Modules.Notes.ViewModels
{
    public class NoteDetailVM : ModuleDetailVMBase<Note>
    {
        public NoteDetailVM() : base()
        {
        }

        /// <summary>
        /// Init and message registration
        /// </summary>
        /// <param name="messenger">Messenger</param>
        public NoteDetailVM(Messenger messenger) : base(messenger, ItemTypeEnum.Note)
        {
        }

        /// <summary>
        /// Message recieve for load recieved message
        /// </summary>
        /// <param name="obj">Message</param>
        protected override void DetailOpen(SelectedDetailItemMsg obj)
        {
            if (obj.ItemType == ItemTypeEnum.Note)
                if (!obj.Edit || obj.ID == DetailItem?.ID)
                {
                    DetailItem = ((NotesManager)App.ManaLoc.GetManager(obj.ManagerID)).GetItem(obj.ID);
                }
        }
    }
}
