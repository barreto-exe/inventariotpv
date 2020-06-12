using System;
using System.Collections.Generic;
using System.Text;
namespace InventarioTPV
{
    /// <summary>
    /// Lógica de interacción para ArticuloEditar.xaml
    /// </summary>
    public partial class ArticuloEditar
    {
        public enum Operacion
        {
            Agregar,
            Editar
        }

        /// <summary>
        /// Ventana para agregar o editar un artículo.
        /// </summary>
        /// <param name="tipo">Enum para identificar si es operación de agregar o editar.</param>
        public ArticuloEditar(Operacion tipo)
        {
            InitializeComponent();

            //Verificando el tipo de operación a realizar en la vista
            if(tipo == Operacion.Agregar)
            {
                //Si es para crear artículo, se colapsa el botón buscar.
                this.btnBuscar.Visibility = System.Windows.Visibility.Collapsed;
            }
            if(tipo == Operacion.Editar)
            {

            }

            this.txtDescripcion.Focus();
        }

        /// <summary>
        /// Refresca la relación proporcional entre Precio-Costo-Ganancia en función del costo y precio.
        /// </summary>
        /// <param name="costo">Costo del artículo.</param>
        /// <param name="precio">Precio del artículo.</param>
        private void ActualizarRelacionGanancia(double costo, double precio)
        {
            //Costo no puede valer 0, crearía indeterminación.
            if (costo == 0)
                return;

            double porcentaje = (precio / costo) * 100;

            this.txtGanancia.Value = porcentaje;
        }
        /// <summary>
        /// Refresca la relación proporcional entre Precio-Costo-Ganancia en función del costo y el porcentaje de ganancia.
        /// </summary>
        /// <param name="costo">Costo del artículo.</param>
        /// <param name="porcentajeGanancia">Porcentaje de ganancia del artículo.</param>
        private void ActualizarRelacionPrecio(double costo, double porcentajeGanancia)
        {
            this.txtPrecio.Value = costo * porcentajeGanancia / 100;
        }
    }
}
