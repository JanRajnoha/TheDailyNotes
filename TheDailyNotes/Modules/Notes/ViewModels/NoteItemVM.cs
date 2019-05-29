using Framework.Classes;
using Framework.Enum;
using Framework.Messages;
using Framework.ViewModel;
using Modules.Notes.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Template10.Mvvm;
using TheDailyNotes.Classes;
using Windows.ApplicationModel.DataTransfer;

namespace TheDailyNotes.Modules.Notes.ViewModels
{
    public class NoteItemVM : ModuleItemVMBase<Note>
    {
        public NoteItemVM() : base()
        {

        }

        /// <summary>
        /// Init and command registration
        /// </summary>
        /// <param name="messenger">Messenger</param>
        public NoteItemVM(Messenger messenger) : base(messenger, ItemTypeEnum.Note)
        {
            this.messenger = messenger;
        }

        protected override void DaTranManaItem_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            shareHtml =
                "Hi<br><br>" +
                "I've shared with you my note.";

            DataRequest dReq = args.Request;
            dReq.Data.Properties.Title = $"Note: {CurrentItem.Name}";

            dReq.Data.SetStorageItems(shareFileList);
            shareMessage = "Your note has been shared";

            base.DaTranManaItem_DataRequested(sender, args);
        }
    }
}
