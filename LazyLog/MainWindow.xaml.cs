using LazyLog.ViewModel;
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
using Xceed.Wpf.DataGrid;

namespace LazyLog
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        /*
        private void OnDataGridContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            FrameworkElement fe = e.Source as FrameworkElement;
            LogViewModel logViewModel = fe.DataContext as LogViewModel;
            if (logViewModel.SelectedItem == null)
            {
                e.Handled = true;
                return;
            }
            IList<string> menuHeaders = logViewModel.GetMenuHeaders();            
            ContextMenu cm = new ContextMenu();
            foreach (string menuHeader in menuHeaders)
            {
                cm.Items.Add(new MenuItem() { Header = menuHeader });
            }
            
            fe.ContextMenu = cm;                        
        }
         */

    }
}
