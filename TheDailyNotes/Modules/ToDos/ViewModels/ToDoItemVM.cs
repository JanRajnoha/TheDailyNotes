using Framework.Classes;
using Framework.Enum;
using Framework.Messages;
using Framework.ViewModel;
using Modules.ToDos.Classes;
using System.Windows.Input;
using Template10.Mvvm;
using Windows.ApplicationModel.DataTransfer;

namespace TheDailyNotes.Modules.ToDos.ViewModels
{
    public class ToDoItemVM : ModuleItemVMBase<ToDo>
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

        public ToDoItemVM() : base()
        {

        }

        /// <summary>
        /// Init and command registration
        /// </summary>
        /// <param name="messenger">Messenger</param>
        public ToDoItemVM(Messenger messenger) : base(messenger, ItemTypeEnum.ToDo)
        {
            CompleteCommand = new DelegateCommand<ToDo>(CompleteToDo);

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
                "I've shared with you my to - do.";

            DataRequest dReq = args.Request;
            dReq.Data.Properties.Title = $"To - Do: {CurrentItem.Name}";

            shareMessage = "Your to - do has benn shared";

            dReq.Data.SetStorageItems(shareFileList);

            base.DaTranManaItem_DataRequested(sender, args);
        }

        /// <summary>
        /// Send message about completed to - do
        /// </summary>
        /// <param name="todo">Completed to - do</param>
        private void CompleteToDo(ToDo todo)
        {
            ((ToDosManager)App.ManaLoc?.GetManager(todo.ManagerID))?.CompleteTask(todo);

            messenger.Send(new ItemCompletedMsg()
            {
                ItemType = ItemTypeEnum.ToDo,
                ID = todo.ID,
                ManagerID = todo.ManagerID
            });
        }
    }
}
