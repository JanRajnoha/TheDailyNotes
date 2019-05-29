using ManagerDLL;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Xml.Serialization;
using TheDailyNotes.Classes;

namespace TheDailyNotes.Modules.Activities.Classes
{
    public class Activity : BaseItem
    {
        public string Description { get; set; }
        public DateTime Start { get; set; } = DateTime.Today;
        public DateTime End { get; set; } = DateTime.Today;
        public bool Neverend { get; set; }
        public DateTime WhenNotify { get; set; } = DateTime.Today;
        public bool Notify { get; set; } = true;
        public ObservableCollection<DayOfWeek> NotifyDays { get; set; } = new ObservableCollection<DayOfWeek>();

        private ObservableCollection<DateTime> dates;

        public ObservableCollection<DateTime> Dates
        {
            get { return dates; }

            set
            {
                dates = value;
            }
        }

        // Commands
        [JsonIgnore]
        [XmlIgnore]
        public ICommand Complete { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public ICommand Remove { get; set; }

        /// <summary>
        /// Init of class Tasks
        /// </summary>
        public Activity()
        {
            Dates = new ObservableCollection<DateTime>();
            
            Complete = new CompletedButtonEvent();
            Remove = new DeleteButtonEvent();
        }

        public Activity(Activity detailActivity)
        {
            ID = detailActivity.ID;
            Name = detailActivity.Name;
            Secured = detailActivity.Secured;
            Description = detailActivity.Description;
            Start = detailActivity.Start;
            End = detailActivity.End;
            Neverend = detailActivity.Neverend;
            WhenNotify = detailActivity.WhenNotify;
            Notify = detailActivity.Notify;
            NotifyDays = detailActivity.NotifyDays;
            Dates = detailActivity.Dates;

            Complete = detailActivity.Complete;
            Remove = detailActivity.Remove;
        }

        /// <summary>
        /// Add/remove today day
        /// </summary>
        public void AddDate()
        {
            if (!Dates.Contains(DateTime.Today))
                Dates.Add(DateTime.Today);
            else
                Dates.Remove(DateTime.Today);

            NotifyPropertyChanged(nameof(Dates));
        }
    }
}
