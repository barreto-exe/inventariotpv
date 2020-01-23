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

            string query = "SELECT * FROM c_articulos";
            SqlCeCommand command = new SqlCeCommand(query, MainWindow.conn);
            SqlCeDataReader dr = command.ExecuteReader();

            int cuentaLinea = 0;

            while(dr.Read())
            {
                var articulo = new ArticuloClase
                {
                    id = dr["id"].ToString(),
                    descripcion = dr["descripcion"].ToString(),
                    precioDolar = dr["precioDolar"].ToString(),
                    costoDolar = dr["costoDolar"].ToString(),
                    LineaRow = cuentaLinea++
                };

                dataBuscarArt.Items.Add(articulo);
            }
            SortDataGrid(dataBuscarArt,1);
            dr.Close();
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
    }
}
