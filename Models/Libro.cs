using System.ComponentModel;

namespace LibreriaWeb.Models
{
    public class Libro
    {
        public int Id { get; set; }
        public required string Titulo { get; set; }
        [DisplayName("Año de Publicación")]
        public int AnioPublicacion { get; set; }

        //Clave foránea y navegación
        [DisplayName("Autor")]
        public int AutorId { get; set; }
        public Autor? Autor { get; set; }
    }
}
