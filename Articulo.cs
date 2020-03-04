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
                txtDescArt.Focus();
                setArticuloBuscado();
            }
        }
        private void btnAceptar_Click(object sender, RoutedEventArgs e)
        {
            decimal n = 0; 

            if( txtCantidadAgg.Text == "" || txtCostoDolar.Text == ""  ||
                txtDescArt.Text == ""     || txtPrecioDolar.Text == "" ||
                txtPrecioBs.Text == ""    || txtPrecioBsEfect.Text == "" ||
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
            string query = "INSERT INTO c_articulos (descripcion,codBarras,precioDolar,costoDolar,precioBs,precioBsEfect) " +
                           "VALUES("
                           + CS(txtDescArt.Text) + ","
                           + CS(txtCodBarras.Text) + ","
                           + txtPrecioDolar.Text.Replace(",",".") + ","
                           + txtCostoDolar.Text.Replace(",", ".") + ","
                           + QC(txtPrecioBs.Text) + ","
                           + QC(txtPrecioBsEfect.Text) + ")";
            SqlCeCommand command = new SqlCeCommand(query, MainWindow.conn);
            command.ExecuteNonQuery();

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
            command.ExecuteNonQuery();

            this.Close();
        }
        private void editarArticulo()
        {
            //Actualizando info de articulo seleccionado
            string idArt = lblIdArt.Content.ToString().Replace("#", "");
            string query = "UPDATE c_articulos SET " +
                           "descripcion = "   + CS(txtDescArt.Text)   + "," +
                           "codBarras = "     + CS(txtCodBarras.Text) + "," +
                           "precioDolar = "   + decimal.Parse(txtPrecioDolar.Text).ToString().Replace(",",".")    + "," +
                           "costoDolar = "    + decimal.Parse(txtCostoDolar.Text).ToString().Replace(",",".")     + "," +
                           "precioBs = "      + decimal.Parse(txtPrecioBs.Text).ToString().Replace(",", ".")      + "," +
                           "precioBsEfect = " + decimal.Parse(txtPrecioBsEfect.Text).ToString().Replace(",", ".") + " " +
                           "WHERE id = " + idArt;
            SqlCeCommand command = new SqlCeCommand(query, MainWindow.conn);
            command.ExecuteNonQuery();

            //Agregando inventario, de ser necesario.
            if(txtCantidadAgg.Text != "0")
            {
                query = "INSERT INTO c_inventario (id_articulo,cantidad,fechaHora) " +
                        "VALUES("
                        + idArt + ","
                        + txtCantidadAgg.Text + ","
                        + CS(String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now)) + ")";
                command = new SqlCeCommand(query, MainWindow.conn);
                command.ExecuteNonQuery();
            }
        }
      
        private ArticuloClase buscarArticulo()
        {
            ArticuloBuscar buscar = new ArticuloBuscar();
            buscar.Owner = this.Owner;
            buscar.ShowDialog();

            return buscar.buscado;
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
                //txtDescArt.IsEnabled = false;

                txtCostoDolar.Text    = articulo.costoDolar;
                txtPrecioDolar.Text   = articulo.precioDolar;
                txtPrecioBs.Text      = articulo.precioBs;
                txtPrecioBsEfect.Text = articulo.precioBsEfect;
                txtCodBarras.Text     = articulo.codBarras;

                decimal precio = Decimal.Parse(txtPrecioDolar.Text);
                decimal costo = Decimal.Parse(txtCostoDolar.Text);
                decimal ganancia = precio / costo * 100 - 100;

                if (ganancia > 0)
                {
                    txtGananciaDolar.Text = ganancia.ToString("#,#0.##");
                }
                else
                {
                    txtGananciaDolar.Text = "";
                }

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
                txtCodBarras.Focus();
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
                txtGananciaDolar.Focus();
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

                decimal precioBs = numNuevo * cambioTasa.tasas()[0];
                decimal precioBsEfect = (precioBs * 100) / cambioTasa.tasas()[1];

                txtPrecioBs.Text      = String.Format("{0:#,0.00}", precioBs);
                txtPrecioBsEfect.Text = String.Format("{0:#,0.00}", precioBsEfect);
                decimal precio = Decimal.Parse(txtPrecioDolar.Text);
                decimal costo  = Decimal.Parse(txtCostoDolar.Text);
                decimal ganancia = precio / costo * 100 - 100;

                if(ganancia > 0)
                {
                    txtGananciaDolar.Text = ganancia.ToString("#,#0.##");
                }
                else
                {
                    txtGananciaDolar.Text = "";
                }
            }
        }
        private void txtCostoDolar_KeyDown(object sender, KeyEventArgs e)
        {
            //Oculta el cursor
            txtCostoDolar.CaretBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

            if (e.Key == Key.Tab)
            {
                txtPrecioDolar.Focus();
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
        private void txtPrecioDolar_TextChanged(object sender, TextChangedEventArgs e)
        {
            decimal precioD;
            decimal precioBs;
            decimal precioBsEfect;

            if(Decimal.TryParse(txtPrecioDolar.Text, out precioD))
            {
                precioBs = precioD * cambioTasa.tasas()[0];
                precioBsEfect = (precioBs * 100) / cambioTasa.tasas()[1];

                txtPrecioBs.Text = String.Format("{0:#,0.00}", precioBs);
                txtPrecioBsEfect.Text = String.Format("{0:#,0.00}", precioBsEfect);
            }
            else
            {
                txtPrecioDolar.Text = "";
            }
        }

        private void txtGananciaDolar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                txtPrecioBs.Focus();
            }

            if ((e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key == Key.OemMinus)
                || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
                e.Handled = false;
            else
                e.Handled = true;

            decimal porcentaje;
            decimal costo;
            
            if(Decimal.TryParse(txtCostoDolar.Text, out costo) && 
               Decimal.TryParse(txtGananciaDolar.Text + e.Key.ToString().Replace("D", ""), out porcentaje))
            {
                decimal precio = costo + costo * porcentaje / 100;
                txtPrecioDolar.Text = precio.ToString("#,#0.##");
            }
        }
        private void txtGananciaDolar_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                txtGananciaDolar.Text = "";
            }
        }

        private void txtPrecioDolar_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                txtPrecioDolar.Text   = "";
                txtGananciaDolar.Text = "";
            }
        }
        private void txtCostoDolar_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                txtCostoDolar.Text = "";
            }
        }

        private void txtPrecioBs_KeyDown(object sender, KeyEventArgs e)
        {
            //Oculta el cursor
            txtPrecioBs.CaretBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

            if (e.Key == Key.Tab)
            {
                txtPrecioBsEfect.Focus();
            }

            e.Handled = true;

            if ((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
            {
                string tecla = e.Key.ToString().Substring(e.Key.ToString().Length - 1, 1);
                decimal numAnterior = 0;
                if (txtPrecioBs.Text != "")
                {
                    numAnterior = Decimal.Parse(txtPrecioBs.Text) * 100;
                }
                string strNumNuevo = numAnterior.ToString().Replace(",00", "") + tecla;
                decimal numNuevo = decimal.Parse(strNumNuevo) / 100;

                txtPrecioBs.Text = String.Format("{0:#,0.00}", numNuevo);
            }
        }
        private void txtPrecioBs_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                txtPrecioBs.Text = "";
            }
        }

        private void txtPrecioBsEfect_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                txtPrecioBsEfect.Text = "";
            }
        }
        private void txtPrecioBsEfect_KeyDown(object sender, KeyEventArgs e)
        {
            //Oculta el cursor
            txtPrecioBsEfect.CaretBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

            if (e.Key == Key.Tab)
            {
                txtCantidadAgg.Focus();
            }

            e.Handled = true;

            if ((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
            {
                string tecla = e.Key.ToString().Substring(e.Key.ToString().Length - 1, 1);
                decimal numAnterior = 0;
                if (txtPrecioBsEfect.Text != "")
                {
                    numAnterior = Decimal.Parse(txtPrecioBsEfect.Text) * 100;
                }
                string strNumNuevo = numAnterior.ToString().Replace(",00", "") + tecla;
                decimal numNuevo = decimal.Parse(strNumNuevo) / 100;

                txtPrecioBsEfect.Text = String.Format("{0:#,0.00}", numNuevo);
            }
        }

        private string QC(string decimalConComa)
        {
            string decimalConPunto = decimalConComa.Replace(".","").Replace(",", ".");
            return decimalConPunto;
        }


    }
}
