using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Framework.Messages
{
    public class ShowModalActivationMsg
    {
        public IReadOnlyList<IStorageItem> Files { get; set; }
    }
}
