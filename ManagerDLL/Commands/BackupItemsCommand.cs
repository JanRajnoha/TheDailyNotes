using Framework.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Framework.Commands
{
    public class BackupItemsCommand : Command
    {
        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public async override void Execute(object parameter)
        {
            if (!(parameter is string fileExtension))
                return;
            else if (!fileExtension.Contains('.') || fileExtension.IndexOf('.') != 0)
                    fileExtension = "." + fileExtension;

            FolderPicker backupLocation = new FolderPicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };

            backupLocation.FileTypeFilter.Add(fileExtension);
            var selectedFolder = await backupLocation.PickSingleFolderAsync();

            var filesList = await ApplicationData.Current.LocalFolder.GetFilesAsync();

            foreach (var item in filesList.Where(x => x.Name.Contains(fileExtension) && (x.Name.IndexOf(fileExtension) + fileExtension.Length == x.Name.Length)))
            {
                await item.CopyAsync(selectedFolder, item.Name, NameCollisionOption.ReplaceExisting);
            }

            CommandCompletedEventArgs args = new CommandCompletedEventArgs()
            {
                CommandName = GetType().Name,
                CommandSuccessful = true,
                ResultText = "Backup was successful"
            };
            CommandCompletedChangedEvent(args);
        }
    }
}
