using Framework.Classes;
using Framework.Enum;
using Framework.Interface;
using Framework.Messages;
using Modules.Activities.Classes;
using System;
using System.Linq;
using System.Windows.Input;
using TheDailyNotes.Classes;
using TheDailyNotes.Modules.Activities.ViewModels;

namespace TheDailyNotes.Modules.Activities.Commands
{
    public class SaveActivityCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private IMessenger messenger;

        private ActivityAddVM activityAddVM { get; }

        public SaveActivityCommand(IMessenger messenger, ActivityAddVM activityAddVM)
        {
            this.messenger = messenger;
            this.activityAddVM = activityAddVM;
        }

        public bool CanExecute(object parameter)
        {
            if (parameter is Activity detailActivity)
            {
                //if (detailActivity.ID == -1)
                //{
                //    ActivitiesManager ActiMan = new ActivitiesManager();

                //    return !ActiMan.GetActivities().Result.Contains(detailActivity) &&
                //        detailActivity.Name != "" &&
                //        (detailActivity.Neverend && (Functions.CheckDayRangeInput(detailActivity.Start, detailActivity.End, detailActivity.NotifyDays)) || !detailActivity.Neverend);
                //}

                if (detailActivity.Name == "")
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

                if (detailActivity.ID == -1)
                {


                    if (true)
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
                if (detailActivity.Neverend == false)
                {
                    if (!Functions.CheckDayRangeInput(detailActivity.Start, detailActivity.End, detailActivity.NotifyDays))
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

                return true;
            }
            else
                return false;
        }

        public void Execute(object parameter)
        {
            if (parameter is Activity detailActivity)
            {
                ActivitiesManager ActiMan = (ActivitiesManager)App.ManaLoc.GetManager(detailActivity.ManagerID);
                
                var xxx = ActiMan.AddItem(detailActivity);

                messenger.Send(new ItemAddSavedMsg()
                {
                    ItemType = ItemTypeEnum.Activity
                });
            }
        }
    }
}
