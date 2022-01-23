using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CatalogoDeTrabajosDeGraduacion.Models
{
    public partial class Autor
    {
        public int AutorId { get; set; }
        [DisplayName("Nombre")]
        [Required(ErrorMessage = "El nombre es requerido.")]
        public string AutorNombre { get; set; }
        [DisplayName("Apellido")]
        [Required(ErrorMessage = "El apellido es requerido.")]
        public string AutorApellido { get; set; }
        public int TrabaId { get; set; }
        [DisplayName("Carrera")]
        public int CarreId { get; set; }

        public virtual Carrera Carre { get; set; }
        public virtual Trabajo Traba { get; set; }
    }
}
