using Framework.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Framework.Classes
{
    public class MessageDialog
    {
        public Windows.UI.Popups.MessageDialog PopupMessageDialog = new Windows.UI.Popups.MessageDialog("");

        public MessageDialog(string description, string header = "", MessageDialogButtonsEnum messageDialogButtons = MessageDialogButtonsEnum.Ok)
        {
            PopupMessageDialog = new Windows.UI.Popups.MessageDialog(description, header);

            switch (messageDialogButtons)
            {
                case MessageDialogButtonsEnum.Ok:
                    PopupMessageDialog.Commands.Add(new UICommand("Ok", new UICommandInvokedHandler(EscapeFce), MessageDialogResultEnum.Ok));

                    PopupMessageDialog.DefaultCommandIndex = 1;
                    PopupMessageDialog.CancelCommandIndex = 1;
                    break;

                case MessageDialogButtonsEnum.YesNo:
                    PopupMessageDialog.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(EscapeFce), MessageDialogResultEnum.Yes));
                    PopupMessageDialog.Commands.Add(new UICommand("No", new UICommandInvokedHandler(EscapeFce), MessageDialogResultEnum.No));

                    PopupMessageDialog.DefaultCommandIndex = 0;
                    PopupMessageDialog.CancelCommandIndex = 1;
                    break;

                case MessageDialogButtonsEnum.AbortRetryIgnore:
                    PopupMessageDialog.Commands.Add(new UICommand("Abort", new UICommandInvokedHandler(EscapeFce), MessageDialogResultEnum.Abort));
                    PopupMessageDialog.Commands.Add(new UICommand("Retry", new UICommandInvokedHandler(EscapeFce), MessageDialogResultEnum.Retry));
                    PopupMessageDialog.Commands.Add(new UICommand("Ignore", new UICommandInvokedHandler(EscapeFce), MessageDialogResultEnum.Ignore));

                    PopupMessageDialog.DefaultCommandIndex = 0;
                    PopupMessageDialog.CancelCommandIndex = 1;
                    break;

                default:
                    break;
            }
        }

        public void SetCustomButtons(List<UICommand> commands, bool clearButtons = true, uint defaultCommand = 0, uint cancelCommand = 1)
        {
            if (clearButtons)
                PopupMessageDialog.Commands.Clear();

            if (commands.Count != 0)
                foreach (var command in commands)
                    PopupMessageDialog.Commands.Add(command);

            PopupMessageDialog.DefaultCommandIndex = defaultCommand;

            PopupMessageDialog.CancelCommandIndex = cancelCommand;
        }

        private void EscapeFce(IUICommand command)
        {
            return;
        }

        public async Task<MessageDialogResultEnum> ShowAsync()
        {
            var res = (await PopupMessageDialog.ShowAsync());

            if (res.Id == null)
                return MessageDialogResultEnum.Ok;
            else
                return (MessageDialogResultEnum)res.Id;
        }
    }
}
