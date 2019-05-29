using Framework.Classes;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Modules.ToDos.Classes;
using System;
using System.Windows.Input;

namespace TheDailyNotes.Modules.ToDos.Commands
{
    public class DeleteItemsCommand : ICommand
    {
        private Messenger messenger;

        public DeleteItemsCommand(Messenger messenger)
        {
            this.messenger = messenger;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (parameter is AdaptiveGridView noList)
            {
                while (noList.SelectedItems.Count != 0)
                {
                    ToDosManager todoMan = (ToDosManager)App.ManaLoc.GetManager(((ToDo)noList.SelectedItems[0]).ManagerID);

                    todoMan.Delete((ToDo)noList.SelectedItems[0]);
                }
            }
        }
    }
}
