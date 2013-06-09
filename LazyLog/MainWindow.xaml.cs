using LazyLog.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Xceed.Wpf.DataGrid;

namespace LazyLog
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // On the first row of the program comes an ugly hack:
        // We have to introduce DataGridCollectionViewCreator so we can create DataGridCollectionView
        //  and we don't want the model to know UI library object
        // Much better solution is to use DataGridCollectionViewSource defined in a XAML instead of creating DataGridCollectionView in code.
        // However, when we used DataGridCollectionViewSource the Filter event was not triggered.
        // TODO: fix this one

        class DataGridCollectionViewCreator : ICollectionViewCreator
        {
            public ICollectionView CreateView<T>(IEnumerable<T> source)
            {
                return new DataGridCollectionView(source);
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(new DataGridCollectionViewCreator());
        }
    }
}
