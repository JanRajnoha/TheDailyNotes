using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Template10.Mvvm;
using Windows.ApplicationModel.DataTransfer;

namespace Framework.Interface
{
    public interface IModuleItemVMBase<T>
    {
        T CurrentItem { get; set; }

        ICommand ShareCommand { get; set; }

        ICommand EditCommand { get; set; }
    }
}
