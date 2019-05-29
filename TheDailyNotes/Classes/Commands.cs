using System;
using System.Windows.Input;
using TheDailyNotes.Modules.Activities.Classes;
using TheDailyNotes.Modules.Activities.Views;

namespace TheDailyNotes.Classes
{
    /// <summary>
    /// Command for completing item
    /// </summary>
    class CompletedButtonEvent : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            ActivitiesManager ActiMan = new ActivitiesManager();
            ActiMan.CompleteTask((Activity)parameter);
        }
    }

    /// <summary>
    /// Command for deleting item
    /// </summary>
    class DeleteButtonEvent : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            ActivitiesManager ActiMan = new ActivitiesManager();
            ActiMan.Delete((Activity)parameter);
        }
    }
}
