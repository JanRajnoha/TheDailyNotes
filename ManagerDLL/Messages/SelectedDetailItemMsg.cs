using Framework.Messages.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Messages
{
    public class SelectedDetailItemMsg : ItemBasedMsg
    {
        public int ID { get; set; } = -1;

        public bool Edit { get; set; } = false;

        public string ManagerID { get; set; } = string.Empty;
    }
}
