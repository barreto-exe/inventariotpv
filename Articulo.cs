using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
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
    /// Lógica de interacción para ArticuloCrear.xaml
    /// </summary>
    public partial class Articulo : Window
    {
        public int tipoInfo;
        public int articuloBuscado = 0;
        public Articulo(int tipo)
        {
            InitializeComponent();

            tipoInfo = tipo;
            //Nuevo artículo.
            if (tipo == 1)
            {
                txtDescArt.Focus();
                btnBuscar.IsEnabled = false;
                lblIdArt.Content = "";
            }
            //Actualizar cantidad y precio artículo
            if(tipo == 2)
            {
                txtPrecioDolar.Focus();
                setArticuloBuscado();
            }
        }
        private void btnAceptar_Click(object sender, RoutedEventArgs e)
        {
            decimal n = 0; 

            if( txtCantidadAgg.Text == "" || txtCostoDolar.Text == ""  ||
                txtDescArt.Text == ""        || txtPrecioDolar.Text == "" ||
                !decimal.TryParse(txtPrecioDolar.Text,out n) ||
                !decimal.TryParse(txtCostoDolar.Text, out n) ||
                !decimal.TryParse(txtCantidadAgg.Text, out n)
                )
            {
                return;
            }

            switch(tipoInfo)
            {
                case 1: crearArticulo();  break;
                case 2: editarArticulo(); break;
            }

            this.Close();
        }
        private void crearArticulo()
        {
            //Insertando en tabla artículos
            string query = "INSERT INTO c_articulos (descripcion,precioDolar,costoDolar) " +
                           "VALUES("
                           + CS(txtDescArt.Text) + ","
                           + txtPrecioDolar.Text.Replace(",",".") + ","
                           + txtCostoDolar.Text.Replace(",", ".") + ")";
            SqlCeCommand command = new SqlCeCommand(query, MainWindow.conn);
            command.ExecuteReader();

            //Generando existencia en inventario
            query = "SELECT TOP 1 id FROM c_articulos ORDER BY id DESC";
            command = new SqlCeCommand(query, MainWindow.conn);
            SqlCeDataReader dr = command.ExecuteReader();
            int idArt = 0;
            if(dr.Read())
            {
                idArt = int.Parse(dr["id"].ToString());
            }
            dr.Close();
            query = "INSERT INTO c_inventario (id_articulo,cantidad,fechaHora) " +
                    "VALUES("
                    + idArt + ","
                    + txtCantidadAgg.Text + ","
                    + CS(String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now)) + ")";
            command = new SqlCeCommand(query, MainWindow.conn);
            command.ExecuteReader();

            this.Close();
        }
        private void editarArticulo()
        {
            //Actualizando info de articulo seleccionado
            string idArt = lblIdArt.Content.ToString().Replace("#", "");
            string query = "UPDATE c_articulos SET " +
                           "precioDolar = " + decimal.Parse(txtPrecioDolar.Text).ToString().Replace(",",".") + "," +
                           "costoDolar = "  + decimal.Parse(txtCostoDolar.Text).ToString().Replace(",",".") + " " +
                           "WHERE id = "    + idArt;
            SqlCeCommand command = new SqlCeCommand(query, MainWindow.conn);
            command.ExecuteReader();

            //Agregando inventario, de ser necesario.
            if(txtCantidadAgg.Text != "0")
            {
                query = "INSERT INTO c_inventario (id_articulo,cantidad,fechaHora) " +
                        "VALUES("
                        + idArt + ","
                        + txtCantidadAgg.Text + ","
                        + CS(String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now)) + ")";
                command = new SqlCeCommand(query, MainWindow.conn);
                command.ExecuteReader();
            }
        }
      
        private ArticuloClase buscarArticulo()
        {
            ArticuloBuscar buscar = new ArticuloBuscar();
            buscar.Owner = this.Owner;
            buscar.ShowDialog();

            return buscar.idBuscado;
        }
        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            setArticuloBuscado();
        }
        private void setArticuloBuscado()
        {
            ArticuloClase articulo = buscarArticulo();
             
            try
            {
                lblIdArt.Content = "#" + articulo.id;
                txtDescArt.Text = articulo.descripcion;
                txtDescArt.IsEnabled = false;

                txtCostoDolar.Text = articulo.costoDolar;
                txtPrecioDolar.Text = articulo.precioDolar;
                txtCantidadAgg.Text = "0";
            }
            catch(Exception e)
            {
                return;
            }

            if(!lblIdArt.Content.ToString().Contains("#id"))
            {
                articuloBuscado = 1;
            }
        }
        
        private string CS(string strSinComillas)
        {
            //Añade comillas simples
            strSinComillas = strSinComillas.Replace("'", "''");
            return "'" + strSinComillas + "'";
        }
        private void soloNum(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                btnAceptar.Focus();
            }

            if ((e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key == Key.OemMinus) 
                || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void txtPrecioDolar_KeyDown(object sender, KeyEventArgs e)
        {
            //Oculta el cursor
            txtPrecioDolar.CaretBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

            if (e.Key == Key.Tab)
            {
                txtCostoDolar.Focus();
            }

            e.Handled = true;

            if ((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
            {
                string tecla = e.Key.ToString().Substring(e.Key.ToString().Length - 1, 1);
                decimal numAnterior = 0;
                if (txtPrecioDolar.Text != "")
                {
                    numAnterior = Decimal.Parse(txtPrecioDolar.Text) * 100;
                }
                string strNumNuevo = numAnterior.ToString().Replace(",00", "") + tecla;
                decimal numNuevo = decimal.Parse(strNumNuevo) / 100;

                txtPrecioDolar.Text = String.Format("{0:#,0.00}", numNuevo);
            }
        }

        private void txtCostoDolar_KeyDown(object sender, KeyEventArgs e)
        {
            //Oculta el cursor
            txtCostoDolar.CaretBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

            if (e.Key == Key.Tab)
            {
                txtCantidadAgg.Focus();
            }

            e.Handled = true;

            if ((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
            {
                string tecla = e.Key.ToString().Substring(e.Key.ToString().Length - 1, 1);
                decimal numAnterior = 0;
                if (txtCostoDolar.Text != "")
                {
                    numAnterior = Decimal.Parse(txtCostoDolar.Text) * 100;
                }
                string strNumNuevo = numAnterior.ToString().Replace(",00", "") + tecla;
                decimal numNuevo = decimal.Parse(strNumNuevo) / 100;

                txtCostoDolar.Text = String.Format("{0:#,0.00}", numNuevo);
            }
        }

        private void txtPrecioDolar_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                txtPrecioDolar.Text = "";
            }
        }
        private void txtCostoDolar_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                txtCostoDolar.Text = "";
            }
        }
    }
}
