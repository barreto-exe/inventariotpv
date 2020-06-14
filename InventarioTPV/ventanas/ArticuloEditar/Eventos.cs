using HandyControl.Controls;
using System;

namespace InventarioTPV
{
    public partial class ArticuloEditar
    {
        private void BtnAceptar_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //Obtengo valores para Artículo
            string descripcion = txtDescripcion.Text;
            string codBarras = txtCodBar.Text;
            decimal costoDolar = Convert.ToDecimal(txtCosto.Value);
            decimal precioDolar = Convert.ToDecimal(txtPrecio.Value);

            //Instanciar artículo y guardo sus datos en base de datos
            Articulo articulo = new Articulo(descripcion, costoDolar, precioDolar, codBarras);

            //Verifica el tipo de operación sobre artículo
            if (this.tipo == Operacion.Crear)
            {
                articulo.RegistraDatosArticulo(true);
                articulo.RegistraPreciosArticulo();
            }
            if(this.tipo == Operacion.Editar)
            {
                //Actualizar datos del artículo
                Articulo.ActualizarDatosById(this.idArticulo, articulo);
            }

            this.Close();
        }

        #region Campos Costo|Precio|Ganancia
        private bool cambiando = false;
        private void TxtCosto_ValueChanged(object sender, HandyControl.Data.FunctionEventArgs<double> e)
        {
            if (cambiando)
                return;
            cambiando = true;
            ActualizarRelacionPrecio(txtCosto.Value, txtGanancia.Value);
            cambiando = false;
        }
        private void TxtPrecio_ValueChanged(object sender, HandyControl.Data.FunctionEventArgs<double> e)
        {
            if (cambiando)
                return;
            cambiando = true;
            ActualizarRelacionGanancia(txtCosto.Value, txtPrecio.Value);
            cambiando = false;
        }
        private void TxtGanancia_ValueChanged(object sender, HandyControl.Data.FunctionEventArgs<double> e)
        {
            if (cambiando)
                return;
            cambiando = true;
            ActualizarRelacionPrecio(txtCosto.Value, txtGanancia.Value);
            cambiando = false;
        }
        #endregion
    }
}
