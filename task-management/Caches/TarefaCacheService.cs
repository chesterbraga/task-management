using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using task_management.Models;
using task_management.Repositories;

namespace task_management.Caches
{
    public class TarefaCacheService : ITarefaCacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ITarefaRepository _tarefaRepository;
        private readonly ILogger<TarefaCacheService> _logger;

        private readonly DistributedCacheEntryOptions _opcoesCache = new DistributedCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(20))
            .SetAbsoluteExpiration(TimeSpan.FromHours(1));

        public TarefaCacheService(
            IDistributedCache cache,
            ITarefaRepository tarefaRepository,
            ILogger<TarefaCacheService> logger)
        {
            _cache = cache;
            _tarefaRepository = tarefaRepository;
            _logger = logger;
        }

        public async Task<List<Tarefa>> ObterTarefasPorUsuarioComCacheAsync(int usuarioId)
        {
            string chaveCache = $"tarefas:{usuarioId}";

            try
            {
                var tarefasCacheadas = await _cache.GetStringAsync(chaveCache);

                if (tarefasCacheadas != null)
                {
                    _logger.LogInformation($"Tarefas do usuário {usuarioId} recuperadas do cache");
                    return JsonSerializer.Deserialize<List<Tarefa>>(tarefasCacheadas);
                }

                var tarefas = await _tarefaRepository.ObterTarefasPorUsuarioAsync(usuarioId);

                if (tarefas != null && tarefas.Any())
                {
                    var tarefasSerializadas = JsonSerializer.Serialize(tarefas);
                    await _cache.SetStringAsync(chaveCache, tarefasSerializadas, _opcoesCache);
                    _logger.LogInformation($"Tarefas do usuário {usuarioId} salvas no cache");
                }

                return tarefas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao processar cache para tarefas do usuário {usuarioId}");
                return await _tarefaRepository.ObterTarefasPorUsuarioAsync(usuarioId);
            }
        }

        public async Task RemoverCacheTarefasAsync(int usuarioId)
        {
            string chaveCache = $"tarefas:{usuarioId}";
            await _cache.RemoveAsync(chaveCache);
        }
    }
}