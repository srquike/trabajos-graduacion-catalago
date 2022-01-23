using System;
using System.Collections.Generic;

namespace CatalogoDeTrabajosDeGraduacion.Models
{
    public partial class Facultad
    {
        public Facultad()
        {
            Carrera = new HashSet<Carrera>();
        }

        public int FaculId { get; set; }
        public string FaculNombre { get; set; }
        public string FaculDescripcion { get; set; }

        public virtual ICollection<Carrera> Carrera { get; set; }
    }
}
