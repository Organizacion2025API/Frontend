using System.ComponentModel.DataAnnotations;

namespace ApexMagnamentFrontend.DTOs
{
    public class Categorias
    {
        public int Id { get; set; }

        public string NombreCategoria { get; set; } = null;
    }

    public class CrearCategoria
    {
        [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
        public string NombreCategoria { get; set; } = null;
    }
}
