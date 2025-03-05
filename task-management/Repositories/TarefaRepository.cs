using task_management.Contexts;
using task_management.Models;
using Microsoft.EntityFrameworkCore;

namespace task_management.Repositories
{
    public class TarefaRepository : ITarefaRepository
    {
        private readonly TaskManagementContext _context;

        public TarefaRepository(TaskManagementContext context)
        {
            _context = context;
        }

        public async Task<Tarefa> AdicionarAsync(Tarefa tarefa)
        {
            _context.Tarefas.Add(tarefa);
            await _context.SaveChangesAsync();
            return tarefa;
        }

        public async Task<List<Tarefa>> ObterTarefasPorUsuarioAsync(int usuarioId)
        {
            return await _context.Tarefas
                .Where(t => t.UsuarioId == usuarioId)
                .ToListAsync();
        }
    }
}