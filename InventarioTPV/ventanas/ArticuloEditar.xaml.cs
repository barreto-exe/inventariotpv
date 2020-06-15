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
        #region Atributos
        public enum Operacion
        {
            Crear,
            Editar
        }
        private Operacion tipo;
        private Articulo articulo;
        private int idArticulo;
        #endregion

        #region Constructores
        /// <summary>
        /// Instancia de una ventana para editar un artículo.
        /// </summary>
        /// <param name="articulo">Artículo a editar.</param>
        public ArticuloEditar(int idArticulo, Articulo articulo)
        {
            InitializeComponent();

            //Obtener datos del artículo a editar
            this.articulo   = articulo;
            this.idArticulo = idArticulo;

            //Identificando en la clase el tipo de operación
            this.tipo = Operacion.Editar;

            //Llenar los campos con las propiedades preexistentes
            this.txtDescripcion.Text = articulo.Descripcion;
            this.txtCosto.Value  = Convert.ToDouble(articulo.CostoDolar);
            this.txtPrecio.Value = Convert.ToDouble(articulo.PrecioDolar);
            this.txtCodBar.Text  = articulo.CodBarras;
            ActualizarRelacionGanancia(txtCosto.Value, txtPrecio.Value);

            //Enfocar primer caja de texto
            this.txtDescripcion.Focus();
        }
        /// <summary>
        /// Instancia de una ventana para crear un artículo.
        /// </summary>
        public ArticuloEditar()
        {
            InitializeComponent();
            
            //Identificando en la clase el tipo de operación
            this.tipo = Operacion.Crear;

            ////Se colapsa el botón buscar.
            //this.btnBuscar.Visibility = System.Windows.Visibility.Collapsed;

            //Enfocar primer caja de texto
            this.txtDescripcion.Focus();
        }
        #endregion
        
        #region Métodos
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

            double porcentaje = (precio / costo) * 100 - 100;

            this.txtGanancia.Value = porcentaje;
        }
        /// <summary>
        /// Refresca la relación proporcional entre Precio-Costo-Ganancia en función del costo y el porcentaje de ganancia.
        /// </summary>
        /// <param name="costo">Costo del artículo.</param>
        /// <param name="porcentajeGanancia">Porcentaje de ganancia del artículo.</param>
        private void ActualizarRelacionPrecio(double costo, double porcentajeGanancia)
        {
            this.txtPrecio.Value = costo + costo * porcentajeGanancia / 100;
        }
        #endregion

    }
}
