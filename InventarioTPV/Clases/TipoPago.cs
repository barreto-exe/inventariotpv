using System.Collections.Generic;
using System.Data.SQLite;

namespace InventarioTPV
{
    public class TipoPago
    {
        #region Atributos
        private string descripcion;
        private Moneda moneda;
        private bool aplicaDescuento;
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
        public bool AplicaDescuento
        {
            get
            {
                return aplicaDescuento;
            }
            set
            {
                aplicaDescuento = value;
            }
        }
        public Moneda Moneda
        {
            get
            {
                return moneda;
            }
            set
            {
                moneda = value;
            }
        }
        public int Id
        {
            get
            {
                //Me aseguro que esté registrada en la BBDD.
                if (!ValidaTipoPago())
                {
                    //Si no lo está la registro
                    RegistraTipoPago();

                    //Ahora la instancia si tiene ID.
                }

                string query =
                    "SELECT * FROM c_tipopagos WHERE " +
                    "descripcion = @Descripcion AND " +
                    "idMoneda = @IdMoneda";

                BDCon con = new BDCon(query);

                con.PasarParametros("Descripcion", this.descripcion);
                con.PasarParametros("IdMoneda", this.moneda.Id);

                SQLiteDataReader dr = con.ComandoSqlite().ExecuteReader();
                dr.Read();

                //Leo el id de la consulta
                int id = (int)dr["id"];

                dr.Close();

                return id;
            }
        }
        #endregion

        /// <summary>
        /// Tipo de dato que representa un tipo de pago.
        /// </summary>
        /// <param name="descripcion">Nombre del tipo de pago.</param>
        /// <param name="moneda">Instantcia del tipo de moneda que utiliza.</param>
        /// <param name="aplicaDescuento">Indica si aplica para el descuento de porcentaje por efectivo.</param>
        public TipoPago(string descripcion, Moneda moneda, bool aplicaDescuento)
        {
            this.descripcion = descripcion;
            this.moneda = moneda;
            this.aplicaDescuento = aplicaDescuento;
        }

        /// <summary>
        /// Verifica la existencia de la instancia en la base de datos.
        /// </summary>
        /// <returns></returns>
        private bool ValidaTipoPago()
        {
            string query =
                "SELECT * FROM c_tipopagos WHERE " +
                "descripcion = @Descripcion AND " +
                "idMoneda = @IdMoneda";

            BDCon con = new BDCon(query);

            con.PasarParametros("Descripcion", this.descripcion);
            con.PasarParametros("IdMoneda", this.moneda.Id);

            SQLiteDataReader dr = con.ComandoSqlite().ExecuteReader();
            dr.Read();

            //Si existe un tipo de pago con exactamente los mismos valores,
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
        public bool RegistraTipoPago()
        {
            //Si ya existe, no la creo y retorno true.
            if (ValidaTipoPago())
                return true;

            string query =
                "INSERT INTO c_tipopagos (descripcion,idMoneda,aplicaDescuento) " +
                "VALUES( @Descripcion, @IdMoneda, @AplicaDescuento )";
            BDCon con = new BDCon(query);

            con.PasarParametros("Descripcion", this.descripcion);
            con.PasarParametros("IdMoneda", this.moneda.Id);

            int aplicaDescuento = 0;
            if (this.aplicaDescuento == true)
                aplicaDescuento = 1;
            con.PasarParametros("AplicaDescuento", aplicaDescuento);

            //Ejecuta el comando y verifica la cantidad de registros afectados
            if (con.EjecutarComando() > 0)
            {
                return true;
            }

            //Si no hubo registros por alguna razón, arroja false
            return false;
        }

        /// <summary>
        /// Devuelve una lista con los tipos de pago disponibles en la BBDD.
        /// </summary>
        /// <returns></returns>
        public static List<TipoPago> TiposDisponibles()
        {
            List<TipoPago> disponibles = new List<TipoPago>();
            TipoPago pago;
            Moneda moneda;

            string query = "SELECT * FROM c_tipopagos";
            BDCon con = new BDCon(query);
            SQLiteDataReader dr = con.ComandoSqlite().ExecuteReader();

            //Mientras haya registros disp. en la consulta.
            while(dr.Read())
            {
                //Obtengo instancia de la moneda asociada 
                moneda = Moneda.MonedaById((int)dr["idMoneda"]);

                //Determino si aplica para descuento o no
                bool aplicaDescuento = false;
                if ((int)dr["aplicaDescuento"] == 1)
                    aplicaDescuento = true;

                //Obtengo instancia con la descripción y moneda asociada
                pago = new TipoPago((string)dr["descripcion"], moneda, aplicaDescuento);

                //Añado el pago a los disponibles 
                disponibles.Add(pago);
            }

            //Cierro para prevenir errores.
            dr.Close();

            return disponibles;
        }
    }
}
