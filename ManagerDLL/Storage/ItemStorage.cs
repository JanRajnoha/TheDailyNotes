using Framework.Enum;
using Framework.Template;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Storage
{
    public class ItemStorage<T> where T : BaseItem
    {
        public ItemTypeEnum TypeOfItem { get; set; }

        public ObservableCollection<T> Items { get; set; } = new ObservableCollection<T>();
    }
}
