using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.ViewManagement;
using Framework.Service;

namespace TheDailyNotes.Classes
{
    // Insp: Not in Framework
    /// <summary>
    /// Convertor return *, if Add Button contnet is Add
    /// </summary>
    class AddCloseVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null && value is Visibility)
                return (Visibility)value == Visibility.Visible ? new GridLength(120) : new GridLength(60);
            else
                return new GridLength(120);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    // Insp: Not in Framework
    /// <summary>
    /// Convertor return 120, if is AddActivityComp in Add Activity mode, 60 when is in editing mode
    /// </summary>
    class ButtonsRowHeight : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int FullSize = (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily.Contains("Mobile") && SettingsService.Instance.UseBiggerButtons) ? 200 : 120;

            if (value != null)
                return (string)value == "Add" ? new GridLength(FullSize) : new GridLength(FullSize / 2);
            else
                return new GridLength(FullSize);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    // Insp: Not in Framework
    /// <summary>
    /// Return height for AddEdit button
    /// </summary>
    class AddEditButtonHeightConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return 50;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    
    // Insp: Not in Framework
    /// <summary>
    /// Check value and return zero width if value is false. For true return current non zero width
    /// </summary>
    class BoolToGridVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null && value is bool)
                if ((bool)value)
                {
                    double MinNormalWidth = (double)Application.Current.Resources["NormalMinWidth"];
                    if (MinNormalWidth > ApplicationView.GetForCurrentView().VisibleBounds.Width)
                    {
                        return new GridLength(1, GridUnitType.Star);
                    }
                    else
                        return new GridLength(365);
                }
                else
                    return new GridLength(0);
            else
                return new GridLength(0);

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }


    // Insp: Not in Framework
    /// <summary>
    /// Return width for slave pane
    /// </summary>
    class SlaveFrameWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null && value is bool && parameter != null && parameter is string)
            {

                if (!(bool)value)
                {
                    return new GridLength(0);
                }

                switch ((string)parameter)
                {
                    case "NarrowMinWidth":
                        return new GridLength(1, GridUnitType.Star);

                    case "NormalMinWidth":
                        return new GridLength(365);

                    case "WideMinWidth":
                        return new GridLength(500);

                    default:
                        return new GridLength(0);
                }
            }
            else
                return new GridLength(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }    
}