namespace LibreriaWeb.Models
{
    public class Autor
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public ICollection<Libro>? Libros { get; set; }

    }
}
