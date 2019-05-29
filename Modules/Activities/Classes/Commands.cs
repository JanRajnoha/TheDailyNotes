using Framework.Locator;
using System;
using System.Windows.Input;

namespace Modules.Activities.Classes
{
    /// <summary>
    /// Command for completing item
    /// </summary>
    public class CompletedButtonEvent : ICommand
    {
        ManagerLocator manaLoc { get; set; }

        public CompletedButtonEvent()
        {

        }

        public CompletedButtonEvent(ManagerLocator manaLoc) : this()
        {
            this.manaLoc = manaLoc;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            ((ActivitiesManager)manaLoc?.GetManager(((Activity)parameter).ManagerID))?.CompleteTask((Activity)parameter);
        }
    }

    /// <summary>
    /// Command for deleting item
    /// </summary>
    public class DeleteButtonEvent : ICommand
    {
        ManagerLocator manaLoc { get; set; }

        public DeleteButtonEvent()
        {

        }

        public DeleteButtonEvent(ManagerLocator manaLoc) : this()
        {
            this.manaLoc = manaLoc;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            ((ActivitiesManager)manaLoc?.GetManager(((Activity)parameter).ManagerID))?.Delete((Activity)parameter);
        }
    }
}
