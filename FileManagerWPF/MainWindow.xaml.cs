using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileManagerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PathProcessInfo _pathProcessInfo;
        public MainWindow()
        {
            InitializeComponent();
            _pathProcessInfo = new PathProcessInfo(ListView, TreeView, TextBox, this);
        }

        private void ComboBoxView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)ComboBox.SelectedItem;
            if(selectedItem.Content != null)
                ListView.View = (ViewBase)this.FindResource(selectedItem.Content);
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListView.SelectedItem != null)
            {
                if (TextBox.Text == String.Empty)
                {
                    _pathProcessInfo.GoTo(((DirectoryContent)ListView.SelectedItem).Name);
                    TextBox.Text = _pathProcessInfo.Path1;
                }
                else
                {
                    _pathProcessInfo.GoToRelative(((DirectoryContent)ListView.SelectedItem).Name);
                    TextBox.Text = _pathProcessInfo.Path1;
                }
            }
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _pathProcessInfo.GoTo(TextBox.Text);
            }
        }
    }
}
