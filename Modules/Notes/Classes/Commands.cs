using Framework.Locator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Modules.Notes.Classes
{

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
            ((NotesManager)manaLoc?.GetManager(((Note)parameter).ManagerID))?.Delete((Note)parameter);
        }
    }
}
