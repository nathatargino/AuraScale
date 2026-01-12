namespace AuraScale.Models.ViewModels
{
    public class HomeViewModel
    {
        // Contadores para exibir números
        public int TotalEscalas { get; set; }
        public int TotalOperadores { get; set; }

        // Booleanos para controlar o "Check"
        public bool TemEscalasCadastradas { get; set; }
        public bool TemOperadoresCadastrados { get; set; }
        public bool TemVinculosRealizados { get; set; }
    }
}