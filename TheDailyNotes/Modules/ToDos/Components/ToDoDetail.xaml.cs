using Framework.Classes;
using Modules.ToDos.Classes;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace TheDailyNotes.Modules.ToDos.Components
{
    public sealed partial class ToDoDetail : UserControl
    {
        public ToDoDetail()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Recalculate value new DataContext
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ToDoDetail_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (DataContext != null && DataContext is ToDo)
            {
                var todo = (ToDo)DataContext;
                Functions Fce = new Functions();

                var AllDays = Fce.GetDaysCount(Fce.GetSpecificDays(todo.NotifyDays, Fce.GetNumberOfWeekDays(todo.Start, todo.End)));

                string toDoProgress, toDoCompleted;

                if (todo.Start <= DateTime.Today && todo.End >= DateTime.Today && !todo.Neverend)
                {
                    toDoCompleted = (Fce.GetDaysCount(todo.NotifyDays, todo.Dates) * 100 / AllDays).ToString() + "%";
                    toDoProgress = (Fce.GetDaysCount(Fce.GetSpecificDays(todo.NotifyDays, Fce.GetNumberOfWeekDays(todo.Start, DateTime.Today))) * 100 / AllDays).ToString() + "%";
                }
                else
                {
                    // Insp
                    toDoProgress = (string)(new GetStatus()).Convert(todo, null, "To - Do", null);
                    toDoCompleted = toDoProgress;
                }

                ToDoStatistic.Text = $"To - Do completed: {toDoCompleted}";
                ToDoStatistic.Text += $"\nTo - Do progress: {toDoProgress}";
            }
        }
    }
}
