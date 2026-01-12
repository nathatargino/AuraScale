using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace AuraScale.Models
{
    public enum TipoTrabalhoSabado
    {
        [Display(Name = "Não Trabalha")]
        NaoTrabalha = 0,
        [Display(Name = "Todos os Sábados")]
        TodosOsSabados = 1,
        [Display(Name = "Sábado Sim, Sábado Não")]
        SabadoSimSabadoNao = 2,
        [Display(Name = "Apenas Um por Mês")]
        ApenasUmPorMes = 3
    }
    public class ModeloEscala
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Dê um nome ao modelo (ex: Administrativo)")]
        [Display(Name = "Nome do Modelo")]
        public string Nome { get; set; } = string.Empty;

        [Display(Name = "Regra de Sábado")]
        public TipoTrabalhoSabado RegraSabado { get; set; } = TipoTrabalhoSabado.TodosOsSabados;

        [Required]
        [Display(Name = "Carga Horária (Seg-Sex)")]
        [DataType(DataType.Time)]
        public TimeSpan CargaHorariaDiaria { get; set; }

        [Display(Name = "Carga Horária Sábado")]
        [DataType(DataType.Time)]
        public TimeSpan? CargaHorariaSabado { get; set; } // Ex: 04:00 (Para o seu caso de 5x2 com HE)

        [Display(Name = "Trabalha Domingo?")]
        public bool TrabalhaDomingo { get; set; }

        // Vincula ao gerente para que cada empresa tenha seus próprios modelos
        public string GerenteId { get; set; } = string.Empty;
        public IdentityUser? Gerente { get; set; }

        public List<Operador> Operadores { get; set; } = new();
    }
}