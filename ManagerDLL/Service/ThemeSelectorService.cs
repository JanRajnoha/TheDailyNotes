using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.ViewManagement;
using Framework.Enum;

namespace Framework.Service
{
    public static class ThemeSelectorService
    {
        public static event EventHandler<AppThemes> OnThemeChanged = (sender, args) => { };

        public static AppThemes Theme { get; set; } = AppThemes.System;

        public static void Initialize()
        {
            Theme = LoadThemeFromSettings();
        }

        public static void SetTheme(AppThemes theme)
        {
            Theme = theme;

            SetRequestedTheme();
            SaveThemeInSettings(Theme);

            OnThemeChanged(null, Theme);
        }

        public static void SetRequestedTheme()
        {
            if (Window.Current?.Content is FrameworkElement frameworkElement)
            {
                if (!SettingsService.Instance.UseSystemAppTheme)
                {
                    ElementTheme trueTheme;

                    trueTheme = (ElementTheme)Theme;

                    if (frameworkElement.RequestedTheme == ElementTheme.Dark)
                    {
                        frameworkElement.RequestedTheme = ElementTheme.Light;
                    }

                    frameworkElement.RequestedTheme = trueTheme;
                }
                else
                    if ((new UISettings().GetColorValue(UIColorType.Background)).R == 0)
                    frameworkElement.RequestedTheme = ElementTheme.Dark;
                else
                    frameworkElement.RequestedTheme = ElementTheme.Light;
            }
        }

        private static AppThemes LoadThemeFromSettings()
        {
            AppThemes cacheTheme = AppThemes.System;

            if (!SettingsService.Instance.UseSystemAppTheme)
                cacheTheme = (AppThemes)SettingsService.Instance.AppTheme;
            else
                cacheTheme = (AppThemes)SettingsService.Instance.SystemTheme;

            return cacheTheme;
        }

        private static void SaveThemeInSettings(AppThemes theme)
        {
            if (!SettingsService.Instance.UseSystemAppTheme)
                SettingsService.Instance.AppTheme = (ApplicationTheme)theme;
        }
    }
}
