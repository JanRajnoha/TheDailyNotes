using Framework.Messages;
using Framework.Service;
using Modules.Notes.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using TheDailyNotes.Classes;
using TheDailyNotes.Modules.Notes.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace TheDailyNotes.Modules.Notes.Components
{
    public sealed partial class NoteItem : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty NoteData = DependencyProperty.Register("Data", typeof(Note), typeof(NoteItem), new PropertyMetadata(null, OnNoteDataChanged));
        public static readonly DependencyProperty NoteItemVM = DependencyProperty.Register("ViewModel", typeof(NoteItemVM), typeof(NoteItem), new PropertyMetadata(null, OnVMChanged));
        public static readonly DependencyProperty CommsEllMar = DependencyProperty.Register("CommsEllipseMargin", typeof(Double), typeof(NoteItem), new PropertyMetadata(null, OnCommsEllipseMarginChanged));
        public static readonly DependencyProperty CommDetMar = DependencyProperty.Register("CommDetailMargin", typeof(Double), typeof(NoteItem), new PropertyMetadata(null, OnCommDetailMarginChanged));
        public static readonly DependencyProperty CommEdMar = DependencyProperty.Register("CommEdittMargin", typeof(Double), typeof(NoteItem), new PropertyMetadata(null, OnCommEditMarginChanged));
        public static readonly DependencyProperty CommShaMar = DependencyProperty.Register("CommShareMargin", typeof(Double), typeof(NoteItem), new PropertyMetadata(null, OnCommShareMarginChanged));
        public static readonly DependencyProperty CommRemMar = DependencyProperty.Register("CommRemoveMargin", typeof(Double), typeof(NoteItem), new PropertyMetadata(null, OnCommRemoveMarginChanged));
        public static readonly DependencyProperty CommsButContMar = DependencyProperty.Register("CommsButtonContentMargin", typeof(Double), typeof(NoteItem), new PropertyMetadata(null, OnCommsButtonContentMarginChanged));

        private static void OnCommsEllipseMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is double)
            {
                var control = (NoteItem)d;

                control.CommsEllipse.Margin = new Thickness((double)e.NewValue, -3, 0, 0);
            }
        }

        private static void OnCommDetailMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is double)
            {
                var control = (NoteItem)d;

                control.CommDetail.Margin = new Thickness((double)e.NewValue, 6, 0, 0);
            }
        }

        private static void OnCommEditMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is double)
            {
                var control = (NoteItem)d;

                control.CommEdit.Margin = new Thickness((double)e.NewValue, 6, 0, 0);
            }
        }

        private static void OnCommShareMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is double)
            {
                var control = (NoteItem)d;

                control.CommShare.Margin = new Thickness((double)e.NewValue, 6, 0, 0);
            }
        }

        private static void OnCommRemoveMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is double)
            {
                var control = (NoteItem)d;

                control.CommRemove.Margin = new Thickness((double)e.NewValue, 6, 0, 0);
            }
        }

        private static void OnCommsButtonContentMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is double)
            {
                var control = (NoteItem)d;

                control.ComButCont.Margin = new Thickness(0, 1, 0, (double)e.NewValue);
            }
        }

        private static void OnVMChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is NoteItemVM)
            {
                var control = (NoteItem)d;
                control.ViewModel = (NoteItemVM)e.NewValue;
            }
        }

        private static void OnNoteDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is Note)
            {
                var control = (NoteItem)d;
                control.Data = (Note)e.NewValue;
            }
        }

        public Note Data
        {
            get
            {
                return (Note)GetValue(NoteData);
            }
            set
            {
                SetValue(NoteData, value);

                if (Data != null)
                    Data.Remove = new DeleteButtonEvent(App.ManaLoc);

                NotifyPropertyChanged(nameof(Data));
            }
        }

        public NoteItemVM ViewModel
        {
            get
            {
                return (NoteItemVM)GetValue(NoteItemVM);
            }
            set
            {
                SetValue(NoteItemVM, value);
                ViewModel.messenger.Register<SelectedDetailItemMsg>(UpdateItem);
                NotifyPropertyChanged(nameof(ViewModel));
            }
        }

        public bool Showed { get; private set; } = false;

        private void UpdateItem(SelectedDetailItemMsg obj)
        {
            if (obj.ItemType == Framework.Enum.ItemTypeEnum.Note)
            if (Data != null && obj.ManagerID != null && obj.ManagerID != string.Empty)
            {
                if (obj.ID == Data.ID)
                {
                    Data = ((NotesManager)App.ManaLoc.GetManager(obj.ManagerID))?.GetItem(obj.ID);
                }
            }
        }

        public void NotifyPropertyChanged([CallerMemberName]string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public event PropertyChangedEventHandler PropertyChanged;

        SolidColorBrush CompButtFill = new SolidColorBrush();
        SolidColorBrush ButtonNormalForeground = new SolidColorBrush(Colors.Black);
        SolidColorBrush Yellow = (SolidColorBrush)Application.Current.Resources["LightNoteBrush"];
        SolidColorBrush Grey = new SolidColorBrush(Color.FromArgb(255, 64, 64, 64));
        SolidColorBrush DarkYellow = (SolidColorBrush)Application.Current.Resources["DarkNoteBrush"];
        SolidColorBrush White = new SolidColorBrush(Colors.White);

        public NoteItem()
        {
            InitializeComponent();

            DescriptionPart.Height = new GridLength(SettingsService.Instance.ShowSmallerNotes ? 0 : 1, GridUnitType.Star);
            Container.Height = SettingsService.Instance.ShowSmallerNotes ? 92 : 225;

            if (SettingsService.Instance.ShowSmallerNotes)
                DetailPart.Height = new GridLength(45);
            else
                DetailPart.Height = new GridLength(1, GridUnitType.Star);
        }

        /// <summary>
        /// Show MenyFlyout on item at cursor position
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Container_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            var flyoutBase = (MenuFlyout)FlyoutBase.GetAttachedFlyout(senderElement);
            var CursorPosition = e.GetPosition(Container);

            flyoutBase.ShowAt(Container, CursorPosition);
        }

        /// <summary>
        /// Change NoteItem for showing commands ... or not
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowCommands_Click(object sender, RoutedEventArgs e)
        {
            if (Showed)
            {
                HideComms.Begin();
            }
            else
            {
                ShowComms.Begin();
            }

            Showed = !Showed;
        }

        #region Functions for styling circle button

        private void ContentPresenter_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Ellipse ButtonEllipse = ((sender as Button).Content as Grid).Children.Where(x => x.GetType() == typeof(Ellipse)).First() as Ellipse;
            ButtonEllipse.StrokeThickness = (sender as Button).Name == "ShowCommands" ? 2 : 0;
            ButtonEllipse.Fill = Grey;

            ContentPresenter ButtonContent = ((sender as Button).Content as Grid).Children.Where(x => x.GetType() == typeof(ContentPresenter)).First() as ContentPresenter;
            if ((sender as Button).IsPressed)
                ButtonContent.Foreground = ButtonNormalForeground;
            else
                ButtonContent.Foreground = new SolidColorBrush(Colors.White);
        }

        private void ContentPresenter_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Ellipse ButtonEllipse = ((sender as Button).Content as Grid).Children.Where(x => x.GetType() == typeof(Ellipse)).First() as Ellipse;
            ButtonEllipse.StrokeThickness = (sender as Button).Name == "ShowCommands" ? 1 : 0;
            ButtonEllipse.Fill = /*(sender as Button).Name == "ShowCommands" ? CompButtFill :*/ DarkYellow;

            ContentPresenter ButtonContent = ((sender as Button).Content as Grid).Children.Where(x => x.GetType() == typeof(ContentPresenter)).First() as ContentPresenter;
            ButtonContent.Foreground = ButtonNormalForeground;
        }

        private void ContentPresenter_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            Ellipse ButtonEllipse = ((sender as Button).Content as Grid).Children.Where(x => x.GetType() == typeof(Ellipse)).First() as Ellipse;

            ButtonEllipse.StrokeThickness = (sender as Button).Name == "ShowCommands" ? 2 : 0;

            if ((sender as Button).IsPressed)
            {
                ButtonEllipse.Fill = Yellow;
            }
            else if ((sender as Button).IsPointerOver)
            {
                ButtonEllipse.Fill = Grey;
            }
            else
            {
                ButtonEllipse.StrokeThickness = (sender as Button).Name == "ShowCommands" ? 1 : 0;
                ButtonEllipse.Fill = /*(sender as Button).Name == "ShowCommands" ? CompButtFill :*/ DarkYellow;
            }

            ContentPresenter ButtonContent = ((sender as Button).Content as Grid).Children.Where(x => x.GetType() == typeof(ContentPresenter)).First() as ContentPresenter;
            if (!(sender as Button).IsPointerOver)
                ButtonContent.Foreground = ButtonNormalForeground;
        }

        private void CompleteActivity_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            Ellipse ButtonEllipse = ((sender as Button).Content as Grid).Children.Where(x => x.GetType() == typeof(Ellipse)).First() as Ellipse;
            ContentPresenter ButtonContent = ((sender as Button).Content as Grid).Children.Where(x => x.GetType() == typeof(ContentPresenter)).First() as ContentPresenter;

            ButtonEllipse.StrokeThickness = (sender as Button).Name == "ShowCommands" ? 2 : 0;
            ButtonContent.Foreground = ButtonNormalForeground;

            if ((sender as Button).IsPressed)
            {
                ButtonEllipse.Fill = Yellow;
            }
            else if ((sender as Button).IsPointerOver)
            {
                ButtonEllipse.StrokeThickness = (sender as Button).Name == "ShowCommands" ? 2 : 0;
                ButtonContent.Foreground = new SolidColorBrush(Colors.White);
                ButtonEllipse.Fill = Grey;
            }
            else
            {
                ButtonEllipse.StrokeThickness = (sender as Button).Name == "ShowCommands" ? 1 : 0;
                ButtonEllipse.Fill = (sender as Button).Name == "ShowCommands" ? CompButtFill : White;
            }
        }

        #endregion
    }
}
