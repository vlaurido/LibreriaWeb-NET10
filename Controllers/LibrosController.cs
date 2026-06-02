
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
    public async Task<IActionResult> Index(string searchString)    
    {
        var libros = _context.Libros
        .Include(l => l.Autor)  //trae los datos del autor del libro, no sólo el Id
        .AsQueryable();  //permite ir agregando filtros dinámicamente

        if (!string.IsNullOrEmpty(searchString))
        {
            libros = libros.Where(l => l.Titulo.Contains(searchString) || l.Autor.Nombre.Contains(searchString));
        }

        ViewData["CurrentFilter"] = searchString; //para mantener el valor de búsqueda en la vista

        return View(await libros.OrderBy(l => l.Titulo).ToListAsync());
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
    public async Task<IActionResult> Create([Bind("Id,Titulo,AnioPublicacion,AutorId,Autor")] Libro libro)
    {
        if (ModelState.IsValid)
        {
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
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Titulo,AnioPublicacion,AutorId,Autor")] Libro libro)
    {
        if (id != libro.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
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
