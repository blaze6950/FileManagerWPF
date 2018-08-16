using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FileManagerWPF
{
    class PathProcessInfo
    {
        private String _path;
        private DirectoryContentRepository _directoryContentRepository;
        private ListView _listView;
        private TreeView _treeView;
        private TextBox _textBox;
        private MainWindow _mainWindow;

        public PathProcessInfo(ListView listView, TreeView treeView, TextBox textBox, MainWindow mainWindow)
        {
            _listView = listView;
            _treeView = treeView;
            _textBox = textBox;
            _mainWindow = mainWindow;
            Load();
        }

        public DirectoryContentRepository ContentRepository
        {
            get { return _directoryContentRepository; }
            set
            {
                _directoryContentRepository = value;
                RefreshData();
            }
        }

        public string Path1
        {
            get { return _path; }
            set { _path = value; }
        }

        public void GoTo(String path)
        {
            try
            {
                if (path != "")
                {
                    if (Path.HasExtension(path))
                    {
                        //start process
                    }
                    else
                    {
                        Path1 = path;
                        ContentRepository = PathProcess.GetDirectoryContentRepository(Path1, true);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void GoToRelative(String relativePath)
        {
            try
            {
                if (relativePath != "")
                {
                    if (Path.HasExtension(relativePath))
                    {
                        //start process
                    }
                    else
                    {
                        if (Path1[Path1.Length - 1] != '\\')
                        {
                            Path1 = Path1 + "\\";
                        }
                        Path1 = Path1 + relativePath;
                        ContentRepository = PathProcess.GetDirectoryContentRepository(Path1, true);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshData()
        {
            // показать, почему не появляются иконки
            //
            //
            //
            //
            //
            //
            //

            ///////////
            _listView.ItemsSource = ContentRepository.DirectoryContent;
            var view = _listView.View;
            if (view == _mainWindow.FindResource("GridView"))
            {
                SetColumnsGridView();
            }
            RefreshDataForTreeView();
        }

        private void RefreshDataForTreeView()
        {
            if (Path1 != null)
            {
                List<String> pathes = new List<string>();
                pathes.Add(Path1);
                while (true)
                {
                    string buf;
                    buf = Path.GetDirectoryName(pathes[pathes.Count - 1]);
                    if (null == buf)
                    {
                        break;
                    }
                    pathes.Add(buf);
                }
                pathes.Reverse();
                ItemCollection treeItemCollection = _treeView.Items;
                foreach (var path in pathes)
                {
                    var path1 = Path.GetFileName(path);
                    if (path1 == "")
                    {
                        path1 = path;
                    }
                    foreach (var treeViewItem in treeItemCollection)
                    {
                        if (((TreeViewItem) treeViewItem).Tag is DirectoryInfo)
                        {
                            if (((DirectoryInfo) ((TreeViewItem) treeViewItem).Tag).Name == path1)
                            {
                                ((TreeViewItem) treeViewItem).IsExpanded = true;
                                treeItemCollection = ((TreeViewItem) treeViewItem).Items;
                                break;
                            }
                        }
                        else
                        {
                            if (((DriveInfo)((TreeViewItem)treeViewItem).Tag).Name == path1)
                            {
                                ((TreeViewItem)treeViewItem).IsExpanded = true;
                                treeItemCollection = ((TreeViewItem)treeViewItem).Items;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                _treeView.Items.Clear();

                foreach (DriveInfo drive in PathProcess.GetDrives())
                {
                    TreeViewItem item = new TreeViewItem();
                    item.Tag = drive;
                    item.Header = drive.ToString();
                    item.Expanded += TreeViewItemExpanded;
                    item.Selected += TreeViewItemSelected;
                    item.Items.Add("*");
                    _treeView.Items.Add(item);
                }
            }
        }

        private void TreeViewItemSelected(object sender, RoutedEventArgs routedEventArgs)
        {
            TreeViewItem item = (TreeViewItem)routedEventArgs.OriginalSource;

            DirectoryInfo dir;
            if (item.Tag is DriveInfo)
            {
                DriveInfo drive = (DriveInfo)item.Tag;
                dir = drive.RootDirectory;
            }
            else
            {
                dir = (DirectoryInfo)item.Tag;
            }
            
            GoTo(dir.FullName);
            _textBox.Text = Path1;
        }

        private void TreeViewItemExpanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)e.OriginalSource;

            // Perform a refresh every time item is expanded.
            // Optionally, you could perform this only the first
            // time, when the placeholder is found (less refreshes),
            // every time an item is selected (more refreshes)
            // or when a message is received by the FileSystemWatcher
            // (intelligent refreshes, requiring the most overhead).
            item.Items.Clear();

            DirectoryInfo dir;
            if (item.Tag is DriveInfo)
            {
                DriveInfo drive = (DriveInfo)item.Tag;
                dir = drive.RootDirectory;
            }
            else
            {
                dir = (DirectoryInfo)item.Tag;
            }

            try
            {
                foreach (DirectoryInfo subDir in dir.GetDirectories())
                {
                    TreeViewItem newItem = new TreeViewItem();
                    newItem.Tag = subDir;
                    newItem.Header = subDir.ToString();
                    newItem.Items.Add("*");
                    item.Items.Add(newItem);
                }
            }
            catch
            {
                // An exception could be thrown in this code if you don't
                // have sufficient security permissions for a file or directory.
                // You can catch and then ignore this exception.
            }
        }

        private void SetColumnsGridView()
        {
            var gridView = _mainWindow.FindResource("GridView");
            ((GridView)gridView).Columns.Clear();
            foreach (var directoryContent in _directoryContentRepository.DirectoryContent)
            {
                if (directoryContent.Icon != null)
                {
                    if (SetColumn("Icon", ((GridView)gridView).Columns))
                    {
                        ((GridView)gridView).Columns.Add((GridViewColumn)_mainWindow.FindResource("DataTemplateForIcon"));
                    }
                }
                if (directoryContent.Name != null)
                {
                    if (SetColumn("Name", ((GridView)gridView).Columns))
                    {
                        ((GridView)gridView).Columns.Add((GridViewColumn)_mainWindow.FindResource("DataTemplateForName"));
                    }
                }
                if (directoryContent.FreeSpace != null)
                {
                    if (SetColumn("Free space", ((GridView)gridView).Columns))
                    {
                        ((GridView)gridView).Columns.Add((GridViewColumn)_mainWindow.FindResource("DataTemplateForFreeSpace"));
                    }
                }
                if (directoryContent.TotalSize != null)
                {
                    if (SetColumn("Total size", ((GridView)gridView).Columns))
                    {
                        ((GridView)gridView).Columns.Add((GridViewColumn)_mainWindow.FindResource("DataTemplateForTotalSize"));
                    }
                }
                if (directoryContent.LastWriteTime != null)
                {
                    if (SetColumn("Last write time", ((GridView)gridView).Columns))
                    {
                        ((GridView)gridView).Columns.Add((GridViewColumn)_mainWindow.FindResource("DataTemplateForLastWriteTime"));
                    }
                }
            }
        }

        private bool SetColumn(String columnHeader, GridViewColumnCollection columns)
        {
            foreach (var column in columns)
            {
                if ((string) column.Header == columnHeader)
                {
                    return false;
                }
            }
            return true;
        }

        private void Load()
        {
            GoTo(Path1);
        }
    }
}
