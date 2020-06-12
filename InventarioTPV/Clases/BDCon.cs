using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows.Controls;

namespace InventarioTPV
{
    public class BDCon
    {
        #region Atributos
        //El mismo path y la misma variable de conexión para todas las consultas
        //La variable de conexión siempre permanece abierta
        private static string BaseDatos = "Recursos\\db.db";
        public static SQLiteConnection conGlobal = ConexionSqlite();

        private string query;
        private List<String> NombreParametros;
        private List<Object> ValorParametros;
        #endregion

        #region Getters y Setters
        public string Query
        {
            get
            {
                return query;
            }
            set
            {
                query = value;
            }
        }
        #endregion

        /// <summary>
        /// Objeto para construir y ejecutar consulta a la base de datos.
        /// </summary>
        /// <param name="query">Query a ejecutar en la base de datos</param>
        public BDCon(String query)
        {
            this.query = query;
            NombreParametros = new List<string>();
            ValorParametros  = new List<Object>();
        }

        #region Métodos estáticos
        /// <summary>
        /// Crea y retorna una variable de conexión única.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Abre una conexión global para todos los BDCon. Si ya existe una abierta, la cierra y la sobreescribe.
        /// </summary>
        public static void AbrirConexionGlobal()
        {
            if(conGlobal.State == ConnectionState.Open)
            {
                conGlobal.Close();
            }

            conGlobal = ConexionSqlite();
        }

        /// <summary>
        /// Cierra la conexión global para los BDCon.
        /// </summary>
        public static void CerrarConexionGlobal()
        {
            if (conGlobal.State == ConnectionState.Open)
            {
                conGlobal.Close();
            }
        }
        #endregion

        #region Métodos y Funciones
        /// <summary>
        /// Enviar parámetros con sus valores 
        /// en caso de que el query lo requiera.
        /// </summary>
        /// <param name="nombre">Nombre del parámetro</param>
        /// <param name="valor">Valor del parámetro</param>
        public void PasarParametros(string nombre, object valor)
        {
            //Asocio a cada parámetro un valor.
            NombreParametros.Add(nombre);
            ValorParametros.Add(valor);
        }
        /// <summary>
        /// Realiza la consulta y llena un DataTable con esta información.
        /// </summary>
        /// <returns>DataTable con el DataReader de la consulta cargado.</returns>
        public DataTable TablaConsulta()
        {
            //Hago consulta
            SQLiteDataReader reader = ComandoSqlite().ExecuteReader();

            //Creo un datatable y cargo los datos consultados
            DataTable data = new DataTable();
            data.Clear();
            data.Load(reader);

            //Cierro para prevenir errores
            reader.Close();

            return data;
        }
        /// <summary>
        /// Construye el SQLiteCommand para consultar a la base de datos.
        /// </summary>
        public SQLiteCommand ComandoSqlite()
        {
            SQLiteCommand command = new SQLiteCommand
            {
                //Los parámetros del query deben tener @
                //Por ejemplo: SELECT @Efectivo as tipopago, @Monto as monto FROM c_tasa
                CommandText = query,
                Connection = conGlobal
            };

            //Añado cada parámetro con su valor en el comando
            String[] nombres = NombreParametros.ToArray();
            Object[] valores = ValorParametros.ToArray();
            for (int i = 0; i < nombres.Length; i++)
            {
                command.Parameters.AddWithValue(nombres[i], valores[i]);
            }

            return command;
        }
        /// <summary>
        /// Ejecuta el comando. Retorna la cantidad de registros afectados.
        /// </summary>
        /// <returns></returns>
        public int EjecutarComando()
        {
            try
            {
                return this.ComandoSqlite().ExecuteNonQuery();
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// Realizar la consulta a la base de datos.
        /// </summary>
        /// <param name="list">Un ListView al que se le requiera llenar con la información consultada.</param>
        public void ConsultaSqlite(ListView list)
        {
            //Actualizo la fuente de datos del datagrid o listview
            list.ItemsSource = TablaConsulta().DefaultView;
        }
        /// <summary>
        /// Realizar la consulta a la base de datos.
        /// </summary>
        /// <param name="grid">Un DataGrid al que se le requiera llenar con la información consultada.</param>
        public void ConsultaSqlite(DataGrid grid)
        {
            //Actualizo la fuente de datos del datagrid o listview
            grid.ItemsSource = TablaConsulta().DefaultView;
        }
        #endregion
    }
}
