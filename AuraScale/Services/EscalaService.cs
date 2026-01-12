using AuraScale.Data;
using AuraScale.Models;
using Microsoft.EntityFrameworkCore;

namespace AuraScale.Services
{
    public class EscalaService
    {
        private readonly ApplicationDbContext _context;

        public EscalaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task GerarEscalaMensal(string gerenteId, int mes, int ano)
        {
            // 1. LIMPEZA: Remove escalas automáticas existentes para este mês antes de gerar novas
            var escalasExistentes = await _context.Escalas
                .Where(e => e.GerenteId == gerenteId &&
                            e.Data.Month == mes &&
                            e.Data.Year == ano &&
                            e.Observacao.Contains("Automática"))
                .ToListAsync();

            if (escalasExistentes.Any())
            {
                _context.Escalas.RemoveRange(escalasExistentes);
                await _context.SaveChangesAsync();
            }

            var operadores = await _context.Operadores
                .Where(o => o.GerenteId == gerenteId)
                .ToListAsync();

            foreach (var operador in operadores)
            {
                // 2. CONTINUIDADE: Busca a última folga nos últimos 7 dias do mês anterior
                var dataInicioMes = new DateTime(ano, mes, 1);
                var dataLimiteBusca = dataInicioMes.AddDays(-7);

                var ultimaFolga = await _context.Escalas
                    .Where(e => e.OperadorId == operador.Id &&
                                e.Data < dataInicioMes &&
                                e.Data >= dataLimiteBusca &&
                                e.Entrada == TimeSpan.Zero)
                    .OrderByDescending(e => e.Data)
                    .FirstOrDefaultAsync();

                // Calcula quantos dias ele já trabalhou desde a última folga
                int contadorDias = 0;
                if (ultimaFolga != null)
                {
                    contadorDias = (dataInicioMes - ultimaFolga.Data).Days - 1;
                }

                var diasNoMes = DateTime.DaysInMonth(ano, mes);

                for (int dia = 1; dia <= diasNoMes; dia++)
                {
                    var dataAtual = new DateTime(ano, mes, dia);
                    contadorDias++;

                    if (contadorDias == 7)
                    {
                        var folga = new Escala
                        {
                            OperadorId = operador.Id,
                            Data = dataAtual,
                            Entrada = TimeSpan.Zero,
                            Saida = TimeSpan.Zero,
                            Observacao = "Folga Automática (Continuidade 6x1)",
                            GerenteId = gerenteId
                        };

                        _context.Escalas.Add(folga);
                        contadorDias = 0;
                    }
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}