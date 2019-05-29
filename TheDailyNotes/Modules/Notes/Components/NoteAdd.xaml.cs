using Modules.Notes.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TheDailyNotes.Modules.Notes.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace TheDailyNotes.Modules.Notes.Components
{
    public sealed partial class NoteAdd : UserControl
    {
        public NoteAdd()
        {
            InitializeComponent();
        }

        public NoteAdd(bool modalActivation, Note currentNote) : this()
        {
            if (modalActivation)
            {
                AddClose.Visibility = Visibility.Collapsed;
                AddCloseRow.Height = new GridLength(0);
                AddText.Text = "Save";

                ((NoteAddVM)DataContext).SetDetailItem(currentNote);
                ((NoteAddVM)DataContext).ModalActivation = true;

                Add.Command = null;
                Cancel.Command = null;

                Add.Click += Add_Click;
                Cancel.Click += Cancel_Click;
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            ((NoteAddVM)DataContext).SaveNote.Execute(((NoteAddVM)DataContext).DetailItem);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ((NoteAddVM)DataContext).CloseModal();
        }
    }
}
