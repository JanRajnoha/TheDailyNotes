using Microsoft.Services.Store.Engagement;

using TheDailyNotes.ViewModels;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace TheDailyNotes.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        /// <summary>
        /// Finish connected animation and show feedback button if available
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            SetFeedbackVisibility();

            ConnectedAnimation imageAnimation = ConnectedAnimationService.GetForCurrentView().GetAnimation($"headerBack{((MainPageViewModel)DataContext).NavigatedPage}");

            if (imageAnimation != null)
            {
                imageAnimation.TryStart(FindName(((MainPageViewModel)DataContext).NavigatedPage + "Text") as TextBlock);
            }

            Shell.Instance.SetSelectedNavItem("home");
        }

        private void SetFeedbackVisibility()
        {
            if (StoreServicesFeedbackLauncher.IsSupported() && false)
            {
                FeedbackButton.Visibility = Visibility.Visible;
                SettingsButton.SetValue(Grid.ColumnSpanProperty, 1);
                SettingsButton.SetValue(Grid.ColumnProperty, 1);
            }
        }

        /// <summary>
        /// Prepare connected animation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            var ButtonName = (sender as Button).Name;
            TextBlock ButtonText = FindName(ButtonName.Substring(0, ButtonName.Length - 6) + "Text") as TextBlock;

            ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("header", ButtonText);
            ((MainPageViewModel)DataContext).NavigatedPage = ButtonText.Name.Substring(0, ButtonText.Name.Length - 4);
            ((MainPageViewModel)DataContext).GoToPage();
        }
    }
}
