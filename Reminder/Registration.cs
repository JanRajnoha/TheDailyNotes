using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace Reminder
{
    /// <summary>
    /// Registers reminder background task
    /// </summary>
    public sealed class Registration
    {
        /// <summary>
        /// Registration of background tasks
        /// </summary>
        public static async void RegistrBackgroundTaskAsync()
        {
            string myTaskName = "TDNReminder";
            bool skip = false;
            BackgroundTaskBuilder taskBuilder;
            ApplicationTrigger trigger = new ApplicationTrigger();

            if (BackgroundTaskRegistration.AllTasks.Any(i => i.Value.Name.Equals(myTaskName)))
                skip = true;

            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
                if (cur.Value.Name == myTaskName)
                {
                    return;
                }
            }

            if (!skip)
            {
                // Windows Phone app must call this to use trigger types (see MSDN)
                await BackgroundExecutionManager.RequestAccessAsync();

                // register a new task
                taskBuilder = new BackgroundTaskBuilder { Name = "TDNReminder", TaskEntryPoint = "Reminder.Reminder" };
                taskBuilder.SetTrigger(new SystemTrigger(SystemTriggerType.SessionConnected, false));
                taskBuilder.SetTrigger(new SystemTrigger(SystemTriggerType.UserPresent, false));
                taskBuilder.SetTrigger(new TimeTrigger(15, false));
                taskBuilder.SetTrigger(trigger);

                taskBuilder.AddCondition(new SystemCondition(SystemConditionType.SessionConnected));

                BackgroundTaskRegistration Reminder = taskBuilder.Register();

                await trigger.RequestAsync();
            }

            // --------------------------------------------------------------------------------

            //myTaskName = "TDNCompleter";

            //if (BackgroundTaskRegistration.AllTasks.Any(i => i.Value.Name.Equals(myTaskName)))
            //    return;

            //// register a new task
            //taskBuilder = new BackgroundTaskBuilder { Name = myTaskName };
            //taskBuilder.SetTrigger(new ToastNotificationActionTrigger());
            //BackgroundTaskRegistration Completer = taskBuilder.Register();
        }
    }
}
