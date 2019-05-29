using CustomSettingsDLL;
using Framework.Manager;
using System.Diagnostics;

namespace Modules.ToDos.Classes
{
    public class ToDosManager : TaskItemManager<ToDo>
    {
        public ToDosManager(string fileName = "ToDos.tdn") : base(fileName, "ToDo", "To - Do")
        {
        }

        protected override void CustomSettings_UserLogChanged(object sender, UserLoggedEventArgs e)
        {
            Debug.WriteLine($"User secure settings has changed. Log called from {GetType().ToString()}.");
        }
    }
}
