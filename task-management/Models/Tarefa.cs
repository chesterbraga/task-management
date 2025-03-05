using System.ComponentModel.DataAnnotations;
using task_management.Enums;

namespace task_management.Models
{
    public class Tarefa
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Título é obrigatório")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Título deve ter entre 3 e 200 caracteres")]
        public string Titulo { get; set; }

        public string Descricao { get; set; }

        public StatusTarefa Status { get; set; }

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
    }
}
