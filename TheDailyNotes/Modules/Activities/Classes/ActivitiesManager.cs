using CustomSettingsDLL;
using ManagerDLL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace TheDailyNotes.Modules.Activities.Classes
{
    public class ActivitiesManager : Manager<Activity>
    {
        public ActivitiesManager(string FileName = "Activities.xml") : base(FileName)
        {
            CustomSettings.UserLogChanged += CustomSettings_UserLogChanged;
        }

        private void CustomSettings_UserLogChanged(object sender, IsUserLoggedEventArgs e)
        {

        }

        /// <summary>
        /// Get index of activity in collection by activity
        /// </summary>
        /// <param name="ExaminedTask">Activity</param>
        /// <returns>Index of activity in collection</returns>
        public int IndexOfTask(Activity ExaminedTask)
        {
            return IndexOfTask(ExaminedTask.ID);
        }

        /// <summary>
        /// Get index of activity in collection by ID
        /// </summary>
        /// <param name="ID">ID of activity</param>
        /// <returns>Index of activity in collection</returns>
        private int IndexOfTask(int ID)
        {
            int Index = -1;

            foreach (Activity task in ItemsSource)
            {
                bool Result = false;

                Result = task.ID == ID;

                Index++;

                if (Result)
                    return Index;
            }

            return -1;
        }

        /// <summary>
        /// Get ID for activity. If any ID missing, return value of this ID.
        /// If not, return next ID from row.
        /// </summary>
        /// <returns>New ID</returns>
        private int GetID()
        {
            int Index = -1;
            for (int i = 0; i < ItemsSource.Count; i++)
            {
                bool ExistID = false;
                for (int j = 0; j < ItemsSource.Count; j++)
                {
                    if (ItemsSource[j].ID == i + 1)
                    {
                        ExistID = true;
                        break;
                    }
                }

                if (ExistID == false)
                {
                    Index = i + 1;
                    break;
                }
            }

            if (Index == -1)
                Index = ItemsSource.Count + 1;

            return Index;
        }

        #region NotUsed
        ///// <summary>
        ///// Edit activity and save it
        ///// </summary>
        ///// <param name="EditedActivity">Edited activity</param>
        //public void Edit(Activity EditedActivity)
        //{
        //    int index = ItemsSource.IndexOf(EditedActivity);

        //    if (index == -1)
        //    {
        //        index = IndexOfTask(EditedActivity);
        //    }
        //}

        ///// <summary>
        ///// Get list of tasks
        ///// </summary>
        ///// <returns>List of names of activities</returns>
        //public List<string> GetListOfTasks()
        //{
        //    return ItemsSource.Select(x => x.Name).ToList();
        //} 
        #endregion

        /// <summary>
        /// Remove activity from collection and save it
        /// </summary>
        /// <param name="DetailedTask">Removed activity</param>
        public async void Delete(Activity DetailedTask)
        {
            int index = ItemsSource.IndexOf(DetailedTask);

            if (index == -1)
            {
                index = IndexOfTask(DetailedTask);
            }

            //UpdatePhraseList();

            ItemsSource.RemoveAt(index);
            await SaveDataAsync();
        }

        /// <summary>
        /// Get activity from collection
        /// </summary>
        /// <param name="ID">Index of activity</param>
        /// <returns>Activity</returns>
        public Activity GetActivity(int ID)
        {
            return ItemsSource[IndexOfTask(ID)];
        }

        /// <summary>
        /// Add new activity to collection and save it
        /// </summary>
        /// <param name="Item">New activity</param>
        /// <returns>Result of add action. True = successful</returns>
        public async Task<bool> AddActivity(Activity Item)
        {
            Activity NewActivity = Item;

            if (NewActivity.ID == -1)
            {
                if (ItemsSource != null)
                    NewActivity.ID = GetID();
                else
                    NewActivity.ID = 0;

                ItemsSource.Add(NewActivity);
            }
            else
            {
                int Index = ItemsSource.IndexOf(NewActivity);
                if (Index == -1)
                {
                    Index = IndexOfTask(NewActivity);
                }

                if (Index == -1)
                {
                    return false;
                }

                ItemsSource[Index] = NewActivity;
            }

            if (Item.NotifyDays.Count == 0)
                Item.NotifyDays = new ObservableCollection<DayOfWeek>
                {
                    DayOfWeek.Monday,
                    DayOfWeek.Tuesday,
                    DayOfWeek.Wednesday,
                    DayOfWeek.Thursday,
                    DayOfWeek.Friday,
                    DayOfWeek.Saturday,
                    DayOfWeek.Sunday
                };

            await SaveDataAsync();

            return true;

            //UpdatePhraseList();
        }

        /// <summary>
        /// Get activities
        /// </summary>
        /// <returns>Collection of activities</returns>
        public async Task<ObservableCollection<Activity>> GetActivities(bool SecureChanged = false)
        {
            if (ItemsSource == null || SecureChanged)
            {
                ItemsSource = await ReadDataAsync();
            }
            return ItemsSource;
            return new ObservableCollection<Activity>(ItemsSource.Where(x => GetValidItems(x)));
        }

        /// <summary>
        /// Update completed activity with adding today date to Dates collection
        /// </summary>
        /// <param name="CompletedActivity">Completed Activity</param>
        public async void CompleteTask(Activity CompletedActivity)
        {
            int Index = ItemsSource.IndexOf(CompletedActivity);
            if (Index == -1)
            {
                Index = IndexOfTask(CompletedActivity);
                if (Index == -1)
                    return;
            }

            ItemsSource[Index].AddDate();

            await SaveDataAsync();
        }

        /// <summary>
        /// Return list of names of activities
        /// </summary>
        /// <returns></returns>
        public List<string> GetActivitiesNameList()
        {
            // To-Do: Implementovat kontrolu přihlášení uživatele
            return ItemsSource?.Select(x => x.Name).ToList() ?? new List<string>();
        }

        /// <summary>
        /// Get source collection of Activity items for serializing
        /// </summary>
        /// <returns>Collection of Activity items</returns>
        public override ObservableCollection<Activity> GetSourceCollection()
        {
            return ItemsSource;
        }

        /// <summary>
        /// Set collection of surce items
        /// </summary>
        /// <param name="source">Source collection</param>
        public override void SetSourceCollection(ObservableCollection<Activity> source)
        {
            ItemsSource = source;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
