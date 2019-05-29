using Modules.Activities.Classes;
using System;
using System.Collections.ObjectModel;
using TheDailyNotes.Modules.Activities.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace TheDailyNotes.Modules.Activities.Components
{
    public sealed partial class ActivityAdd : UserControl
    {
        int iD = -1;
        public int ID { get => iD; set => iD = value; }
        ObservableCollection<DateTime> ActiDates = new ObservableCollection<DateTime>();

        public ActivityAdd()
        {
            InitializeComponent();
            SelectedDays.ParentFlyout = ParentFlyout;
            Cancel.Focus(FocusState.Pointer);
        }

        public ActivityAdd(bool modalActivation, Activity currentActivity) : this()
        {
            if (modalActivation)
            {
                AddClose.Visibility = Visibility.Collapsed;
                AddCloseRow.Height = new GridLength(0);
                AddText.Text = "Save";

                ((ActivityAddVM)DataContext).SetDetailItem(currentActivity);
                ((ActivityAddVM)DataContext).ModalActivation = true;

                Add.Command = null;
                Cancel.Command = null;

                Add.Click += Add_Click;
                Cancel.Click += Cancel_Click;
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            ((ActivityAddVM)DataContext).SaveActivity.Execute(((ActivityAddVM)DataContext).DetailItem);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ((ActivityAddVM)DataContext).CloseModal();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
