using Framework.Classes;
using Framework.Enum;
using Framework.Interface;
using Framework.Messages;
using Framework.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Template10.Mvvm;

namespace Framework.ViewModel
{
    public abstract class ModuleDetailVMBase<T> : ViewModelBase, IModuleDetailVMBase<T> where T : BaseItem
    {
        protected Messenger messenger;
        ItemTypeEnum ItemType;

        private T detailItem;
        public T DetailItem
        {
            get { return detailItem; }
            set
            {
                detailItem = value;
                RaisePropertyChanged(nameof(DetailItem));
            }
        }

        protected abstract void DetailOpen(SelectedDetailItemMsg obj);

        public ICommand Close { get; set; }

        public ModuleDetailVMBase()
        {

        }

        public ModuleDetailVMBase(Messenger messenger, ItemTypeEnum itemType) : this()
        {
            this.messenger = messenger;
            ItemType = itemType;

            Close = new RelayCommand(() => this.messenger.Send(new ItemDetailCloseMsg()
            {
                ItemType = ItemType
            }));

            messenger.Register<SelectedDetailItemMsg>(DetailOpen);
        }
    }
}
