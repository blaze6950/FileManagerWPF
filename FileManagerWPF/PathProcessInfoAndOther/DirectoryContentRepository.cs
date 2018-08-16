using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManagerWPF
{
    public class DirectoryContentRepository
    {
        private ObservableCollection<DirectoryContent> _directoryContent;

        public DirectoryContentRepository()
        {
            _directoryContent = new ObservableCollection<DirectoryContent>();
        }

        public ObservableCollection<DirectoryContent> DirectoryContent
        {
            get { return _directoryContent; }
            set { _directoryContent = value; }
        }
    }
}
