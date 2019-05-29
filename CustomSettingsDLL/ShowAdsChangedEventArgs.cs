using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSettingsDLL
{
    public class ShowAdsChangedEventArgs : EventArgs
    {
        public bool ShowAdsChangedOldState { get; set; }
        public bool ShowAdsChangedNewState { get; set; }
    }
}
