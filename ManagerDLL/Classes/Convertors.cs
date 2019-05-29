using Framework.Service;
using Framework.Template;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.UserProfile;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Framework.Classes
{
    /// <summary>
    /// Converter return true, if value is null. Switch enabled
    /// </summary>
    public class NullToBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter is string param)
                if (param.ToLower() == "not")
                    return value != null;

            return value == null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter return Add or Edit based on value string
    /// </summary>
    public class AddButtonTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string strValue)
                return strValue.ToLower().Contains("add") ? "Add" : "Edit";
            else
                return "Add";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter return true, if selected TaskBaseItem is set to today day
    /// </summary>
    public class EnabledTodayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is TaskBaseItem tBItem)
            {
                return tBItem.NotifyDays.Contains(DateTime.Today.DayOfWeek) && tBItem.Start.Date <= DateTime.Today && (tBItem.End.Date >= DateTime.Today || tBItem.Neverend) && SettingsService.Instance.UseSlidableItems;
            }
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter return true, if selected TaskBaseItem is set to today day
    /// </summary>
    public class ContainsTodayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ObservableCollection<DateTime> dates)
            {
                return dates.Contains(DateTime.Today);
            }
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter return color for current state
    /// </summary>
    public class CurrentStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is TaskBaseItem tBItem)
            {
                return tBItem.Start.Date > DateTime.Today ? new SolidColorBrush(Color.FromArgb(255, 0, 174, 255)) :
                        (tBItem.End.Date < DateTime.Today && !tBItem.Neverend) ? new SolidColorBrush(Color.FromArgb(255, 255, 162, 0)) :
                        tBItem.Dates.Contains(DateTime.Today) ? new SolidColorBrush(Color.FromArgb(123, 9, 201, 0)) : new SolidColorBrush(Color.FromArgb(0, 64, 64, 64));
            }
            else
                return new SolidColorBrush(Color.FromArgb(255, 255, 162, 0));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter return red color if task base item is completed
    /// </summary>
    public class EndTaskBaseItemColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is TaskBaseItem tBItem)
            {
                if (tBItem.Neverend)
                    return Color.FromArgb(0, 247, 17, 17);
                else
                    return tBItem.End < DateTime.Today ? Color.FromArgb(96, 247, 17, 17) : Color.FromArgb(0, 247, 17, 17);
            }
            else
                return Color.FromArgb(96, 247, 17, 17);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter return *, if visibility value is visible
    /// </summary>
    public class VisibilityToGridLength : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Visibility visiValue)
                return visiValue == Visibility.Visible ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
            else
                return new GridLength(1, GridUnitType.Star);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter return selected value based on parameter, if task base item is completed
    /// </summary>
    public class CompletedTodayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ObservableCollection<DateTime> dateCollection && parameter is string param)
                switch (param.ToLower())
                {
                    case "flyout":
                        return dateCollection.Contains(DateTime.Today) ? new SolidColorBrush(Color.FromArgb(187, 93, 93, 93)) : new SolidColorBrush(Color.FromArgb(0, 93, 93, 93));

                    case "symbol":
                    case "icon":
                        return dateCollection.Contains(DateTime.Today) ? Symbol.Clear : Symbol.Accept;

                    case "text":
                        return dateCollection.Contains(DateTime.Today) ? "Uncomplete" : "Complete";

                    case "slider":
                        return dateCollection.Contains(DateTime.Today) ? new SolidColorBrush(Color.FromArgb(96, 247, 27, 17)) : new SolidColorBrush(Color.FromArgb(97, 2, 199, 2));

                    default:
                        LogService.AddLogMessage("Parameter is wrong: " + param);
                        return "err";
                }

            LogService.AddLogMessage($"CompletedTodayConverter: Value or parameter is wrong: Value: {value.ToString()}; Parameter: {parameter.ToString()}");
            return "err";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter return current status of task base item
    /// </summary>
    public class GetStatus : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is TaskBaseItem tBItem && parameter is string itemName)
            {
                return tBItem.Start > DateTime.Today ? "Not started" :
                        tBItem.Neverend ? "Neverending " + itemName :
                        tBItem.End < DateTime.Today ? "Ended" : "In progress";
            }
            else
                return "Problem with getting status";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter return date or time from DateTime type based on parameter
    /// </summary>
    public class ExtractDateTimeInfoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime dateTime && parameter is string param)
            {
                try
                {
                    switch (param.ToLower())
                    {
                        case "date":
                            return dateTime.ToString("d", new CultureInfo(GlobalizationPreferences.Languages[0].ToString()));

                        case "time":
                            return dateTime.ToString("t", new CultureInfo(GlobalizationPreferences.Languages[0].ToString()));

                        default:
                            LogService.AddLogMessage("Parameter is wrong: " + param);
                            return "err";
                    }

                }
                catch (Exception e)
                {
                    LogService.AddLogMessage($"ExtractDateTimeInfoConverter: {e.Message}");
                    return ((DateTime)value);
                }
            }

            LogService.AddLogMessage($"ExtractDateTimeInfoConverter: Value or parameter is wrong: Value: {value.ToString()}; Parameter: {parameter.ToString()}");
            return "err";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Check date and return it or if is null, return DateTime.Today
    /// </summary>
    public class DateToDateTimeOffset : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime)
                return new DateTimeOffset((DateTime)value);

            return new DateTimeOffset(DateTime.Today);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTimeOffset)
                return ((DateTimeOffset)value).DateTime;

            return DateTime.Today;
        }
    }

    /// <summary>
    /// Swtich text of label based on value, which is type of SelectionMode
    /// </summary>
    public class LabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value.ToString() == SelectionMode.Multiple.ToString() ? "Cancel select" : "Select";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Return true of value is not zero
    /// </summary>
    public class IntToBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int intValue)
                return intValue != 0;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// If value is null, return Visibility.Collapsed. Switch enabled
    /// </summary>
    public class NullToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter is string param)
                if (param.ToLower() == "not")
                    return value == null ? Visibility.Visible : Visibility.Collapsed;

            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Switch value based on parameter for SlavePane
    /// </summary>
    public class SlavePaneVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is GridLength lengthValue && parameter is string param)
            {
                switch (param.ToLower())
                {
                    case "symbol":
                    case "icon":
                        return lengthValue == new GridLength(0) ? new SymbolIcon(Symbol.ClosePane) : new SymbolIcon(Symbol.OpenPane);

                    case "text":
                        return lengthValue == new GridLength(0) ? "Open Pane" : "Close Pane";

                    default:
                        LogService.AddLogMessage("Parameter is wrong: " + param);
                        return "err";
                }
            }

            LogService.AddLogMessage($"SlavePaneVisibilityConverter: Value or parameter is wrong: Value: {value.ToString()}; Parameter: {parameter.ToString()}");
            return "err";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Switch value based on parameter for SecodaryTile
    /// </summary>
    public class SecondaryTileExistConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool boolValue && parameter is string param)
            {
                switch (param.ToLower())
                {
                    case "icon":
                    case "symbol":
                        return boolValue ? new SymbolIcon(Symbol.UnPin) : new SymbolIcon(Symbol.Pin);

                    case "text":
                        return boolValue ? "Unpin from Start" : "Pin to Start";

                    default:
                        LogService.AddLogMessage("Parameter is wrong: " + param);
                        return "err";
                }
            }

            LogService.AddLogMessage($"SecondaryTileExistConverter: Value or parameter is wrong: Value: {value.ToString()}; Parameter: {parameter?.ToString()}");
            return "err";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Switch visibility based on value, which is bool. Switch is enabled
    /// </summary>
    public class BoolToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter is string param && value is bool boolValue)
                if (param.ToLower() == "not")
                    return boolValue ? Visibility.Collapsed : Visibility.Visible;
                else
                    return boolValue ? Visibility.Visible : Visibility.Collapsed;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Add "Error: " to message (value)
    /// </summary>
    public class ErrorMessageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null && value is string)
                return "Error: " + (string)value;
            else
                return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Return statistics for selected task base item. Item is specified by parameter
    /// </summary>
    public class GetStatistic : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is TaskBaseItem tBItem && parameter is string param)
            {
                var Fce = new Functions();

                var AllDays = Fce.GetDaysCount(Fce.GetSpecificDays(tBItem.NotifyDays, Fce.GetNumberOfWeekDays(tBItem.Start, tBItem.End)));

                string tBItemProgress, tBItemCompleted;

                if (tBItem.Start <= DateTime.Today && tBItem.End >= DateTime.Today && !tBItem.Neverend)
                {
                    tBItemCompleted = (Fce.GetDaysCount(tBItem.NotifyDays, tBItem.Dates) * 100 / AllDays).ToString() + "%";
                    tBItemProgress = (Fce.GetDaysCount(Fce.GetSpecificDays(tBItem.NotifyDays, Fce.GetNumberOfWeekDays(tBItem.Start, DateTime.Today))) * 100 / AllDays).ToString() + "%";
                }
                else
                {
                    tBItemProgress = (string)(new GetStatus()).Convert(tBItem, null, null, null);
                    tBItemCompleted = tBItemProgress;
                }

                tBItemCompleted = $"{param} completed: {tBItemCompleted}";
                tBItemProgress = $"\n{param} progress: {tBItemProgress}";

                return tBItemCompleted + tBItemProgress;
            }
            else
                return "Problem with getting statistic";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Convert ID to selected value based on parameter
    /// </summary>
    public class IdConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int intValue && parameter is string param)
            {
                switch (param.ToLower())
                {
                    case "visibility":
                        return intValue == -1 ? Visibility.Visible : Visibility.Collapsed;

                    case "text":
                        return intValue == -1 ? "Add" : "Edit";

                    default:
                        LogService.AddLogMessage("Parameter is wrong: " + param);
                        return "err";
                }
            }

            LogService.AddLogMessage($"IdConverter: Value or parameter is wrong: Value: {value.ToString()}; Parameter: {parameter.ToString()}");
            return "err";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// If item is secured, return non zero width
    /// </summary>
    public class SecuredToWidth : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool boolValue)
                return boolValue ? new GridLength(40) : new GridLength(0);
            else
                return new GridLength(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Select from value (DateTime) TimeOfDay
    /// </summary>
    public class DateTimeToTimeSpan : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((DateTime)value).TimeOfDay;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var helper = DateTime.Now;
            TimeSpan UV = (TimeSpan)value;
            helper = new DateTime(helper.Year, helper.Month, helper.Day, UV.Hours, UV.Minutes, UV.Seconds);

            return helper;
        }
    }

    /// <summary>
    /// Create shortcut from value and parameter
    /// </summary>
    public class StringConcatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string text && parameter is string param)
            {
                switch (param.ToLower())
                {
                    case "reverse":
                        return param + " " + text;

                    case "normal":
                    default:
                        return text + " " + param;
                }
            }

            LogService.AddLogMessage($"StringConcatConverter: Value or parameter is wrong: Value: {value.ToString()}; Parameter: {parameter.ToString()}");
            return "err";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// For theme selector
    /// </summary>
    public class EnumToBooleanConverter : IValueConverter
    {
        public Type EnumType { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter is string enumString)
            {
                if (SettingsService.Instance.UseSystemAppTheme)
                    return enumString == "System";
                else
                    if (enumString == "System")
                    return false;

                if (enumString == "System")
                    return SettingsService.Instance.UseSystemAppTheme;

                if (!System.Enum.IsDefined(EnumType, value))
                {
                    throw new ArgumentException("value must be an Enum!");
                }

                var enumValue = System.Enum.Parse(EnumType, enumString);

                return enumValue.Equals(value);
            }

            throw new ArgumentException("parameter must be an Enum name!");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (parameter is string enumString)
            {
                SettingsService.Instance.UseSystemAppTheme = enumString == "System";

                return System.Enum.Parse(EnumType, enumString);
            }

            throw new ArgumentException("parameter must be an Enum name!");
        }
    }

    /// <summary>
    /// Invert bool value
    /// </summary>
    public class InvertBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool)
                return !(bool)value;

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is bool)
                return !(bool)value;

            return value;
        }
    }

    /// <summary>
    /// Invert Visibility value
    /// </summary>
    public class InvertVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Visibility)
                return (Visibility)value == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is Visibility)
                return (Visibility)value == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

            return value;
        }
    }

    /// <summary>
    /// Just for testing data
    /// </summary>
    public class TestingData : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
                return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
