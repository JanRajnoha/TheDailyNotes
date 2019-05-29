using CustomSettingsDLL;

using Framework.Classes;
using Framework.Commands;
using Framework.Enum;
using Framework.Messages;
using Framework.Service;
using Framework.Template;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

using Template10.Mvvm;
using Template10.Services.NavigationService;

using TheDailyNotes.Classes;
using TheDailyNotes.Views;
using Windows.ApplicationModel;
using Windows.Security.Credentials;
using Windows.Services.Store;
using Windows.System;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace TheDailyNotes.ViewModels
{
    public class SettingsPageViewModel : TDNVMBase<BaseItem>
    {
        public SettingsPageViewModel(Messenger messenger) : this()
        {
            Messenger = messenger;
        }

        public SettingsPageViewModel()
        {
            Messenger = (Application.Current.Resources["VMLocator"] as VMLocator).GetMessenger();
        }

        public SettingsPartViewModel SettingsPartViewModel { get; } = new SettingsPartViewModel();
        public AboutPartViewModel AboutPartViewModel { get; } = new AboutPartViewModel();

        public override Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            return base.OnNavigatingFromAsync(args);
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            SettingsPartViewModel.PremiumFeaturesActivated = await SettingsPartViewModel.GetAddOnLicenseInfo();

            await Task.CompletedTask;
        }

        public async void GotoSettingsApp() =>
           await Launcher.LaunchUriAsync(new Uri("ms-settings:signinoptions"));

        public async void RefreshHelloSecurity()
        {
            var HelloAvailable = await KeyCredentialManager.IsSupportedAsync();
            if (HelloAvailable)
                SettingsPartViewModel.EnableHelloSecurity = Visibility.Collapsed;
            else
            {
                SettingsPartViewModel.UseHelloSecurity = false;
                SettingsPartViewModel.EnableHelloSecurity = Visibility.Visible;
            }
        }
    }

    public class SettingsPartViewModel : ViewModelBase
    {
        SettingsService _settings;

        public SettingsPartViewModel()
        {
            if (DesignMode.DesignModeEnabled)
            {
                // designtime
            }
            else
            {
                _settings = SettingsService.Instance;

                if (_settings.UseSystemAppTheme)
                    SelectedAppTheme = (AppThemes)_settings.SystemTheme;
                else
                    SelectedAppTheme = (AppThemes)_settings.AppTheme;
            }
        }

        public async Task<bool> GetAddOnLicenseInfo()
        {
            bool IsActive = false;

            var users = await User.FindAllAsync();
            StoreContext context = StoreContext.GetForUser(users[0]);

            StoreAppLicense appLicense = await context.GetAppLicenseAsync();

            System.Diagnostics.Debug.WriteLine(appLicense.AddOnLicenses.Count);

            foreach (KeyValuePair<string, StoreLicense> item in appLicense.AddOnLicenses)
            {
                StoreLicense addOnLicense = item.Value;

                if (addOnLicense.SkuStoreId.ToLower().Contains("9pc587j41rs1"))
                {
                    PremiumFeaturesActivatedType = "Month";
                }
                else if (addOnLicense.SkuStoreId.ToLower().Contains("9pf5zlm4lprg"))
                {
                    PremiumFeaturesActivatedType = "Year";
                }
                else
                    return false;

                PremiumFeaturesActivatedRemain = (addOnLicense.ExpirationDate - DateTime.Now).Days.ToString();

                IsActive = addOnLicense.IsActive;
            }

            return IsActive;
        }

        private ICommand switchThemeCommand;
        public ICommand SwitchThemeCommand
        {
            get
            {
                if (switchThemeCommand == null)
                {
                    switchThemeCommand = new RelayCommand<AppThemes>(
                        (param) =>
                        {
                            _settings.UseSystemAppTheme = false;
                            ThemeSelectorService.SetTheme(param);
                        });
                }

                return switchThemeCommand;
            }
        }

        private ICommand useSystemThemeCommand;
        public ICommand UseSystemThemeCommand
        {
            get
            {
                if (useSystemThemeCommand == null)
                {
                    useSystemThemeCommand = new RelayCommand(() =>
                    {
                        _settings.UseSystemAppTheme = true;
                        ThemeSelectorService.SetTheme(AppThemes.Dark);
                    });
                }

                return useSystemThemeCommand;
            }
        }

        public ICommand BackupItemsCommand => new RelayCommand(() =>
        {
            BackupItemsCommand bIC = new BackupItemsCommand();
            bIC.CommandCompletedChanged += Command_CommandCompletedChanged;
            bIC.Execute("tdn");
        });

        private void Command_CommandCompletedChanged(object sender, Framework.EventArgs.CommandCompletedEventArgs e)
        {
            (Application.Current.Resources["VMLocator"] as VMLocator).GetMessenger().Send(new ShowNotificationMsg() { Text = e.ResultText });
        }

        public ICommand ImportItemsCommand => new RelayCommand(() =>
        {
            ImportItemsCommand iIC = new ImportItemsCommand();
            iIC.CommandCompletedChanged += Command_CommandCompletedChanged;
            iIC.Execute("tdn");
        });

        private AppThemes selectedAppTheme;
        public AppThemes SelectedAppTheme
        {
            get
            {
                return selectedAppTheme;
            }
            set
            {
                if (selectedAppTheme != value)
                {
                    selectedAppTheme = value;
                    RaisePropertyChanged(nameof(SelectedAppTheme));
                }
            }
        }

        private ICommand buyPremium;
        public ICommand BuyPremium
        {
            get
            {
                if (buyPremium == null)
                {
                    buyPremium = new RelayCommand(async () =>
                    {
                        try
                        {
                            var users = await User.FindAllAsync();
                            StoreContext context = StoreContext.GetForUser(users[0]);

                            var res = await context.RequestPurchaseAsync("9pc587j41rs1");

                            switch (res.Status)
                            {
                                case StorePurchaseStatus.Succeeded:
                                case StorePurchaseStatus.AlreadyPurchased:
                                    PremiumFeaturesActivated = true;
                                    await GetAddOnLicenseInfo();
                                    break;

                                case StorePurchaseStatus.NotPurchased:
                                    PremiumFeaturesActivated = false;
                                    break;

                                case StorePurchaseStatus.NetworkError:
                                    await (new MessageDialog("Please, check your internet connection", "Network error")).ShowAsync();
                                    break;

                                case StorePurchaseStatus.ServerError:
                                    await (new MessageDialog("Please, try again after few minutes.", "External error")).ShowAsync();
                                    break;

                                default:
                                    break;
                            }
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine(e);
                        }


                    });

                    RaisePropertyChanged(nameof(BuyPremium));
                }

                return buyPremium;
            }
        }

        private bool premiumFeaturesActivated = false;
        public bool PremiumFeaturesActivated
        {
            get
            {
                return premiumFeaturesActivated;
            }
            set
            {
                premiumFeaturesActivated = value;

                PremiumFeaturesActivatedStatus = value ? "Activated" : "Not activated";

                RaisePropertyChanged(nameof(PremiumFeaturesActivated));
            }
        }

        private string premiumFeaturesActivatedStatus = string.Empty;
        public string PremiumFeaturesActivatedStatus
        {
            get
            {
                return premiumFeaturesActivatedStatus;
            }
            set
            {
                premiumFeaturesActivatedStatus = value;
                RaisePropertyChanged(nameof(PremiumFeaturesActivatedStatus));
            }
        }

        private string premiumFeaturesActivatedRemain = string.Empty;
        public string PremiumFeaturesActivatedRemain
        {
            get
            {
                return premiumFeaturesActivatedRemain;
            }
            set
            {
                premiumFeaturesActivatedRemain = value;
                RaisePropertyChanged(nameof(PremiumFeaturesActivatedRemain));
            }
        }

        private string premiumFeaturesActivatedType = string.Empty;
        public string PremiumFeaturesActivatedType
        {
            get
            {
                return premiumFeaturesActivatedType;
            }
            set
            {
                premiumFeaturesActivatedType = value;
                RaisePropertyChanged(nameof(PremiumFeaturesActivatedType));
            }
        }


        public bool UseShellBackButton
        {
            get { return _settings.UseShellBackButton; }
            set { _settings.UseShellBackButton = value; base.RaisePropertyChanged(); }
        }

        public bool MainPageInBackStack
        {
            get { return _settings.MainPageInBackStack; }
            set { _settings.MainPageInBackStack = value; base.RaisePropertyChanged(); }
        }

        //public bool SaveTemporaryNewItemCompData
        //{
        //    get { return _settings.SaveTemporaryNewItemCompData; }
        //    set { _settings.SaveTemporaryNewItemCompData = value; base.RaisePropertyChanged(); }
        //}

        public bool ShowPanelAfterLeavePage
        {
            get { return _settings.ShowPanelAfterLeavePage; }
            set { _settings.ShowPanelAfterLeavePage = value; base.RaisePropertyChanged(); }
        }

        public bool ShowMoreInfo
        {
            get { return _settings.ShowMoreInfo; }
            set { _settings.ShowMoreInfo = value; base.RaisePropertyChanged(); }
        }

        public bool ShowSmallerNotes
        {
            get { return _settings.ShowSmallerNotes; }
            set { _settings.ShowSmallerNotes = value; base.RaisePropertyChanged(); }
        }

        public bool ShowLongerTitle
        {
            get { return _settings.ShowLongerTitle; }
            set { _settings.ShowLongerTitle = value; base.RaisePropertyChanged(); }
        }

        public bool UseSlidableItems
        {
            get { return _settings.UseSlidableItems; }
            set { _settings.UseSlidableItems = value; base.RaisePropertyChanged(); }
        }

        public bool UseBiggerButtons
        {
            get { return _settings.UseBiggerButtons; }
            set { _settings.UseBiggerButtons = value; base.RaisePropertyChanged(); }
        }

        public bool UseHelloSecurity
        {
            get
            {
                return _settings.UseHelloSecurity;
            }

            set
            {
                HelloSecurityChangedEventArgs args = new HelloSecurityChangedEventArgs()
                {
                    HelSecOldState = _settings.UseHelloSecurity,
                    HelSecNewState = value
                };
                _settings.UseHelloSecurity = value;
                HelloSecurityChangedEvent(args);
                base.RaisePropertyChanged();
            }
        }

        public string AppDisplayName
        {
            get
            {
                return Package.Current.DisplayName + (Constants.Instance.BetaMode ? " - Beta version" : "");
            }
        }

        void HelloSecurityChangedEvent(HelloSecurityChangedEventArgs e)
        {
            HelloSecurityChanged?.Invoke(null, e);
        }

        public bool ActivateAdBlocker
        {
            get { return _settings.ActivateAdBlocker; }
            set
            {
                CustomSettings.ShowAds = value;
                _settings.ActivateAdBlocker = value;
                base.RaisePropertyChanged();
            }
        }

        public bool ActivateBackupItems
        {
            get { return _settings.ActivateBackupItems; }
            set { _settings.ActivateBackupItems = value; base.RaisePropertyChanged(); }
        }

        public bool ActivateBackupItemsType
        {
            get { return _settings.ActivateBackupItemsType; }
            set { _settings.ActivateBackupItemsType = value; base.RaisePropertyChanged(); }
        }

        public bool ActivateBackupItemsInterval
        {
            get { return _settings.ActivateBackupItemsInterval; }
            set { _settings.ActivateBackupItemsInterval = value; base.RaisePropertyChanged(); }
        }

        public bool ActivateShareItems
        {
            get { return _settings.ActivateShareItems; }
            set { _settings.ActivateShareItems = value; base.RaisePropertyChanged(); }
        }

        public bool ActivateActionCenterControl
        {
            get { return _settings.ActivateActionCenterControl; }
            set { _settings.ActivateActionCenterControl = value; base.RaisePropertyChanged(); }
        }

        public bool ActivateSecondaryItemTiles
        {
            get { return _settings.ActivateSecondaryItemTiles; }
            set { _settings.ActivateSecondaryItemTiles = value; base.RaisePropertyChanged(); }
        }

        public bool ActivateCustomSecuredNotificationInfo
        {
            get { return _settings.ActivateCustomSecuredNotificationInfo; }
            set { _settings.ActivateCustomSecuredNotificationInfo = value; base.RaisePropertyChanged(); }
        }

        public string CustomSecuredNotificationInfoText
        {
            get { return _settings.CustomSecuredNotificationInfoText; }
            set { _settings.CustomSecuredNotificationInfoText = value; base.RaisePropertyChanged(); }
        }

        public static event EventHandler<HelloSecurityChangedEventArgs> HelloSecurityChanged;

        public class HelloSecurityChangedEventArgs : EventArgs
        {
            public bool HelSecOldState { get; set; }
            public bool HelSecNewState { get; set; }
        }

        private Visibility enableHelloSecurity;
        public Visibility EnableHelloSecurity
        {
            get { return enableHelloSecurity; }
            set { enableHelloSecurity = value; base.RaisePropertyChanged(); }
        }

        public bool UseLightThemeButton
        {
            get { return _settings.AppTheme.Equals(ApplicationTheme.Light); }
            set
            {
                _settings.AppTheme = value ? ApplicationTheme.Light : ApplicationTheme.Dark; base.RaisePropertyChanged();
            }
        }

        public void LogButton_Tapped(object sender, RoutedEventArgs e)
        {
            AccountsSettingsPane.Show();

            //if (!CustomSettings.IsUserLogged)
            //{
            //    switch ((await KeyCredentialManager.RequestCreateAsync("User", KeyCredentialCreationOption.ReplaceExisting)).Status)
            //    {
            //        case KeyCredentialStatus.Success:
            //            CustomSettings.IsUserLogged = true;
            //            break;

            //        case KeyCredentialStatus.UnknownError:
            //        case KeyCredentialStatus.NotFound:
            //        case KeyCredentialStatus.UserCanceled:
            //        case KeyCredentialStatus.UserPrefersPassword:
            //        case KeyCredentialStatus.CredentialAlreadyExists:
            //        case KeyCredentialStatus.SecurityDeviceLocked:
            //        default:
            //            CustomSettings.IsUserLogged = false;
            //            break;
            //    }
            //}
            //else
            // CustomSettings.IsUserLogged = false;
        }
    }

    public class AboutPartViewModel : ViewModelBase
    {
        public Uri Logo => Package.Current.Logo;

        public string DisplayName => Package.Current.DisplayName;

        public string Publisher => Package.Current.PublisherDisplayName;

        public string VersionFull
        {
            get
            {
                var v = Package.Current.Id.Version;
                return $"{v.Major}.{v.Minor}.{v.Build}";
            }
        }

        public string Version
        {
            get
            {
                var v = Package.Current.Id.Version;
                return $"{v.Major}.{v.Minor}";
            }
        }

        public Uri RateMe => new Uri("http://aka.ms/template10");

        public RelayCommand ShowRelNotes => new RelayCommand(() =>
        {
            Framework.Controls.ModalWindow.SetVisibility(true, new ReleaseNotes());
        });

        public RelayCommand BetaSignUp => new RelayCommand(async () =>
        {
            await Launcher.LaunchUriAsync(new Uri(@"https://forms.office.com/Pages/ResponsePage.aspx?id=oBzDhDusrk6tEVGdgCM-bz_dBKdugltFpBItqyviRWxUNzJLVzZQQ0NUVUVSWFhBRzQ0MEdDT1M0WC4u"));
        });

        public Visibility BetaSignVisibility
        {
            get
            {
                return Constants.Instance.BetaMode ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public string FullReport => "New features:\n" +
            "- Subitems\n" +
            "Updated design\n" +
            //"- Timeline integration - in development\n" +
            "Premium features\n" +
            //"- Ad block\n" +
            ////"- Share items via internet - in development\n" +
            //"- Backup - in development\n" +
            ////"- Action Center contols - in development\n" +
            ////"- Tiles for every item - in development\n" +
            //"- Custom notification info for secured items - in development\n" +
            "Improved encrypting system - in development\n" + 
            "Improved background functions:\n" +
            "Bug fix\n\n" +
            "Enhanced for Windows 10, version 1809";
    }
}

