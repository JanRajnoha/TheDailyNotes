using CustomSettingsDLL;

using Framework.Classes;
using Framework.Enum;
using Framework.Service;

using Microsoft.Services.Store.Engagement;

using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

//using Microsoft.Services.Store.Engagement;

using Template10.Controls;
using Template10.Services.NavigationService;

using TheDailyNotes.Modules.Activities.Views;
using TheDailyNotes.Modules.Notes.Views;
using TheDailyNotes.Modules.ToDos.Views;
using TheDailyNotes.ViewModels;

using Windows.ApplicationModel.Core;
using Windows.Data.Json;
using Windows.Security.Authentication.Web.Core;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.UI.ApplicationSettings;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TheDailyNotes.Views
{
    public sealed partial class Shell : Page
    {
        const string MSAProviderId = "https://login.microsoft.com";
        const string ConsumerAuthority = "consumers";
        const string AccountScopeRequested = "wl.basic";
        const string AccountClientId = "none";
        const string StoredAccountKey = "accountid";
        const string StoredAccountProviderKey = "accountProviderid";

        private readonly UISettings uiSettings = new UISettings();
        public static Shell Instance { get; set; }
        public static HamburgerMenu HamburgerMenu => Instance.HamMen;
        public HamburgerMenu HamMen { get; set; }
        public RelayCommand SendFeedback => new RelayCommand(async () =>
        {
            var launcher = StoreServicesFeedbackLauncher.GetDefault();
            await launcher.LaunchAsync();
        });

        public Shell()
        {
            InitializeComponent();

            Instance = this;

            HamMen = new HamburgerMenu();

            if (StoreServicesFeedbackLauncher.IsSupported())
            {
                FeedbackButton.Visibility = Visibility.Visible;
            }

            CustomSettings.UserLogChanged -= CustomSettings_IsUserLoggedChanged;
            CustomSettings.UserLogChanged += CustomSettings_IsUserLoggedChanged;

            CustomSettings.ShowAdsChanged -= CustomSettings_ShowAdsChanged;
            CustomSettings.ShowAdsChanged += CustomSettings_ShowAdsChanged;

            SettingsPartViewModel.HelloSecurityChanged -= SettingsPartViewModel_HelloSecurityChanged;
            SettingsPartViewModel.HelloSecurityChanged += SettingsPartViewModel_HelloSecurityChanged;

            AccountsSettingsPane.GetForCurrentView().AccountCommandsRequested -= BuildLoginPaneAsync;
            AccountsSettingsPane.GetForCurrentView().AccountCommandsRequested += BuildLoginPaneAsync;

            CoreApplicationViewTitleBar titleBar = CoreApplication.GetCurrentView().TitleBar;

            titleBar.LayoutMetricsChanged -= TitleBar_LayoutMetricsChanged;
            titleBar.LayoutMetricsChanged += TitleBar_LayoutMetricsChanged;

            LoggingButton.IsEnabled = SettingsService.Instance.UseHelloSecurity && KeyCredentialManager.IsSupportedAsync().AsTask().Result;

            uiSettings.ColorValuesChanged += ThemeChanger;

            NavView.ShowAd = Visibility.Collapsed;
        }

        /// <summary>
        /// Change visibility of ad
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomSettings_ShowAdsChanged(object sender, ShowAdsChangedEventArgs e)
        {
            NavView.ShowAd = !e.ShowAdsChangedNewState ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Change theme to system
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ThemeChanger(UISettings sender, object args)
        {
            if (SettingsService.Instance.UseSystemAppTheme)
                ThemeSelectorService.SetTheme(AppThemes.Dark);
        }

        /// <summary>
        /// Set navigation service which is gived by argument
        /// </summary>
        /// <param name="navigationService">New navigation service</param>
        public Shell(INavigationService navigationService) : this()
        {
            SetNavigationService(navigationService);
        }

        /// <summary>
        /// Make space for back button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void TitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            //AppTitle.Margin = new Thickness(CoreApplication.GetCurrentView().TitleBar.SystemOverlayLeftInset + 12, 8, 0, 0);
            AppTitle.Margin = new Thickness(0, 8, 0, 0);
        }

        /// <summary>
        /// Set frame for navigating
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            NavView.Content = HamMen.NavigationService.FrameFacade._Frame;

            foreach (NavigationViewItem item in NavView.MenuItems)
            {
                if (item.Tag.ToString() == "home")
                {
                    NavView.SelectedItem = item;
                    break;
                }
            }
        }

        /// <summary>
        /// Enable or disable login button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsPartViewModel_HelloSecurityChanged(object sender, SettingsPartViewModel.HelloSecurityChangedEventArgs e)
        {
            LoggingButton.IsEnabled = e.HelSecNewState;
        }

        /// <summary>
        /// Switch login button text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomSettings_IsUserLoggedChanged(object sender, UserLoggedEventArgs e)
        {
            LoggingButton.Content = e.UserLoggedNewState ? "Log out" : "Log in";
        }

        /// <summary>
        /// Set Navigation Service
        /// </summary>
        /// <param name="navigationService"></param>
        public void SetNavigationService(INavigationService navigationService)
        {
            HamMen.NavigationService = navigationService;
        }

        /// <summary>
        /// Set current selected item from NavView
        /// </summary>
        /// <param name="tag">Tag of item</param>
        public void SetSelectedNavItem(string tag)
        {
            if (tag.Contains("Settings"))
                NavView.SelectedItem = NavView.SettingsItem;
            else
            {
                NavView.SelectedItem = NavView.MenuItems.FirstOrDefault(x =>
                    {
                        if (x is NavigationViewItem NavItem)
                            if (NavItem.Content.ToString().ToUpper() == tag.ToUpper())
                                return true;

                        return false;
                    });
            }
        }

        /// <summary>
        /// Navigationm from shell menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                HamMen.NavigationService.Navigate(typeof(Settings));
            }
            else
            {
                switch (args.InvokedItem)
                {
                    case "Home":
                        HamMen.NavigationService.Navigate(typeof(MainPage));
                        break;

                    case "Activities":
                        HamMen.NavigationService.Navigate(typeof(Activities));
                        break;

                    case "Notes":
                        HamMen.NavigationService.Navigate(typeof(Notes));
                        break;

                    case "To - Dos":
                        HamMen.NavigationService.Navigate(typeof(ToDos));
                        break;

                    case "Feedback":
                        SendFeedback.Execute(null);
                        break;
                }
            }
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            //if (args.IsSettingsSelected)
            //{
            //    HamMen.NavigationService.Navigate(typeof(SettingsPage));
            //}
            //else
            //{
            //    NavigationViewItem item = args.SelectedItem as NavigationViewItem;

            //    switch (item.Tag)
            //    {
            //        case "home":
            //            HamMen.NavigationService.Navigate(typeof(MainPage));
            //            HamMen.NavigationService.ClearHistory();
            //            break;

            //        case "activities":
            //            HamMen.NavigationService.Navigate(typeof(Activities));
            //            break;

            //        case "feedback":
            //            SendFeedback.Execute(null);
            //            break;
            //    }
            //}
        }

        /// <summary>
        /// Create login pane
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void BuildLoginPaneAsync(AccountsSettingsPane sender, AccountsSettingsPaneCommandsRequestedEventArgs args)
        {
            AccountsSettingsPaneEventDeferral deferral = args.GetDeferral();

            bool isPresent = ApplicationData.Current.LocalSettings.Values.ContainsKey(StoredAccountKey);

            if (isPresent)
            {
                await AddAccount(args);
            }
            else
            {
                await AddAccountProvider(args);
            }

            args.HeaderText = "Describe what adding an account to your application will do for the user";

            // You can add links such as privacy policy, help, general account settings
            //args.Commands.Add(new SettingsCommand("privacypolicy", "Privacy policy", PrivacyPolicyInvoked));
            //args.Commands.Add(new SettingsCommand("otherlink", "Other link", OtherLinkInvoked));

            deferral.Complete();
        }

        /// <summary>
        /// Add account provider into login pane
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task AddAccountProvider(AccountsSettingsPaneCommandsRequestedEventArgs e)
        {
            WebAccountProvider provider = await WebAuthenticationCoreManager.FindAccountProviderAsync(MSAProviderId, ConsumerAuthority);

            WebAccountProviderCommand providerCommand = new WebAccountProviderCommand(provider, WebAccountProviderCommandInvoked);
            e.WebAccountProviderCommands.Add(providerCommand);
        }

        /// <summary>
        /// Add account into login pane.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task AddAccount(AccountsSettingsPaneCommandsRequestedEventArgs e)
        {
            WebAccount account = await GetSignedUser();

            WebAccountCommand command = new WebAccountCommand(account, WebAccountInvoked, SupportedWebAccountActions.Remove);
            e.WebAccountCommands.Add(command);
        }

        private async Task<WebAccount> GetSignedUser()
        {
            WebAccountProvider provider = await WebAuthenticationCoreManager.FindAccountProviderAsync(MSAProviderId, ConsumerAuthority);

            String accountID = (String)ApplicationData.Current.LocalSettings.Values[StoredAccountKey];
            WebAccount account = await WebAuthenticationCoreManager.FindAccountAsync(provider, accountID);

            if (account == null)
            {
                ApplicationData.Current.LocalSettings.Values.Remove(StoredAccountKey);
                ApplicationData.Current.LocalSettings.Values.Remove(StoredAccountProviderKey); 
            }

            return account;
        }

        /// <summary>
        /// Function for handling provider command
        /// </summary>
        /// <param name="command"></param>
        private async void WebAccountProviderCommandInvoked(WebAccountProviderCommand command)
        {
            // ClientID is ignored by MSA
            await RequestTokenAndSaveAccount(command.WebAccountProvider, AccountScopeRequested, AccountClientId);
        }

        /// <summary>
        /// Get token and save account
        /// </summary>
        /// <param name="Provider">Provider</param>
        /// <param name="Scope">Scope</param>
        /// <param name="ClientID">Client ID</param>
        /// <returns></returns>
        private async Task RequestTokenAndSaveAccount(WebAccountProvider Provider, String Scope, String ClientID)
        {
            try
            {
                WebTokenRequest webTokenRequest = new WebTokenRequest(Provider, Scope, ClientID);
                WebTokenRequestResult tokenResult = await WebAuthenticationCoreManager.RequestTokenAsync(webTokenRequest);

                if (tokenResult.ResponseStatus == WebTokenRequestStatus.Success)
                {
                    ApplicationData.Current.LocalSettings.Values.Remove(StoredAccountKey);
                    ApplicationData.Current.LocalSettings.Values.Remove(StoredAccountProviderKey);

                    ApplicationData.Current.LocalSettings.Values[StoredAccountKey] = tokenResult.ResponseData[0].WebAccount.Id;
                    ApplicationData.Current.LocalSettings.Values[StoredAccountProviderKey] = tokenResult.ResponseData[0].WebAccount.WebAccountProvider.Id;
                }
                else
                    CustomSettings.IsUserLogged = false;

                OutputTokenResult(tokenResult);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Function for handling Web Account command
        /// </summary>
        /// <param name="command"></param>
        /// <param name="args"></param>
        private async void WebAccountInvoked(WebAccountCommand command, WebAccountInvokedArgs args)
        {
            if (args.Action == WebAccountAction.Remove)
            {
                await LogoffAndRemoveAccount();
            }
        }

        /// <summary>
        /// Log off and remove user from stored data
        /// </summary>
        /// <returns></returns>
        private async Task LogoffAndRemoveAccount()
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey(StoredAccountKey))
            {
                WebAccountProvider providertoDelete = await WebAuthenticationCoreManager.FindAccountProviderAsync(MSAProviderId, ConsumerAuthority);
                WebAccount accountToDelete = await WebAuthenticationCoreManager.FindAccountAsync(providertoDelete, (string)ApplicationData.Current.LocalSettings.Values[StoredAccountKey]);

                if (accountToDelete != null)
                {
                    await accountToDelete.SignOutAsync();
                }

                ApplicationData.Current.LocalSettings.Values.Remove(StoredAccountKey);
                ApplicationData.Current.LocalSettings.Values.Remove(StoredAccountProviderKey);
                CustomSettings.IsUserLogged = false;
            }
        }

        /// <summary>
        /// Handle token result
        /// </summary>
        /// <param name="result">Token result</param>
        private async void OutputTokenResult(WebTokenRequestResult result)
        {
            if (result.ResponseStatus == WebTokenRequestStatus.Success)
            {
                CustomSettings.IsUserLogged = true;

                string token = result.ResponseData[0].Token;

                var restApi = new Uri(@"https://apis.live.net/v5.0/me?access_token=" + token);

                using (var client = new HttpClient())
                {
                    var infoResult = await client.GetAsync(restApi);
                    string content = await infoResult.Content.ReadAsStringAsync();

                    var jsonObject = JsonObject.Parse(content);
                    LoggingButton.Content = jsonObject["name"].GetString();
                }


                //var photoApi = new Uri(@"https://apis.live.net/v5.0/me/picture?access_token=" + token);
                //using (var client = new HttpClient())
                //{
                //    var photoResult = await client.GetAsync(photoApi);
                //    using (var imageStream = await photoResult.Content.ReadAsStreamAsync())
                //    {
                //        BitmapImage image = new BitmapImage();
                //        using (var randomStream = imageStream.AsRandomAccessStream())
                //        {
                //            await image.SetSourceAsync(randomStream);
                //            this.image.Source = image;
                //            var wid = image.PixelWidth;
                //            var hei = image.PixelHeight;
                //        }

                //    }
                //}
            }
            else
            {
                CustomSettings.IsUserLogged = false;
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await GetTokenSilentlyAsync();
        }

        private async Task<string> GetTokenSilentlyAsync()
        {
            LoggingButton.Content = "Signing";
            string providerId = ApplicationData.Current.LocalSettings.Values[StoredAccountProviderKey]?.ToString();
            string accountId = ApplicationData.Current.LocalSettings.Values[StoredAccountKey]?.ToString();

            if (null == providerId || null == accountId)
            {
                CustomSettings.IsUserLogged = false;
                return null;
            }

            WebAccountProvider provider = await WebAuthenticationCoreManager.FindAccountProviderAsync(providerId);
            WebAccount account = await WebAuthenticationCoreManager.FindAccountAsync(provider, accountId);

            WebTokenRequest request = new WebTokenRequest(provider, "wl.basic");

            WebTokenRequestResult result = await WebAuthenticationCoreManager.GetTokenSilentlyAsync(request, account);
            if (result.ResponseStatus == WebTokenRequestStatus.UserInteractionRequired)
            {
                CustomSettings.IsUserLogged = false;
                LoggingButton.Content = "Error";
                AccountsSettingsPane.Show();
                return null;
            }
            else if (result.ResponseStatus == WebTokenRequestStatus.Success)
            {
                OutputTokenResult(result);
                return result.ResponseData[0].Token;
            }
            else
            {
                CustomSettings.IsUserLogged = false;
                return null;
            }
        }
    }
}