using Framework.Classes;
using Framework.Enum;
using Framework.Messages;
using Framework.ViewModel;
using Framework.Win10.Notifications;
using Modules.ToDos.Classes;
using System;
using System.Threading.Tasks;
using Template10.Mvvm;

namespace TheDailyNotes.Modules.ToDos.ViewModels
{
    public class ToDoAddVM : ModuleAddVMBase<ToDo>
    {
        /// <summary>
        /// Save to - do and close pane
        /// </summary>
        public DelegateCommand<ToDo> SaveToDoClose => new DelegateCommand<ToDo>(async (ToDo todo) =>
        {
            ToDosManager todoMan = (ToDosManager)App.ManaLoc.GetManager("ToDos.tdn");

            if (todoMan == null)
            {
                messenger.Send(new ItemAddErrorMsg()
                {
                    ItemType = ItemTypeEnum.ToDo,
                    Message = "Unknown error."
                });
                return;
            }

            if (!await CanExecute(todo))
                return;

            await todoMan.AddItem(todo);

            ScheduleToastNotification(todo, todoMan);

            messenger.Send(new ItemAddSavedMsg()
            {
                ItemType = ItemTypeEnum.ToDo,
                ManagerID = "ToDos.tdn"
            });
        });

        /// <summary>
        /// Schedule new toast notification and if to - do is edited, remove the old one
        /// </summary>
        /// <param name="todo">Schduling to - do</param>
        /// <param name="todoMan">Manager</param>
        /// <param name="edited">ToDo was edited</param>
        private void ScheduleToastNotification(ToDo todo, ToDosManager todoMan, bool edited = false)
        {
            if (edited)
            {
                Notifications.RemoveScheduledToast("todo" + todo.ID);
            }

            todoMan.ScheduleToastNotification(todo);
        }

        /// <summary>
        /// Save to - do or edit to - do and close pane
        /// </summary>
        public DelegateCommand<ToDo> SaveToDo => new DelegateCommand<ToDo>(async (ToDo todo) =>
        {
            ToDosManager todoMan = (ToDosManager)App.ManaLoc.GetManager("ToDos.tdn");
            bool edited = todo.ID != -1;

            if (todoMan == null)
            {
                messenger.Send(new ItemAddErrorMsg()
                {
                    ItemType = ItemTypeEnum.ToDo,
                    Message = "Unknown error."
                });
                return;
            }

            if (!await CanExecute(todo))
                return;

            if (todo.Name == null)
            {
                messenger.Send(new ItemAddErrorMsg()
                {
                    ItemType = ItemTypeEnum.ToDo,
                    Message = "ToDo name can not be empty!"
                });

                return;
            }

            var msg = new SelectedDetailItemMsg()
            {
                ItemType = ItemTypeEnum.ToDo,
                ID = todo.ID,
                ManagerID = "ToDos.tdn"
            };

            if (!await todoMan.AddItem(todo))
            {
                MessageDialog ToDOE404 = new MessageDialog("ToDo was unable to find in database.\n" +
                    "ToDo could be removed.\n\n" +
                    "Do you want create new to - do with this properties?", "ToDo couldn't be found", MessageDialogButtonsEnum.YesNo);

                if (await ToDOE404.ShowAsync() == MessageDialogResultEnum.Yes)
                {
                    todo.ID = -1;

                    await todoMan.AddItem(todo);
                }
                else
                {
                    messenger.Send(new ItemAddCloseMsg()
                    {
                        ItemType = ItemTypeEnum.ToDo
                    });
                }
            }
            
            messenger.Send(new ItemAddSavedMsg()
            {
                ItemType = ItemTypeEnum.ToDo,
                ID = msg.ID,
                ClosePane = msg.ID != -1,
                ManagerID = "ToDos.tdn"
            });

            CloseModal();

            ScheduleToastNotification(todo, todoMan, edited);

            AddNewItem(null);
        });

        public ToDoAddVM() : base()
        {
        }

        /// <summary>
        /// Init and message registration
        /// </summary>
        /// <param name="messenger">Message</param>
        public ToDoAddVM(Messenger messenger) : base(messenger, ItemTypeEnum.ToDo)
        {
        }

        /// <summary>
        /// Check validity of values
        /// </summary>
        /// <param name="detailedToDo">Checked to - do</param>
        /// <returns>True, if values valid</returns>
        public async Task<bool> CanExecute(ToDo detailedToDo)
        {
            if (detailedToDo != null)
            {
                if (detailedToDo.Name == "" || detailedToDo.Name == null)
                {
                    messenger.Send(new ItemAddErrorMsg()
                    {
                        ItemType = ItemTypeEnum.ToDo,
                        Message = "ToDo name can not be empty!"
                    });
                    return false;
                }

                if (detailedToDo.ID == -1)
                {
                    var nameList = await ((ToDosManager)App.ManaLoc.GetManager("ToDos.tdn")).GetItemsNameList();

                    if (nameList.Contains(detailedToDo.Name))
                    {
                        messenger.Send(new ItemAddErrorMsg()
                        {
                            ItemType = ItemTypeEnum.ToDo,
                            Message = "ToDo with this name is already existing!"
                        });
                        return false;
                    }
                }
                
                if (detailedToDo.Neverend == false)
                {
                    if (detailedToDo.Start.Date > detailedToDo.End.Date)
                    {
                        messenger.Send(new ItemAddErrorMsg()
                        {
                            ItemType = ItemTypeEnum.ToDo,
                            Message = "Start date must be lower than end date!"
                        });
                        return false;
                    }

                    if (detailedToDo.ID != -1 && detailedToDo.End.Date < DateTime.Today)
                    {
                        messenger.Send(new ItemAddErrorMsg()
                        {
                            ItemType = ItemTypeEnum.ToDo,
                            Message = "ToDo can not end in past!"
                        });
                        return false;
                    }

                    if (!Functions.CheckDayRangeInput(detailedToDo.Start, detailedToDo.End, detailedToDo.NotifyDays))
                    {
                        messenger.Send(new ItemAddErrorMsg()
                        {
                            ItemType = ItemTypeEnum.ToDo,
                            Message = "Selected range has not valid days!"
                        });
                        return false;
                    }
                }

                messenger.Send(new ItemAddValidMsg()
                {
                    ItemType = ItemTypeEnum.ToDo
                });
                return true;
            }
            else
            {
                messenger.Send(new ItemAddErrorMsg()
                {
                    ItemType = ItemTypeEnum.ToDo,
                    Message = "Inner error. Please try again after few moments."
                });
                return false;
            }
        }

        /// <summary>
        /// New to - do message recieved
        /// </summary>
        /// <param name="obj">Message</param>
        protected override void AddNewItem(ItemAddNewMsg obj)
        {
            if (obj != null)
                if (obj.ItemType != ItemTypeEnum.ToDo)
                    return;

            DetailItem = new ToDo();
            RaisePropertyChanged(nameof(DetailItem));
        }

        /// <summary>
        /// Selected to - do message recieved
        /// </summary>
        /// <param name="obj">Message</param>
        protected override void SelectedItemChanged(SelectedAddItemMsg obj)
        {
            if (obj.ItemType == ItemTypeEnum.ToDo)
                DetailItem = ((ToDosManager)App.ManaLoc.GetManager(obj.ManagerID))?.GetItem(obj.ID);
        }
    }
}
