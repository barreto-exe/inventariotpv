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
using System.Windows.Shapes;

namespace Inventario_y_Contabilidad
{
    /// <summary>
    /// Lógica de interacción para VentaCantidad.xaml
    /// </summary>
    public partial class VentaCantidad : Window
    {
        public VentaCantidad()
        {
            InitializeComponent();

            txtCant.Focus();
            txtCant.SelectAll();
        }

        private void txtCant_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
                e.Handled = false;
            else
                e.Handled = true;

            if (e.Key == Key.Enter && txtCant.Text != "")
                this.Close();
        }

        private void btnAceptar_Click(object sender, RoutedEventArgs e)
        {
            if(txtCant.Text != "")
                this.Close();
        }
    }
}
