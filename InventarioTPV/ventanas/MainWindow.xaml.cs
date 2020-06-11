using HandyControl.Tools;
using System;
using System.Data.SQLite;
using System.Windows.Threading;

namespace InventarioTPV
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            CargarSkin();
            SelectRegion("es-VE");
            ConfigHelper.Instance.SetLang("en");
            ConsultarTasa();


            expIngresoPorMoneda.IsExpanded = false;
            expConsultarVentas.IsExpanded = false;

            CodigoTest();

            //Dispatcher para actualizar la hora
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Actualiza_HoraAndGrid;
            timer.Start();
        }

        private void Actualiza_HoraAndGrid(object sender, EventArgs e)
        {
            string fechahora = String.Format("{0:dddd, dd/MM/yyyy - hh:mm:ss tt}", DateTime.Now);

            //Colocar la primera letra en mayúscula
            this.txtFechaHora.Text = fechahora.Insert(1, fechahora[0].ToString().ToUpper()).Substring(1);

            //Para acomodar la altura del grid
            ActualizarAlturaGridVentas();
        }

        private void ConsultarTasa()
        {
            Tasa tasa = Tasa.ConsultarTasa();

            txtTasaDolar.Text = "Bs.S. " + tasa.ValorDolar.ToString("#,0.00");
            txtTasaEfectivo.Text = Convert.ToInt16(tasa.PorcentajeEfect).ToString() + "% de ganancia";
            txtFechaCambioTasa.Text = tasa.Fecha;
            txtHoraCambioTasa.Text  = tasa.Hora;
        }

        public void CodigoTest()
        {
            BDCon con = new BDCon("SELECT @Efectivo as tipopago, @Monto as monto FROM c_tasa");
            con.PasarParametros("Efectivo", "Aaacaa");
            con.PasarParametros("Monto", "Bbbbbcb");

            con.ConsultaSqlite(dgVentas);
            con.ConsultaSqlite(listIngresosMonedas);

        }


    }
}
