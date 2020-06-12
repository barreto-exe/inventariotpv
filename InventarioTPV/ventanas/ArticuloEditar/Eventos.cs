using System;

namespace InventarioTPV
{
    public partial class ArticuloEditar
    {
        private void BtnAceptar_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string descripcion = txtDescripcion.Text;
            string codBarras   = txtCodBar.Text;
            decimal costoDolar  = Convert.ToDecimal(txtCosto.Value);
            decimal precioDolar = Convert.ToDecimal(txtPrecio.Value);

            Articulo articulo = new Articulo(descripcion, costoDolar, precioDolar, codBarras);
            articulo.RegistraDatosArticulo(true);
            articulo.RegistraPreciosArticulo();

            this.Close();
        }
        private void TxtCosto_ValueChanged(object sender, HandyControl.Data.FunctionEventArgs<double> e)
        {
            ActualizarRelacionPrecio(txtCosto.Value, txtGanancia.Value);
        }
        private void TxtPrecio_ValueChanged(object sender, HandyControl.Data.FunctionEventArgs<double> e)
        {
            ActualizarRelacionGanancia(txtCosto.Value, txtPrecio.Value);
        }
        private void TxtGanancia_ValueChanged(object sender, HandyControl.Data.FunctionEventArgs<double> e)
        {
            ActualizarRelacionPrecio(txtCosto.Value, txtGanancia.Value);
        }
    }
}
