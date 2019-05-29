using CustomSettingsDLL;
using Framework.Classes;
using Framework.Locator;
using Framework.Messages;
using Framework.Service;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Push;

using Recover;

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Template10.Common;
using Template10.Controls;

using TheDailyNotes.Classes;
using TheDailyNotes.Modules.Activities.Views;
using TheDailyNotes.Modules.Notes.Views;
using TheDailyNotes.ViewModels;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace TheDailyNotes
{
    /// Documentation on APIs used in this page:
    /// https://github.com/Windows-XAML/Template10/wiki

    [Bindable]
    sealed partial class App : BootStrapper
    {
        public static ManagerLocator ManaLoc = new ManagerLocator();

        public App()
        {
            InitializeComponent();
            SplashFactory = (e) => new Views.Splash(e);
            
            Constants.SetConstants(betaMode: true, logEnable: true);

            if (ManaLoc == null)
                ManaLoc = new ManagerLocator();

            AppCenter.Start("3dd92a82-f647-4e33-97eb-4225f3fee03f", typeof(Analytics));
            AppCenter.Start("3dd92a82-f647-4e33-97eb-4225f3fee03f", typeof(Push));

            //Reminder.Registration.RegistrBackgroundTaskAsync();

            RegisterBA();

            #region App settings

            var _settings = SettingsService.Instance;

            if (!_settings.UseSystemAppTheme)
            {
                RequestedTheme = _settings.AppTheme;
            }

            if (!_settings.UseHelloSecurity)
                _settings.UseHelloSecurity = true;

            CacheMaxDuration = _settings.CacheMaxDuration;
            ShowShellBackButton = _settings.UseShellBackButton;

            Suspending += App_Suspending;

            #endregion
        }

        /// <summary>
        /// Registration of background tasks
        /// </summary>
        private void RegisterBA()
        {
            RegisterRecover();
            //RegisterLongTermReminder();

            BGTasks bGTasks = new BGTasks();

            //myTaskName = "TDNCompleter";

            //if (BackgroundTaskRegistration.AllTasks.Any(i => i.Value.Name.Equals(myTaskName)))
            //    return;

            //// register a new task
            //taskBuilder = new BackgroundTaskBuilder { Name = myTaskName };
            //taskBuilder.SetTrigger(new ToastNotificationActionTrigger());
            //BackgroundTaskRegistration Completer = taskBuilder.Register();

            // --------------------------------------------------------------------------------
        }

        private async void RegisterRecover()
        {
            // --------- Recovering background task -------------------------------------------

            string myTaskName = "TDNRecovery";
            bool skip = false;
            BackgroundTaskBuilder taskBuilder;
            ApplicationTrigger trigger = new ApplicationTrigger();

            if (BackgroundTaskRegistration.AllTasks.Any(i => i.Value.Name.Equals(myTaskName)))
                skip = true;

            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
                if (cur.Value.Name == myTaskName)
                {
                    skip = true;
                }
            }

            if (!skip)
            {
                BackgroundExecutionManager.RemoveAccess();
                // Windows Phone app must call this to use trigger types (see MSDN)
                var res = await BackgroundExecutionManager.RequestAccessAsync();

                // register a new task
                taskBuilder = new BackgroundTaskBuilder { Name = myTaskName, TaskEntryPoint = "Recover.Recover" };
                taskBuilder.SetTrigger(new SystemTrigger(SystemTriggerType.SessionConnected, false));
                taskBuilder.SetTrigger(new SystemTrigger(SystemTriggerType.UserPresent, false));
                taskBuilder.SetTrigger(new TimeTrigger(15, false));

                BackgroundTaskRegistration Recovery = taskBuilder.Register();
            }
        }

        /// <summary>
        /// Save data before enter to suspend
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_Suspending(object sender, SuspendingEventArgs e)
        {
            var state = NavigationService.FrameFacade.GetNavigationState();

            var localSettings = ApplicationData.Current.LocalSettings;

            localSettings.Values["NavState"] = state;
        }

        /// <summary>
        /// Set colors and frame
        /// </summary>
        /// <param name="args">Argmunets</param>
        /// <returns></returns>
        public override async Task OnInitializeAsync(IActivatedEventArgs args)
        {
            //draw into the title bar
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;

            //remove the solid-colored backgrounds behind the caption controls and system back button
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;

            SolidColorBrush btnForegroundColor = Current.Resources["TitleBarForegroundThemeBrush"] as SolidColorBrush;
            SolidColorBrush btnBackgroundHoverColor = Current.Resources["TitleBarPressedBackgroundThemeBrush"] as SolidColorBrush;
            SolidColorBrush btnHoverColor = Current.Resources["TitleBarButtonHoverThemeBrush"] as SolidColorBrush;
            SolidColorBrush btnBackgroundPressedColor = Current.Resources["TitleBarHoverBackgroundThemeBrush"] as SolidColorBrush;
            SolidColorBrush btnPressedColor = Current.Resources["TitleBarButtonPressedThemeBrush"] as SolidColorBrush;

            titleBar.BackgroundColor = Colors.Transparent;
            titleBar.ForegroundColor = btnForegroundColor.Color;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonForegroundColor = btnForegroundColor.Color;
            titleBar.ButtonHoverBackgroundColor = btnBackgroundHoverColor.Color;
            titleBar.ButtonHoverForegroundColor = btnHoverColor.Color;
            titleBar.ButtonPressedBackgroundColor = btnBackgroundPressedColor.Color;
            titleBar.ButtonPressedForegroundColor = btnPressedColor.Color;
            titleBar.InactiveBackgroundColor = Colors.Transparent;
            titleBar.InactiveForegroundColor = btnForegroundColor.Color;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveForegroundColor = btnForegroundColor.Color;

            if (Window.Current.Content as ModalDialog == null)
            {
                // create a new frame 
                var nav = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include);

                // create modal root
                Window.Current.Content = new ModalDialog
                {
                    DisableBackButtonWhenModal = true,
                    Content = new Views.Shell(nav),
                    ModalContent = new Views.Busy(),
                };
            }
            await Task.CompletedTask;
        }

        /// <summary>
        /// Select start type
        /// </summary>
        /// <param name="startKind">Start kind</param>
        /// <param name="args">Arguments</param>
        /// <returns></returns>
        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            // long-running startup tasks go here

            //await Task.Delay(500000); // overit, zda je obrazek dobre nastaven
            var _settings = SettingsService.Instance;

            if (! await (new SettingsPartViewModel()).GetAddOnLicenseInfo())
            {
                _settings.ActivateActionCenterControl = false;
                _settings.ActivateAdBlocker = false;
                _settings.ActivateBackupItems = false;
                _settings.ActivateBackupItemsInterval = false;
                _settings.ActivateBackupItemsType = false;
                _settings.ActivateShareItems = false;
                _settings.ActivateCustomSecuredNotificationInfo = false;
                _settings.ActivateSecondaryItemTiles = false;
            }

            CustomSettings.ShowAds = _settings.ActivateAdBlocker;

            if (args != null)
            {
                switch (args.Kind)
                {
                    case ActivationKind.Protocol:
                        NavigationService.Navigate(typeof(Views.MainPage));
                        break;

                    #region Switch bullshit
                    case ActivationKind.Search:
                    case ActivationKind.ShareTarget:
                    case ActivationKind.FileOpenPicker:
                    case ActivationKind.FileSavePicker:
                    case ActivationKind.CachedFileUpdater:
                    case ActivationKind.ContactPicker:
                    case ActivationKind.Device:
                    case ActivationKind.PrintTaskSettings:
                    case ActivationKind.CameraSettings:
                    case ActivationKind.RestrictedLaunch:
                    case ActivationKind.AppointmentsProvider:
                    case ActivationKind.Contact:
                    case ActivationKind.LockScreenCall:
                    case ActivationKind.VoiceCommand:
                    case ActivationKind.LockScreen:
                    case ActivationKind.PickerReturned:
                    case ActivationKind.WalletAction:
                    case ActivationKind.PickFileContinuation:
                    case ActivationKind.PickSaveFileContinuation:
                    case ActivationKind.PickFolderContinuation:
                    case ActivationKind.WebAuthenticationBrokerContinuation:
                    case ActivationKind.WebAccountProvider:
                    case ActivationKind.ComponentUI:
                    case ActivationKind.ProtocolForResults:
                    case ActivationKind.ToastNotification:
                    case ActivationKind.Print3DWorkflow:
                    case ActivationKind.DialReceiver:
                    case ActivationKind.DevicePairing:
                    case ActivationKind.UserDataAccountsProvider:
                    case ActivationKind.FilePickerExperience:
                    case ActivationKind.LockScreenComponent:
                    case ActivationKind.ContactPanel:
                    case ActivationKind.PrintWorkflowForegroundTask:
                    case ActivationKind.GameUIProvider:
                    case ActivationKind.StartupTask:
                    case ActivationKind.CommandLineLaunch:
                    #endregion
                    case ActivationKind.Launch:
                        switch (((LaunchActivatedEventArgs)args).Arguments)
                        {
                            case "Activities":
                                NavigationService.Navigate(typeof(Activities));
                                break;

                            case "Notes":
                                NavigationService.Navigate(typeof(Notes));
                                break;

                            default:

                                var state = ApplicationData.Current.LocalSettings.Values["NavState"];

                                Debug.WriteLine(CurrentState);

                                if (CurrentState == BootstrapperStates.Initialized || (state == null && string.IsNullOrWhiteSpace(state?.ToString())))
                                    NavigationService.Navigate(typeof(Views.MainPage));
                                else
                                {
                                    NavigationService.FrameFacade.SetNavigationState(state.ToString());
                                    ApplicationData.Current.LocalSettings.Values["NavState"] = null;
                                }
                                break;
                        }
                        break;

                    case ActivationKind.File:
                        if (!NavigationService.FrameFacade.GetNavigationState().Contains("MainPage"))
                            NavigationService.Navigate(typeof(Views.MainPage));

                        (Application.Current.Resources["VMLocator"] as VMLocator).SendMessage(new ShowModalActivationMsg()
                        {
                            Files = ((FileActivatedEventArgs)args).Files
                        });
                        break;

                    default:
                        break;
                }
            }
            else
            {
                NavigationService.Navigate(typeof(Views.MainPage));
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// Activation from background
        /// </summary>
        /// <param name="args">Arguments</param>
        protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            var deferral = args.TaskInstance.GetDeferral();

            switch (args.TaskInstance.Task.Name)
            {
                case "TDN_Completer":
                    var details = args.TaskInstance.TriggerDetails as ToastNotificationActionTriggerDetail;
                    if (details != null)
                    {
                        var arguments = details.Argument;
                        var userInput = details.UserInput;

                        // Perform tasks
                    }
                    break;
            }

            deferral.Complete();
        }
    }
}

