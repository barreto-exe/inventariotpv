using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace InventarioTPV
{
    public class Articulo
    {
        #region Atributos
        private List<TipoPago> tiposDePago;
        private List<decimal> preciosCalculados;
        private List<decimal> preciosRedondos;
        private string descripcion;
        private decimal costoDolar;
        private decimal precioDolar;
        private decimal ganancia; //La diferencia entre precio menos el costo.
        private string codBarras;
        #endregion

        #region Getters y Setters
        /// <summary>
        /// ID del artículo en la BBDD. Si no está registrado, el ID es nulo.
        /// </summary>
        public int? Id
        {
            get
            {
                return ValidaIdArticulo();
            }
        }
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
        public decimal CostoDolar
        {
            get
            {
                return costoDolar;
            }
            set
            {
                ActualizarPrecios();
                costoDolar = value;
            }
        }
        public decimal PrecioDolar
        {
            get
            {
                return precioDolar;
            }
            set
            {
                ActualizarPrecios();
                precioDolar = value;
            }
        }
        public decimal Ganancia
        {
            get
            {
                return precioDolar - costoDolar;
            }
        }
        public string CodBarras
        {
            get
            {
                return codBarras;
            }
            set
            {
                codBarras = value;
            }
        }
        public List<TipoPago> TiposDePago
        {
            get
            {
                return tiposDePago;
            }
        }
        public List<decimal> PreciosCalculados
        {
            get
            {
                return preciosCalculados;
            }
        }
        public List<decimal> PreciosRedondos
        {
            get
            {
                return preciosRedondos;
            }
        }
        #endregion

        public Articulo(string descripcion, decimal costoDolar, decimal precioDolar, string codBarras ="")
        {
            this.descripcion = descripcion;
            this.costoDolar  = costoDolar;
            this.precioDolar = precioDolar;
            this.ganancia  = precioDolar - costoDolar;
            this.codBarras = codBarras;

            ActualizarPrecios();
        }

        #region Métodos de Registro en BBDD
        /// <summary>
        /// Registra el artículo y sus precios en la base de datos, con el estado de actividad indicado.
        /// </summary>
        /// <param name="articulo">Artículo a registrar.</param>
        /// <param name="activo">Activo o Inactivo.</param>
        /// <returns></returns>
        public bool RegistraDatosArticulo(bool activo)
        {
            //Si el artículo tiene id, es porque ya está registrado.
            if(this.Id != null)
            {
                return true;
            }

            string query =
                "INSERT INTO c_articulos (descripcion,precioDolar,costoDolar,codBarras,activo) " +
                "VALUES( @Descripcion, @PrecioDolar, @CostoDolar, @CodBarras, @Activo )";
            BDCon con = new BDCon(query);

            con.PasarParametros("Descripcion", this.Descripcion);
            con.PasarParametros("PrecioDolar", this.PrecioDolar);
            con.PasarParametros("CostoDolar",  this.CostoDolar);
            con.PasarParametros("CodBarras",   this.CodBarras);
            con.PasarParametros("Activo", activo);

            //Ejecuta el comando y verifica la cantidad de registros afectados
            if(con.EjecutarComando() > 0)
            {
                return true;
            }

            //Si no hubo registros por alguna razón, arroja false
            return false;
        }
        /// <summary>
        /// Registra los precios asociados al artículo en la base de datos.
        /// </summary>
        /// <param name="articulo">Artículo con precios inicializados.</param>
        /// <returns></returns>
        public void RegistraPreciosArticulo()
        {
            //Si el artículo no tiene id, entonces no ha sido registrado.
            int? id = this.Id;
            if (id == null)
            {
                //Retorno, indicando que no se pueden registrar sus precios.
                return;
            }

            decimal[] preciosRedondos   = this.PreciosRedondos.ToArray();
            decimal[] preciosCalculados = this.PreciosCalculados.ToArray();
            TipoPago[] tiposPago = this.TiposDePago.ToArray();

            #region Variables auxiliares
            string query;
            BDCon con;
            #endregion

            for (int i = 0; i < preciosRedondos.Length; i++)
            {
                //Construyo el query
                query =
                    "INSERT INTO c_articulos_precios (idArticulo,idTipoPago,precioCalculado,precioRedondo) " +
                    "VALUES( @IdArticulo, @IdTipoPago, @PrecioCalculado, @PrecioRedondo )";
                con = new BDCon(query);

                //Id del artículo
                con.PasarParametros("IdArticulo", id);
                //Id del tipo de pago de turno en el ciclo
                con.PasarParametros("IdTipoPago", tiposPago[i].Id);
                //Precios correspondientes que se asociaron al tipo de pago en cuestión
                con.PasarParametros("PrecioCalculado", preciosCalculados[i]);
                con.PasarParametros("PrecioRedondo", preciosRedondos[i]);

                //Ejecuto el comando 
                con.EjecutarComando();
            }
        }
        /// <summary>
        /// Actualiza los precios calculados y redondeados.
        /// </summary>
        private void ActualizarPrecios()
        {
            //Consulto tasa
            Tasa tasa = Tasa.ConsultarTasa();

            //Enlisto los tipo de pago disponibles
            tiposDePago = TipoPago.TiposDisponibles();

            //Inicializo las listas de precios
            preciosCalculados = new List<decimal>();
            preciosRedondos = new List<decimal>();
            decimal precioCalculado, precioRedondo;

            //Por cada tipo de pago, calculo su precio convertido
            foreach (TipoPago pago in tiposDePago)
            {
                precioCalculado = precioDolar * tasa.ValorDolar;
                if (pago.AplicaDescuento)
                {
                    precioCalculado = (precioCalculado * 100) / (tasa.PorcentajeEfect+100);
                }

                //Redondea basada en la configuración del usuario
                precioRedondo = RedondeaArriba(precioCalculado, Properties.Settings.Default.FactorRedondeo);

                //Añado precios a la lista
                preciosCalculados.Add(precioCalculado);
                preciosRedondos.Add(precioRedondo);
            }
        }
        /// <summary>
        /// Agregar unidades del artículo en el inventario. Si no existe en la BBDD, retorna false.
        /// </summary>
        /// <param name="cantidad">Cantidad a añadir.</param>
        /// <returns></returns>
        public bool AgregarInventario(int cantidad)
        {
            int? id = this.Id;

            //Si el artículo no tiene id, no se le pueden agregar unidades.
            if (id == null)
            {
                return false;
            }

            string query =
                "INSERT INTO c_articulos (fechaHora, idArticulo, cantidad) " +
                "VALUES( @FechaHora, @IdArticulo, @Cantidad )";

            BDCon con = new BDCon(query);
            con.PasarParametros("FechaHora", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            con.PasarParametros("IdArticulo", id);
            con.PasarParametros("IdArticulo", cantidad);

            //Ejecuta el comando y verifica la cantidad de registros afectados
            if (con.EjecutarComando() > 0)
            {
                return true;
            }

            //Si no hubo registros por alguna razón, arroja false
            return false;
        }
        /// <summary>
        /// Restar unidades del artículo en el inventario. Si no existe en la BBDD, retorna false.
        /// </summary>
        /// <param name="cantidad">Cantidad a añadir.</param>
        /// <returns></returns>
        public bool RestarInventario(int cantidad)
        {
            return AgregarInventario(-1*cantidad);
        }
        #endregion

        #region Misceláneos
        /// <summary>
        /// Verifica la existencia de la instancia en la base de datos. Retorna el id del artículo
        /// </summary>
        /// <returns></returns>
        private int? ValidaIdArticulo()
        {
            string query =
                "SELECT id FROM c_articulos WHERE " +
                "descripcion = @Descripcion AND " +
                "precioDolar = @PrecioDolar AND " +
                "costoDolar  = @CostoDolar  AND " +
                "codBarras   = @CodBarras";

            BDCon con = new BDCon(query);

            con.PasarParametros("Descripcion", this.descripcion);
            con.PasarParametros("PrecioDolar", this.precioDolar);
            con.PasarParametros("CostoDolar", this.costoDolar);
            con.PasarParametros("CodBarras", this.codBarras);

            SQLiteDataReader dr = con.ComandoSqlite().ExecuteReader();

            //Si existe un artículo con exactamente los mismos valores,
            //Se retorna el id.
            if (dr.Read())
            {
                int id = Convert.ToInt32(dr["id"]);
                dr.Close();
                return id;
            }

            dr.Close();

            //Si no existe, retorno null para indicar inexistencia
            return null;
        }
        /// <summary>
        /// Consulta si un artículo está activo o inactivo.
        /// </summary>
        /// <param name="id">Id del artículo a consultar</param>
        /// <returns></returns>
        public static bool ActivoById(int id)
        {
            //Construyendo consulta
            string query =
                "SELECT * FROM c_articulos WHERE id = @Id AND activo = 1";
            BDCon con = new BDCon(query);
            con.PasarParametros("Id", id);
            SQLiteDataReader dr = con.ComandoSqlite().ExecuteReader();
            dr.Read();

            //Si existe un artículo activo con dicha Id,
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
        /// Redondea un monto al mayor múltiplo de 10 más cercano.
        /// </summary>
        /// <param name="monto">Monto a redonder.</param>
        /// <param name="espacios">Factor de redondeo. 1 para decenas, 2 para centenas, 3 para unidad de mil...</param>
        /// <returns></returns>
        public static decimal RedondeaArriba(decimal monto, int espacios)
        {
            //Valido que sea factor válido
            if (espacios < 1)
                return monto;

            //Calculo el factor. Posibles: 10, 100, 1000...
            int factor = Convert.ToInt32(Math.Pow(10D, Convert.ToDouble(espacios)));

            //Hago redondeo
            if (monto % factor == 0) return monto;
            return (factor - monto % factor) + monto;
        }
        /// <summary>
        /// Actualiza los datos de un artículo en los campos asociados al id.
        /// </summary>
        /// <param name="id">ID del artículo.</param>
        /// <param name="articulo">Instancia con datos a refrescar.</param>
        /// <returns></returns>
        public static void ActualizarDatosById(int id, Articulo articulo)
        {
            //Borrar artículo y precios de la BD
            string query = 
                "DELETE FROM c_articulos WHERE id = @Id;" +
                "DELETE FROM c_articulos_precios WHERE idArticulo = @Id;";
            BDCon con = new BDCon(query);
            con.PasarParametros("Id", id);
            con.EjecutarComando();

            //Registrar de nuevo artículo de la BD, con el mismo id
            query =
                "INSERT INTO c_articulos (id,descripcion,precioDolar,costoDolar,codBarras,activo) " +
                "VALUES( @Id, @Descripcion, @PrecioDolar, @CostoDolar, @CodBarras, @Activo )";
            con = new BDCon(query);

            con.PasarParametros("Id", id);
            con.PasarParametros("Descripcion", articulo.Descripcion);
            con.PasarParametros("PrecioDolar", articulo.PrecioDolar);
            con.PasarParametros("CostoDolar", articulo.CostoDolar);
            con.PasarParametros("CodBarras", articulo.CodBarras);
            con.PasarParametros("Activo", true);

            //Registrar el artículo
            con.EjecutarComando();

            //Volver a registrar sus precios
            articulo.RegistraPreciosArticulo();
        }
        /// <summary>
        /// Actualiza los precios de los artículos según la tasa actual.
        /// </summary>
        public static void ActualizarPreciosBBDD()
        {
            //Armando consulta a BBDD.
            string query = "SELECT * FROM c_articulos";
            BDCon con = new BDCon(query);
            SQLiteDataReader dr = con.ComandoSqlite().ExecuteReader();

            #region Variables Auxiliares
            Articulo articulo;
            int id;
            string descripcion, codBarras;
            decimal costoDolar, precioDolar;
            #endregion

            //Mientras hayan artículos leídos 
            while (dr.Read())
            {
                //Obtengo valores para Artículo
                id = Convert.ToInt32(dr["id"]);
                descripcion = (string)dr["descripcion"];
                codBarras   = (string)dr["codBarras"];
                precioDolar = Convert.ToDecimal(dr["precioDolar"]);
                costoDolar  = Convert.ToDecimal(dr["costoDolar"]);

                //Instanciar artículo y guardo sus datos en base de datos
                articulo = new Articulo(descripcion, costoDolar, precioDolar, codBarras);

                //Actualizar del artículo para refrescar su precio
                Articulo.ActualizarDatosById(id, articulo);
            }

            //Cierro para prevenir errores
            dr.Close();
        }
        #endregion
    }
}
