using task_management.Models;

namespace task_management.Repositories
{
    public interface ITarefaRepository
    {
        Task<Tarefa> AdicionarAsync(Tarefa tarefa);
        Task<List<Tarefa>> ObterTarefasPorUsuarioAsync(int usuarioId);
    }
}
