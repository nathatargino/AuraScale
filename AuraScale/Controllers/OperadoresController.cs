using AuraScale.Data;
using AuraScale.Models;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Security.Claims;

namespace AuraScale.Controllers
{
    [Authorize]
    public class OperadoresController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OperadoresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Operadores
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var operadores = await _context.Operadores
                    .Include(o => o.ModeloEscala) 
                    .Where(o => o.GerenteId == userId)
                    .ToListAsync();

            return View(operadores);
        }

        // GET: Operadores/Create
        public IActionResult Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Busca os modelos de escala criados por este gerente
            var modelos = _context.ModelosEscala.Where(m => m.GerenteId == userId).ToList();

            // Passa para a View. O SelectList permite que o campo seja opcional
            ViewData["ModeloEscalaId"] = new SelectList(modelos, "Id", "Nome");

            return View();
        }

        // POST: Operadores/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Email,Funcao,ModeloEscalaId")] Operador operador)
        {
            if (ModelState.IsValid)
            {
                operador.GerenteId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                operador.HorarioEntrada = TimeSpan.FromHours(8);

                _context.Add(operador);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(operador);
        }

        // GET: Operadores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var operador = await _context.Operadores
                .FirstOrDefaultAsync(m => m.Id == id && m.GerenteId == userId);

            if (operador == null) return NotFound();

            var modelos = _context.ModelosEscala.Where(m => m.GerenteId == userId);
            ViewData["ModeloEscalaId"] = new SelectList(modelos, "Id", "Nome", operador.ModeloEscalaId);

            return View(operador);
        }

        // POST: Operadores/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Email,Funcao,ModeloEscalaId,HorarioEntrada")] Operador operador)
        {
            if (id != operador.Id) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            operador.GerenteId = userId;

            ModelState.Remove("Gerente");
            ModelState.Remove("ModeloEscala");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(operador);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OperadorExists(operador.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            var modelos = _context.ModelosEscala.Where(m => m.GerenteId == userId);
            ViewData["ModeloEscalaId"] = new SelectList(modelos, "Id", "Nome", operador.ModeloEscalaId);

            return View(operador);
        }

        // GET: Operadores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var operador = await _context.Operadores
                .Include(o => o.ModeloEscala) 
                .FirstOrDefaultAsync(m => m.Id == id && m.GerenteId == userId);

            if (operador == null) return NotFound();

            return View(operador);
        }

        // GET: Operadores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var operador = await _context.Operadores
                .FirstOrDefaultAsync(m => m.Id == id && m.GerenteId == userId);

            if (operador == null) return NotFound();

            return View(operador);
        }

        // POST: Operadores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var operador = await _context.Operadores
                .FirstOrDefaultAsync(m => m.Id == id && m.GerenteId == userId);

            if (operador != null)
            {
                _context.Operadores.Remove(operador);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Operadores/Importar
        public IActionResult Importar()
        {
            return View();
        }

        // POST: Operadores/Importar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Importar(IFormFile arquivoExcel)
        {
            if (arquivoExcel == null || arquivoExcel.Length == 0)
            {
                ModelState.AddModelError("", "Por favor, selecione um arquivo válido.");
                return View();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int countNovos = 0;

            try
            {
                using (var stream = new MemoryStream())
                {
                    await arquivoExcel.CopyToAsync(stream);
                    using (var workbook = new XLWorkbook(stream))
                    {
                        var worksheet = workbook.Worksheets.First();
                        var rows = worksheet.RangeUsed().RowsUsed().Skip(1);

                        // Carrega modelos existentes em memória para evitar SELECTs repetitivos
                        var modelosCache = await _context.ModelosEscala
                            .Where(m => m.GerenteId == userId)
                            .ToListAsync();

                        foreach (var row in rows)
                        {
                            var nome = row.Cell(1).GetValue<string>();
                            var email = row.Cell(2).GetValue<string>();
                            var funcao = row.Cell(3).GetValue<string>();
                            var entradaStr = row.Cell(4).GetValue<string>();
                            var nomeModeloExcel = row.Cell(5).GetValue<string>()?.Trim();

                            if (string.IsNullOrWhiteSpace(nome)) continue;

                            // 1. Lógica Get or Create para o Modelo de Escala
                            int? modeloId = null;
                            if (!string.IsNullOrEmpty(nomeModeloExcel))
                            {
                                var modelo = modelosCache.FirstOrDefault(m =>
                                    m.Nome.Equals(nomeModeloExcel, StringComparison.OrdinalIgnoreCase));

                                if (modelo == null)
                                {
                                    // Cria um modelo novo "rascunho" se não existir
                                    modelo = new ModeloEscala
                                    {
                                        Nome = "[REVISAR] " + nomeModeloExcel,
                                        GerenteId = userId,
                                        CargaHorariaDiaria = TimeSpan.FromHours(8),
                                        RegraSabado = TipoTrabalhoSabado.NaoTrabalha
                                    };
                                    _context.ModelosEscala.Add(modelo);
                                    await _context.SaveChangesAsync(); 

                                    modelosCache.Add(modelo); // Adiciona no cache da memória
                                }
                                modeloId = modelo.Id;
                            }

                            // 2. Tenta adicionar o Operador
                            var horario = TimeSpan.TryParse(entradaStr, out var t) ? t : TimeSpan.FromHours(8);
                            bool adicionou = await ProcessarOperador(nome, email, funcao, userId, modeloId, horario);

                            if (adicionou) countNovos++;
                        }
                    }
                }

                await _context.SaveChangesAsync();
                TempData["MensagemSucesso"] = $"{countNovos} operadores processados!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Erro ao ler o arquivo: " + ex.Message);
                return View();
            }
        }

        // --- MÉTODO AUXILIAR ---
        private async Task<bool> ProcessarOperador(string nome, string? email, string funcao, string userId, int? modeloId, TimeSpan entrada)
        {
            // Verifica por Email ou Nome para evitar duplicidade
            bool existe = await _context.Operadores
                .AnyAsync(o => (o.Email == email && email != null || o.Nome == nome) && o.GerenteId == userId);

            if (!existe)
            {
                var novoOperador = new Operador
                {
                    Nome = nome,
                    Email = email,
                    Funcao = funcao,
                    GerenteId = userId,
                    ModeloEscalaId = modeloId,
                    HorarioEntrada = entrada
                };
                _context.Add(novoOperador);
                return true;
            }
            return false;
        }

        private bool OperadorExists(int id)
        {
            return _context.Operadores.Any(e => e.Id == id);
        }


        // Método para Planilha Exemplo
        public IActionResult BaixarPlanilhaExemplo()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Modelo Importação");

                // 1. Cabeçalho
                worksheet.Cell(1, 1).Value = "Nome";
                worksheet.Cell(1, 2).Value = "Email";
                worksheet.Cell(1, 3).Value = "Função";
                worksheet.Cell(1, 4).Value = "HorarioEntrada";
                worksheet.Cell(1, 5).Value = "ModeloEscala";

                // 2. Estilo do Cabeçalho (Opcional - Deixa profissional)
                var range = worksheet.Range("A1:E1");
                range.Style.Font.Bold = true;
                range.Style.Fill.BackgroundColor = XLColor.FromHtml("#212529");
                range.Style.Font.FontColor = XLColor.White;

                // 3. Exemplo de preenchimento (Opcional)
                worksheet.Cell(2, 1).Value = "João Silva";
                worksheet.Cell(2, 2).Value = "joao@email.com";
                worksheet.Cell(2, 3).Value = "Suporte";
                worksheet.Cell(2, 4).Value = "08:00";
                worksheet.Cell(2, 5).Value = "Teste";

                worksheet.Columns().AdjustToContents(); 

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "AuraScale_Modelo_Importacao.xlsx");
                }
            }
        }
    }
}