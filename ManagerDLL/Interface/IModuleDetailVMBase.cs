using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Framework.Interface
{
    public interface IModuleDetailVMBase<T>
    {
        T DetailItem { get; set; }

        ICommand Close { get; set; }
    }
}
