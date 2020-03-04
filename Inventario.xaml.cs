using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Inventario_y_Contabilidad
{
    /// <summary>
    /// Lógica de interacción para Inventario.xaml
    /// </summary>
    public partial class Inventario : Window
    {
        public Inventario()
        {
            InitializeComponent();

            cargarInventario();
        }
        private void cargarInventario()
        {
            dataReportePrin.Items.Clear();

            decimal tasa = cambioTasa.tasas()[0],
            porcentaje   = cambioTasa.tasas()[1];

            string query;
            SqlCeCommand command;
            SqlCeDataReader dr;

            //Consultando artículos
            query = "SELECT * FROM c_articulos WHERE activo = 1";
            command = new SqlCeCommand(query, MainWindow.conn);
            SqlCeDataReader dr_art = command.ExecuteReader();

            decimal valorInventario = 0;

            //Por cada artículo en inventario
            while (dr_art.Read())
            {
                //Consultando cantidad
                query = "SELECT sum(cantidad) FROM c_inventario WHERE " +
                        "id_articulo = " + dr_art["id"].ToString();
                command = new SqlCeCommand(query, MainWindow.conn);
                dr = command.ExecuteReader();
                dr.Read();
                int cantidadAct = int.Parse(dr.GetValue(0).ToString());
                dr.Close();

                if (cantidadAct > 0)
                {
                    //Consultando última actualización
                    query = "SELECT TOP 1 fechaHora FROM c_inventario WHERE " +
                            "id_articulo = " + dr_art["id"].ToString();
                    command = new SqlCeCommand(query, MainWindow.conn);
                    SqlCeDataReader drFecha = command.ExecuteReader();
                    drFecha.Read();

                    decimal precioDolar = decimal.Parse(dr_art["precioDolar"].ToString());
                    decimal costoDolar = decimal.Parse(dr_art["costoDolar"].ToString());
                    decimal precioBsRec = precioDolar * tasa;
                    decimal precioBsEfectRec = (precioBsRec * 100) / porcentaje;
                    decimal precioBs, precioBsEfect;

                    if(dr_art["precioBs"].ToString() != "")
                    {
                        precioBs = Decimal.Parse(dr_art["precioBs"].ToString());
                        precioBsEfect = Decimal.Parse(dr_art["precioBsEfect"].ToString());
                    }
                    else
                    {
                        precioBs = precioBsRec; 
                        precioBsEfect = precioBsEfectRec;
                    }

                    var articulo = new ArticuloClase
                    {
                        id = dr_art["id"].ToString(),
                        descripcion = dr_art["descripcion"].ToString(),
                        cantAct = cantidadAct.ToString(),
                        precioDolar = Decimal.Round(precioDolar,2).ToString("#,#0.##"),
                        costoDolar = Decimal.Round(costoDolar, 2).ToString("#,#0.##"),
                        fechaHora = drFecha.GetValue(0).ToString(),
                        precioBs = Decimal.Round(precioBs, 2).ToString("#,#0.##"),
                        precioBsEfect = Decimal.Round(precioBsEfect, 2).ToString("#,#0.##"),
                        precioBsRec = Decimal.Round(precioBsRec, 2).ToString("#,#0.##"),
                        precioBsEfectRec = Decimal.Round(precioBsEfectRec, 2).ToString("#,#0.##")
                    };

                    drFecha.Close();
                    dataReportePrin.Items.Add(articulo);

                    valorInventario += precioDolar * cantidadAct;
                }
            }
            dr_art.Close();

            lblValorInventario.Content = valorInventario.ToString("#,#0.##") + "$ - Bs.S. " + (valorInventario * tasa).ToString("#,#0.##");


            //Advertir de precio no recomendado
            advertencia();
        }

        private void advertencia()
        {
            int cantArt = dataReportePrin.Items.Count;
            ArticuloClase art;

            DataGrid aux = new DataGrid();

            for (int i = 0; i < cantArt; i++)
            {
                art = dataReportePrin.ItemContainerGenerator.Items[i] as ArticuloClase;
                aux.Items.Add(art);
            }

            dataReportePrin.Items.Clear();

            for (int i = 0; i < cantArt; i++)
            {
                art = aux.ItemContainerGenerator.Items[i] as ArticuloClase;

                if (Decimal.Parse(art.precioBs) < Decimal.Parse(art.precioBsRec) ||
                    Decimal.Parse(art.precioBsEfect) < Decimal.Parse(art.precioBsEfectRec))
                {
                    art.LineaRow = 1;
                }
                else 
                if (Decimal.Parse(art.precioBs) == Decimal.Parse(art.precioBsRec) ||
                    Decimal.Parse(art.precioBsEfect) == Decimal.Parse(art.precioBsEfectRec))
                {
                    art.LineaRow = 2;
                }
                else
                {
                    art.LineaRow = 0;
                }

                dataReportePrin.Items.Add(art);
            }
        }

        private void btnAgregar_Click(object sender, RoutedEventArgs e)
        {
            Articulo articulo = new Articulo(1);
            articulo.Owner = this;
            articulo.ShowDialog();

            cargarInventario();
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            Articulo articulo = new Articulo(2);
            articulo.Owner = this;

            //Sólo si seleccionó algún artículo
            if (articulo.articuloBuscado == 1)
            {
                articulo.ShowDialog();
            }

            cargarInventario();
        }

        private void btnBorrar_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            ArticuloClase itemDelete = button.DataContext as ArticuloClase;

            MessageBoxResult messageBoxResult = MessageBox.Show("¿Desea confirmar la operación?", "Confirmar", MessageBoxButton.YesNo);
            if (messageBoxResult != MessageBoxResult.Yes)
                return;

            string query = "UPDATE c_articulos SET activo = 0 WHERE id = " + CS(itemDelete.id);
            SqlCeCommand cm = new SqlCeCommand(query, MainWindow.conn);
            cm.ExecuteNonQuery();

            cargarInventario();
        }

        private string CS(string strSinComillas)
        {
            //Añade comillas simples
            strSinComillas = strSinComillas.Replace("'", "''");
            return "'" + strSinComillas + "'";
        }

    }
}
