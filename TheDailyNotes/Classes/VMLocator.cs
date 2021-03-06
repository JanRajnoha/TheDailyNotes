﻿using Framework.Classes;
using Framework.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheDailyNotes.ViewModels;

namespace TheDailyNotes.Classes
{
    public partial class VMLocator
    {
        private readonly Messenger messenger = new Messenger();

        public void SendMessage(ShowModalActivationMsg showModalActivation)
        {
            messenger.Send(showModalActivation);
        }

        public Messenger GetMessenger()
        {
            return messenger;
        }
    }
}
