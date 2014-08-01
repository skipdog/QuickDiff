using System.Collections.Generic;
using Models;
using System.Windows.Input;
using Helpers;
using System.Threading;
using System.IO;
using System;
using System.Windows;

namespace ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {        

        public FileItemsViewModel FileItemsLeft { get; set; }

        public FileItemsViewModel FileItemsRight { get; set; }

        public ICommand BackFolders { get { return new DelegateCommand(OnBackFolders); } }

        public ICommand RefreshFolders { get { return new DelegateCommand(OnRefreshFolders); } }

        public MainWindowViewModel()
        {
            FileItemsLeft = new FileItemsViewModel();            
            FileItemsRight = new FileItemsViewModel();
            FileItemsLeft.FolderLocation = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            FileItemsRight.FolderLocation = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            FileItemsLeft.context = SynchronizationContext.Current;
            FileItemsRight.context = SynchronizationContext.Current;
            FileItemsLeft.Sister = FileItemsRight;
            FileItemsLeft.Refresh(true);            
        }

        private void OnBackFolders()
        {
            FileItemsLeft.NavigateSubFolder();
            FileItemsRight.NavigateSubFolder();
            FileItemsLeft.Refresh(true);
        }

        private void OnRefreshFolders()
        {
            FileItemsLeft.Refresh(true);
        }

    }
}
