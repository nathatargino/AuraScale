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

            // 1. Verifica se ele J� � um Operador vinculado pelo ID
            var ehColaborador = await _context.Operadores.AnyAsync(o => o.UsuarioId == userId);

            // 2. Se n�o for pelo ID, tenta vincular agora pelo EMAIL 
            if (!ehColaborador && !string.IsNullOrEmpty(userEmail))
            {
                var operadorPorEmail = await _context.Operadores
                    .FirstOrDefaultAsync(o => o.Email == userEmail);

                if (operadorPorEmail != null)
                {
                    // Faz o v�nculo autom�tico agora!
                    operadorPorEmail.UsuarioId = userId;
                    _context.Update(operadorPorEmail);
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
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            // Verifica se é colaborador pelo ID
            bool ehColaborador = await _context.Operadores.AnyAsync(o => o.UsuarioId == userId);
            
            // Se não for pelo ID, verifica pelo email
            if (!ehColaborador && !string.IsNullOrEmpty(userEmail))
            {
                var operadorPorEmail = await _context.Operadores
                    .FirstOrDefaultAsync(o => o.Email == userEmail);

                if (operadorPorEmail != null)
                {
                    operadorPorEmail.UsuarioId = userId;
                    _context.Update(operadorPorEmail);
                    await _context.SaveChangesAsync();
                    ehColaborador = true;
                }
            }

            if (ehColaborador) return RedirectToAction("Index", "Colaborador");

            var hoje = DateTime.Today;

            // Estat�sticas Simplificadas
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