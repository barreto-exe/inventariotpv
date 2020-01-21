using System;
using System.Collections.Generic;
using System.IO;
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

namespace Inventario_y_Contabilidad
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            if(!!File.Exists("bd.sdf"))
            {
                MessageBox.Show("No existe bd.sdf", "Error");
                Environment.Exit(0);
            }

            InitializeComponent();
        }

        private void btnDetalle_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCambiarTasa_Click(object sender, RoutedEventArgs e)
        {

        }

        private void imgVender_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Venta venta = new Venta();
            venta.Owner = this;
            venta.ShowDialog();
        }
    }
}
