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

        public async Task<IActionResult> Index()
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
                // Procura um operador com esse email que ainda não tenha "dono"
                var operadorDisponivel = await _context.Operadores
                    .FirstOrDefaultAsync(o => o.Email == userEmail && o.UsuarioId == null);

                if (operadorDisponivel != null)
                {
                    // ACHOU! Faz o vínculo agora.
                    operadorDisponivel.UsuarioId = userId;
                    _context.Update(operadorDisponivel);
                    await _context.SaveChangesAsync();

                    operador = operadorDisponivel; // Define para exibir na tela
                }
            }

            // 4. Retorna a View
            if (operador == null)
            {
                // Se chegou aqui, é um usuário comum, não é operador cadastrado por gerente
                return View("NaoEncontrado");
            }

            return View(operador);
        }
    }
}