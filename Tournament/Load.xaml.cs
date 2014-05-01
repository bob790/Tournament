using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;

namespace Tournament
{
    /// <summary>
    /// Interaction logic for Load.xaml
    /// </summary>
    public partial class Load : Page
    {
        public Load()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.DefaultExt = ".txt";
            dlg.Filter = "New Files (*.txt)|*.txt|Save Files (*.xml)|*.xml|All Files|*.*";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                if (File.Exists(dlg.FileName))
                {
                    string filename = dlg.FileName;
                    txtFile.Text = filename;
                }
            }
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            MainPage mainPage = new MainPage(txtFile.Text);
            this.NavigationService.Navigate(mainPage);
        }
    }
}
