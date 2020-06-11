using System;
using System.Data.SQLite;

namespace InventarioTPV
{
    public class Tasa
    {
        #region Atributos
        private decimal tasaDolar;
        private decimal porcentajeEfect;
        private string fecha;
        private string hora;
        private DateTime fechaCreacion;
        #endregion

        #region Getters and Setters
        public decimal ValorDolar
        {
            get
            {
                return tasaDolar;
            }
            set
            {
                tasaDolar = value;
            }
        }
        public decimal PorcentajeEfect
        {
            get
            {
                return porcentajeEfect;
            }
            set
            {
                porcentajeEfect = value;
            }
        }
        public DateTime FechaCreacion
        {
            get
            {
                return fechaCreacion;
            }
            set
            {
                fechaCreacion = value;
            }
        }
        public string Fecha
        {
            get
            {
                return fecha;
            }
        }
        public string Hora
        {
            get
            {
                return fechaCreacion.ToString("hh:mm:ss tt"); ;
            }
        }
        #endregion
        
        /// <summary>
        /// Objeto para manipular una tasa de conversión de bolívares a divisas.
        /// </summary>
        /// <param name="valorDolar">Valor de la unidad de la divisa en bolívares.</param>
        /// <param name="porcentajeEfect">Porcentaje de la ganancia que se quiere para el efectivo</param>
        /// <param name="fechaCreacion">Fecha de la instancia.</param>
        public Tasa(decimal valorDolar, decimal porcentajeEfect, DateTime fechaCreacion)
        {
            this.tasaDolar      = valorDolar;
            this.porcentajeEfect = porcentajeEfect;
            this.fechaCreacion   = fechaCreacion;
            this.fecha = fechaCreacion.ToString("dd/MM/yyyy");
            this.hora  = fechaCreacion.ToString("HH:mm:ss");
        }
        public Tasa(decimal valorDolar, decimal porcentajeEfect, string fecha, string hora)
        {
            this.tasaDolar = valorDolar;
            this.porcentajeEfect = porcentajeEfect;
            this.fecha = fecha;
            this.hora  = hora;

            //Parseo string a DateTime
            this.fechaCreacion = 
                DateTime.ParseExact(fecha + " " + hora, 
                "dd/MM/yyyy HH:mm:ss", 
                System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Verifica la existencia de la tasa en la base de datos.
        /// </summary>
        /// <returns></returns>
        private bool ValidarTasa()
        {
            string query =
                "SELECT * FROM c_tasa WHERE " +
                "tasaDolar = @TasaDolar AND " +
                "porcentajeEfectivo = @PorcentajeEfectivo AND " +
                "fecha = @Fecha AND " +
                "hora = @Hora";

            BDCon con = new BDCon(query);

            con.PasarParametros("TasaDolar", tasaDolar);
            con.PasarParametros("PorcentajeEfectivo", porcentajeEfect);
            con.PasarParametros("Fecha", fecha);
            con.PasarParametros("Hora", hora);

            SQLiteDataReader dr = con.ComandoSqlite().ExecuteReader();
            dr.Read();

            //Si existe una tasa con exactamente los mismos valores,
            //Se retorna verdadero.
            if(dr.HasRows)
            {
                dr.Close();
                return true;
            }

            dr.Close();
            return false;
        }

        /// <summary>
        /// Registra la instancia en la base de datos.
        /// </summary>
        /// <returns>Verdadero si logra hacer el registro</returns>
        public bool RegistrarTasa()
        {
            string query =
                "INSERT INTO c_tasa (tasaDolar,porcentajeEfectivo,fecha,hora) " +
                "VALUES( @TasaDolar, @PorcentajeEfectivo, @Fecha, @Hora )";
            BDCon con = new BDCon(query);

            con.PasarParametros("TasaDolar", tasaDolar);
            con.PasarParametros("PorcentajeEfectivo", porcentajeEfect);
            con.PasarParametros("Fecha", fecha);
            con.PasarParametros("Hora", hora);

            try
            {
                //Ejecuta el comando y verifica la cantidad de registros afectados
                if (con.ComandoSqlite().ExecuteNonQuery() > 0)
                {
                    return true;
                }

                //Si no hubo registros por alguna razón, arroja false
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Consultar la última tasa registrada.
        /// </summary>
        /// <returns>Última tasa registrada.</returns>
        public static Tasa ConsultarTasa()
        {
            string query =
                "SELECT * FROM c_tasa ORDER BY id DESC LIMIT 1";

            BDCon con = new BDCon(query);
            SQLiteDataReader dr = con.ComandoSqlite().ExecuteReader();
            dr.Read();

            return 
                new Tasa(
                Convert.ToDecimal(dr["tasaDolar"]), 
                Convert.ToDecimal(dr["porcentajeEfectivo"]), 
                Convert.ToString(dr["fecha"]), 
                Convert.ToString(dr["hora"]));
        }
    }
}
