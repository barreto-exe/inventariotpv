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
            this.Height = this.MinHeight;
            this.Title = titulo;
            this.txtMensaje.Text = mensaje;
            this.Height = double.NaN;
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
    }
}
