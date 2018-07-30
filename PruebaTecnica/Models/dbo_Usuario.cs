using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace PruebaTecnica.Models
{
    [Table("Usuario", Schema="dbo")]
    public class dbo_Usuario
    {
        [Key]
        [Column(Order = 0)]
        [Required]
        [Display(Name = "Usuario")]
        public Int32 IdUsuario { get; set; }

        [StringLength(80)]
        [Display(Name = "Nombre")]
        public String Nombre { get; set; }

        [StringLength(100)]
        [Display(Name = "Direccion")]
        public String Direccion { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha Nacimiento")]
        public DateTime? FechaNacimiento { get; set; }

        [Display(Name = "Ciudad Nacimiento")]
        public Int32? IdCiudadNacimiento { get; set; }

        [Display(Name = "Tipo Documento")]
        public Int32? IdTipoDocumento { get; set; }

        [StringLength(20)]
        [Display(Name = "Numero Documento")]
        public String NumeroDocumento { get; set; }

        [Display(Name = "Ciudad Documento")]
        public Int32? IdCiudad { get; set; }

        // ComboBox
        public virtual dbo_Ciudad dbo_Ciudad { get; set; }
        public virtual dbo_TipoDocumento dbo_TipoDocumento { get; set; }
    }
}
 
