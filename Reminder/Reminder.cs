using Extensions;
using Framework.Win10.Notifications;
using Modules.Activities.Classes;

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Windows.ApplicationModel.Background;
using Windows.System.Threading;
using Windows.UI.Notifications;

namespace Reminder
{
    /// <summary>
    /// Reminder background task class
    /// </summary>
    public sealed class Reminder : IBackgroundTask
    {
        private ThreadPoolTimer Timer;
        private BackgroundTaskDeferral defferal;
        private ActivitiesManager ActiMan;
        private ObservableCollection<Activity> ActiList;
        private bool cancelRequested;
        private BackgroundTaskCancellationReason cancelReason;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            defferal = taskInstance.GetDeferral();

            taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);

            ActiMan = new ActivitiesManager();

            //await Notify(8);

            Timer = ThreadPoolTimer.CreatePeriodicTimer(async (timer) =>
            { await PeriodicTimerCallbackAsync(timer); }, TimeSpan.FromMinutes(1));
            //defferal.Complete();
        }

        /// <summary>
        /// On canceled method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="reason"></param>
        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            cancelRequested = true;
            cancelReason = reason;

            Timer.Cancel();
            defferal.Complete();
        }

        /// <summary>
        /// Periodic timer for checking items to remind
        /// </summary>
        /// <param name="timer"></param>
        /// <returns></returns>
        private async Task PeriodicTimerCallbackAsync(ThreadPoolTimer timer)
        {
            if (!cancelRequested)
            {
                await Notify(2);
            }
            else
            {
                Timer.Cancel();
                defferal.Complete();
            }

            //defferal.Complete();
        }

        /// <summary>
        /// Notifying function
        /// </summary>
        /// <param name="minuteRange">Acceptable disperse in minutes</param>
        /// <returns></returns>
        private async Task Notify(int minuteRange)
        {
            Debug.WriteLine(DateTime.Now);

            var ToastNotifier = ToastNotificationManager.CreateToastNotifier();
            var ScheduledToastList = ToastNotifier.GetScheduledToastNotifications();

            ActiList = await ActiMan.GetItemsAsync(true);
            var ValidItems = ActiList.Where(x => Math.Abs((DateTime.Now.TimeOfDay - x.WhenNotify.TimeOfDay).TotalMinutes) < minuteRange)?.ToList();

            if (ValidItems == null)
                return;
            else if (ValidItems.Count == 0)
                return;


            foreach (var item in ValidItems)
            {
                if (!item.Notify || item.Secured || !item.NotifyDays.Contains(DateTime.Today.DayOfWeek))
                    continue;

                if ((!item.Dates.Contains(DateTime.Today)) && ((item.End >= DateTime.Today) && (item.Start <= DateTime.Today)))
                {
                    if (ScheduledToastList.Select(x => x.Id).Contains("Acti" + item.ID))
                        continue;

                    Notifications.SendNotification(new string[2] { "Uncompleted activity", $"Activity {item.Name} isn't completed." }, ToastTemplateType.ToastText02);

                    //if (NotifyQueue <= 5)
                    //{
                    //    NotifyQueue++;

                    //    Queue.Add(item);
                    //}
                }
            }

            defferal.Complete();
        }
    }
}
