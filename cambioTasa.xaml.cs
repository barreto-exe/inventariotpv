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
using System.Data.SqlServerCe;

namespace Inventario_y_Contabilidad
{
    /// <summary>
    /// Lógica de interacción para cambioTasa.xaml
    /// </summary>
    public partial class cambioTasa : Window
    {
        public cambioTasa()
        {
            InitializeComponent();

            txtMontoTasa.Focus();

            SqlCeCommand command = new SqlCeCommand("SELECT TOP 1 * FROM c_tasa ORDER BY id DESC", MainWindow.conn);
            SqlCeDataReader dr = command.ExecuteReader();
            dr.Read();

            txtMontoTasa.Text    = decimal.Parse(dr["tasaDolar"].ToString()).ToString("#,#0.##");
            txtPorcentajeBS.Text = dr["porcentajeEfectivo"].ToString();

            dr.Close();
        }

        private void btnAceptar_Click(object sender, RoutedEventArgs e)
        {
            decimal n = 0;

            //Validar que no sea texto vacío ni caracteres alfabéticos o especiales.
            if (               txtMontoTasa.Text == ""   || txtPorcentajeBS.Text == "" ||
               !decimal.TryParse(txtMontoTasa.Text, out n) || !decimal.TryParse(txtPorcentajeBS.Text, out n))
            {
                return;
            }

            string query = "INSERT INTO c_tasa (tasaDolar,porcentajeEfectivo,fechaHora) " +
                            "VALUES(" 
                            + decimal.Parse(txtMontoTasa.Text).ToString().Replace(",",".") + "," 
                            + txtPorcentajeBS.Text + "," 
                            + CS(String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now)) + ")";

            SqlCeCommand command = new SqlCeCommand(query, MainWindow.conn);
            command.ExecuteReader();

            this.Close();
        }
        
        private string CS(string strSinComillas)
        {
            //Añade comillas simples
            strSinComillas = strSinComillas.Replace("'", "''");
            return "'" + strSinComillas + "'";
        }

        private void txtMontoTasa_KeyDown(object sender, KeyEventArgs e)
        {
            //Oculta el cursor
            txtMontoTasa.CaretBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

            if (e.Key == Key.Tab)
            {
                txtPorcentajeBS.Focus();
            }

            e.Handled = true;

            if ((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
            {
                string tecla = e.Key.ToString().Substring(e.Key.ToString().Length - 1, 1);
                decimal numAnterior = 0;
                if (txtMontoTasa.Text != "")
                {
                    numAnterior = Decimal.Parse(txtMontoTasa.Text) * 100;
                }
                string strNumNuevo = numAnterior.ToString().Replace(",00", "") + tecla;
                decimal numNuevo = decimal.Parse(strNumNuevo) / 100;

                txtMontoTasa.Text = String.Format("{0:#,0.00}", numNuevo);
            }
        }
        private void txtMontoTasa_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                txtMontoTasa.Text = "";
            }
        }
        private void txtMontoTasa_GotFocus(object sender, RoutedEventArgs e)
        {
            txtMontoTasa.SelectAll();
        }

        private void txtPorcentajeBS_GotFocus(object sender, RoutedEventArgs e)
        {
            txtPorcentajeBS.SelectAll();
        }
    }
}
