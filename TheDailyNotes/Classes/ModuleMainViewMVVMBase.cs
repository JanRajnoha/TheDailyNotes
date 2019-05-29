using CustomSettingsDLL;
using ManagerDLL;
using ManagerDLL.Templates;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

using Template10.Mvvm;

using TheDailyNotes.Classes.Interface;

using Windows.UI.Core;
using Windows.UI.StartScreen;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace TheDailyNotes.Classes
{
    public abstract class ModuleMainViewMVVMBase<T> : ViewModelBase, IModuleMainViewMVVMBase<T> where T : BaseItem
    {
        protected const string AddPivotItemName = "AddPivot";
        protected const string DetailPivotItemName = "DetailPivot";

        private Messenger messenger;
        public Messenger Messenger
        {
            get { return messenger; }
            set
            {
                messenger = value;
                RaisePropertyChanged(nameof(Messenger));
            }
        }

        private ListViewSelectionMode listSelectionMode;
        public ListViewSelectionMode ListSelectionMode
        {
            get { return listSelectionMode; }
            set
            {
                listSelectionMode = value;
                RaisePropertyChanged(nameof(ListSelectionMode));
            }
        }

        private ObservableCollection<T> source = new ObservableCollection<T>();
        public ObservableCollection<T> Source
        {
            get { return source; }
            set
            {
                source = value;
                RaisePropertyChanged(nameof(Source));
            }
        }

        private ObservableCollection<PivotItem> pivotPanes;
        public ObservableCollection<PivotItem> PivotPanes
        {
            get { return pivotPanes; }
            set
            {
                pivotPanes = value;
                RaisePropertyChanged(nameof(PivotPanes));
            }
        }

        private bool paneVisibility;
        public bool PaneVisibility
        {
            get { return paneVisibility; }
            set
            {
                paneVisibility = value;
                RaisePropertyChanged(nameof(PaneVisibility));
            }
        }

        private bool startTileAdded;
        public bool StartTileAdded
        {
            get { return startTileAdded; }
            set
            {
                startTileAdded = value;
                RaisePropertyChanged(nameof(StartTileAdded));
            }
        }

        private string inAppNotifyText;
        public string InAppNotifyText
        {
            get { return inAppNotifyText; }
            set
            {
                inAppNotifyText = value;
                RaisePropertyChanged(nameof(InAppNotifyText));
            }
        }

        private bool inAppNotifyShow = false;
        public bool InAppNotifyShow
        {
            get { return inAppNotifyShow; }
            set
            {
                inAppNotifyShow = value;
                RaisePropertyChanged(nameof(InAppNotifyShow));
            }
        }

        private string moduleName;
        public string ModuleName
        {
            get { return moduleName; }
            set
            {
                moduleName = value;
                RaisePropertyChanged(nameof(ModuleName));
            }
        }

        private GridLength masterFrame;
        public GridLength MasterFrame
        {
            get { return masterFrame; }
            set
            {
                masterFrame = value;
                RaisePropertyChanged(nameof(MasterFrame));
            }
        }

        private GridLength slaveFrame;
        public GridLength SlaveFrame
        {
            get { return slaveFrame; }
            set
            {
                slaveFrame = value;
                RaisePropertyChanged(nameof(SlaveFrame));
            }
        }

        public SecondaryTile SecTile { get; set; }

        public ICommand SlavePaneVisibilityCommand { get; set; }

        public ICommand DeleteItems { get; set; }

        public ICommand SelectAllItems { get; set; }

        public ICommand AddStartTile { get; set; }

        public ICommand ChangePaneVisibility { get; set; }

        public ICommand AddItem { get; set; }

        public ICommand ChangeSelectionMode { get; set; }

        public ICommand DetailCommand { get; set; }

        public ICommand EditCommand { get; set; }

        protected abstract void AddPane<TMessage>(string paneName, TMessage msg);

        public abstract Task OnLoadAsync(bool SecureChanged = false);

        public ModuleMainViewMVVMBase()
        {
            PivotPanes = new ObservableCollection<PivotItem>();

            PaneVisibility = false;
            MasterFrame = new GridLength(1, GridUnitType.Star);
            SlaveFrame = new GridLength(0);
        }

        public ModuleMainViewMVVMBase(Messenger messenger) : this()
        {
            Messenger = messenger;

            ChangeSelectionMode = new RelayCommand(() =>
            {
                if (ListSelectionMode == ListViewSelectionMode.None)
                {
                    ListSelectionMode = ListViewSelectionMode.Multiple;
                }
                else
                {
                    ListSelectionMode = ListViewSelectionMode.None;
                }
            });

            AddStartTile = new RelayCommand(() => SecTileManageAsync());
            ChangePaneVisibility = new RelayCommand(InversePaneVisibility);

            CustomSettings.UserLogChanged += CustomSettings_UserLogChanged;
        }

        protected virtual void CustomSettings_UserLogChanged(object sender, UserLoggedEventArgs e)
        {
            OnLoadAsync(true);
        }


        public ModuleMainViewMVVMBase(Messenger messenger, SecondaryTile secondaryTile) : this(messenger)
        {
            SecTile = secondaryTile;
            StartTileAdded = SecondaryTile.Exists(SecTile.TileId) ? true : false;
        }

        /// <summary>
        /// Set Back button to close current pane
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CloseCurrentSlavePane(object sender, BackRequestedEventArgs e)
        {
            PaneVisibility = false;
            e.Handled = true;
        }

        public void ClosePane()
        {
            if (PivotPanes.Count == 0)
            {
                PaneVisibility = false;
                MasterFrame = new GridLength(1, GridUnitType.Star);
            }
        }

        /// <summary>
        /// Create or delete secondary tile for activities
        /// </summary>
        public async void SecTileManageAsync()
        {
            if (SecondaryTile.Exists(SecTile.TileId))
            {
                await SecTile.RequestDeleteAsync();
                StartTileAdded = false;
            }
            else
            {
                SecTile.VisualElements.ShowNameOnSquare150x150Logo = true;
                SecTile.VisualElements.ForegroundText = ForegroundText.Light;
                SecTile.RoamingEnabled = true;

                await SecTile.RequestCreateAsync();

                if (SecondaryTile.Exists(SecTile.TileId))
                    StartTileAdded = true;
            }
        }

        /// <summary>
        /// Get collection of activites
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="mode"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Set ShowAddActPage to custom settings ShowPanelAfterLeavePage
        /// </summary>
        /// <param name="pageState"></param>
        /// <param name="suspending"></param>
        /// <returns></returns>
        public override async Task OnNavigatedFromAsync(IDictionary<string, object> pageState, bool suspending)
        {
            //foreach (var Item in ShowSlaveActPage.ToList())
            //{
            //    ShowSlaveActPage[Item.Key] = Services.SettingsServices.SettingsService.Instance.ShowPanelAfterLeavePage && Item.Value;
            //}

            if (!Services.SettingsServices.SettingsService.Instance.ShowPanelAfterLeavePage)
            {
                PivotPanes.Clear();
            }

            await Task.CompletedTask;
        }

        public void InversePaneVisibility()
        {
            PaneVisibility = !PaneVisibility;

            double MinNormalWidth = (double)Application.Current.Resources["NormalMinWidth"];
            if (MinNormalWidth > ApplicationView.GetForCurrentView().VisibleBounds.Width)
            {
                if (PaneVisibility)
                    MasterFrame = new GridLength(0);
                else
                    MasterFrame = new GridLength(1, GridUnitType.Star);
            }
        }

        public virtual async Task Loaded()
        {
            await OnLoadAsync();

            if (PivotPanes.Count == 0)
            {
                PaneVisibility = false;
            }
        }
    }
}
