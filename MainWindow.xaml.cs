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
        }

        void timer_Tick(object sender, EventArgs e)
        {
            this.lblFechaHora.Content = String.Format("{0:dddd, dd/MM/yyyy - hh:m:ss tt}", DateTime.Now);

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
    }
}
