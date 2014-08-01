using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Models;
using System.IO;
using Helpers;
using System.Windows.Input;
using System.Windows;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Threading;
using QuickDiff;


namespace ViewModels
{
    public class FileItemsViewModel : BaseViewModel
    {

        public event Action<object> BuildComplete;

        public event Action<object> CompareComplete;

        private string folderLocation;

        public string FolderLocation
        {
            get { return folderLocation; }
            set
            {
               if (folderLocation != value)
                {
                    folderLocation = value;
                    RaisePropertyChanged(() => FolderLocation);
                }
            }
        }

        public ObservableCollection<FileItem> FileItems { get; set; }

        public FileItemsViewModel Sister { get; set; }

        public ICommand OpenFolder { get { return new DelegateCommand(OnOpenFolder); } }

        public ICommand BackFolder { get { return new DelegateCommand(OnBackFolder); } }

        public SynchronizationContext context { get; set; }

        public FileItemsViewModel()
        {
            FileItems = new ObservableCollection<FileItem>();
        }

        private void OnOpenFolder()
        {
            FolderBrowserDialog ofd = new FolderBrowserDialog();
            ofd.SelectedPath = FolderLocation;
            ofd.ShowNewFolderButton = true;
            ofd.Description = "Select Folder";            
            DialogResult dr = ofd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                FolderLocation = ofd.SelectedPath;
                Refresh();
            }
        }

        private void OnBackFolder()
        {
            NavigateSubFolder();
            Refresh();
        }

        public void Refresh(bool rebuildSisterList = false)
        {
            new Thread(() =>
            {
                BuildFileList();
                if (Sister != null)
                {
                    if (rebuildSisterList)
                        Sister.BuildFileList();
                    else
                        Sister.RemoveAllNotFound();
                    CompareFiles(Sister);
                    Sister.CompareFiles(this);
                }
            }).Start();
        }

        void fi_DoubleClicked(FileItem obj)
        {
            if (obj.IsFolder)
            {
                if (Directory.Exists(Path.Combine(FolderLocation, obj.FileName)))
                    FolderLocation = Path.Combine(FolderLocation, obj.FileName);
                if (Directory.Exists(Path.Combine(Sister.FolderLocation, obj.ParentFileItem.FileName)))
                    Sister.FolderLocation = Path.Combine(Sister.FolderLocation, obj.ParentFileItem.FileName);
                Refresh(true);
            }
            else
            {
                Helpers.Globals.GetExamDiffPath();
                if (string.IsNullOrEmpty(Helpers.Globals.ApplicationPath))
                    throw new Exception("The ExamDiff Application path could not be found or is not installed.");
                string arguments = string.Format(Helpers.Globals.ApplicationParameters,
                    obj.FileInformation.FullName, obj.ParentFileItem.FileInformation.FullName);
                Process.Start(new ProcessStartInfo(Helpers.Globals.ApplicationPath, arguments));
            }            
        }

        public void RemoveAllNotFound()
        {
            context.Send(x=> FileItems.RemoveAll(o => o.FileState == FileStatus.NotFound), null);
        }

        public void BuildFileList()
        {
            foreach (FileItem fi in FileItems)
            {
                fi.DoubleClicked -= fi_DoubleClicked;
            }
            context.Send(x =>  FileItems.Clear(), null);
            if (Directory.Exists(Path.GetFullPath(FolderLocation)))
            {
                string[] files = Directory.GetFiles(FolderLocation, "*.*", SearchOption.TopDirectoryOnly);
                foreach (string file in files)
                {
                    FileItem fi = new FileItem(file, false);
                    fi.DoubleClicked += new Action<FileItem>(fi_DoubleClicked);
                    context.Send(x=> FileItems.Add(fi), null);
                }
                string[] folders = Directory.GetDirectories(FolderLocation);
                foreach (string folder in folders)
                {
                    FileItem fi = new FileItem(folder, true);
                    fi.DoubleClicked += new Action<FileItem>(fi_DoubleClicked);
                    context.Send(x=> FileItems.Add(fi),null);
                }
            }
            if (BuildComplete != null)
                BuildComplete(this);
        }

        public void CompareFiles(FileItemsViewModel fileItems)
        {
            Sister = fileItems;

            foreach (FileItem item in fileItems.FileItems)
            {
                FileItem fileItem = FileItems.FirstOrDefault(fi => fi.FileName == item.FileName);
                if (fileItem != null)
                {
                    fileItem.ParentFileItem = item;
                    item.ParentFileItem = fileItem;
                    if (item.FileState == FileStatus.NotFound)
                    {
                        fileItem.FileState = FileStatus.UnMatched;                        
                    }
                    else
                        fileItem.FileState = fileItem.Equals(item) ? FileStatus.Matched : FileStatus.UnMatched;
                }
                else
                {
                    if (item.IsFolder)
                    {
                        FileItem fi = new FileItem(Path.Combine(FolderLocation, item.DirectoryInformation.Name), item.IsFolder);
                        fi.ParentFileItem = item;
                        item.ParentFileItem = fi;
                        context.Send(x=> FileItems.Add(fi), null);
                    }
                    else
                    {
                        FileItem fi = new FileItem(Path.Combine(FolderLocation, item.FileInformation.Name), item.IsFolder);
                        fi.ParentFileItem = item;
                        item.ParentFileItem = fi;
                        context.Send(x=> FileItems.Add(fi), null);
                    }
                }
            }
            if (CompareComplete != null)
                CompareComplete(this);
        }

        public void NavigateSubFolder()
        {
            try
            {
                FolderLocation = Directory.GetParent(FolderLocation).FullName;
            }
            catch { }
        }

    }
    
}
