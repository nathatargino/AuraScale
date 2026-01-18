using AuraScale.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AuraScale.Controllers
{
    [Authorize] 
    public class ColaboradorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ColaboradorController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(DateTime? dataRef)
        {
            // 1. Pega os dados do usuário que acabou de logar
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            // 2. Verifica se ele já está vinculado a um Operador
            var operador = await _context.Operadores
                .Include(o => o.ModeloEscala) // Pega a Escala vinculada
                .FirstOrDefaultAsync(o => o.UsuarioId == userId);

            // 3. Se não achou pelo ID, TENTA VINCULAR PELO EMAIL
            if (operador == null && !string.IsNullOrEmpty(userEmail))
            {
                // Procura um operador com esse email (mesmo que já tenha UsuarioId)
                var operadorPorEmail = await _context.Operadores
                    .Include(o => o.ModeloEscala)
                    .FirstOrDefaultAsync(o => o.Email == userEmail);

                if (operadorPorEmail != null)
                {
                    // ACHOU! Faz o vínculo agora.
                    operadorPorEmail.UsuarioId = userId;
                    _context.Update(operadorPorEmail);
                    await _context.SaveChangesAsync();

                    operador = operadorPorEmail; // Define para exibir na tela
                }
            }

            // 4. Retorna a View
            if (operador == null)
            {
                // Se chegou aqui, é um usuário comum, não é operador cadastrado por gerente
                return View("NaoEncontrado");
            }

            // 5. Busca as escalas do operador para a semana atual
            var dataBase = dataRef ?? DateTime.Today;

            // Lógica para achar a Segunda-feira anterior
            int diff = dataBase.DayOfWeek - DayOfWeek.Monday;
            if (diff < 0) diff += 7; // Ajuste para Domingo

            var inicioSemana = dataBase.AddDays(-diff).Date; // Segunda-feira
            var fimSemana = inicioSemana.AddDays(6).Date;    // Domingo

            // Busca as escalas do operador para essa semana
            var escalas = await _context.Escalas
                .Where(e => e.OperadorId == operador.Id && e.Data >= inicioSemana && e.Data <= fimSemana)
                .OrderBy(e => e.Data)
                .ToListAsync();

            // Passa os dados para a View
            ViewBag.Escalas = escalas;
            ViewBag.InicioSemana = inicioSemana;
            ViewBag.FimSemana = fimSemana;

            return View(operador);
        }
    }
}