using System;

namespace InventarioTPV
{
    public partial class MainWindow
    {
        private void BtnCambiarTasas_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //Muestro al usuario el control para cambiar tasa
            TasaModificar modificar = new TasaModificar();
            modificar.ShowDialog();

            //Si canceló la operación, retorno
            if (!modificar.Consultado)
                return;

            //Registro la nueva tasa
            Tasa tasa = new Tasa(modificar.Tasa, modificar.Porcentaje, DateTime.Now);
            if(!tasa.RegistrarTasa())
            {
                App.MensajeModal("Error al registrar la tasa.", this);
            }
            else
            {
                //Tasa Registrada con exito, así que la actualizo en la vista.
                ConsultarTasa();
            }
        }
    }
}
