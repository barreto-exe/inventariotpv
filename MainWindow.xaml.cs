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
            if(!File.Exists("bd.sdf"))
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

            actualizaVentas();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            this.lblFechaHora.Content = String.Format("{0:dddd, dd/MM/yyyy - hh:mm:ss tt}", DateTime.Now);

            //Consultano Tasa Actual
            SqlCeCommand command = new SqlCeCommand("SELECT TOP 1 * FROM c_tasa ORDER BY id DESC", conn);
            SqlCeDataReader dr = command.ExecuteReader();
            dr.Read();
            lblTasaDia.Content = "Bs.S. " + String.Format("{0:#,#.00}", dr["tasaDolar"].ToString());
            lblPorcentaje.Content = dr["porcentajeEfectivo"].ToString()+"%";
            dr.Close();
        }

        private void btnDetalle_Click(object sender, RoutedEventArgs e)
        {

        }

        private void actualizaVentas()
        {
            dataReportePrin.Items.Clear();

            string query = "SELECT * FROM c_ventas WHERE fechaHora BETWEEN '" +
                           String.Format("{0:yyyy-MM-dd}", DateTime.Now) + "'  AND '" +
                           String.Format("{0:yyyy-MM-dd}", DateTime.Now) + " 23:59:59' ORDER BY id DESC; ";
            SqlCeCommand command = new SqlCeCommand(query, conn);
            SqlCeDataReader dr = command.ExecuteReader();

            while (dr.Read())
            {
                var venta = new VentaClase
                {
                    id = dr["id"].ToString(),
                    fechaHora = dr["fechaHora"].ToString(),
                    pagoDolar    = RoundDecimalString(dr["pagoDolar"].ToString()),
                    conversionBs = RoundDecimalString(dr["conversionBs"].ToString()),
                    tasaVenta    = RoundDecimalString(dr["tasaVenta"].ToString()),
                    porcentajeEfectivoVenta = RoundDecimalString(dr["porcentajeEfectivoVenta"].ToString())
                };
                
                if (dr["pagoBsEfect"].ToString() == "1")
                {
                    venta.pagoBsEfect = "Sí";
                }
                else
                {
                    venta.pagoBsEfect = "No";
                }

                dataReportePrin.Items.Add(venta);
            }

            dr.Close();

            //Seteando Ingresos y Ganancias
            query = "SELECT sum(costoVenta), sum(pagoDolar) FROM c_ventas WHERE fechaHora BETWEEN '" +
                     String.Format("{0:yyyy-MM-dd}", DateTime.Now) + "'  AND '" +
                     String.Format("{0:yyyy-MM-dd}", DateTime.Now) + " 23:59:59'";
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

            decimal tasa = decimal.Parse(lblTasaDia.Content.ToString().Replace("Bs.S. ", ""));

            //Ingresos Bs Punto
            query = "SELECT sum(conversionBs) FROM c_ventas WHERE pagoBsEfect = 0 AND fechaHora BETWEEN '" +
                     String.Format("{0:yyyy-MM-dd}", DateTime.Now) + "'  AND '" +
                     String.Format("{0:yyyy-MM-dd}", DateTime.Now) + " 23:59:59'";
            command = new SqlCeCommand(query, conn);
            dr = command.ExecuteReader();
            decimal ingresoBsPunto = 0;

            if(dr.Read() && dr.GetValue(0).ToString() != "")
            {
                ingresoBsPunto = decimal.Parse(dr.GetValue(0).ToString());
                dr.Close();
            }

            //Ingresos Bs Efectivo
            query = "SELECT sum(conversionBs) FROM c_ventas WHERE pagoBsEfect = 1 AND fechaHora BETWEEN '" +
                     String.Format("{0:yyyy-MM-dd}", DateTime.Now) + "'  AND '" +
                     String.Format("{0:yyyy-MM-dd}", DateTime.Now) + " 23:59:59'";
            command = new SqlCeCommand(query, conn);
            dr = command.ExecuteReader();
            decimal ingresoBsEfect = 0;

            if (dr.Read() && dr.GetValue(0).ToString() != "")
            {
                ingresoBsEfect = decimal.Parse(dr.GetValue(0).ToString());
                dr.Close();
            }

            //Seteando Etiquetas
            lblIngresoDia.Content  = "$ " + Decimal.Round(ingresos, 2).ToString();
            lblGananciaDia.Content = "$ " + Decimal.Round(ganancias, 2).ToString();
            lblBsEfectivo.Content  = "Bs.S. " + Decimal.Round(ingresoBsEfect, 2).ToString();
            lblBsPunto.Content     = "Bs.S. " + Decimal.Round(ingresoBsPunto, 2).ToString();
        }

        private void btnCambiarTasa_Click(object sender, RoutedEventArgs e)
        {
            cambioTasa cambio = new cambioTasa();
            cambio.Owner = this;
            cambio.ShowDialog();
        }

        private void imgVender_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Venta venta = new Venta();
            venta.Owner = this;
            venta.ShowDialog();

            actualizaVentas();
        }

        private void imgCargar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Inventario inventario = new Inventario();
            inventario.Owner = this;
            inventario.ShowDialog();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            conn.Close();
        }
    
        private string RoundDecimalString(string num)
        {
            decimal dec = Decimal.Parse(num);
            return decimal.Round(dec, 2).ToString();
        }
    }
}
