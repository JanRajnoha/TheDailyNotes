using Framework.Classes;
using Modules.Activities.Classes;
using System;
using TheDailyNotes.Classes;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace TheDailyNotes.Modules.Activities.Components
{
    public sealed partial class ActivityDetail : UserControl
    {
        public ActivityDetail()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Recalculate value new DataContext
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ActivityDetail_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (DataContext != null && DataContext is Activity)
            {
                var Acti = (Activity)DataContext;
                Functions Fce = new Functions();

                var AllDays = Fce.GetDaysCount(Fce.GetSpecificDays(Acti.NotifyDays, Fce.GetNumberOfWeekDays(Acti.Start, Acti.End)));

                string ActiProgress, ActiCompleted;

                if (Acti.Start <= DateTime.Today && Acti.End >= DateTime.Today && !Acti.Neverend)
                {
                    ActiCompleted = (Fce.GetDaysCount(Acti.NotifyDays, Acti.Dates) * 100 / AllDays).ToString() + "%";
                    ActiProgress = (Fce.GetDaysCount(Fce.GetSpecificDays(Acti.NotifyDays, Fce.GetNumberOfWeekDays(Acti.Start, DateTime.Today))) * 100 / AllDays).ToString() + "%";
                }
                else
                {
                    // Insp
                    ActiProgress = (string)(new Framework.Classes.GetStatus()).Convert(Acti, null, "Activity", null);
                    ActiCompleted = ActiProgress;
                }

                ActivityStatistic.Text = $"Activity completed: {ActiCompleted}";
                ActivityStatistic.Text += $"\nActivity progress: {ActiProgress}";
            }
        }
    }
}
