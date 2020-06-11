using System.Windows;

namespace InventarioTPV
{
    public partial class App
    {
        /// <summary>
        /// Muestra un mensaje en ventana modal. Botón de cancelar y aceptar.
        /// </summary>
        /// <param name="mensaje">Mensaje a mostrar.</param>
        /// <returns>Retorna true si se presiona el botón aceptar.</returns>
        public static bool MensajeModal(string mensaje, Window owner)
        {
            VentanaMensaje vmensaje = new VentanaMensaje(mensaje);
            vmensaje.Owner = owner;
            vmensaje.ShowDialog();

            //Si se presionó aceptar en la ventana, retorno true;
            if(vmensaje.Aceptado)
            {
                return true;
            }

            return false;
        }
    }
}
