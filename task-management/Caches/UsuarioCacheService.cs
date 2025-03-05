using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using task_management.Models;
using task_management.Repositories;

namespace task_management.Caches
{
    public class UsuarioCacheService : IUsuarioCacheService
    {
        private readonly IDistributedCache _cache;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILogger<UsuarioCacheService> _logger;

        // Configurações de cache
        private readonly DistributedCacheEntryOptions _opcoesCache = new DistributedCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(30))  // Expira após 30 minutos de inatividade
            .SetAbsoluteExpiration(TimeSpan.FromHours(2));   // Expira após 2 horas independente de uso

        public UsuarioCacheService(
            IDistributedCache cache,
            IUsuarioRepository usuarioRepository,
            ILogger<UsuarioCacheService> logger)
        {
            _cache = cache;
            _usuarioRepository = usuarioRepository;
            _logger = logger;
        }

        public async Task<Usuario> ObterPorIdComCacheAsync(int id)
        {
            // Chave de cache única para cada usuário
            string chaveCache = $"usuario:{id}";

            try
            {
                // Tentar buscar do cache primeiro
                var usuarioCacheado = await _cache.GetStringAsync(chaveCache);

                if (usuarioCacheado != null)
                {
                    _logger.LogInformation($"Usuário {id} recuperado do cache");
                    return JsonSerializer.Deserialize<Usuario>(usuarioCacheado);
                }

                // Se não está no cache, buscar do repositório
                var usuario = await _usuarioRepository.ObterPorIdAsync(id);

                if (usuario != null)
                {
                    // Salvar no cache
                    await SalvarCacheUsuarioAsync(usuario);
                    _logger.LogInformation($"Usuário {id} salvo no cache");
                }

                return usuario;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao processar cache para usuário {id}");
                // Em caso de erro, retornar direto do repositório
                return await _usuarioRepository.ObterPorIdAsync(id);
            }
        }

        public async Task SalvarCacheUsuarioAsync(Usuario usuario)
        {
            string chaveCache = $"usuario:{usuario.Id}";

            var usuarioSerializado = JsonSerializer.Serialize(usuario);
            await _cache.SetStringAsync(chaveCache, usuarioSerializado, _opcoesCache);
        }

        public async Task RemoverCacheUsuarioAsync(int id)
        {
            string chaveCache = $"usuario:{id}";
            await _cache.RemoveAsync(chaveCache);
        }
    }
}