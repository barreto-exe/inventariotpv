using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Controls;

namespace InventarioTPV
{
    /// <summary>
    /// Lógica de interacción para ArticuloBuscar.xaml
    /// </summary>
    public partial class ArticuloBuscar
    {
        public ArticuloBuscar()
        {
            InitializeComponent();

            //Enfocar texto buscar
            this.txtBuscar.Focus();

            //Llamar evento buscar para refrescar tabla con todos los artículos
            TxtBuscar_TextChanged(null, null);
        }

        private void TxtBuscar_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            //Armar query de consulta
            string query =
                "SELECT " +
                "id," +
                "descripcion," +
                "precioDolar," +
                "costoDolar, " +
                "codBarras " +
                "FROM c_articulos " +
                "WHERE " +
                "descripcion like @Descripcion " +
                "OR " +
                "codBarras like @CodBarras";
            BDCon con = new BDCon(query);
            con.PasarParametros("Descripcion", "%" + this.txtBuscar.Text  + "%");
            con.PasarParametros("CodBarras",   "%" + this.txtBuscar.Text + "%");

            //Llenar datagrid con los datos consultados
            con.ConsultaSqlite(this.dataBuscados);
        }

        private void BtnEditarArticulo_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //Casteo súper mega heavy poderoso para obtener los datos a editar
            object[] datos = ((DataRowView)((Button)sender).DataContext).Row.ItemArray;

            //Obteniendo datos del arreglo
            int id = Convert.ToInt32(datos[0]);
            string descripcion = (string)datos[1];
            decimal precio = Convert.ToDecimal(datos[2]);
            decimal costo  = Convert.ToDecimal(datos[3]);
            string codBarras = (string)datos[4];

            //Instanciando artículo a editar
            Articulo articulo = new Articulo(descripcion, costo, precio, codBarras);

            ArticuloEditar editar = new ArticuloEditar(id,articulo);
            editar.Owner = this;
            editar.ShowDialog();

            //Actualizo tabla para refrescar nuevos resultados
            this.txtBuscar.Text = "";
            this.TxtBuscar_TextChanged(null, null);
        }
    }
}
