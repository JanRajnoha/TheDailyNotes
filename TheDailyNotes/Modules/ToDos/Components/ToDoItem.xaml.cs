using Framework.Classes;
using Framework.Enum;
using Framework.Messages;
using Framework.Service;
using Modules.ToDos.Classes;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using TheDailyNotes.Modules.ToDos.ViewModels;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace TheDailyNotes.Modules.ToDos.Components
{
    public sealed partial class ToDoItem : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty ToDoData = DependencyProperty.Register("Data", typeof(ToDo), typeof(ToDoItem), new PropertyMetadata(null, OnToDoDataChanged));
        public static readonly DependencyProperty ToDoItmVM = DependencyProperty.Register("ViewModel", typeof(ToDoItemVM), typeof(ToDoItem), new PropertyMetadata(null, OnVMChanged));
        public static readonly DependencyProperty MainRHei = DependencyProperty.Register("MainRowHeight", typeof(Double), typeof(ToDoItem), new PropertyMetadata(null, OnMainRHeiChanged));
        public static readonly DependencyProperty DetRHei = DependencyProperty.Register("DetailRowHeight", typeof(Double), typeof(ToDoItem), new PropertyMetadata(null, OnDetRHeiChanged));
        public static readonly DependencyProperty ButRHei = DependencyProperty.Register("ButtonRowHeight", typeof(Double), typeof(ToDoItem), new PropertyMetadata(null, OnButRHeiChanged));

        private static void OnMainRHeiChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is double)
            {
                var control = (ToDoItem)d;
                control.MainPart.Height = new GridLength((double)e.NewValue);

                if ((double)e.OldValue == 0)
                {
                    return;
                }

                control.Container.Height += (double)e.NewValue - (double)e.OldValue;
            }
        }

        private static void OnDetRHeiChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is double)
            {
                var control = (ToDoItem)d;
                control.DetailPart.Height = new GridLength((double)e.NewValue);

                control.Container.Height += (double)e.NewValue - (double)e.OldValue;
            }
        }

        private static void OnButRHeiChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is double)
            {
                var control = (ToDoItem)d;
                control.ButtonPart.Height = new GridLength((double)e.NewValue);

                control.Container.Height += (double)e.NewValue - (double)e.OldValue;
            }
        }

        private static void OnVMChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ToDoItemVM)
            {
                var control = (ToDoItem)d;
                control.ViewModel = (ToDoItemVM)e.NewValue;
            }
        }

        private static void OnToDoDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ToDo)
            {
                var control = (ToDoItem)d;
                control.Data = (ToDo)e.NewValue;
            }
        }

        public ToDo Data
        {
            get
            {
                return (ToDo)GetValue(ToDoData);
            }
            set
            {
                if (value != (ToDo)GetValue(ToDoData))
                { }

                SetValue(ToDoData, value);

                if (Data != null)
                {
                    Data.Remove = new DeleteButtonEvent(App.ManaLoc);
                    //Data.Complete = new DeleteButtonEvent(App.ManaLoc);

                    if (!EventAdded)
                    {
                        Data.Dates.CollectionChanged += Dates_CollectionChanged;
                        EventAdded = true;
                        Dates_CollectionChanged(null, null);
                    }
                }

                NotifyPropertyChanged(nameof(Data));
            }
        }

        public ToDoItemVM ViewModel
        {
            get
            {
                return (ToDoItemVM)GetValue(ToDoItmVM);
            }
            set
            {
                SetValue(ToDoItmVM, value);
                ViewModel.EnableTextSelect = !SettingsService.Instance.UseSlidableItems;
                ViewModel.messenger.Register<SelectedDetailItemMsg>(UpdateItem);
                NotifyPropertyChanged(nameof(ViewModel));
            }
        }

        private double whiteGradiOff;
        public double WhiteGradiOff
        {
            get { return whiteGradiOff; }
            set
            {
                whiteGradiOff = value;
                NotifyPropertyChanged(nameof(WhiteGradiOff));
            }
        }

        private double greenGradiOff;
        public double GreenGradiOff
        {
            get { return greenGradiOff; }
            set
            {
                greenGradiOff = value;
                NotifyPropertyChanged(nameof(GreenGradiOff));
            }
        }

        private string compToDoStr;
        public string CompToDoStr
        {
            get { return compToDoStr; }
            set
            {
                compToDoStr = value;
                NotifyPropertyChanged(nameof(CompToDoStr));
            }
        }

        double maxHeight = 400;

        public bool EventAdded { get; private set; }

        public void NotifyPropertyChanged([CallerMemberName]string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public event PropertyChangedEventHandler PropertyChanged;

        bool Showed = false;
        bool ShowedMore = false;
        SolidColorBrush CompButtFill = new SolidColorBrush();
        SolidColorBrush Green = new SolidColorBrush(Color.FromArgb(123, 9, 201, 0));
        SolidColorBrush Grey = new SolidColorBrush(Color.FromArgb(255, 64, 64, 64));
        SolidColorBrush Transparent = new SolidColorBrush(Color.FromArgb(0, 64, 64, 64));
        SolidColorBrush White = new SolidColorBrush(Colors.White);

        /// <summary>
        /// Init
        /// </summary>
        public ToDoItem()
        {
            CompButtFill.Color = Transparent.Color;

            InitializeComponent();

            HideDesc.Completed += HideDesc_Completed;
            HideMoreDesc.Completed += HideMoreDesc_Completed;

            ShowDescPartMain.From = MainPart.Height.Value;

            DetailPart.Height = new GridLength(SettingsService.Instance.ShowMoreInfo ? 40 : 0);
            Container.Height = (int)MainPart.Height.Value + (int)DetailPart.Height.Value + (int)ButtonPart.Height.Value;
        }

        /// <summary>
        /// Code for working with item especially when is checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Dates_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Functions Fce = new Functions();

            var DaysOfWeek = Fce.GetNumberOfWeekDays(Data.Start, Data.End);
            int AllDays = Fce.GetDaysCount(DaysOfWeek);

            GetProgressValue(Data.Neverend == true ? 50 : AllDays == 0 ? 0 : (Fce.GetDaysCount(Data.NotifyDays, Data.Dates) / (double)AllDays) * 100);

            CompButtFill.Color = Data.Dates.Contains(DateTime.Today) ? Green.Color : Transparent.Color;
            CompToDoStr = Data.Dates.Contains(DateTime.Today) ? "Uncomplete" : "Complete";

            if (Data.NotifyDays.Contains(DateTime.Today.DayOfWeek) && Data.Start.Date <= DateTime.Today && Data.End.Date >= DateTime.Today)
                CompleteToDo_PointerMoved(CompleteToDo, null);

            if (Data.Start.Date > DateTime.Today || (Data.End.Date < DateTime.Today && !Data.Neverend))
            {
                CompleteToDoContent.Foreground = new SolidColorBrush(Colors.White);
            }
        }

        /// <summary>
        /// Notify change
        /// </summary>
        /// <param name="obj"></param>
        private void UpdateItem(SelectedDetailItemMsg obj)
        {
            if (obj.ItemType == ItemTypeEnum.ToDo)
                if (Data != null && obj.ManagerID != null && obj.ManagerID != string.Empty)
                {
                    if (obj.ID == Data.ID)
                    {
                        Data = ((ToDosManager)App.ManaLoc?.GetManager(obj.ManagerID))?.GetItem(obj.ID);
                    }
                }
        }

        /// <summary>
        /// Compute offsets for showing progress value on item
        /// </summary>
        /// <param name="ProgressValue">Completed percentage of to - do</param>
        private void GetProgressValue(double ProgressValue)
        {
            if (ProgressValue <= 50)
            {
                WhiteGradiOff = (50 - ProgressValue) / 50 == 1 ? 0.999 : (50 - ProgressValue) / 50;
                GreenGradiOff = 1;
            }
            else
            {
                WhiteGradiOff = 0;
                GreenGradiOff = (100 - ProgressValue) / 50;
            }

            WhiteGradiAnim.From = WhiteGradi.Offset;
            GreenGradiAnim.From = GreenGradi.Offset;

            WhiteGradiAnim.To = WhiteGradiOff;
            GreenGradiAnim.To = GreenGradiOff;

            ToDoItemAnim.Begin();

            GC.Collect();
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
        /// Change ToDoItem for showing more informations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowMore_Click(object sender, RoutedEventArgs e)
        {
            double height = 0;

            if (Showed)
            {
                ((CompositeTransform)ShowMoreDescription.RenderTransform).ScaleY = 1;
                ShowMoreDescription.Padding = new Thickness(0, 0, 0, 2);

                HideDescPartDetail.From = DetailPart.Height.Value;
                HideDescPartMain.From = MainPart.Height.Value;

                HideDescPartDetail.To = SettingsService.Instance.ShowMoreInfo ? 40 : 0;

                HideDesc.Begin();

                //MainPart.Height = new GridLength(60);
                //DetailPart.Height = new GridLength(Services.SettingsServices.SettingsService.Instance.ShowMoreInfo ? 40 : 0);
                //ButtonPart.Height = new GridLength(0);

                height = (int)HideDescPartMain.To + (int)HideDescPartDetail.To + (int)HideDescPartButton.To;

                ShowedMore = false;
                Container.MaxHeight = 400;
            }
            else
            {
                ToDoDescription.Visibility = Visibility.Visible;

                int DescDesiredHeight = GetDesiredHeight(ToDoDescription);

                ShowDescPartDetail.From = DetailPart.Height.Value;
                ShowDescPartMain.From = MainPart.Height.Value;

                if (DescDesiredHeight >= (Container.MaxHeight - (MainPart.Height.Value + 60))) // 60 for button part
                {
                    ShowMoreDescription.Visibility = Visibility.Visible;
                    DescDesiredHeight = (int)(Container.MaxHeight - 130);
                }

                ShowDescPartDetail.To = ToDoDescription.Text != "" ? DescDesiredHeight + 10 : 10;
                ShowDescPartMain.To = 48;// Services.SettingsServices.SettingsService.Instance.ShowLongerTitle ? GetDesiredHeight(ToDoTitle, false) + 22 : 60;


                ShowDesc.Begin();

                height = (int)ShowDescPartMain.To + (int)ShowDescPartDetail.To + (int)ShowDescPartButton.To;
            }
            
            Showed = !Showed;
        }

        /// <summary>
        /// States for hiden description
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HideDesc_Completed(object sender, object e)
        {
            ToDoDescription.Visibility = SettingsService.Instance.ShowMoreInfo ? Visibility.Visible : Visibility.Collapsed;
            ShowMoreDescription.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Get desired heights of element
        /// </summary>
        /// <param name="DesignElement">Element for calculating</param>
        /// <returns>Desired size for element</returns>
        private int GetDesiredHeight(TextBlock DesignElement, bool Desc = true)
        {
            // Insp: opravit -> Problém s rozdílnými výsledky pro komponenty Title a Desc
            int LastHeight;
            int Increment = 100;
            //var old = DesignElement;

            do
            {
                LastHeight = (int)DesignElement.DesiredSize.Height;
                DesignElement.Measure(new Windows.Foundation.Size(Desc ? Increment : DesignElement.ActualWidth + 10, Increment));
                Increment += 100;

            } while ((int)DesignElement.DesiredSize.Height != LastHeight);

            //DesignElement = old;

            return LastHeight;
        }

        /// <summary>
        /// Show more description on ToDoItem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowMoreDescription_Click(object sender, RoutedEventArgs e)
        {
            if (ShowedMore)
            {
                HideMoreDescPartDetail.From = DetailPart.Height.Value;

                HideMoreDescPartDetail.To = maxHeight - 120;

                HideMoreDesc.Begin();
            }
            else
            {
                ShowMoreDescPartDetail.From = DetailPart.Height.Value;

                ShowMoreDescPartDetail.To = GetDesiredHeight(ToDoDescription) + 10;

                ShowMoreDesc.Begin();

                Container.Height = (int)MainPart.Height.Value + (int)ShowMoreDescPartDetail.To + (int)ButtonPart.Height.Value;
                Container.MaxHeight = Container.Height + 10;
            }

            ShowedMore = !ShowedMore;
        }

        /// <summary>
        /// States for hiden more description
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HideMoreDesc_Completed(object sender, object e)
        {
            Container.MaxHeight = maxHeight;
            Container.Height = maxHeight;
        }

        #region Functions for styling circle button

        private void ContentPresenter_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Ellipse ButtonEllipse = ((sender as Button).Content as Grid).Children.Where(x => x.GetType() == typeof(Ellipse)).First() as Ellipse;
            ButtonEllipse.StrokeThickness = 2;
            ButtonEllipse.Fill = Grey;

            ContentPresenter ButtonContent = ((sender as Button).Content as Grid).Children.Where(x => x.GetType() == typeof(ContentPresenter)).First() as ContentPresenter;
            if ((sender as Button).IsPressed)
                ButtonContent.Foreground = ItemTitle.Foreground;
            else
                ButtonContent.Foreground = new SolidColorBrush(Colors.White);
        }

        private void ContentPresenter_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Ellipse ButtonEllipse = ((sender as Button).Content as Grid).Children.Where(x => x.GetType() == typeof(Ellipse)).First() as Ellipse;
            ButtonEllipse.StrokeThickness = 1;
            ButtonEllipse.Fill = (sender as Button).Name == "CompleteToDo" ? CompButtFill : Transparent;

            ContentPresenter ButtonContent = ((sender as Button).Content as Grid).Children.Where(x => x.GetType() == typeof(ContentPresenter)).First() as ContentPresenter;
            ButtonContent.Foreground = ItemTitle.Foreground;
        }

        private void ContentPresenter_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            Ellipse ButtonEllipse = ((sender as Button).Content as Grid).Children.Where(x => x.GetType() == typeof(Ellipse)).First() as Ellipse;

            ButtonEllipse.StrokeThickness = 2;

            if ((sender as Button).IsPressed)
            {
                ButtonEllipse.Fill = Green;
            }
            else if ((sender as Button).IsPointerOver)
            {
                ButtonEllipse.Fill = Grey;
            }
            else
            {
                ButtonEllipse.StrokeThickness = 1;
                ButtonEllipse.Fill = (sender as Button).Name == "CompleteToDo" ? CompButtFill : Transparent;
            }

            ContentPresenter ButtonContent = ((sender as Button).Content as Grid).Children.Where(x => x.GetType() == typeof(ContentPresenter)).First() as ContentPresenter;
            if (!(sender as Button).IsPointerOver)
                ButtonContent.Foreground = ItemTitle.Foreground;
        }

        private void CompleteToDo_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (((sender as ContentPresenter).Content as Grid).Children.Where(x => x.GetType() == typeof(Ellipse)).First() is Ellipse ButtonEllipse)
                ButtonEllipse.Fill = Green;

            if (((sender as ContentPresenter).Content as Grid).Children.Where(x => x.GetType() == typeof(ContentPresenter)).First() is ContentPresenter ButtonContent)
                ButtonContent.Foreground = ItemTitle.Foreground;
        }

        private void CompleteToDo_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (((sender as ContentPresenter).Content as Grid).Children.Where(x => x.GetType() == typeof(Ellipse)).First() is Ellipse ButtonEllipse)
                ButtonEllipse.Fill = Grey;
        }

        private void CompleteToDo_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            Ellipse ButtonEllipse = ((sender as Button).Content as Grid).Children.Where(x => x.GetType() == typeof(Ellipse)).First() as Ellipse;
            ContentPresenter ButtonContent = ((sender as Button).Content as Grid).Children.Where(x => x.GetType() == typeof(ContentPresenter)).First() as ContentPresenter;

            ButtonEllipse.StrokeThickness = 2;
            ButtonContent.Foreground = ItemTitle.Foreground;

            if ((sender as Button).IsPressed)
            {
                ButtonEllipse.Fill = Green;
            }
            else if ((sender as Button).IsPointerOver)
            {
                ButtonEllipse.StrokeThickness = 2;
                ButtonContent.Foreground = new SolidColorBrush(Colors.White);
                ButtonEllipse.Fill = Grey;
            }
            else
            {
                ButtonEllipse.StrokeThickness = 1;
                ButtonEllipse.Fill = (sender as Button).Name == "CompleteToDo" ? CompButtFill : White;
            }
        }

        private void Tap(object sender, TappedRoutedEventArgs e)
        {
            if (((sender as ContentPresenter).Content as Grid).Children.Where(x => x.GetType() == typeof(Ellipse)).First() is Ellipse ButtonEllipse)
            {
                ButtonEllipse.StrokeThickness = 2;
                ButtonEllipse.Fill = Grey;
            }

            if (((sender as ContentPresenter).Content as Grid).Children.Where(x => x.GetType() == typeof(ContentPresenter)).First() is ContentPresenter ButtonContent)
                ButtonContent.Foreground = new SolidColorBrush(Colors.White);
        }

        #endregion
    }
}
