using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace PruebaTecnica.Models
{
    [Table("TipoDocumento", Schema="dbo")]
    public class dbo_TipoDocumento
    {
        [Key]
        [Column(Order = 0)]
        [Required]
        [Display(Name = "Id")]
        public Int32 Id { get; set; }

        [StringLength(20)]
        [Display(Name = "Descripcion")]
        public String Descripcion { get; set; }

    }
}
 
