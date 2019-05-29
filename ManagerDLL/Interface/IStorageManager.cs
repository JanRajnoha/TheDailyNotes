using Framework.Enum;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Framework.Interface
{
    public interface IStorageManager<T>
    {
        void SetSourceCollection(ObservableCollection<T> source);

        Type GetClassType();
    }
}
