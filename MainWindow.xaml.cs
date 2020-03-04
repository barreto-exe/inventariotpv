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
using System.Data.SqlServerCe;
using System.Windows.Threading;

namespace Inventario_y_Contabilidad
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static SqlCeConnection conn;
        public void ConectarBD()
        {
            string dataSource = Environment.CurrentDirectory + "\\bd.sdf";
            conn = new SqlCeConnection("Data Source =" + dataSource + "; Password = contabilidad");
        }

        public MainWindow()
        {
            if (!File.Exists("bd.sdf"))
            {
                MessageBox.Show("No existe bd.sdf", "Error");
                Environment.Exit(0);
            }

            ConectarBD();
            conn.Open();

            InitializeComponent();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();

            txtFechaDesde.SelectedDate = DateTime.Now;
            txtFechaHasta.SelectedDate = DateTime.Now;

            actualizaVentas(DateTime.Now, DateTime.Now);
        }
        void timer_Tick(object sender, EventArgs e)
        {
            this.lblFechaHora.Content = String.Format("{0:dddd, dd/MM/yyyy - hh:mm:ss tt}", DateTime.Now);

            //Consultano Tasa Actual
            SqlCeCommand command = new SqlCeCommand("SELECT TOP 1 * FROM c_tasa ORDER BY id DESC", conn);
            SqlCeDataReader dr = command.ExecuteReader();
            dr.Read();
            lblTasaDia.Content = "Bs.S. " + decimal.Parse(dr["tasaDolar"].ToString()).ToString("#,#0.##");
            lblPorcentaje.Content = dr["porcentajeEfectivo"].ToString()+"%";
            dr.Close();
        }

        private void actualizaVentas(DateTime desde, DateTime hasta)
        {
            dataReportePrin.Items.Clear();

            string query = "SELECT * FROM c_ventas WHERE fechaHora BETWEEN '" +
                           String.Format("{0:yyyy-MM-dd}", desde) + "'  AND '" +
                           String.Format("{0:yyyy-MM-dd}", hasta) + " 23:59:59' ORDER BY id DESC; ";
            SqlCeCommand command = new SqlCeCommand(query, conn);
            SqlCeDataReader dr = command.ExecuteReader();

            while (dr.Read())
            {
                var venta = new VentaClase
                {
                    id = dr["id"].ToString(),
                    fechaHora = dr["fechaHora"].ToString(),
                    pagoDolar = RoundDecimalString(dr["pagoDolar"].ToString()),
                    tasaVenta = RoundDecimalString(dr["tasaVenta"].ToString()),
                    pagoBsEfect = RoundDecimalString(dr["pagoBsEfect"].ToString()),
                    conversionBs = RoundDecimalString(dr["pagoBsPunto"].ToString()),
                    porcentajeEfectivoVenta = RoundDecimalString(dr["porcentajeEfectivoVenta"].ToString()),
                    detalle = detalleVenta(dr["id"].ToString())
                };

                dataReportePrin.Items.Add(venta);
            }

            dr.Close();

            //Seteando Ingresos y Ganancias
            query = "SELECT sum(costoVenta), sum(pagoDolar) FROM c_ventas WHERE fechaHora BETWEEN '" +
                     String.Format("{0:yyyy-MM-dd}", desde) + "'  AND '" +
                     String.Format("{0:yyyy-MM-dd}", hasta) + " 23:59:59'";
            command = new SqlCeCommand(query, conn);
            dr = command.ExecuteReader();

            decimal costoVentas=0, ingresos=0, ganancias=0;

            if(dr.Read() && dr.GetValue(0).ToString() != "")
            {
                costoVentas = decimal.Parse(dr.GetValue(0).ToString());
                ingresos = decimal.Parse(dr.GetValue(1).ToString());
                ganancias = ingresos - costoVentas;

                dr.Close();
            }

            //decimal tasa = decimal.Parse(lblTasaDia.Content.ToString().Replace("Bs.S. ", ""));

            //Ingresos Bs Punto
            query = "SELECT sum(pagoBsPunto) FROM c_ventas WHERE fechaHora BETWEEN '" +
                     String.Format("{0:yyyy-MM-dd}", desde) + "'  AND '" +
                     String.Format("{0:yyyy-MM-dd}", hasta) + " 23:59:59'";
            command = new SqlCeCommand(query, conn);
            dr = command.ExecuteReader();
            decimal ingresoBsPunto = 0;

            if(dr.Read() && dr.GetValue(0).ToString() != "")
            {
                ingresoBsPunto = decimal.Parse(dr.GetValue(0).ToString());
                dr.Close();
            }

            //Ingresos Bs Efectivo
            query = "SELECT sum(pagoBsEfect) FROM c_ventas WHERE fechaHora BETWEEN '" +
                     String.Format("{0:yyyy-MM-dd}", desde) + "'  AND '" +
                     String.Format("{0:yyyy-MM-dd}", hasta) + " 23:59:59'";
            command = new SqlCeCommand(query, conn);
            dr = command.ExecuteReader();
            decimal ingresoBsEfect = 0;

            if (dr.Read() && dr.GetValue(0).ToString() != "")
            {
                ingresoBsEfect = decimal.Parse(dr.GetValue(0).ToString());
                dr.Close();
            }

            //Seteando Etiquetas
            if(desde.Date == DateTime.Now.Date && hasta.Date == DateTime.Now.Date)
            {
                lblMontosRango.Text = "Montos\n"+ "de hoy " + String.Format("{0:dddd, dd-MM-yyyy}", DateTime.Now) + ":";
            }
            else
            {
                lblMontosRango.Text = "Montos:\n"+ "Desde " + String.Format("{0:dd-MM-yyyy}", desde) + ", hasta " + String.Format("{0:dd-MM-yyyy}", hasta) +":";
            }
            lblIngresoDia.Content  = "$ " + Decimal.Round(ingresos, 2).ToString("#,#0.##");
            lblGananciaDia.Content = "$ " + Decimal.Round(ganancias, 2).ToString("#,#0.##");
            lblBsEfectivo.Content  = "Bs.S. " + Decimal.Round(ingresoBsEfect, 2).ToString("#,#0.##");
            lblBsPunto.Content     = "Bs.S. " + Decimal.Round(ingresoBsPunto, 2).ToString("#,#0.##");
        }

        private string detalleVenta(string id)
        {
            string detalle = "";

            string query = "SELECT * FROM c_ventas_detalles WHERE id_venta = " + id;
            SqlCeCommand cm = new SqlCeCommand(query, MainWindow.conn);
            SqlCeDataReader dr = cm.ExecuteReader();

            while(dr.Read())
            {
                query = "SELECT * FROM c_articulos WHERE id = " + dr["id_articulo"].ToString();
                SqlCeCommand cm2 = new SqlCeCommand(query, MainWindow.conn);
                SqlCeDataReader dr2 = cm2.ExecuteReader();
                dr2.Read();

                detalle += dr["cantidad"].ToString() + " " + dr2["descripcion"].ToString() + ". ";
            }

            dr.Close();

            return detalle;
        }

        private void btnCambiarTasa_Click(object sender, RoutedEventArgs e)
        {
            cambioTasa cambio = new cambioTasa();
            cambio.Owner = this;
            cambio.ShowDialog();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            conn.Close();
        }
    
        private string RoundDecimalString(string num)
        {
            decimal dec = Decimal.Parse(num);
            return decimal.Round(dec, 2).ToString("#,#0.##");
        }

        private void btnVender_Click(object sender, RoutedEventArgs e)
        {
            Venta venta = new Venta();
            venta.Owner = this;
            venta.ShowDialog();

            actualizaVentas(DateTime.Now,DateTime.Now);
        }

        private void btnInventario_Click(object sender, RoutedEventArgs e)
        {
            Inventario inventario = new Inventario();
            inventario.Owner = this;
            inventario.ShowDialog();
        }

        private void btnConsultarVentas_Click(object sender, RoutedEventArgs e)
        {
            if(txtFechaDesde.SelectedDate != null && txtFechaHasta.SelectedDate != null)
                actualizaVentas((DateTime)txtFechaDesde.SelectedDate, (DateTime)txtFechaHasta.SelectedDate);
        }

        private void btnBorrar_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            VentaClase itemDelete = button.DataContext as VentaClase;

            MessageBoxResult messageBoxResult = MessageBox.Show("¿Desea confirmar la operación?", "Confirmar", MessageBoxButton.YesNo);
            if (messageBoxResult != MessageBoxResult.Yes)
                return;

            string query = "DELETE FROM c_ventas WHERE id = " + CS(itemDelete.id);
            SqlCeCommand cm = new SqlCeCommand(query, MainWindow.conn);
            cm.ExecuteNonQuery();

            query = "SELECT * FROM c_ventas_detalles WHERE id_venta = " + CS(itemDelete.id);
            cm = new SqlCeCommand(query, MainWindow.conn);
            SqlCeDataReader dr = cm.ExecuteReader();

            while (dr.Read())
            {
                query = "INSERT INTO c_inventario (id_articulo,cantidad,fechaHora) " +
                "VALUES("
                + dr["id_articulo"].ToString() + ","
                + dr["cantidad"].ToString()    + ","
                + CS(String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now)) + ")";
                SqlCeCommand cm2 = new SqlCeCommand(query, MainWindow.conn);
                cm2.ExecuteNonQuery();

                query = "DELETE FROM c_ventas_detalles WHERE id_venta = " + CS(itemDelete.id);
                cm2 = new SqlCeCommand(query, MainWindow.conn);
                cm2.ExecuteNonQuery();
            }
            actualizaVentas((DateTime)txtFechaDesde.SelectedDate, (DateTime)txtFechaHasta.SelectedDate);
        }
        private string CS(string strSinComillas)
        {
            //Añade comillas simples
            strSinComillas = strSinComillas.Replace("'", "''");
            return "'" + strSinComillas + "'";
        }

    }
}
