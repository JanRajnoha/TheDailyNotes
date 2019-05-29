using CustomSettingsDLL;
using Framework.Manager;
using System.Diagnostics;

namespace Modules.Activities.Classes
{
    public class ActivitiesManager : TaskItemManager<Activity>
    {
        public ActivitiesManager(string fileName = "Activities.tdn") : base(fileName, "Acti", "Activity")
        {
        }

        protected override void CustomSettings_UserLogChanged(object sender, UserLoggedEventArgs e)
        {
            Debug.WriteLine($"User secure settings has changed. Log called from {GetType().ToString()}.");
        }
    }
}
