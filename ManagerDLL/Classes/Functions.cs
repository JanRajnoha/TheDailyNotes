using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Classes
{
    public class Functions
    {
        /// <summary>
        /// Invert color
        /// </summary>
        /// <param name="defaultColor">Color to inversion</param>
        /// <returns>Inverted color</returns>
        public static System.Drawing.Color InvertColor(System.Drawing.Color defaultColor)
        {
            return System.Drawing.Color.FromArgb(defaultColor.ToArgb() ^ 0xffffff);
        }

        /// <summary>
        /// Invert color
        /// </summary>
        /// <param name="defaultColor">Color to inversion</param>
        /// <returns>Inverted color</returns>
        public static Windows.UI.Color InvertColor(Windows.UI.Color defaultColor)
        {
            return Windows.UI.Color.FromArgb(
                defaultColor.A,
                (byte)(255 - defaultColor.R),
                (byte)(255 - defaultColor.G),
                (byte)(255 - defaultColor.B));
        }

        /// <summary>
        /// Return array with numbers of days for every day of week
        /// </summary>
        /// <param name="NumberOfDays">Days count</param>
        /// <param name="StartingDay">Starting day</param>
        /// <returns>Number of days in specified Day of Week</returns>
        public int[] GetNumberOfWeekDays(int NumberOfDays, int StartingDay)
        {
            int[] DaysOfWeek = new int[7];

            for (int i = 0; i < DaysOfWeek.Length; i++)
            {
                DaysOfWeek[i] = NumberOfDays / 7 - 1;
            }

            int RestDays = NumberOfDays - ((NumberOfDays / 7 - 1) * 7);

            for (int i = 0; i < RestDays; i++)
            {
                int FullWeeks = 0;
                while (StartingDay + i - FullWeeks * 7 > 6)
                    FullWeeks++;

                DaysOfWeek[StartingDay + i - FullWeeks * 7]++;
            }

            return DaysOfWeek;
        }

        /// <summary>
        /// Return array with numbers of days for every day of week
        /// </summary>
        /// <param name="StartDate">Start date</param>
        /// <param name="EndDate">End date</param>
        /// <returns>Number of days in specified Day of Week</returns>
        public int[] GetNumberOfWeekDays(DateTime StartDate, DateTime EndDate)
        {
            return GetNumberOfWeekDays((EndDate - StartDate).Days + 1, (int)StartDate.DayOfWeek);
        }

        /// <summary>
        /// Function select and return selected days from array of days
        /// </summary>
        /// <param name="SpecificDays">Collection of selected days</param>
        /// <param name="Days">Array of days</param>
        /// <returns>Filtered array of days</returns>
        public int[] GetSpecificDays(ObservableCollection<DayOfWeek> SpecificDays, int[] Days)
        {
            int[] SelectedDays = new int[SpecificDays.Count];
            int i = 0;

            foreach (var Item in SpecificDays)
            {
                SelectedDays[i] = Days[(int)Item];
                i++;
            }

            return SelectedDays;
        }

        /// <summary>
        /// Function count all days in array of days
        /// </summary>
        /// <param name="Days">Array of days</param>
        /// <returns>Count of days in array of days</returns>
        public int GetDaysCount(int[] Days)
        {
            int AllDays = 0;

            for (int i = 0; i < Days.Length; i++)
            {
                AllDays += Days[i];
            }

            return AllDays;
        }

        /// <summary>
        /// Function count all days in collection of days filtered by collection of specific days
        /// </summary>
        /// <param name="SpecificDays">Collection of specific days</param>
        /// <param name="Days">Collection of days</param>
        /// <returns>Count of days in collection of days filtered by specific days</returns>
        public int GetDaysCount(ObservableCollection<DayOfWeek> SpecificDays, ObservableCollection<DateTime> Days)
        {
            int AllDays = 0;

            foreach (var Item in Days)
            {
                if (SpecificDays.Contains(Item.DayOfWeek))
                    AllDays++;
            }

            return AllDays;
        }

        /// <summary>
        /// Function check, if is passed array of days empty -> values in array are equal 0
        /// </summary>
        /// <param name="Days">Array of days</param>
        /// <returns>True, if is array of days empty</returns>
        public bool IsEmpty(int[] Days)
        {
            return GetDaysCount(Days) == 0;
        }

        /// <summary>
        /// Check range of dates for free dates (rozsah muze byt sice ok, ale nebudou tam dny, ve ktere je polozka nastavena)
        /// </summary>
        /// <returns>True, if is range of days valid</returns>
        public static bool CheckDayRangeInput(DateTime Start, DateTime End, ObservableCollection<DayOfWeek> SelectedDays)
        {
            if (Start.Date != null && End.Date != null)
            {
                if (SelectedDays.Count == 0 || SelectedDays.Count == 7)
                {
                    return true;
                }
                else
                {
                    Functions Fce = new Functions();

                    var DaysOfWeek = Fce.GetNumberOfWeekDays(
                        new DateTime(Start.Date.Year, Start.Date.Month, Start.Date.Day),
                        new DateTime(End.Date.Year, End.Date.Month, End.Date.Day));

                    return !Fce.IsEmpty(Fce.GetSpecificDays(SelectedDays, DaysOfWeek));
                }
            }
            else
            {
                return false;
            }
        }
    }
}
