using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace Framework.Controls
{
    public sealed class NavigationView : Windows.UI.Xaml.Controls.NavigationView
    {
        public static readonly DependencyProperty ShowAdProperty = DependencyProperty.Register("ShowAd", typeof(Visibility), typeof(NavigationView), new PropertyMetadata(false));

        private static void OnShowAdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is Visibility)
            {
                var control = (NavigationView)d;
                control.ShowAd = (Visibility)e.NewValue;
            }
        }

        public static void SetShowAd(UIElement element, Visibility value)
        {
            element.SetValue(ShowAdProperty, value);
        }

        public static Visibility GetShowAd(UIElement element)
        {
            return (Visibility)element.GetValue(ShowAdProperty);
        }

        public Visibility ShowAd
        {
            get => (Visibility)GetValue(ShowAdProperty);
            set
            {
                SetValue(ShowAdProperty, value);

                NotifyPropertyChanged(nameof(ShowAdProperty));
            }
        }

        public void NotifyPropertyChanged([CallerMemberName]string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public event PropertyChangedEventHandler PropertyChanged;

        public NavigationView()
        {
               DefaultStyleKey = typeof(NavigationView);
        }
    }
}
