using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuraScale.Models
{
    public class Operador
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [Display(Name = "Nome do Operador")]
        public string Nome { get; set; } = string.Empty;

        [Display(Name = "Função")]
        public string? Funcao { get; set; }
        [Display(Name = "E-mail")]
        [EmailAddress]
        public string? Email { get; set; }

        public string? UsuarioId { get; set; }

        [Display(Name = "Modelo de Escala")]
        public int? ModeloEscalaId { get; set; } // Chave Estrangeira

        [ForeignKey("ModeloEscalaId")]
        public virtual ModeloEscala? ModeloEscala { get; set; } // Navegação

        [Display(Name = "Horário de Entrada")]
        [DataType(DataType.Time)]
        public TimeSpan HorarioEntrada { get; set; }

        // --- Relacionamento com o Gerente (Dono do registro) ---
        public string? GerenteId { get; set; } = string.Empty;
        public IdentityUser? Gerente { get; set; }

        // --- Relacionamento com as Escalas ---
        public List<Escala> Escalas { get; set; } = new();
    }
}