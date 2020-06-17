using System.Windows.Threading;
using System;
using System.ComponentModel;

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
            if (!modificar.Valido)
                return;

            //Registro la nueva tasa
            Tasa tasa = new Tasa(modificar.CampoTasa, modificar.CampoPorcentaje, DateTime.Now);
            if(!tasa.RegistrarTasa())
            {
                App.MensajeModal("Error al registrar la tasa.", this);
            }
            else
            {
                //Tasa Registrada con exito, así que la actualizo en la vista.
                ConsultarTasa();

                //Instanciar worker para actualizar Precios en BBDD.
                BackgroundWorker actualizadorBD = new BackgroundWorker();
                actualizadorBD.DoWork += ActualizadorBD_DoWork;
                actualizadorBD.RunWorkerCompleted += ActualizadorBD_RunWorkerCompleted;
                actualizadorBD.RunWorkerAsync();
            }
        }

        #region Actualizador Worker
        private void ActualizadorBD_DoWork(object sender, DoWorkEventArgs e)
        {
            Articulo.ActualizarPreciosBBDD();
        }
        private void ActualizadorBD_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }
        #endregion


        private void BtnInventario_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Inventario inventario = new Inventario();
            inventario.Owner = this;
            inventario.ShowDialog();
        }
    }
}
