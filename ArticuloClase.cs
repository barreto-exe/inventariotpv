using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventario_y_Contabilidad
{
    public class ArticuloClase
    {
        public string id { get; set; }
        public string descripcion { get; set; }
        public string cantAct { get; set; }
        public string fechaHora { get; set; }
        public string costoDolar { get; set; }
        public string precioDolar { get; set; }
        public string precioBs { get; set; }
        public string precioBsEfect { get; set; }
        public string precioBsRec { get; set; }
        public string precioBsEfectRec { get; set; }
        public string codBarras { get; set; }
        public int LineaRow { get; set; }
    }
}
