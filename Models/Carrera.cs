using System;
using System.Collections.Generic;

namespace CatalogoDeTrabajosDeGraduacion.Models
{
    public partial class Carrera
    {
        public Carrera()
        {
            Autor = new HashSet<Autor>();
        }

        public int CarreId { get; set; }
        public string CarreNombre { get; set; }
        public string CarreDescripcion { get; set; }
        public int FaculId { get; set; }

        public virtual Facultad Facul { get; set; }
        public virtual ICollection<Autor> Autor { get; set; }
    }
}
