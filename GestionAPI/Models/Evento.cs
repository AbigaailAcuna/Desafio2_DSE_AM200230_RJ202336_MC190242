using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionAPI.Models
{
    public class Evento
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100, MinimumLength = 5)]
        [Required(ErrorMessage = "El nombre del evento es requerido")]
        [Display(Name = "Nombre de Evento")]
        public string NombreEvento { get; set; }

        [Required(ErrorMessage = "La fecha del evento es requerido")]
        [Display(Name = "Fecha del Evento")]
        [DataType(DataType.Date)]
        public DateTime FechaEvento { get; set; }

        [StringLength(100, MinimumLength = 5)]
        [Required(ErrorMessage = "El lugar del evento es requerido")]
        [Display(Name = "Lugar de Evento")]
        public string LugarEvento { get; set; }

        //propiedad de navegación
        public ICollection<Participante> Participantes { get; set; }
        public ICollection<Organizador> Organizadores { get; set; }


    }
}
