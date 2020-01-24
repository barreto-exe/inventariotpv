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
        public decimal costoVenta;
        public decimal tasa;
        public decimal porcentaje;
        public Venta()
        {
            InitializeComponent();
            this.txtIdArt.Focus();
            totalVentaDolar = 0;
            costoVenta = 0;

            SqlCeCommand command = new SqlCeCommand("SELECT TOP 1 * FROM c_tasa ORDER BY id DESC", MainWindow.conn);
            SqlCeDataReader dr = command.ExecuteReader();
            dr.Read();

            tasa = decimal.Parse(dr["tasaDolar"].ToString());
            porcentaje = decimal.Parse(dr["porcentajeEfectivo"].ToString())+100;

            dr.Close();

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
            decimal subtotalBs = subtotalDolar * tasa;

            articulo.precioDolar = subtotalDolar.ToString();
            articulo.costoDolar  = costoDolar.ToString();
            articulo.precioBs    = subtotalBs.ToString();
            if (checkEfectivo.IsChecked == true)
            {
                articulo.precioBs = ((subtotalBs * tasa * 100) / porcentaje).ToString();
            }

            dataArticulosVenta.Items.Add(articulo);

            actualizaDatos();
        }
       
        private void actualizaDatos()
        {
            int cantArt = dataArticulosVenta.Items.Count;
            totalVentaDolar = 0;
            costoVenta = 0;
            ArticuloClase articulo;
            decimal subtotalDolar;
            decimal costoDolar;
            decimal subtotalBs;

            DataGrid aux = new DataGrid();

            for (int i = 0; i < cantArt; i++)
            {
                articulo = dataArticulosVenta.ItemContainerGenerator.Items[i] as ArticuloClase;

                subtotalDolar = decimal.Parse(articulo.precioDolar);
                costoDolar =    decimal.Parse(articulo.costoDolar);
                subtotalBs = subtotalDolar * tasa;
                totalVentaDolar += subtotalDolar;
                costoVenta      += costoDolar;

                articulo.precioDolar = subtotalDolar.ToString();
                articulo.costoDolar  = costoDolar.ToString();
                articulo.precioBs = subtotalBs.ToString();
                if (checkEfectivo.IsChecked == true)
                {
                    articulo.precioBs = ((subtotalBs * 100) / porcentaje).ToString();
                }

                aux.Items.Add(articulo);
            }
            dataArticulosVenta.Items.Clear();
            for(int i = 0; i < cantArt; i++)
            {
                articulo = aux.ItemContainerGenerator.Items[i] as ArticuloClase;
                dataArticulosVenta.Items.Add(articulo);
            }

            lblTotalDolar.Content = "$ " + totalVentaDolar.ToString();
            lblTotalBs.Content = "Bs.S. " + (totalVentaDolar * tasa).ToString();
            
            if (checkEfectivo.IsChecked == true)
            {
                lblTotalBs.Content = "Bs.S. " + ((totalVentaDolar * tasa * 100) / porcentaje).ToString();
            }
        }
       
        private void aceptarCompra(object sender, RoutedEventArgs e)
        {
            if (totalVentaDolar == 0)
                return;

            MessageBoxResult messageBoxResult = MessageBox.Show("¿Desea confirmar la operación?", "Confirmar", MessageBoxButton.YesNo);
            if (messageBoxResult != MessageBoxResult.Yes)
                return;

            decimal totalVentaBs;
            int pagoEnEfectivo = 0;

            if(checkEfectivo.IsChecked == true)
            {
                totalVentaBs = (totalVentaDolar * tasa * 100) / porcentaje;
                pagoEnEfectivo = 1;
            }
            else
            {
                totalVentaBs = totalVentaDolar * tasa;
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
                        + articulo.precioDolar.Replace(",", ".") + ","
                        + articulo.precioBs.Replace(",", ".") + ")";
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
      
        private string CS(string strSinComillas)
        {
            //Añade comillas simples
            strSinComillas = strSinComillas.Replace("'", "''");
            return "'" + strSinComillas + "'";
        }

    }
}
