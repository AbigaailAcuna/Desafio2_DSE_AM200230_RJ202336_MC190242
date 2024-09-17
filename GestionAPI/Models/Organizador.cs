using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionAPI.Models
{
    public class Organizador
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50, MinimumLength = 3)]
        [Required(ErrorMessage = "El nombre del organizador es requerido")]
        [Display(Name = "Nombre de Organizador")]
        public string NombreOrganizador { get; set; }

        [StringLength(50, MinimumLength = 3)]
        [Required(ErrorMessage = "El cargo del organizador es requerido")]
        [Display(Name = "Cargo de Participante")]
        public string CargoOrganizador { get; set; }

        //evento asociado

        [Required]
        [ForeignKey("Evento")]
        public int? EventoId { get; set; }


        //propiedad de navegación
      // public Evento? Evento { get; set; }
    }
}
