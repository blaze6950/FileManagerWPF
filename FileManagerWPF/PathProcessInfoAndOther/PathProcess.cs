using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace FileManagerWPF
{
    public static class PathProcess
    {
        public static List<DriveInfo> GetDrives()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            List<DriveInfo> newAllDrives = new List<DriveInfo>();
            foreach (var driveInfo in allDrives)
            {
                if (driveInfo.IsReady)
                {
                    newAllDrives.Add(driveInfo);
                }
            }
            return newAllDrives;
        }

        public static List<DirectoryInfo> GetDirectories(String path)
        {
            if (path == "" || path == null)
            {
                return null;
            }
            if (Path.HasExtension(path))
            {
                path = Path.GetDirectoryName(path);
            }
            return new DirectoryInfo(path).GetDirectories().ToList();
        }

        public static List<FileInfo> GetFiles(String path)
        {
            if (path == "" || path == null)
            {
                return null;
            }
            if (Path.HasExtension(path))
            {
                path = Path.GetDirectoryName(path);
            }
            return new DirectoryInfo(path).GetFiles().ToList();
        }

        public static DirectoryContentRepository GetDirectoryContentRepository(String path, bool full)
        {
            DirectoryContentRepository newDirectoryContentRepository = new DirectoryContentRepository();
            DirectoryContent directoryContent;
            if (path == "" || path == null)
            {
                var driveList = GetDrives();
                newDirectoryContentRepository = new DirectoryContentRepository();
                foreach (var driveInfo in driveList)
                {
                    directoryContent = new DirectoryContent();
                    directoryContent.Name = driveInfo.Name;
                    directoryContent.TotalSize = driveInfo.TotalSize;
                    directoryContent.FreeSpace = driveInfo.AvailableFreeSpace;
                    directoryContent.Icon = new BitmapImage(new Uri(@"\FlashUSBSSSSuuuurrr\FileManagerWPF\FileManagerWPF\icons\drive.png", UriKind.Relative));
                    newDirectoryContentRepository.DirectoryContent.Add(directoryContent);
                }
                return newDirectoryContentRepository;
            }
            var directoriesList = GetDirectories(path);
            foreach (var directoryInfo in directoriesList)
            {
                directoryContent = new DirectoryContent();
                directoryContent.Name = directoryInfo.Name;
                directoryContent.LastWriteTime = directoryInfo.LastWriteTime;
                directoryContent.Icon = new BitmapImage(new Uri(@"\FlashUSBSSSSuuuurrr\FileManagerWPF\FileManagerWPF\icons\folder.png", UriKind.Relative));
                newDirectoryContentRepository.DirectoryContent.Add(directoryContent);
            }
            if (full)
            {
                var filesList = GetFiles(path);
                foreach (var file in filesList)
                {
                    directoryContent = new DirectoryContent();
                    directoryContent.Name = file.Name;
                    directoryContent.LastWriteTime = file.LastWriteTime;
                    directoryContent.TotalSize = file.Length;
                    directoryContent.Icon = new BitmapImage(new Uri(@"\FlashUSBSSSSuuuurrr\FileManagerWPF\FileManagerWPF\icons\file.png", UriKind.Relative));
                    newDirectoryContentRepository.DirectoryContent.Add(directoryContent);
                }
            }            
            return newDirectoryContentRepository;
        }
    }
}
