using Framework.Classes;
using Modules.Activities.Classes;
using System;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace TheDailyNotes.Modules.Activities.Commands
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
            if (parameter is ListView ActList)
            {
                while (ActList.SelectedItems.Count != 0)
                {
                    ActivitiesManager ActiMan = (ActivitiesManager)App.ManaLoc.GetManager(((Activity)ActList.SelectedItems[0]).ManagerID);

                    ActiMan.Delete((Activity)ActList.SelectedItems[0]);
                }
            }
        }
    }
}
