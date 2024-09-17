using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionAPI.Models
{
    public class Participante
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50, MinimumLength = 3)]
        [Required(ErrorMessage = "El nombre del participante es requerido")]
        [Display(Name = "Nombre de Participante")]
        public string NombreParticipante { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Debe ser un formato de correo electrónico válido.")]
        public string CorreoParticipante { get; set; }

        //evento asociado
        [Required]
        [ForeignKey("Evento")]
        public int EventoId { get; set; }

        //propiedad de navegación
        public Evento Evento { get; set; }
    }
}
