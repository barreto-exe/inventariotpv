namespace InventarioTPV
{
    /// <summary>
    /// Lógica de interacción para Inventario.xaml
    /// </summary>
    public partial class Inventario
    {
        public Inventario()
        {
            InitializeComponent();
        }

        private void BtnEditar_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ArticuloBuscar buscar = new ArticuloBuscar();
            buscar.Owner = this;
            buscar.ShowDialog();
        }
    }
}
