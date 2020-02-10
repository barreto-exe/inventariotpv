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
    /// Lógica de interacción para Venta.xaml
    /// </summary>
    public partial class Venta : Window
    {
        public decimal totalVentaDolar;
        public decimal totalVentaBs;
        public decimal costoVenta;
        public decimal tasa;
        public decimal porcentaje;
        public Venta()
        {
            InitializeComponent();
            //this.txtIdArt.Focus();
            totalVentaDolar = 0;
            costoVenta = 0;

            tasa = cambioTasa.tasas()[0];
            porcentaje = cambioTasa.tasas()[1];

            actualizaDatos();
        }
        
        private void btnBorrar_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            ArticuloClase itemSelecto = button.DataContext as ArticuloClase;

            dataArticulosVenta.Items.Remove(itemSelecto);

            actualizaDatos();
        }
        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            ArticuloClase articulo = buscarArticulo();

            //Si no seleccionó artículo
            if (articulo == null)
                return;

            //Seleccionando cantidad
            VentaCantidad cantidad = new VentaCantidad();
            cantidad.Owner = this;
            cantidad.ShowDialog();
            articulo.cantAct = cantidad.txtCant.Text;

            //Seteando monto * cantidad
            decimal subtotalDolar = decimal.Parse(articulo.precioDolar) * decimal.Parse(articulo.cantAct);
            decimal costoDolar    = decimal.Parse(articulo.costoDolar)  * decimal.Parse(articulo.cantAct);
            decimal subtotalBs    = decimal.Parse(articulo.precioBs)    * decimal.Parse(articulo.cantAct);
            decimal subtotalBsEfect    = decimal.Parse(articulo.precioBsEfect) * decimal.Parse(articulo.cantAct);

            articulo.precioDolar = Decimal.Round(subtotalDolar, 2).ToString();
            articulo.costoDolar  = Decimal.Round(costoDolar, 2).ToString();
            articulo.precioBs    = Decimal.Round(subtotalBs, 2).ToString();
            articulo.precioBsEfect = Decimal.Round(subtotalBsEfect, 2).ToString();

            dataArticulosVenta.Items.Add(articulo);

            actualizaDatos();
        }

        /*
        private void txtIdArt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            //Este if es redundante con la función btnBuscar
            {
                string query = "SELECT * FROM c_articulos WHERE id = " + txtIdArt.Text;
                SqlCeCommand command = new SqlCeCommand(query, MainWindow.conn);
                SqlCeDataReader dr = command.ExecuteReader();
                txtIdArt.Text = "";

                //Si no seleccionó artículo
                if (!dr.Read())
                {
                    MessageBox.Show("No existe el artículo buscado");
                    dr.Close();
                    return;
                }

                var articulo = new ArticuloClase
                {
                    id = dr["id"].ToString(),
                    descripcion = dr["descripcion"].ToString(),
                    precioDolar = dr["precioDolar"].ToString(),
                    costoDolar = dr["costoDolar"].ToString()
                };

                //Seleccionando cantidad
                VentaCantidad cantidad = new VentaCantidad();
                cantidad.Owner = this;
                cantidad.ShowDialog();
                articulo.cantAct = cantidad.txtCant.Text;

                //Seteando monto * cantidad
                decimal subtotalDolar = decimal.Parse(articulo.precioDolar) * decimal.Parse(articulo.cantAct);
                decimal costoDolar = decimal.Parse(articulo.costoDolar) * decimal.Parse(articulo.cantAct);
                decimal subtotalBs = subtotalDolar * tasa;

                articulo.precioDolar = Decimal.Round(subtotalDolar, 2).ToString();
                articulo.costoDolar = Decimal.Round(costoDolar, 2).ToString();
                articulo.precioBs = Decimal.Round(subtotalBs, 2).ToString();
                if (checkEfectivo.IsChecked == true)
                {
                    articulo.precioBs = Decimal.Round(((subtotalBs * tasa * 100) / porcentaje), 2).ToString();
                }

                dataArticulosVenta.Items.Add(articulo);
                dr.Close();

                actualizaDatos();
            }

            if ((e.Key >= Key.D0 && e.Key <= Key.D9)
                || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
                e.Handled = false;
            else
                e.Handled = true;
        }
        */

        private void actualizaDatos()
        {
            int cantArt = dataArticulosVenta.Items.Count;
            totalVentaDolar = 0;
            totalVentaBs = 0;
            costoVenta = 0;
            ArticuloClase articulo;
            decimal subtotalDolar;
            decimal costoDolar;
            decimal subtotalBs;

            DataGrid aux = new DataGrid();

            for (int i = 0; i < cantArt; i++)
            {
                articulo = dataArticulosVenta.ItemContainerGenerator.Items[i] as ArticuloClase;

                string query;
                SqlCeCommand command;
                SqlCeDataReader dr;
                query = "SELECT * FROM c_articulos WHERE id = " + articulo.id;
                command = new SqlCeCommand(query, MainWindow.conn);
                dr = command.ExecuteReader();
                dr.Read();

                if(dr["precioBs"].ToString() != "")
                {
                    if (checkEfectivo.IsChecked == true)
                    {
                        articulo.precioBs = dr["precioBsEfect"].ToString();
                    }
                    else
                    {
                        articulo.precioBs = dr["precioBs"].ToString();
                    }
                }
                else
                {
                    if (checkEfectivo.IsChecked == true)
                    {
                        articulo.precioBs = (Decimal.Parse(dr["precioDolar"].ToString()) * tasa * 100 / porcentaje).ToString();
                    }
                    else
                    {
                        articulo.precioBs = (Decimal.Parse(dr["precioDolar"].ToString()) * tasa).ToString();
                    }
                }

                dr.Close();

                articulo.precioBs = (Decimal.Parse(articulo.precioBs) * int.Parse(articulo.cantAct)).ToString();

                subtotalDolar = decimal.Parse(articulo.precioDolar);
                costoDolar = decimal.Parse(articulo.costoDolar);
                subtotalBs = decimal.Parse(articulo.precioBs);

                totalVentaDolar += subtotalDolar;
                totalVentaBs    += subtotalBs;
                costoVenta      += costoDolar;

                articulo.precioDolar = Decimal.Round(subtotalDolar, 2).ToString("#,#0.##");
                articulo.costoDolar  = Decimal.Round(costoDolar, 2).ToString("#,#0.##");
                articulo.precioBs    = Decimal.Round(subtotalBs, 2).ToString("#,#0.##");

                aux.Items.Add(articulo);
            }
            dataArticulosVenta.Items.Clear();
            for(int i = 0; i < cantArt; i++)
            {
                articulo = aux.ItemContainerGenerator.Items[i] as ArticuloClase;
                dataArticulosVenta.Items.Add(articulo);
            }

            lblTotalDolar.Content = "$ "  + Decimal.Round(totalVentaDolar, 2).ToString("#,#0.##");
            lblTotalBs.Content = "Bs.S. " + Decimal.Round(totalVentaBs, 2).ToString("#,#0.##");
            
            //txtIdArt.Focus();
        }
       
        private void aceptarCompra(object sender, RoutedEventArgs e)
        {
            if (totalVentaDolar == 0)
                return;

            MessageBoxResult messageBoxResult = MessageBox.Show("¿Desea confirmar la operación?", "Confirmar", MessageBoxButton.YesNo);
            if (messageBoxResult != MessageBoxResult.Yes)
                return;

            int pagoEnEfectivo = 0;

            if(checkEfectivo.IsChecked == true)
            {
                pagoEnEfectivo = 1;
            }

            string query = "INSERT INTO c_ventas (fechaHora,pagoDolar,conversionBs,pagoBsEfect,costoVenta,tasaVenta,porcentajeEfectivoVenta) " +
                           "VALUES("
                           + CS(String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now)) + ","
                           + totalVentaDolar.ToString().Replace(",",".") + ","
                           + totalVentaBs.ToString().Replace(",",".") + ","
                           + pagoEnEfectivo.ToString() + ","
                           + costoVenta.ToString().Replace(",", ".") + ","
                           + tasa.ToString().Replace(",", ".") + ","
                           + porcentaje.ToString().Replace(",", ".") + ")";
            SqlCeCommand command = new SqlCeCommand(query, MainWindow.conn);
            command.ExecuteReader();

            query = "SELECT TOP 1 id FROM c_ventas ORDER BY id DESC";
            command = new SqlCeCommand(query, MainWindow.conn);
            SqlCeDataReader dr = command.ExecuteReader();
            dr.Read();
            int idVenta = int.Parse(dr.GetValue(0).ToString());
            dr.Close();

            int cantArt = dataArticulosVenta.Items.Count;
            for(int i = 0; i<cantArt; i++)
            {
                ArticuloClase articulo = dataArticulosVenta.ItemContainerGenerator.Items[i] as ArticuloClase;
                query = "INSERT INTO c_ventas_detalles (id_venta,id_articulo,cantidad,subtotal,subtotalBs) " +
                        "VALUES("
                        + idVenta.ToString() + ","
                        + articulo.id + ","
                        + articulo.cantAct + ","
                        + QC(articulo.precioDolar) + ","
                        + QC(articulo.precioBs)    + ")";
                command = new SqlCeCommand(query, MainWindow.conn);
                command.ExecuteReader();

                query = "INSERT INTO c_inventario (fechaHora,id_articulo,cantidad) " +
                        "VALUES("
                        + CS(String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now)) + ","
                        + articulo.id + ","
                        + "-" + articulo.cantAct + ")";
                command = new SqlCeCommand(query, MainWindow.conn);
                command.ExecuteReader();
            }

            this.Close();
        }
        private void cancelarCompra(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("¿Desea cancelar la operación?", "Cancelar", MessageBoxButton.YesNo);
            if (messageBoxResult != MessageBoxResult.Yes)
                return;

            this.Close();
        }
      
        private ArticuloClase buscarArticulo()
        {
            ArticuloBuscar buscar = new ArticuloBuscar();
            buscar.Owner = this.Owner;
            buscar.ShowDialog();

            return buscar.idBuscado;
        }
     
        private void checkEfectivo_Checked(object sender, RoutedEventArgs e)
        {
            actualizaDatos();
        }

        private string QC(string decimalConComa)
        {
            string decimalConPunto = decimalConComa.Replace(".","").Replace(",", ".");
            return decimalConPunto;
        }
        private string CS(string strSinComillas)
        {
            //Añade comillas simples
            strSinComillas = strSinComillas.Replace("'", "''");
            return "'" + strSinComillas + "'";
        }
    }
}
