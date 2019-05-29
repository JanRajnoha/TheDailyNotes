using System;
using Template10.Common;
using Template10.Utils;
using Windows.UI.Xaml;

namespace TheDailyNotes.Services.SettingsServices
{
    public class SettingsService
    {
        public static SettingsService Instance { get; } = new SettingsService();
        Template10.Services.SettingsService.ISettingsHelper _helper;

        private SettingsService()
        {
            _helper = new Template10.Services.SettingsService.SettingsHelper();
        }

        public ApplicationTheme SystemTheme { get; set; }

        public bool UseShellBackButton
        {
            get { return _helper.Read<bool>(nameof(UseShellBackButton), true); }
            set
            {
                _helper.Write(nameof(UseShellBackButton), value);
                BootStrapper.Current.NavigationService.Dispatcher.Dispatch(() =>
                {
                    BootStrapper.Current.ShowShellBackButton = value;
                    BootStrapper.Current.UpdateShellBackButton();
                    BootStrapper.Current.NavigationService.Refresh();
                });
            }
        }

        public ApplicationTheme AppTheme
        {
            get
            {
                var theme = ApplicationTheme.Light;
                var value = _helper.Read<string>(nameof(AppTheme), theme.ToString());
                return Enum.TryParse<ApplicationTheme>(value, out theme) ? theme : ApplicationTheme.Dark;
            }
            set
            {
                _helper.Write(nameof(AppTheme), value.ToString());
                if (Window.Current != null)
                    (Window.Current.Content as FrameworkElement).RequestedTheme = value.ToElementTheme();
                //Views.Shell.HamburgerMenu.RefreshStyles(value);
            }
        }

        public bool UseSystemAppTheme
        {
            get => _helper.Read<bool>(nameof(UseSystemAppTheme), true);
            set => _helper.Write(nameof(UseSystemAppTheme), value);
        }

        public bool MainPageInBackStack
        {
            get => _helper.Read<bool>(nameof(MainPageInBackStack), true);
            set => _helper.Write(nameof(MainPageInBackStack), value);
        }

        //public bool SaveTemporaryNewItemCompData
        //{
        //    get => _helper.Read<bool>(nameof(SaveTemporaryNewItemCompData), true);
        //    set => _helper.Write(nameof(SaveTemporaryNewItemCompData), value);
        //}

        public bool ShowPanelAfterLeavePage
        {
            get => _helper.Read<bool>(nameof(ShowPanelAfterLeavePage), true);
            set => _helper.Write(nameof(ShowPanelAfterLeavePage), value);
        }

        //public bool AddMoreItem
        //{
        //    get => _helper.Read<bool>(nameof(AddMoreItem), true);
        //    set => _helper.Write(nameof(AddMoreItem), value);
        //}
        
        public bool ShowMoreInfo
        {
            get => _helper.Read<bool>(nameof(ShowMoreInfo), false);
            set => _helper.Write(nameof(ShowMoreInfo), value);
        }

        public bool ShowSmallerNotes
        {
            get => _helper.Read<bool>(nameof(ShowSmallerNotes), false);
            set => _helper.Write(nameof(ShowSmallerNotes), value);
        }

        public bool ShowLongerTitle
        {
            get => _helper.Read<bool>(nameof(ShowLongerTitle), true);
            set => _helper.Write(nameof(ShowLongerTitle), value);
        }

        public bool UseSlidableItems
        {
            get => _helper.Read<bool>(nameof(UseSlidableItems), true);
            set => _helper.Write(nameof(UseSlidableItems), value);
        }

        public bool UseBiggerButtons
        {
            get => _helper.Read<bool>(nameof(UseBiggerButtons), true);
            set => _helper.Write(nameof(UseBiggerButtons), value);
        }

        public bool UseHelloSecurity
        {
            get => _helper.Read<bool>(nameof(UseHelloSecurity), true);
            set => _helper.Write(nameof(UseHelloSecurity), value);
        }

        public TimeSpan CacheMaxDuration
        {
            get { return _helper.Read<TimeSpan>(nameof(CacheMaxDuration), TimeSpan.FromDays(2)); }
            set
            {
                _helper.Write(nameof(CacheMaxDuration), value);
                BootStrapper.Current.CacheMaxDuration = value;
            }
        }

        public string CurrentAppVersion
        {
            get => _helper.Read(nameof(CurrentAppVersion), "0.0.0");
            set
            {
                while (_helper.Read(nameof(CurrentAppVersion), "0.0.0") != value)
                {
                    _helper.Write(nameof(CurrentAppVersion), value);
                }
            }
        }

        public ApplicationTheme GetTheme()
        {
            return ApplicationTheme.Dark; 
        }
    }
}

