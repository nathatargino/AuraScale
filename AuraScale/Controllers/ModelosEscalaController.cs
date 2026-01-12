using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuraScale.Data;
using AuraScale.Models;
using System.Security.Claims;

namespace AuraScale.Controllers
{
    [Authorize]
    public class ModelosEscalaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ModelosEscalaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ModelosEscala
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Lista apenas os modelos criados pelo usuário logado
            return View(await _context.ModelosEscala
                .Where(m => m.GerenteId == userId)
                .ToListAsync());
        }

        // GET: ModelosEscala/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ModelosEscala/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ModeloEscala modelo)
        {
            if (ModelState.IsValid)
            {
                // Vincula ao usuário logado
                modelo.GerenteId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                _context.Add(modelo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(modelo);
        }

        // GET: ModelosEscala/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Busca e garante que o modelo pertence ao usuário logado
            var modelo = await _context.ModelosEscala
                .FirstOrDefaultAsync(m => m.Id == id && m.GerenteId == userId);

            if (modelo == null) return NotFound();

            return View(modelo);
        }

        // POST: ModelosEscala/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ModeloEscala modelo)
        {
            if (id != modelo.Id) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            modelo.GerenteId = userId; // Garante que o ID do gerente não se perca

            // Remove validação do Gerente para não dar erro de ModelState
            ModelState.Remove("Gerente");
            ModelState.Remove("Operadores");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(modelo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModeloExists(modelo.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(modelo);
        }

        // GET: ModelosEscala/Details/1
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var modelo = await _context.ModelosEscala
                .FirstOrDefaultAsync(m => m.Id == id);

            if (modelo == null) return NotFound();

            return View(modelo); // O erro acontece aqui se a View não existir
        }

        // GET: ModelosEscala/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var modelo = await _context.ModelosEscala
                .FirstOrDefaultAsync(m => m.Id == id && m.GerenteId == userId);

            if (modelo == null) return NotFound();

            return View(modelo);
        }

        // POST: ModelosEscala/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var modelo = await _context.ModelosEscala
                 .FirstOrDefaultAsync(m => m.Id == id && m.GerenteId == userId);

            if (modelo != null)
            {
                _context.ModelosEscala.Remove(modelo);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ModeloExists(int id)
        {
            return _context.ModelosEscala.Any(e => e.Id == id);
        }
    }
}