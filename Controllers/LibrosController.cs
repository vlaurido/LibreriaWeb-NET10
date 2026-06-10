
using LibreriaWeb.Data;
using LibreriaWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class LibrosController : Controller
{
    private readonly AppDbContext _context;

    public LibrosController(AppDbContext context)
    {
        _context = context;
    }

    // GET: LIBROS
    public async Task<IActionResult> Index(string searchString, int page = 1)    
    {
        int pageSize = 5; //Número de libros por página

        var libros = _context.Libros
        .Include(l => l.Autor)  //trae los datos del autor del libro, no sólo el Id
        .AsQueryable();  //permite ir agregando filtros dinámicamente

        if (!string.IsNullOrEmpty(searchString))
        {
            libros = libros.Where(l => l.Titulo.Contains(searchString) || l.Autor.Nombre.Contains(searchString));
        }

        int totalLibros = await libros.CountAsync(); //Número total de libros después de aplicar el filtro

        var libros2 = await libros
        .OrderBy(l => l.Titulo)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

        ViewData["CurrentFilter"] = searchString; //para mantener el valor de búsqueda en la vista
        ViewData["CurrentPage"] = page; //para mantener la página actual en la vista
        ViewData["TotalPages"] = (int)Math.Ceiling((double)totalLibros / pageSize); //para mostrar el número total de páginas en la vista

        return View(libros2);
    }

    // GET: LIBROS POR AUTOR
    public async Task<IActionResult> PorAutor(int Id)
    {
        var libros = _context.Libros
        .Include(l => l.Autor)  //trae los datos del autor del libro, no sólo el Id
        .Where(l => l.AutorId == Id);  //filtra por autor

        ViewData["AutorId"] = Id;   //Para poder regresar al autor desde la vista

        ViewData["AutorNombre"] = libros
            .FirstOrDefault()?.Autor?.Nombre;   //Para mostrar el nombre del autor en la vista

        return View(await libros.OrderBy(l => l.Titulo).ToListAsync());
    }

    // GET: LIBROS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var libro = await _context.Libros
            .Include(l => l.Autor)          //Nuevo
            .FirstOrDefaultAsync(m => m.Id == id);
        if (libro == null)
        {
            return NotFound();
        }

        return View(libro);
    }

    // GET: LIBROS/Create
    public IActionResult Create()
    {
        ViewData["AutorId"] = new SelectList(_context.Autores, "Id", "Nombre"); //Nuevo
        return View();
    }

    // POST: LIBROS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Titulo,AnioPublicacion,AutorId,Autor,ImagenArchivo")] Libro libro)
    {
        if (ModelState.IsValid)
        {
            if (libro.ImagenArchivo != null && libro.ImagenArchivo.Length > 0)
            {
                // Procesar el archivo de imagen
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(libro.ImagenArchivo.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagenes", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await libro.ImagenArchivo.CopyToAsync(stream);
                }
                libro.ImagenRuta = "/imagenes/" + fileName;
            }
            _context.Add(libro);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(libro);
    }

    // GET: LIBROS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var libro = await _context.Libros.FindAsync(id);
        if (libro == null)
        {
            return NotFound();
        }

        ViewData["AutorId"] = new SelectList(_context.Autores, "Id", "Nombre"); //Nuevo
        return View(libro);
    }

    // POST: LIBROS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Titulo,AnioPublicacion,AutorId,Autor,ImagenArchivo")] Libro libro)
    {
        if (id != libro.Id)
        {
            return NotFound();
        }

        // Recuperar libro anterior de la base de datos
        var libroExistente = await _context.Libros.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id);
        if (libroExistente == null)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                if (libro.ImagenArchivo != null && libro.ImagenArchivo.Length > 0)
                {
                    // Eliminar la imagen anterior del servidor
                    if (!string.IsNullOrEmpty(libroExistente.ImagenRuta))
                    {
                        var rutaAnterior = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", libroExistente.ImagenRuta.TrimStart('/'));
                        if (System.IO.File.Exists(rutaAnterior))
                        {
                            System.IO.File.Delete(rutaAnterior);
                        }
                    }

                    // Procesar el archivo de imagen
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(libro.ImagenArchivo.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagenes", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await libro.ImagenArchivo.CopyToAsync(stream);
                    }
                    libro.ImagenRuta = "/imagenes/" + fileName;
                }
                _context.Update(libro);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LibroExists(libro.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(libro);
    }

    // GET: LIBROS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var libro = await _context.Libros
            .Include(l => l.Autor)          //Nuevo
            .FirstOrDefaultAsync(m => m.Id == id);
        if (libro == null)
        {
            return NotFound();
        }

        return View(libro);
    }

    // POST: LIBROS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var libro = await _context.Libros.FindAsync(id);
        if (libro != null)
        {
            // Eliminar la imagen del servidor
            if (!string.IsNullOrEmpty(libro.ImagenRuta))
            {
                // Obtener la ruta física en el servidor
                var rutaCompleta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", libro.ImagenRuta.TrimStart('/'));

                if (System.IO.File.Exists(rutaCompleta))
                {
                    System.IO.File.Delete(rutaCompleta);
                }
            }

            _context.Libros.Remove(libro);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool LibroExists(int? id)
    {
        return _context.Libros.Any(e => e.Id == id);
    }
}
