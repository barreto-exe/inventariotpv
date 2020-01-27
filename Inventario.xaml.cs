using System;
using System.Collections.Generic;
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

            string tasaStr, porcentajeStr;

            //Consultando Tasa Actual
            string query = "SELECT TOP 1 * FROM c_tasa ORDER BY id DESC";
            SqlCeCommand command = new SqlCeCommand(query, MainWindow.conn);
            SqlCeDataReader dr = command.ExecuteReader();
            dr.Read();
            tasaStr = dr["tasaDolar"].ToString();
            porcentajeStr = dr["porcentajeEfectivo"].ToString();
            dr.Close();

            decimal tasa = decimal.Parse(tasaStr),
            porcentaje = decimal.Parse(porcentajeStr) + 100;

            //Consultando artículos
            query = "SELECT * FROM c_articulos";
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
                    decimal precioBs = precioDolar * tasa;
                    decimal precioBsEfect = (precioBs * 100) / porcentaje;
                    


                    var articulo = new ArticuloClase
                    {
                        id = dr_art["id"].ToString(),
                        descripcion = dr_art["descripcion"].ToString(),
                        cantAct = cantidadAct.ToString(),
                        precioDolar = Decimal.Round(precioDolar,2).ToString(),
                        costoDolar = Decimal.Round(costoDolar, 2).ToString(),
                        fechaHora = drFecha.GetValue(0).ToString(),
                        precioBs = Decimal.Round(precioBs, 2).ToString(),
                        precioBsEfect = Decimal.Round(precioBsEfect, 2).ToString()
                    };

                    drFecha.Close();
                    dataReportePrin.Items.Add(articulo);

                    valorInventario += precioDolar * cantidadAct;
                }
            }
            dr_art.Close();

            lblValorInventario.Content = valorInventario.ToString() + "$ - Bs.S. " + (valorInventario * tasa).ToString();

        }

        private void imgAgregarArt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Articulo articulo = new Articulo(1);
            articulo.Owner = this;
            articulo.ShowDialog();

            cargarInventario();
        }

        private void imgEditar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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
    }
}
