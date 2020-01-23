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

            SqlCeCommand command = new SqlCeCommand("SELECT TOP 1 * FROM c_tasa ORDER BY id DESC", MainWindow.conn);
            SqlCeDataReader dr = command.ExecuteReader();
            dr.Read();

            txtMontoTasa.Text    = dr["tasaDolar"].ToString();
            txtPorcentajeBS.Text = dr["porcentajeEfectivo"].ToString();

            dr.Close();
        }

        private void btnAceptar_Click(object sender, RoutedEventArgs e)
        {
            float n = 0;

            //Validar que no sea texto vacío ni caracteres alfabéticos o especiales.
            if (               txtMontoTasa.Text == ""   || txtPorcentajeBS.Text == "" ||
               !float.TryParse(txtMontoTasa.Text, out n) || !float.TryParse(txtPorcentajeBS.Text, out n))
            {
                return;
            }

            string query = "INSERT INTO c_tasa (tasaDolar,porcentajeEfectivo,fechaHora) " +
                            "VALUES(" 
                            + CS(txtMontoTasa.Text) + "," 
                            + CS(txtPorcentajeBS.Text) + "," 
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
    }
}
