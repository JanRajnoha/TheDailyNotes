using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace Recover
{
    public sealed class BGTasks
    {
        public async void RegisterReminder()
        {
            // --------- Reminder background task ---------------------------------------------
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
                    skip = true;
                }
            }

            if (!skip)
            {
                BackgroundExecutionManager.RemoveAccess();
                // Windows Phone app must call this to use trigger types (see MSDN)
                var res = await BackgroundExecutionManager.RequestAccessAsync();

                // register a new task
                taskBuilder = new BackgroundTaskBuilder { Name = myTaskName, TaskEntryPoint = "Reminder.Reminder" };
                taskBuilder.SetTrigger(new SystemTrigger(SystemTriggerType.SessionConnected, false));
                taskBuilder.SetTrigger(new SystemTrigger(SystemTriggerType.UserPresent, false));
                taskBuilder.SetTrigger(new TimeTrigger(15, false));
                taskBuilder.SetTrigger(trigger);

                BackgroundTaskRegistration Reminder = taskBuilder.Register();

                await trigger.RequestAsync();
            }
        }

        public async void RegisterLongTermReminder()
        {
            // --------- Long Term Reminder background task -----------------------------------

            string myTaskName = "TDNLongReminder";
            bool skip = false;
            BackgroundTaskBuilder taskBuilder;
            ApplicationTrigger trigger = new ApplicationTrigger();

            if (BackgroundTaskRegistration.AllTasks.Any(i => i.Value.Name.Equals(myTaskName)))
                skip = true;

            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
                if (cur.Value.Name == myTaskName)
                {
                    skip = true;
                }
            }

            if (!skip)
            {
                BackgroundExecutionManager.RemoveAccess();
                // Windows Phone app must call this to use trigger types (see MSDN)
                var res = await BackgroundExecutionManager.RequestAccessAsync();

                // register a new task
                taskBuilder = new BackgroundTaskBuilder { Name = myTaskName, TaskEntryPoint = "LongTermReminder.LongTermReminder" };
                taskBuilder.SetTrigger(new SystemTrigger(SystemTriggerType.SessionConnected, false));
                taskBuilder.SetTrigger(new SystemTrigger(SystemTriggerType.UserPresent, false));
                taskBuilder.SetTrigger(new TimeTrigger(15, false));
                taskBuilder.SetTrigger(trigger);

                BackgroundTaskRegistration Reminder = taskBuilder.Register();

                await trigger.RequestAsync();
            }
        }
    }
}
