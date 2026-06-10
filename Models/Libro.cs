using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using LibreriaWeb.Models.Validations;

namespace LibreriaWeb.Models
{
    public class Libro
    {
        public int Id { get; set; }
        public required string Titulo { get; set; }

        [DisplayName("Año de Publicación")]
        [NoFuture]  //Validación personalizada
        public int AnioPublicacion { get; set; }

        //Clave foránea y navegación
        [DisplayName("Autor")]
        public int AutorId { get; set; }
        public Autor? Autor { get; set; }

        [DisplayName("Portada")]
        public string? ImagenRuta { get; set; } //Nueva propiedad para almacenar la ruta de la imagen

        [NotMapped] //Indica que esta propiedad no se mapeará a la base de datos
        public IFormFile? ImagenArchivo { get; set; } //Propiedad para recibir el archivo de imagen desde el formulario
    }
}
