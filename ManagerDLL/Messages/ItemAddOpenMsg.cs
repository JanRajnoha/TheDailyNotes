using Framework.Messages.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Messages
{
    public class ItemAddOpenMsg : ItemBasedMsg
    {
        public int ID { get; set; } = -1;
    }
}
