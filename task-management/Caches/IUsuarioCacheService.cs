using task_management.Models;

namespace task_management.Caches
{    
    public interface IUsuarioCacheService
    {
        Task<Usuario> ObterPorIdComCacheAsync(int id);
        Task RemoverCacheUsuarioAsync(int id);
        Task SalvarCacheUsuarioAsync(Usuario usuario);
    }
}