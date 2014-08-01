using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.IO;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows;
using Models;

namespace Helpers
{

    public enum FileStatus
    {
        Matched = 0,
        UnMatched = 1,
        NotFound = 2
    }

    public class AlertColorConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            FileStatus v = (FileStatus)value;
            if (v == FileStatus.Matched)
                return new SolidColorBrush(Colors.Green);
            else if (v == FileStatus.UnMatched)
                return new SolidColorBrush(Colors.Blue);
            else if (v == FileStatus.NotFound)
                return new SolidColorBrush(Colors.Red);
            else
                return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FileIconConverter : IValueConverter
    {
        [DllImport("shell32.dll")]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        [DllImport("User32.dll")]
        public static extern int DestroyIcon(IntPtr hIcon);

        private const uint SHGFI_ICON = 0x100;
        private const uint SHGFI_LARGEICON = 0x0;
        private const uint SHGFI_SMALLICON = 0x1;
        private const int FILE_ATTRIBUTE_NORMAL = 0x80;
        private const int FILE_ATTRIBUTE_DIRECTORY = 0x10;
        private const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;

        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        private static Dictionary<string, ImageSource> imageList = new Dictionary<string, ImageSource>();
 
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            FileItem fileItem = (FileItem)value;
            string extension = string.Empty;
            if (fileItem != null)
            {
                if (fileItem.IsFolder)
                    extension = fileItem.DirectoryInformation.Extension;
                else
                    extension = fileItem.FileInformation.Extension;
                if (!imageList.ContainsKey(extension))
                {
                    ImageSource source = (fileItem.IsFolder) ? 
                        Extract(fileItem.DirectoryInformation.FullName, true) : 
                        Extract(fileItem.FileInformation.FullName, false);
                    if (source != null)
                    {
                        imageList.Add(extension, source);
                        return imageList[extension];
                    }
                    else
                        return null;

                }
                else
                    return imageList[extension];
            }
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private ImageSource Extract(string fileName, bool isFolder)
        {
            var shinfo = new SHFILEINFO();
            uint flags;
            uint fileFlags;
            BitmapSource bSource = null;

            flags = SHGFI_ICON | SHGFI_SMALLICON;
            flags |= SHGFI_USEFILEATTRIBUTES;

            if (isFolder)
                fileFlags = FILE_ATTRIBUTE_DIRECTORY;
            else
                fileFlags = FILE_ATTRIBUTE_NORMAL;

            IntPtr hIcon = SHGetFileInfo(fileName, fileFlags, ref shinfo, (uint)Marshal.SizeOf(shinfo), flags);
            try
            {
                if (hIcon != null)
                    bSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(shinfo.hIcon,
                        Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()).Clone();
            }
            finally
            {
                DestroyIcon(shinfo.hIcon);                
            }
            return bSource;
        }

    }


}
