using task_management.Models;

namespace task_management.Caches
{
    public interface ITarefaCacheService
    {
        Task<List<Tarefa>> ObterTarefasPorUsuarioComCacheAsync(int usuarioId);
        Task RemoverCacheTarefasAsync(int usuarioId);
    }
}
