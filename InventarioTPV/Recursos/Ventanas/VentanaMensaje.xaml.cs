namespace InventarioTPV
{
    /// <summary>
    /// Lógica de interacción para VentanaMensaje.xaml
    /// </summary>
    public partial class VentanaMensaje 
    {
        public bool Aceptado { get; private set; } = false;

        public VentanaMensaje(string mensaje, string titulo = "Mensaje")
        {
            InitializeComponent();
            this.Title = titulo;
            this.txtMensaje.Text = mensaje;
        }

        private void BtnAceptar_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Aceptado = true;
            this.Close();
        }

        private void BtnCancelar_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Actualizar la altura de la ventana.
        /// </summary>
        private void ActualizaAltura(object sender, System.Windows.RoutedEventArgs e)
        {
            //60 por los márgenes entre los controles 
            this.Height = txtMensaje.ActualHeight + grdBotones.ActualHeight + 80;
            //Corro la ventana un poco hacia abajo
            this.Top += 150;
        }
    }
}
