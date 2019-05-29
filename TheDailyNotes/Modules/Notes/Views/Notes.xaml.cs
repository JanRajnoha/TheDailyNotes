using TheDailyNotes.Modules.Notes.ViewModels;
using TheDailyNotes.Views;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TheDailyNotes.Modules.Notes.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Notes : Page
    {
        public Notes()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Finish connected animations
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

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

        /// <summary>
        /// Notify VM page is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ((NotesVM)DataContext)?.Loaded();
        }

        /// <summary>
        /// Select all activities from NotesList
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectAllItems_Click(object sender, RoutedEventArgs e)
        {
            NotesList.SelectAll();
        }
    }
}
