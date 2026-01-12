using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuraScale.Data;
using AuraScale.Models;
using System.Security.Claims;

namespace AuraScale.Controllers
{
    [Authorize]
    public class EscalasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EscalasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1.  TELA ESCALAS (Visualização Semanal)
        public async Task<IActionResult> Index(DateTime? dataRef)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 1. Define a semana (Segunda a Domingo)
            var dataBase = dataRef ?? DateTime.Today;

            // Lógica para achar a Segunda-feira anterior
            int diff = dataBase.DayOfWeek - DayOfWeek.Monday;
            if (diff < 0) diff += 7; // Ajuste para Domingo

            var inicioSemana = dataBase.AddDays(-diff).Date; // Segunda-feira
            var fimSemana = inicioSemana.AddDays(6).Date;    // Domingo

            // 2. Busca as escalas APENAS dessa semana
            var escalas = await _context.Escalas
                .Where(e => e.GerenteId == userId && e.Data >= inicioSemana && e.Data <= fimSemana)
                .ToListAsync();

            // 3. Busca TODOS os operadores (para mostrar linha mesmo se estiver de folga a semana toda)
            var operadores = await _context.Operadores
                .Where(o => o.GerenteId == userId)
                .OrderBy(o => o.Nome)
                .ToListAsync();

            // 4. Passa tudo para a View
            ViewBag.InicioSemana = inicioSemana;
            ViewBag.FimSemana = fimSemana;
            ViewBag.Operadores = operadores;

            return View(escalas);
        }

        // --- 2. TELA DE CONFIGURAÇÃO  ---
        public IActionResult Configurar()
        {
            return View();
        }

        // --- 3. O MOTOR DE GERAÇÃO ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Gerar(DateTime dataInicio, DateTime dataFim)
        {
            if (dataFim < dataInicio)
            {
                ModelState.AddModelError("", "A data final deve ser maior que a inicial.");
                return View("Configurar");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // A) Busca os operadores do gerente que já têm modelo definido
            var operadores = await _context.Operadores
                .Include(o => o.ModeloEscala)
                .Where(o => o.GerenteId == userId && o.ModeloEscalaId != null)
                .ToListAsync();

            if (!operadores.Any())
            {
                TempData["Erro"] = "Nenhum operador com modelo de escala configurado encontrado. Vá em 'Minha Equipe' e edite seus funcionários.";
                return View("Configurar");
            }

            // B) Limpa escalas antigas nesse período para não duplicar
            var escalasAntigas = _context.Escalas
            .Where(e => e.Data >= dataInicio && e.Data <= dataFim && e.GerenteId == userId);

            _context.Escalas.RemoveRange(escalasAntigas);
            await _context.SaveChangesAsync();

            // C) Loop dia a dia
            int totalGerado = 0;

            for (var dia = dataInicio; dia <= dataFim; dia = dia.AddDays(1))
            {
                foreach (var op in operadores)
                {
                    // Pule operadores sem modelo vinculado para evitar NullReferenceException
                    if (op.ModeloEscala == null) continue;

                    if (DeveTrabalhar(op.ModeloEscala, dia))
                    {
                        var novaEscala = new Escala
                        {
                            Data = dia,
                            OperadorId = op.Id,
                            GerenteId = userId,
                            Entrada = op.HorarioEntrada,
                            // O método CalcularSaida que corrigimos antes já lida com o novo Enum
                            Saida = Escala.CalcularSaida(op.HorarioEntrada, dia, op.ModeloEscala)
                        };

                        _context.Add(novaEscala);
                        totalGerado++;
                    }
                }
            }

            await _context.SaveChangesAsync();

            TempData["Mensagem"] = $"Sucesso! {totalGerado} turnos gerados para o período.";
            return RedirectToAction(nameof(Index));
        }

        private bool DeveTrabalhar(ModeloEscala modelo, DateTime data)
        {
            var diaSemana = data.DayOfWeek;

            // Regra de Domingo (Booleana)
            if (diaSemana == DayOfWeek.Sunday) return modelo.TrabalhaDomingo;

            // Regra de Sábado (Baseada no Enum RegraSabado)
            if (diaSemana == DayOfWeek.Saturday)
            {
                switch (modelo.RegraSabado)
                {
                    case TipoTrabalhoSabado.NaoTrabalha:
                        return false;
                    case TipoTrabalhoSabado.TodosOsSabados:
                        return true;
                    case TipoTrabalhoSabado.SabadoSimSabadoNao:
                        // Lógica de paridade de semana
                        int semanaDoAno = System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                            data, System.Globalization.DateTimeFormatInfo.CurrentInfo.CalendarWeekRule, DayOfWeek.Sunday);
                        return (semanaDoAno % 2 == 0);
                    case TipoTrabalhoSabado.ApenasUmPorMes:
                        return (data.Day <= 7);
                    default:
                        return false;
                }
            }

            // Segunda a Sexta trabalha sempre por padrão
            return true;
        }

        // --- 4. EXCLUIR UMA ESCALA ESPECÍFICA ---
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var escala = await _context.Escalas
                .Include(e => e.Operador)
                .FirstOrDefaultAsync(m => m.Id == id && m.GerenteId == userId);

            if (escala == null) return NotFound();

            return View(escala);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var escala = await _context.Escalas
             .FirstOrDefaultAsync(e => e.Id == id && e.GerenteId == userId);

            if (escala != null && escala.GerenteId == userId)
            {
                _context.Escalas.Remove(escala);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}