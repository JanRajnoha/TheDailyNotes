using Modules.ToDos.Classes;
using System;
using System.Collections.ObjectModel;
using TheDailyNotes.Modules.ToDos.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace TheDailyNotes.Modules.ToDos.Components
{
    public sealed partial class ToDoAdd : UserControl
    {
        int iD = -1;
        public int ID { get => iD; set => iD = value; }
        ObservableCollection<DateTime> todoDates = new ObservableCollection<DateTime>();

        public ToDoAdd()
        {
            InitializeComponent();
            SelectedDays.ParentFlyout = ParentFlyout;
            Cancel.Focus(FocusState.Pointer);
        }

        public ToDoAdd(bool modalActivation, ToDo currentToDo) : this()
        {
            if (modalActivation)
            {
                AddClose.Visibility = Visibility.Collapsed;
                AddCloseRow.Height = new GridLength(0);
                AddText.Text = "Save";

                ((ToDoAddVM)DataContext).SetDetailItem(currentToDo);
                ((ToDoAddVM)DataContext).ModalActivation = true;

                Add.Command = null;
                Cancel.Command = null;

                Add.Click += Add_Click;
                Cancel.Click += Cancel_Click;
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            ((ToDoAddVM)DataContext).SaveToDo.Execute(((ToDoAddVM)DataContext).DetailItem);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ((ToDoAddVM)DataContext).CloseModal();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
