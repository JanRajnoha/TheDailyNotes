using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Framework.Controls
{
    public sealed partial class NavigationView : Windows.UI.Xaml.Controls.NavigationView, INotifyPropertyChanged
    {
        public static readonly DependencyProperty ShowMenuAd = DependencyProperty.Register("ShowAd", typeof(Visibility), typeof(NavigationView), new PropertyMetadata(null, OnShowAdChanged));

        private static void OnShowAdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is Visibility)
            {
                var control = (NavigationView)d;
                control.ShowAd = (Visibility)e.NewValue;
            }
        }

        public Visibility ShowAd
        {
            get
            {
                return (Visibility)GetValue(ShowMenuAd);
            }
            set
            {
                SetValue(ShowMenuAd, value);

                NotifyPropertyChanged(nameof(ShowMenuAd));
            }
        }

        public void NotifyPropertyChanged([CallerMemberName]string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public event PropertyChangedEventHandler PropertyChanged;

        public NavigationView() : base()
        {
            InitializeComponent();

            ShowAd = Visibility.Visible;
        }
    }
}
