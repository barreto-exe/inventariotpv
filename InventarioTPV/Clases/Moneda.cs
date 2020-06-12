using System;
using System.Data.SQLite;

namespace InventarioTPV
{
    public class Moneda
    {
        #region Atributos
        private string descripcion;
        private string simbolo;
        #endregion

        #region Getters y Setters
        public string Descripcion
        {
            get
            {
                return descripcion;
            }
            set
            {
                descripcion = value;
            }
        }
        public string Simbolo
        {
            get
            {
                return simbolo;
            }
            set
            {
                simbolo = value;
            }
        }
        public int Id
        {
            get
            {
                //Me aseguro que esté registrada en la BBDD.
                if(!ValidaMoneda())
                {
                    //Si no lo está la registro
                    RegistraMoneda();

                    //Ahora la instancia si tiene ID.
                }
                string query =
                    "SELECT * FROM c_monedas WHERE " +
                    "descripcion = @Descripcion AND " +
                    "simbolo = @Simbolo";

                BDCon con = new BDCon(query);

                con.PasarParametros("Descripcion", this.descripcion);
                con.PasarParametros("Simbolo", this.simbolo);

                SQLiteDataReader dr = con.ComandoSqlite().ExecuteReader();
                dr.Read();

                //Leo el id de la consulta
                int id = Convert.ToInt32(dr["id"]);

                dr.Close();

                return id;
            }
        }
        #endregion

        /// <summary>
        /// Tipo de dato que representa un tipo de moneda.
        /// </summary>
        /// <param name="descripcion">Nombre de la moneda.</param>
        /// <param name="simbolo">Símbolo de la moneda.</param>
        public Moneda(string descripcion, string simbolo)
        {
            this.descripcion = descripcion;
            this.simbolo = simbolo;
        }

        /// <summary>
        /// Verifica la existencia de la instancia en la base de datos.
        /// </summary>
        /// <returns></returns>
        private bool ValidaMoneda()
        {
            string query =
                "SELECT * FROM c_monedas WHERE " +
                "descripcion = @Descripcion AND " +
                "simbolo = @Simbolo";

            BDCon con = new BDCon(query);

            con.PasarParametros("Descripcion", this.descripcion);
            con.PasarParametros("Simbolo", this.simbolo);

            SQLiteDataReader dr = con.ComandoSqlite().ExecuteReader();
            dr.Read();

            //Si existe una moneda con exactamente los mismos valores,
            //Se retorna verdadero.
            if (dr.HasRows)
            {
                dr.Close();
                return true;
            }

            dr.Close();
            return false;
        }

        /// <summary>
        /// Registrar instancia de moneda en la base de datos.
        /// </summary>
        /// <returns></returns>
        public bool RegistraMoneda()
        {
            //Si ya existe, no la creo y retorno true.
            if (ValidaMoneda())
                return true;

            string query =
                "INSERT INTO c_monedas (descripcion,simbolo) " +
                "VALUES( @Descripcion, @Simbolo )";
            BDCon con = new BDCon(query);

            con.PasarParametros("Descripcion", this.descripcion);
            con.PasarParametros("Simbolo", this.simbolo);

            //Ejecuta el comando y verifica la cantidad de registros afectados
            if (con.EjecutarComando() > 0)
            {
                return true;
            }

            //Si no hubo registros por alguna razón, arroja false
            return false;
        }
        
        /// <summary>
        /// Devuelve instancia de moneda dado un id de la BBDD. Si no existe, retorna null.
        /// </summary>
        /// <param name="id">Id de la moneda</param>
        /// <returns></returns>
        public static Moneda MonedaById(int id)
        {
            Moneda moneda = null;

            //Construyo la consulta
            string query = "SELECT * FROM c_monedas WHERE id = @Id";
            BDCon con = new BDCon(query);
            con.PasarParametros("Id", id);
            SQLiteDataReader dr = con.ComandoSqlite().ExecuteReader();
            
            //Si existe el objeto consultado, lo instancio
            if(dr.Read())
            {
                moneda = new Moneda((string)dr["descripcion"], (string)dr["simbolo"]);
            }

            //Cierro para prevenir errores
            dr.Close();

            return moneda;
        }
    }
}
