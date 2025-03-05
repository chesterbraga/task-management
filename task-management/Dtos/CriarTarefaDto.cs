using System.ComponentModel.DataAnnotations;
using task_management.Enums;

namespace task_management.Dtos
{
    public class CriarTarefaDto
    {
        [Required(ErrorMessage = "Título é obrigatório")]
        public string Titulo { get; set; }

        public string Descricao { get; set; }

        [Required(ErrorMessage = "Status é obrigatório")]
        public StatusTarefa Status { get; set; }
    }
}
