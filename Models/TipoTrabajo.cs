using System;
using System.Collections.Generic;

namespace CatalogoDeTrabajosDeGraduacion.Models
{
    public partial class TipoTrabajo
    {
        public TipoTrabajo()
        {
            Trabajo = new HashSet<Trabajo>();
        }

        public int TpTrabaId { get; set; }
        public string TpTrabaNombre { get; set; }
        public string TpTrabaDescripcion { get; set; }

        public virtual ICollection<Trabajo> Trabajo { get; set; }
    }
}
