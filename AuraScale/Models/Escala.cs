using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuraScale.Models
{
    public class Escala
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Data { get; set; }

        [Display(Name = "Entrada")]
        public TimeSpan Entrada { get; set; }

        [Display(Name = "Saída")]
        public TimeSpan Saida { get; set; }

        public string? Observacao { get; set; }

        // Relacionamentos
        public int OperadorId { get; set; }
        public Operador? Operador { get; set; }

        public string? GerenteId { get; set; }


        public static TimeSpan CalcularSaida(TimeSpan entrada, DateTime data, ModeloEscala modelo)
        {
            var diaSemana = data.DayOfWeek;

            // Regra de Domingo
            if (diaSemana == DayOfWeek.Sunday)
            {
                if (!modelo.TrabalhaDomingo) return TimeSpan.Zero; // Folga
                return entrada.Add(modelo.CargaHorariaDiaria);
            }

            // Regra de Sábado 
            if (diaSemana == DayOfWeek.Saturday)
            {
                bool trabalhaNesteSabado = false;

                switch (modelo.RegraSabado)
                {
                    case TipoTrabalhoSabado.NaoTrabalha:
                        trabalhaNesteSabado = false;
                        break;

                    case TipoTrabalhoSabado.TodosOsSabados:
                        trabalhaNesteSabado = true;
                        break;

                    case TipoTrabalhoSabado.SabadoSimSabadoNao:
                        int semanaDoAno = System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                            data, System.Globalization.DateTimeFormatInfo.CurrentInfo.CalendarWeekRule, DayOfWeek.Sunday);
                        trabalhaNesteSabado = (semanaDoAno % 2 == 0);
                        break;

                    case TipoTrabalhoSabado.ApenasUmPorMes:
                        trabalhaNesteSabado = (data.Day <= 7);
                        break;
                }

                if (!trabalhaNesteSabado) return TimeSpan.Zero; // Folga

                var carga = (modelo.CargaHorariaSabado != TimeSpan.Zero)
                            ? (modelo.CargaHorariaSabado ?? modelo.CargaHorariaDiaria)
                            : modelo.CargaHorariaDiaria;

                return entrada.Add(carga);
            }

            // Segunda a Sexta
            return entrada.Add(modelo.CargaHorariaDiaria);
        }
    }
}