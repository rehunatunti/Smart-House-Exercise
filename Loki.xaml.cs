using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AlytaloWPF
{
    /// <summary>
    /// Interaction logic for Loki.xaml
    /// </summary>
    public partial class Loki : Window
    {
        public Loki()
        {
            InitializeComponent();
        }
        public void VieLokiin(string lokiteksti)
        {
            DateTime aika = DateTime.Now;
            string formaattiAika = "HH:mm:ss ";
            
            txtLokiWindow.AppendText(aika.ToString(formaattiAika) + lokiteksti);
            txtLokiWindow.AppendText("\n");
        }

        private void btnSulje_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txtLokiWindow_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBoxBase tbb = sender as TextBoxBase;
            tbb.ScrollToEnd();
        }
    }
}
