using Helpers;
using System;
using System.IO;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows;
using ViewModels;

namespace Models
{
    public class FileItem : BaseModel
    {
        public event Action<FileItem> DoubleClicked;

        public FileItem ParentFileItem { get; set; }

        public FileInfo FileInformation { get; set; }

        public DirectoryInfo DirectoryInformation { get; set; }

        public string FileName { get; set; }        

        public long Size { get; set; }

        public string Modified { get; set; }

        public bool IsFolder { get; set; }

        private FileStatus fileState;
        public FileStatus FileState 
        {
            get { return fileState; }
            set
            {
                fileState = value;
                RaisePropertyChanged(() => FileState);
            }
        }

        public ICommand DblClickFileItem { get { return new DelegateCommand(OnDblClickFileItem, CanOnDblClickFileItem); } }

        public FileItem(string fileName, bool isFolder)
        {            

            IsFolder = isFolder;

            if (IsFolder)
            {
                DirectoryInformation = new DirectoryInfo(fileName);
                FileName = DirectoryInformation.Name;
                if (DirectoryInformation.Exists)
                {
                    Modified = DirectoryInformation.LastWriteTimeUtc.ToString();
                    Size = 0;
                    FileState = FileStatus.UnMatched;
                }
                else
                {
                    Modified = "N/A";
                    Size = 0;
                    FileState = FileStatus.NotFound;
                }
            }
            else
            {
                FileInformation = new FileInfo(fileName);
                FileName = FileInformation.Name;
                if (FileInformation.Exists)
                {
                    Modified = FileInformation.LastWriteTimeUtc.ToString();
                    Size = FileInformation.Length;
                    FileState = FileStatus.UnMatched;
                }
                else
                {
                    Modified = "N/A";
                    Size = 0;
                    FileState = FileStatus.NotFound;
                }
            }
        }        

        private void OnDblClickFileItem()
        {
            if (DoubleClicked != null)
                DoubleClicked(this);
        }

        bool CanOnDblClickFileItem()
        {
            if (IsFolder)
            {
                return DirectoryInformation.Exists || ParentFileItem.DirectoryInformation.Exists;
            }
            else
                return FileInformation.Exists && ParentFileItem.FileInformation.Exists;
        }

        public bool Equals(FileItem fileInfo)
        {
            if (IsFolder)
            {
                return true;
                //need to add code to diff folder contents and sub folder contents.
            }
            else
            {
                if (FileInformation.FullName.Equals(fileInfo.FileInformation.FullName))
                    return true;
                if (FileInformation.Length != fileInfo.FileInformation.Length)
                    return false;
                int iterations = (int)Math.Ceiling((double)FileInformation.Length / sizeof(Int64));
                using (FileStream fs1 = FileInformation.OpenRead())
                using (FileStream fs2 = fileInfo.FileInformation.OpenRead())
                {
                    byte[] one = new byte[sizeof(Int64)];
                    byte[] two = new byte[sizeof(Int64)];

                    for (int i = 0; i < iterations; i++)
                    {
                        fs1.Read(one, 0, sizeof(Int64));
                        fs2.Read(two, 0, sizeof(Int64));

                        if (BitConverter.ToInt64(one, 0) != BitConverter.ToInt64(two, 0))
                            return false;
                    }
                }
                return true;
            }
        }

    }
}
