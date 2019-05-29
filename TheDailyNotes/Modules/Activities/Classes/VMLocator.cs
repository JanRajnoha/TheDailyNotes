using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheDailyNotes.Modules.Activities.ViewModels;

namespace TheDailyNotes.Classes
{
    public partial class VMLocator
    {
        private ActivitiesVM activitiesVM;
        public ActivitiesVM ActivitiesVM
        {
            get
            {
                if (activitiesVM == null)
                    activitiesVM = new ActivitiesVM(messenger);
                return activitiesVM;
            }
        }

        private ActivityAddVM activityAddVM;
        public ActivityAddVM ActivityAddVM
        {
            get
            {
                if (activityAddVM == null)
                    activityAddVM = new ActivityAddVM(messenger);
                return activityAddVM;
            }
        }

        private ActivityDetailVM activityDetailVM;
        public ActivityDetailVM ActivityDetailVM
        {
            get
            {
                if (activityDetailVM == null)
                    activityDetailVM = new ActivityDetailVM(messenger);
                return activityDetailVM;
            }
        }

        private ActivityProgressVM activityProgressVM;
        public ActivityProgressVM ActivityProgressVM
        {
            get
            {
                if (activityProgressVM == null)
                    activityProgressVM = new ActivityProgressVM(messenger);
                return activityProgressVM;
            }
        }
    }
}
