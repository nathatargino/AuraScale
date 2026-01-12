using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AuraScale.Models;
using System.Diagnostics;
using AuraScale.Data;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace AuraScale.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return View();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            // 1. Verifica se ele JÁ É um Operador vinculado pelo ID
            var ehColaborador = await _context.Operadores.AnyAsync(o => o.UsuarioId == userId);

            // 2. Se não for pelo ID, tenta vincular agora pelo EMAIL 
            if (!ehColaborador && !string.IsNullOrEmpty(userEmail))
            {
                var operadorPendente = await _context.Operadores
                    .FirstOrDefaultAsync(o => o.Email == userEmail && o.UsuarioId == null);

                if (operadorPendente != null)
                {
                    // Faz o vínculo automático agora!
                    operadorPendente.UsuarioId = userId;
                    _context.Update(operadorPendente);
                    await _context.SaveChangesAsync();
                    ehColaborador = true;
                }
            }

            // --- REDIRECIONAMENTO ---

            if (ehColaborador)
            {
                // Rota do Colaborador (Minha Escala)
                return RedirectToAction("Index", "Colaborador");
            }
            else
            {
                // Rota do Gerente (Dashboard)
                return RedirectToAction("Dashboard");
            }
        }

        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            bool ehColaborador = await _context.Operadores.AnyAsync(o => o.UsuarioId == userId);
            if (ehColaborador) return RedirectToAction("Index", "Colaborador");

            var hoje = DateTime.Today;

            // Estatísticas Simplificadas
            ViewBag.TotalOperadores = await _context.Operadores.CountAsync(o => o.GerenteId == userId);
            ViewBag.TotalModelos = await _context.ModelosEscala.CountAsync(m => m.GerenteId == userId);
            ViewBag.TrabalhandoHoje = await _context.Escalas
                .CountAsync(e => e.GerenteId == userId && e.Data.Date == hoje);
            ViewBag.FolgaHoje = (int)ViewBag.TotalOperadores - (int)ViewBag.TrabalhandoHoje;

            ViewBag.TemVinculos = await _context.Operadores
                .AnyAsync(o => o.GerenteId == userId && o.ModeloEscalaId != null);

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}