using CustomSettingsDLL;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.ComponentModel;
using Framework.Manager;

namespace Modules.Notes.Classes
{
    public class NotesManager : ItemManager<Note>
    {
        public NotesManager(string FileName = "Notes.tdn") : base(FileName)
        {
            CustomSettings.UserLogChanged += CustomSettings_UserLogChanged;
        }

        protected override void CustomSettings_UserLogChanged(object sender, UserLoggedEventArgs e)
        {
            Debug.WriteLine($"User secure settings has changed. Log called from {GetType().ToString()}.");
        }
    }
}
