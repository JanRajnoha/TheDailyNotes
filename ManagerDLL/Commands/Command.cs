using Framework.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Framework.Commands
{
    public abstract class Command : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public abstract bool CanExecute(object parameter);

        public abstract void Execute(object parameter);

        public void CommandCompletedChangedEvent(CommandCompletedEventArgs e)
        {
            CommandCompletedChanged?.Invoke(null, e);
        }

        public event EventHandler<CommandCompletedEventArgs> CommandCompletedChanged;
    }
}
