using Template10.Mvvm;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using TheDailyNotes.Modules.Activities.Views;
using TheDailyNotes.Views;
using Windows.UI.Xaml;
using Windows.ApplicationModel;
using Template10.Services.SettingsService;
using Windows.UI.Xaml.Media.Animation;
using Microsoft.Services.Store.Engagement;
using TheDailyNotes.Classes;
using System.Windows.Input;
using Framework.Classes;
using Framework.Template;
using Framework.Messages;
using TheDailyNotes.Modules.Notes.Views;
using TheDailyNotes.Modules.ToDos.Views;

namespace TheDailyNotes.ViewModels
{
    public class MainPageViewModel : TDNVMBase<BaseItem>
    {
        public string NavigatedPage { get; set; }

        public Version CurrentVersion { get; set; }

        private bool showMinorUpdate = false;

        public bool ShowMinorUpdate
        {
            get { return showMinorUpdate; }
            set
            {
                showMinorUpdate = value;
                RaisePropertyChanged(nameof(ShowMinorUpdate));
            }
        }

        public ICommand CloseMinor { get; set; }

        public MainPageViewModel()
        {
            CloseMinor = new RelayCommand(() => ShowMinorUpdate = false);

            var v = Package.Current.Id.Version;

            CurrentVersion = new Version(v.Major, v.Minor, v.Build);

            Messenger = (Application.Current.Resources["VMLocator"] as VMLocator).GetMessenger();
        }

        public MainPageViewModel(Messenger messenger) : this()
        {
            Messenger = messenger;
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            var OldVer = new Version(Framework.Service.SettingsService.Instance.CurrentAppVersion);
            var CurVer = new Version(Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build);

            if (OldVer != CurVer)
            {
                Framework.Service.SettingsService.Instance.CurrentAppVersion = $"{CurVer.Major}.{CurVer.Minor}.{CurVer.Build}";

                if (CurVer.Minor != OldVer.Minor || OldVer.Major == 0)
                {
                    Framework.Controls.ModalWindow.SetVisibility(true, new ReleaseNotes(), useAnimation: false);
                }
                else
                {
                    ShowMinorUpdate = true;
                }
            }

            Messenger.Register<ShowModalActivationMsg>(ShowModal);

            await Task.CompletedTask;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            Messenger.UnRegister<ShowModalActivationMsg>(ShowModal);

            await Task.CompletedTask;
        }

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            await Task.CompletedTask;
        }

        //public void GotoDetailsPage() =>
        //    NavigationService.Navigate(typeof(Views.DetailPage), Value);

        public void GotoSettings() =>
            NavigationService.Navigate(typeof(Settings), infoOverride: new SuppressNavigationTransitionInfo());

        public void GoToActivities() =>
            NavigationService.Navigate(typeof(Activities), infoOverride: new SuppressNavigationTransitionInfo());

        public void GoToNotes() =>
            NavigationService.Navigate(typeof(Notes), infoOverride: new SuppressNavigationTransitionInfo());

        public void GoToToDo() =>
            NavigationService.Navigate(typeof(ToDos), infoOverride: new SuppressNavigationTransitionInfo());

        public RelayCommand GoToFeedback() => new RelayCommand(async () =>
        {
            var launcher = StoreServicesFeedbackLauncher.GetDefault();
            await launcher.LaunchAsync();
        });

        public void GoToPage()
        {
            switch (NavigatedPage)
            {
                case "Activities":
                    GoToActivities();
                    break;

                case "Settings":
                    GotoSettings();
                    break;

                case "Notes":
                    GoToNotes();
                    break;

                case "Feedback":
                    GoToFeedback().Execute(null);
                    break;

                case "ToDos":
                    GoToToDo();
                    break;

                case "Time schedule":
                default:
                    break;
            }
        }
    }
}

