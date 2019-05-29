using Framework.Classes;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Modules.Notes.Classes;
using System;
using System.Windows.Input;

namespace TheDailyNotes.Modules.Notes.Commands
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
                    NotesManager noMan = (NotesManager)App.ManaLoc.GetManager(((Note)noList.SelectedItems[0]).ManagerID);

                    noMan.Delete((Note)noList.SelectedItems[0]);
                }
            }
        }
    }
}
