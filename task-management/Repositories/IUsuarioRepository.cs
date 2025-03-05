using task_management.Models;

namespace task_management.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario> ObterPorIdAsync(int id);
        Task<List<Usuario>> ListarTodosAsync();
        Task<Usuario> AdicionarAsync(Usuario usuario);
        Task<bool> EmailExisteAsync(string email);
    }
}
