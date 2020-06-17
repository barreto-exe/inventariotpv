using HandyControl.Controls;
using System;

namespace InventarioTPV
{
    /// <summary>
    /// Lógica de interacción para TasaModificar.xaml
    /// </summary>
    public partial class TasaModificar
    {
        public decimal CampoTasa
        {
            get
            {
                return Convert.ToDecimal(txtTasa.Value);
            }
        }
        public decimal CampoPorcentaje
        {
            get
            {
                return Convert.ToDecimal(txtPorcentaje.Value);
            }
        }
        public bool Aceptado { get; set; } = false;

        /// <summary>
        /// Si el usuario presionó aceptar, consultado es true.
        /// </summary>
        public bool Valido
        {
            get
            {
                //Si no presionó aceptar o ambos están en 0, 
                //entonces se retorna falso para no actualizar nada
                return (txtPorcentaje.Value != 0 || txtTasa.Value != 0) && Aceptado;
            }
        }

        public TasaModificar()
        {
            InitializeComponent();

            //Consultar la tasa actual
            Tasa tasa = Tasa.ConsultarTasa();

            //Colocar la tasa actual en los controles 
            this.txtTasa.Value = Convert.ToDouble(tasa.ValorDolar);
            this.txtPorcentaje.Value = Convert.ToDouble(tasa.PorcentajeEfect);

            //Enfocar campo de texto
            this.txtTasa.Focus();
        }

        private void BtnAceptar_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if(txtPorcentaje.Value == 0 && txtTasa.Value == 0)
            {
                return;
            }
            Aceptado = true;
            this.Close();
        }
    }
}
