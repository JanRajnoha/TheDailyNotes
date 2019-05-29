using Extensions;
using Framework.Win10.Notifications;
using Modules.Activities.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace LongTermReminder
{
    public sealed class LongTermReminder : IBackgroundTask
    {
        private BackgroundTaskDeferral defferal;
        private ActivitiesManager ActiMan;
        private ObservableCollection<Activity> ActiList;
        //private bool cancelRequested;
        //private BackgroundTaskCancellationReason cancelReason;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            defferal = taskInstance.GetDeferral();

            taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);

            ActiMan = new ActivitiesManager();

            await CreateNotifications();
        }

        private async Task CreateNotifications()
        {
            var ToastNotifier = ToastNotificationManager.CreateToastNotifier();
            var ScheduledToastList = ToastNotifier.GetScheduledToastNotifications();

            ActiList = await ActiMan.GetItemsAsync(true);
            var ValidItems = ActiList.Where(x => (DateTime.Now.TimeOfDay - x.WhenNotify.TimeOfDay).TotalMinutes <= 0)?.Where(x => !ScheduledToastList.Select(y => y.Id).Contains("Acti" + x.ID)).ToList();

            if (ValidItems == null)
                return;
            else if (ValidItems.Count == 0)
                return;


            foreach (var item in ValidItems)
            {
                if (!item.Notify || item.Secured || !item.NotifyDays.Contains(DateTime.Today.DayOfWeek))
                    break;

                if ((!item.Dates.Contains(DateTime.Today)) && ((item.End >= DateTime.Today) && (item.Start <= DateTime.Today)))
                {
                    XmlDocument xml = Notifications.GetXml(new string[2] { "Uncompleted activity", $"Activity {item.Name} isn't completed." }, ToastTemplateType.ToastText02);

                    DateTime now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, item.WhenNotify.Hour, item.WhenNotify.Minute, item.WhenNotify.Second);

                    if (now.Minute == DateTime.Now.Minute)
                        now.AddMinutes(1);

                    ScheduledToastNotification newScheduled = new ScheduledToastNotification(xml, now)
                    {
                        Id = "Acti" + item.ID
                    };

                    ToastNotificationManager.CreateToastNotifier().AddToSchedule(newScheduled);
                    //if (NotifyQueue <= 5)
                    //{
                    //    NotifyQueue++;

                    //    Queue.Add(item);
                    //}
                }

                if (item.Dates.Contains(DateTime.Today))
                {
                    var scheduledNotification = ToastNotificationManager.CreateToastNotifier().GetScheduledToastNotifications().FirstOrDefault(x => x.Id == "Acti" + item.ID);

                    if (scheduledNotification != null)
                        ToastNotificationManager.CreateToastNotifier().RemoveFromSchedule(scheduledNotification);
                }
            }

            defferal.Complete();
        }

        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            //cancelRequested = true;
            //cancelReason = reason;

            defferal.Complete();
        }
    }
}
