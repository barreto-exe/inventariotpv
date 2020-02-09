using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Inventario_y_Contabilidad
{
    /// <summary>
    /// Lógica de interacción para ArticuloBuscar.xaml
    /// </summary>
    public partial class ArticuloBuscar : Window
    {

        public ArticuloClase idBuscado;
        public ArticuloBuscar()
        {
            InitializeComponent();

            txtBuscar_TextChanged(null, null);
            txtBuscar.Focus();
        }
        private void btnSeleccionar_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            ArticuloClase itemSelecto = button.DataContext as ArticuloClase;

            idBuscado = itemSelecto;
            this.Close();
        }
        void SortDataGrid(DataGrid dataGrid, int columnIndex = 0, ListSortDirection sortDirection = ListSortDirection.Ascending)
        {
            var column = dataGrid.Columns[columnIndex];

            // Clear current sort descriptions
            dataGrid.Items.SortDescriptions.Clear();

            // Add the new sort description
            dataGrid.Items.SortDescriptions.Add(new SortDescription(column.SortMemberPath, sortDirection));

            // Apply sort
            foreach (var col in dataGrid.Columns)
            {
                col.SortDirection = null;
            }
            column.SortDirection = sortDirection;

            // Refresh items to display sort
            dataGrid.Items.Refresh();
        }

        private void txtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            dataBuscarArt.Items.Clear();

            decimal tasa = cambioTasa.tasas()[0],
            porcentaje = cambioTasa.tasas()[1],
            precioDolar, precioBs, precioBsEfect;

            string query;
            SqlCeCommand command;
            SqlCeDataReader dr;

            query = "SELECT * FROM c_articulos WHERE descripcion like '%"+ txtBuscar.Text +"%'";
            command = new SqlCeCommand(query, MainWindow.conn);
            dr = command.ExecuteReader();

            int cuentaLinea = 0;

            while (dr.Read())
            {
                precioDolar = Decimal.Parse(dr["precioDolar"].ToString());

                if(dr["precioBs"].ToString() != "")
                {
                    precioBs = Decimal.Parse(dr["precioBs"].ToString());
                    precioBsEfect = Decimal.Parse(dr["precioBsEfect"].ToString());
                }
                else
                {
                    precioBs = precioDolar * tasa;
                    precioBsEfect = (precioBs * 100) / porcentaje;
                }

                var articulo = new ArticuloClase
                {
                    id = dr["id"].ToString(),
                    descripcion = dr["descripcion"].ToString(),
                    precioDolar = Decimal.Round(precioDolar, 2).ToString("#,#0.##"),
                    costoDolar = dr["costoDolar"].ToString(),
                    precioBs = Decimal.Round(precioBs, 2).ToString("#,#0.##"),
                    precioBsEfect = Decimal.Round(precioBsEfect, 2).ToString("#,#0.##"),
                    LineaRow = cuentaLinea++
                };

                dataBuscarArt.Items.Add(articulo);
            }

            SortDataGrid(dataBuscarArt, 1);
            dr.Close();
        }
    }
}
