using HandyControl.Controls;
using System;

namespace InventarioTPV
{
    /// <summary>
    /// Lógica de interacción para TasaModificar.xaml
    /// </summary>
    public partial class TasaModificar
    {
        public decimal Tasa
        {
            get
            {
                return Convert.ToDecimal(txtTasa.Value);
            }
        }
        public decimal Porcentaje
        {
            get
            {
                return Convert.ToDecimal(txtPorcentaje.Value);
            }
        }
        
        /// <summary>
        /// Si el usuario presionó aceptar, consultado es true.
        /// </summary>
        public bool Consultado
        {
            get
            {
                //Si no presionó aceptar, entonces ambos están en 0
                return !(txtPorcentaje.Value == 0 && txtTasa.Value == 0);
            }
        }

        public TasaModificar()
        {
            InitializeComponent();
        }

        private void BtnAceptar_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if(txtPorcentaje.Value == 0 && txtTasa.Value == 0)
            {
                return;
            }
            this.Close();
        }
    }
}
