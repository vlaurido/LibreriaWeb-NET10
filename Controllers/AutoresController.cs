
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibreriaWeb.Models;
using LibreriaWeb.Data;

public class AutoresController : Controller
{
    private readonly AppDbContext _context;

    public AutoresController(AppDbContext context)
    {
        _context = context;
    }

    // GET: AUTORS
    public async Task<IActionResult> Index(string searchString)
    {
        var autores = _context.Autores
            .Include(a => a.Libros) //Incluir libros
            .AsQueryable();   //permite ir agregando filtros dinámicamente

        if (!string.IsNullOrEmpty(searchString))
        {
            autores = autores.Where(a => a.Nombre.Contains(searchString));
        }

        ViewData["CurrentFilter"] = searchString; //para mantener el valor de búsqueda en la vista

        return View(await autores.OrderBy(a => a.Nombre).ToListAsync());
    }

    // GET: AUTORS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var autor = await _context.Autores
            .FirstOrDefaultAsync(m => m.Id == id);
        if (autor == null)
        {
            return NotFound();
        }

        return View(autor);
    }

    // GET: AUTORS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: AUTORS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Nombre,Libros")] Autor autor)
    {
        if (ModelState.IsValid)
        {
            _context.Add(autor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(autor);
    }

    // GET: AUTORS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var autor = await _context.Autores.FindAsync(id);
        if (autor == null)
        {
            return NotFound();
        }
        return View(autor);
    }

    // POST: AUTORS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Nombre,Libros")] Autor autor)
    {
        if (id != autor.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(autor);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AutorExists(autor.Id))
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
        return View(autor);
    }

    // GET: AUTORS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var autor = await _context.Autores
            .FirstOrDefaultAsync(m => m.Id == id);
        if (autor == null)
        {
            return NotFound();
        }

        return View(autor);
    }

    // POST: AUTORS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var autor = await _context.Autores.FindAsync(id);
        if (autor != null)
        {
            _context.Autores.Remove(autor);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool AutorExists(int? id)
    {
        return _context.Autores.Any(e => e.Id == id);
    }
}
