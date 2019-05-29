using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheDailyNotes.Modules.ToDos.ViewModels;

namespace TheDailyNotes.Classes
{
    public partial class VMLocator
    {
        private ToDosVM toDosVM;
        public ToDosVM ToDosVM
        {
            get
            {
                if (toDosVM == null)
                    toDosVM = new ToDosVM(messenger);
                return toDosVM;
            }
        }

        private ToDoAddVM toDoAddVM;
        public ToDoAddVM ToDoAddVM
        {
            get
            {
                if (toDoAddVM == null)
                    toDoAddVM = new ToDoAddVM(messenger);
                return toDoAddVM;
            }
        }

        private ToDoDetailVM toDoDetailVM;
        public ToDoDetailVM ToDoDetailVM
        {
            get
            {
                if (toDoDetailVM == null)
                    toDoDetailVM = new ToDoDetailVM(messenger);
                return toDoDetailVM;
            }
        }

        private ToDoItemVM toDoItemVM;
        public ToDoItemVM ToDoItemVM
        {
            get
            {
                if (toDoItemVM == null)
                    toDoItemVM = new ToDoItemVM(messenger);
                return toDoItemVM;
            }
        }
    }
}
