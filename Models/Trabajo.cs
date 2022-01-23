using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CatalogoDeTrabajosDeGraduacion.Models
{
    public partial class Trabajo
    {
        public Trabajo()
        {
            Autores = new HashSet<Autor>();
        }

        [DisplayName("Id")]
        public int TrabaId { get; set; }
        [DisplayName("Titulo")]
        [Required(ErrorMessage = "El titulo es requerido.")]
        public string TrabaTitulo { get; set; }
        [DisplayName("Fecha")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime TrabaFecha { get; set; }
        [DisplayName("Tipo de trabajo")]

        public string TrabaFile { get; set; }
        [DisplayName("Tipo de trabajo")]
        public int TpTrabaId { get; set; }

        [DisplayName("Tipo de trabajo")]
        public virtual TipoTrabajo TpTraba { get; set; }
        public virtual ICollection<Autor> Autores { get; set; }
    }
}
