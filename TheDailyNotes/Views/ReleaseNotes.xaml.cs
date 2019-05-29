using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace TheDailyNotes.Views
{
    public sealed partial class ReleaseNotes : UserControl
    {
        public string ReleaseNote { get; set; }

        public string Version { get; set; }

        public ReleaseNotes()
        {
            InitializeComponent();

            var v = Windows.ApplicationModel.Package.Current.Id.Version;
            Version = $"{v.Major}.{v.Minor}.{v.Build}";

            ReleaseNote = "- New features\n" +
                "- New module\n" +
                //"- Premium features\n" +
                "- Improved encrypting system\n" +
                "- Improved background functions\n" +
                "- Bug fix\n" +
                "Full report in Settings";
            //"- Get beta updates\n" +
            //"Sign up in Settings\n" +
            //"- Improved design\n" +
        }
    }
}
