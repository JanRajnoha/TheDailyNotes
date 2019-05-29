using Windows.ApplicationModel.Background;

namespace Recover
{
    public sealed class Recover : IBackgroundTask
    {
        private BackgroundTaskDeferral defferal;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            defferal = taskInstance.GetDeferral();

            BGTasks bGTasks = new BGTasks();

            //bGTasks.RegisterReminder();
            bGTasks.RegisterLongTermReminder();

            defferal.Complete();
        }
    }
}
