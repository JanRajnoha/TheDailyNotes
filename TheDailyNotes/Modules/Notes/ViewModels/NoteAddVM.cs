using CustomSettingsDLL;
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
using Windows.UI.Popups;

namespace TheDailyNotes.Modules.Notes.ViewModels
{
    public class NoteAddVM : ModuleAddVMBase<Note>
    {
        /// <summary>
        /// Save note and close pane
        /// </summary>
        public DelegateCommand<Note> SaveNoteClose => new DelegateCommand<Note>(async (Note note) =>
        {
            NotesManager noMan = (NotesManager)App.ManaLoc.GetManager("Notes.tdn");

            if (noMan == null)
            {
                messenger.Send(new ItemAddErrorMsg()
                {
                    ItemType = ItemTypeEnum.Note,
                    Message = "Unknown error."
                });
                return;
            }

            if (!await CanExecute(note))
                return;

            await noMan.AddItem(note);

            messenger.Send(new ItemAddSavedMsg()
            {
                ItemType = ItemTypeEnum.Note,
                ManagerID = "Notes.tdn"
            });
        });

        /// <summary>
        /// Save note or edit activity and close pane
        /// </summary>
        public DelegateCommand<Note> SaveNote => new DelegateCommand<Note>(async (Note note) =>
        {
            NotesManager noMan = (NotesManager)App.ManaLoc.GetManager("Notes.tdn");

            if (noMan == null)
            {
                messenger.Send(new ItemAddErrorMsg()
                {
                    ItemType = ItemTypeEnum.Note,
                    Message = "Unknown error."
                });
                return;
            }

            if (!await CanExecute(note))
                return;

            if (note.Name == null)
            {
                messenger.Send(new ItemAddErrorMsg()
                {
                    ItemType = ItemTypeEnum.Note,
                    Message = "Note title can not be empty!"
                });

                return;
            }

            var msg = new SelectedDetailItemMsg()
            {
                ItemType = ItemTypeEnum.Note,
                ID = note.ID,
                Edit = true
            };

            if (!await noMan.AddItem(note))
            {
                Framework.Classes.MessageDialog NoE404 = new Framework.Classes.MessageDialog("Note was unable to find in database.\n" +
                    "Note could be removed.\n\n" +
                    "Do you want create new note with this properties?", "Note couldn't be found", MessageDialogButtonsEnum.YesNo);

                if (await NoE404.ShowAsync() == MessageDialogResultEnum.Yes)
                {
                    note.ID = -1;

                    await noMan.AddItem(note);
                }
                else
                {
                    messenger.Send(new ItemAddCloseMsg()
                    {
                        ItemType = ItemTypeEnum.Note,
                    });
                }

                //messenger.Send(new NoteAddSavedMsg()
                //{
                //    ID = act.ID
                //});

                //AddNewNote(null);

                //return;
            }

            //if (msg.ID != -1)
            messenger.Send(new ItemAddSavedMsg()
            {
                ItemType = ItemTypeEnum.Note,
                ID = msg.ID,
                ClosePane = msg.ID != -1,
                ManagerID = "Notes.tdn"
            });

            CloseModal();

            AddNewItem(null);
        });

        public NoteAddVM() : base()
        {
        }

        /// <summary>
        /// Init and message registration
        /// </summary>
        /// <param name="messenger">Message</param>
        public NoteAddVM(Messenger messenger) : base(messenger, ItemTypeEnum.Note)
        {
        }

        /// <summary>
        /// Check validity of values
        /// </summary>
        /// <param name="detailedNote">Checked note</param>
        /// <returns>True, if values valid</returns>
        private async Task<bool> CanExecute(Note detailedNote)
        {
            if (detailedNote != null)
            {
                if (detailedNote.Name == "" || detailedNote.Name == null)
                {
                    //ActivityName.BorderBrush = new SolidColorBrush(Colors.Red);
                    //ActivityInputError.Visibility = Visibility.Visible;

                    messenger.Send(new ItemAddErrorMsg()
                    {
                        ItemType = ItemTypeEnum.Note,
                        Message = "Note title can not be empty!"
                    });
                    return false;
                }

                if (detailedNote.ID == -1)
                {
                    var nameList = await ((NotesManager)App.ManaLoc.GetManager("Notes.tdn")).GetItemsNameList();

                    if (nameList.Contains(detailedNote.Name))
                    {
                        //ActivityName.BorderBrush = new SolidColorBrush(Colors.Red);
                        //ActivityInputError.Visibility = Visibility.Visible;

                        messenger.Send(new ItemAddErrorMsg()
                        {
                            ItemType = ItemTypeEnum.Note,
                            Message = "Name with this title is already existing!"
                        });
                        return false;
                    }
                }

                messenger.Send(new ItemAddValidMsg()
                {
                    ItemType = ItemTypeEnum.Note
                });
                return true;
            }
            else
            {
                messenger.Send(new ItemAddErrorMsg()
                {
                    ItemType = ItemTypeEnum.Note,
                    Message = "Inner error. Please try again after few moments."
                });
                return false;
            }
        }

        /// <summary>
        /// New note message recieved
        /// </summary>
        /// <param name="obj">Message</param>
        protected override void AddNewItem(ItemAddNewMsg obj)
        {
            if (obj != null)
                if (obj.ItemType != ItemTypeEnum.Note)
                return;

            DetailItem = new Note();
            RaisePropertyChanged(nameof(DetailItem));
        }

        /// <summary>
        /// Selected note message recieved
        /// </summary>
        /// <param name="obj">Message</param>
        protected override void SelectedItemChanged(SelectedAddItemMsg obj)
        {
            if (obj.ItemType == ItemTypeEnum.Note)
                DetailItem = ((NotesManager)App.ManaLoc.GetManager(obj.ManagerID)).GetItem(obj.ID);
        }
    }
}
