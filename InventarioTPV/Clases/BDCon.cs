using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows.Controls;

namespace InventarioTPV.Clases
{
    public class BDCon
    {
        //El mismo path y la misma variable de conexión para todas las consultas
        //La variable de conexión siempre permanece abierta
        private static string BaseDatos = "Recursos\\db.db";
        private static SQLiteConnection conn = ConexionSqlite();

        private String Query;
        private List<String> NombreParametros;
        private List<String> ValorParametros;

        public BDCon(String query)
        {
            this.Query = query;
            NombreParametros = new List<string>();
            ValorParametros  = new List<string>();
        }
        public void PasarParametros(string nombre, string valor)
        {
            //Asocio a cada parámetro un valor.
            NombreParametros.Add(nombre);
            ValorParametros.Add(valor);
        }
        
        //Retorna una variable de conexión única
        public static SQLiteConnection ConexionSqlite()
        {
            //Valido existencia de la base de datos
            if (!File.Exists(BaseDatos))
            {
                throw new Exception("No hay una base de datos en la ruta indicada.");
            }

            //Instancio el string de conexión
            var connection_string = String.Format("Data Source={0};ServerVersion=3;", BaseDatos);

            //Instancio la variable de conexión
            SQLiteConnection conn = new SQLiteConnection(connection_string);
            try
            {
                //Si la conexión es posible, la retorno
                conn.Open();
                return conn;
            }
            catch
            {
                return null;
            }
        }
        
        //Abre una conexión global para todos los BDCon
        public static void AbrirConexionGlobal()
        {
            if(conn.State == ConnectionState.Open)
            {
                conn.Close();
            }

            conn = ConexionSqlite();
        }

        //Cierra la conexión global para los BDCon
        public static void CerrarConexionGlobal()
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }

        public SQLiteDataReader ConsultaSqlite()
        {
            SQLiteCommand command = new SQLiteCommand
            {
                //Los parámetros del query deben tener @
                //Por ejemplo: SELECT @Efectivo as tipopago, @Monto as monto FROM c_tasa
                CommandText = Query,
                Connection = conn
            };

            //Añado cada parámetro con su valor en el comando
            String[] nombres = NombreParametros.ToArray();
            String[] valores = ValorParametros.ToArray();
            for (int i = 0; i < nombres.Length; i++)
            {
                command.Parameters.AddWithValue(nombres[i], valores[i]);
            }

            try
            {
                //Ejecuto comando
                SQLiteDataReader dr = command.ExecuteReader();
                return dr;
            }
            catch
            {
                return null;
            }
        }

        //Si mi consulta sea para actualizar una tabla (DataGrid o ListView),
        //Entonces necesitaré un DataTable para el itemsSource.
        public DataTable TablaConsulta()
        {
            //Hago consulta
            SQLiteDataReader reader = ConsultaSqlite();

            //Creo un datatable y cargo los datos consultados
            DataTable data = new DataTable();
            data.Clear();
            data.Load(reader);

            //Cierro para prevenir errores
            reader.Close();
            
            return data;
        }

        public void ConsultaSqlite(ListView list)
        {
            //Actualizo la fuente de datos del datagrid o listview
            list.ItemsSource = TablaConsulta().DefaultView;
        }
        public void ConsultaSqlite(DataGrid grid)
        {
            //Actualizo la fuente de datos del datagrid o listview
            grid.ItemsSource = TablaConsulta().DefaultView;
        }
    }
}
