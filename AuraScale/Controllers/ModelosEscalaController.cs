using AuraScale.Data;
using AuraScale.Models;
using AuraScale.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AuraScale.Controllers
{
    [Authorize]
    public class ModelosEscalaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EscalaService _escalaService;

        public ModelosEscalaController(ApplicationDbContext context, EscalaService escalaService)
        {
            _context = context;
            _escalaService = escalaService;
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

            return View(modelo); 
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
                .Include(m => m.Operadores)
                .FirstOrDefaultAsync(m => m.Id == id && m.GerenteId == userId);

            if (modelo != null)
            {
                foreach (var op in modelo.Operadores)
                {
                    op.ModeloEscalaId = null;
                }

                _context.ModelosEscala.Remove(modelo);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Modelo e vínculos removidos com sucesso!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ModeloExists(int id)
        {
            return _context.ModelosEscala.Any(e => e.Id == id);
        }

        // POST: ModelosEscala/GerarEscala

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GerarEscala(int mes, int ano)
        {
            ModelState.Clear();
            // 1. Captura o ID do Gerente Logado 
            var gerenteId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 2. Validação de Segurança: Se o ID for nulo, força o login
            if (string.IsNullOrEmpty(gerenteId))
            {
                return Challenge();
            }

            try
            {
                // 3. Chama o serviço para processar a lógica 6x1
                await _escalaService.GerarEscalaMensal(gerenteId, mes, ano);

                // Usando TempData para exibir o feedback na Index.cshtml
                TempData["Success"] = $"Sucesso! A escala de {mes}/{ano} foi gerada para sua equipe.";
            }
            catch (DbUpdateException dbEx)
            {
                // Trata especificamente erros de Banco de Dados
                TempData["Error"] = "Erro de banco de dados: Certifique-se de que seus operadores estão vinculados corretamente ao seu perfil.";
                // Log do erro interno para debug: dbEx.InnerException?.Message
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocorreu um erro inesperado: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}