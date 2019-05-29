using System.Threading.Tasks;
using TheDailyNotes.ViewModels;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace TheDailyNotes.Views
{
    public sealed partial class Settings : Page
    {
        Template10.Services.SerializationService.ISerializationService _SerializationService;

        public Settings()
        {
            InitializeComponent();
            _SerializationService = Template10.Services.SerializationService.SerializationService.Json;
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }
        
        /// <summary>
        /// Finish connected animations
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataContext = new SettingsPageViewModel();

            var index = (e.Parameter is int) ? (int)e.Parameter : 0;
            MyPivot.SelectedIndex = index;
            ((SettingsPageViewModel)DataContext)?.RefreshHelloSecurity();

            ConnectedAnimation imageAnimation = ConnectedAnimationService.GetForCurrentView().GetAnimation("header");

            if (imageAnimation != null)
            {
                imageAnimation.TryStart(HeaderText);
            }

            Shell.Instance.SetSelectedNavItem(HeaderText.Text);
        }

        /// <summary>
        /// Clear DataContext and prepare for connected animations
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            this.DataContext = null;
            base.OnNavigatingFrom(e);

            try
            {
                ConnectedAnimationService.GetForCurrentView().PrepareToAnimate($"headerBack{GetType().Name}", HeaderText);
            }
            catch { }
        }
    }
}
