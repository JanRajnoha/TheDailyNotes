using Framework.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheDailyNotes.Modules.Notes.ViewModels;

namespace TheDailyNotes.Classes
{
    public partial class VMLocator
    {
        private NotesVM notesVM;
        public NotesVM NotesVM
        {
            get
            {
                if (notesVM == null)
                    notesVM = new NotesVM(messenger);
                return notesVM;
            }
        }

        private NoteAddVM noteAddVM;
        public NoteAddVM NoteAddVM
        {
            get
            {
                if (noteAddVM == null)
                    noteAddVM = new NoteAddVM(messenger);
                return noteAddVM;
            }
        }

        private NoteDetailVM noteDetailVM;
        public NoteDetailVM NoteDetailVM
        {
            get
            {
                if (noteDetailVM == null)
                    noteDetailVM = new NoteDetailVM(messenger);
                return noteDetailVM;
            }
        }

        private NoteItemVM noteItemVM;
        public NoteItemVM NoteItemVM
        {
            get
            {
                if (noteItemVM == null)
                    noteItemVM = new NoteItemVM(messenger);
                return noteItemVM;
            }
        }
    }
}
