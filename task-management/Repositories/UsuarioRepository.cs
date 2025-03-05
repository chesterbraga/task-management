using Microsoft.EntityFrameworkCore;
using task_management.Contexts;
using task_management.Models;

namespace task_management.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly TaskManagementContext _context;

        public UsuarioRepository(TaskManagementContext context)
        {
            _context = context;
        }

        public async Task<Usuario> ObterPorIdAsync(int id)
        {
            return await _context.Usuarios
                .Include(u => u.Tarefas)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<Usuario>> ListarTodosAsync()
        {
            return await _context.Usuarios.ToListAsync();
        }

        public async Task<Usuario> AdicionarAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task<bool> EmailExisteAsync(string email)
        {
            return await _context.Usuarios.AnyAsync(u => u.Email == email);
        }
    }
}
