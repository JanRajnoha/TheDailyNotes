using Framework.Classes;
using Framework.Enum;
using Framework.Messages;
using Framework.ViewModel;
using Framework.Win10.Notifications;
using Modules.Activities.Classes;
using System;
using System.Threading.Tasks;
using Template10.Mvvm;

namespace TheDailyNotes.Modules.Activities.ViewModels
{
    public class ActivityAddVM : ModuleAddVMBase<Activity>
    {
        /// <summary>
        /// Save activity and close pane
        /// </summary>
        public DelegateCommand<Activity> SaveActivityClose => new DelegateCommand<Activity>(async (Activity act) =>
        {
            ActivitiesManager actiMan = (ActivitiesManager)App.ManaLoc.GetManager("Activities.tdn");

            if (actiMan == null)
            {
                messenger.Send(new ItemAddErrorMsg()
                {
                    ItemType = ItemTypeEnum.Activity,
                    Message = "Unknown error."
                });
                return;
            }

            if (!await CanExecute(act))
                return;

            await actiMan.AddItem(act);

            ScheduleToastNotification(act, actiMan);

            messenger.Send(new ItemAddSavedMsg()
            {
                ItemType = ItemTypeEnum.Activity,
                ManagerID = "Activities.tdn"
            });
        });

        /// <summary>
        /// Schedule new toast notification and if activity is edited, remove the old one
        /// </summary>
        /// <param name="act">Schduling activity</param>
        /// <param name="actiMan">Manager</param>
        /// <param name="edited">Activity was edited</param>
        private void ScheduleToastNotification(Activity act, ActivitiesManager actiMan, bool edited = false)
        {
            if (edited)
            {
                Notifications.RemoveScheduledToast("Acti" + act.ID);
            }

            actiMan.ScheduleToastNotification(act);
        }

        /// <summary>
        /// Save activity or edit activity and close pane
        /// </summary>
        public DelegateCommand<Activity> SaveActivity => new DelegateCommand<Activity>(async (Activity act) =>
        {
            ActivitiesManager actiMan = (ActivitiesManager)App.ManaLoc.GetManager("Activities.tdn");
            bool edited = act.ID != -1;

            if (actiMan == null)
            {
                messenger.Send(new ItemAddErrorMsg()
                {
                    ItemType = ItemTypeEnum.Activity,
                    Message = "Unknown error."
                });
                return;
            }

            if (!await CanExecute(act))
                return;

            if (act.Name == null)
            {
                messenger.Send(new ItemAddErrorMsg()
                {
                    ItemType = ItemTypeEnum.Activity,
                    Message = "Activity name can not be empty!"
                });

                return;
            }

            var msg = new SelectedDetailItemMsg()
            {
                ItemType = ItemTypeEnum.Activity,
                ID = act.ID,
                ManagerID = "Activities.tdn"
            };

            if (!await actiMan.AddItem(act))
            {
                Framework.Classes.MessageDialog ActE404 = new Framework.Classes.MessageDialog("Activity was unable to find in database.\n" +
                    "Activity could be removed.\n\n" +
                    "Do you want create new activity with this properties?", "Activity couldn't be found", Framework.Enum.MessageDialogButtonsEnum.YesNo);

                if (await ActE404.ShowAsync() == Framework.Enum.MessageDialogResultEnum.Yes)
                {
                    act.ID = -1;

                    await actiMan.AddItem(act);
                }
                else
                {
                    messenger.Send(new ItemAddCloseMsg()
                    {
                        ItemType = ItemTypeEnum.Activity
                    });
                }

                //messenger.Send(new ActivityAddSavedMsg()
                //{
                //    ID = act.ID
                //});

                //AddNewActivity(null);

                //return;
            }

            //if (msg.ID != -1)
            messenger.Send(new ItemAddSavedMsg()
            {
                ItemType = ItemTypeEnum.Activity,
                ID = msg.ID,
                ClosePane = msg.ID != -1,
                ManagerID = "Activities.tdn"
            });

            CloseModal();

            ScheduleToastNotification(act, actiMan, edited);

            AddNewItem(null);
        });

        public ActivityAddVM() : base()
        {
        }

        /// <summary>
        /// Init and message registration
        /// </summary>
        /// <param name="messenger">Message</param>
        public ActivityAddVM(Messenger messenger) : base(messenger, ItemTypeEnum.Activity)
        {
        }

        /// <summary>
        /// Check validity of values
        /// </summary>
        /// <param name="detailedActivity">Checked activity</param>
        /// <returns>True, if values valid</returns>
        public async Task<bool> CanExecute(Activity detailedActivity)
        {
            if (detailedActivity != null)
            {
                if (detailedActivity.Name == "" || detailedActivity.Name == null)
                {
                    //ActivityName.BorderBrush = new SolidColorBrush(Colors.Red);
                    //ActivityInputError.Visibility = Visibility.Visible;

                    messenger.Send(new ItemAddErrorMsg()
                    {
                        ItemType = ItemTypeEnum.Activity,
                        Message = "Activity name can not be empty!"
                    });
                    return false;
                }

                if (detailedActivity.ID == -1)
                {
                    var nameList = await ((ActivitiesManager)App.ManaLoc.GetManager("Activities.tdn")).GetItemsNameList();

                    if (nameList.Contains(detailedActivity.Name))
                    {
                        //ActivityName.BorderBrush = new SolidColorBrush(Colors.Red);
                        //ActivityInputError.Visibility = Visibility.Visible;

                        messenger.Send(new ItemAddErrorMsg()
                        {
                            ItemType = ItemTypeEnum.Activity,
                            Message = "Activity with this name is already existing!"
                        });
                        return false;
                    }
                }

                //Result &= ActivityEnd.IsEnabled == true ? CheckDayRangeInput() : true;
                if (detailedActivity.Neverend == false)
                {
                    if (detailedActivity.Start.Date > detailedActivity.End.Date)
                    {
                        messenger.Send(new ItemAddErrorMsg()
                        {
                            ItemType = ItemTypeEnum.Activity,
                            Message = "Start date must be lower than end date!"
                        });
                        return false;
                    }

                    if (detailedActivity.ID != -1 && detailedActivity.End.Date < DateTime.Today)
                    {
                        messenger.Send(new ItemAddErrorMsg()
                        {
                            ItemType = ItemTypeEnum.Activity,
                            Message = "Activity can not end in past!"
                        });
                        return false;
                    }

                    //if (detailedActivity.Start < DateTime.Today)
                    //{
                    //    messenger.Send(new ItemAddErrorMsg()
                    //    {
                    //        ItemType = ItemTypeEnum.Activity,
                    //        Message = "Start date must be today date or later!"
                    //    });
                    //    return false;
                    //}

                    if (!Functions.CheckDayRangeInput(detailedActivity.Start, detailedActivity.End, detailedActivity.NotifyDays))
                    {
                        //ActivityEnd.BorderBrush = new SolidColorBrush(Colors.Red);
                        //ActivityStart.BorderBrush = new SolidColorBrush(Colors.Red);
                        //ActivityInputError.Visibility = Visibility.Visible;

                        messenger.Send(new ItemAddErrorMsg()
                        {
                            ItemType = ItemTypeEnum.Activity,
                            Message = "Selected range has not valid days!"
                        });
                        return false;
                    }
                }

                messenger.Send(new ItemAddValidMsg()
                {
                    ItemType = ItemTypeEnum.Activity
                });
                return true;
            }
            else
            {
                messenger.Send(new ItemAddErrorMsg()
                {
                    ItemType = ItemTypeEnum.Activity,
                    Message = "Inner error. Please try again after few moments."
                });
                return false;
            }
        }

        /// <summary>
        /// New activity message recieved
        /// </summary>
        /// <param name="obj">Message</param>
        protected override void AddNewItem(ItemAddNewMsg obj)
        {
            if (obj != null)
                if (obj.ItemType != ItemTypeEnum.Activity)
                    return;

            DetailItem = new Activity();
            RaisePropertyChanged(nameof(DetailItem));
        }

        /// <summary>
        /// Selected activity message recieved
        /// </summary>
        /// <param name="obj">Message</param>
        protected override void SelectedItemChanged(SelectedAddItemMsg obj)
        {
            if (obj.ItemType == ItemTypeEnum.Activity)
                DetailItem = ((ActivitiesManager)App.ManaLoc.GetManager(obj.ManagerID))?.GetItem(obj.ID);
        }
    }
}