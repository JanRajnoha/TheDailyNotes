﻿using Framework.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Core;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Framework.Interface
{
    public interface IModuleVMBase<T>
    {
        Messenger Messenger { get; set; }

        DataTransferManager DaTranManaItems { get; set; }

        ListViewSelectionMode ListSelectionMode { get; set; }

        ObservableCollection<T> Source { get; set; }

        ObservableCollection<PivotItem> PivotPanes { get; set; }

        bool PaneVisibility { get; set; }

        bool StartTileAdded { get; set; }

        string InAppNotifyText { get; set; }

        bool InAppNotifyShow { get; set; }

        string ModuleName { get; set; }

        GridLength MasterFrame { get; set; }

        GridLength SlaveFrame { get; set; }

        SecondaryTile SecTile { get; set; }

        ICommand SlavePaneVisibilityCommand { get; set; }

        ICommand DeleteItems { get; set; }

        ICommand SelectAllItems { get; set; }

        ICommand AddStartTile { get; set; }

        ICommand ChangePaneVisibility { get; set; }

        ICommand AddItem { get; set; }

        ICommand ChangeSelectionMode { get; set; }

        ICommand DetailCommand { get; set; }

        ICommand EditCommand { get; set; }

        ICommand ShareItems { get; set; }

        void CloseCurrentSlavePane(object sender, BackRequestedEventArgs e);

        void ClosePane();

        void SecTileManageAsync();

        void InversePaneVisibility();

        Task Loaded();

        Task OnLoadAsync(bool SecureChanged = false);
    }
}
