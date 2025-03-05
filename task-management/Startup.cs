using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using task_management.Caches;
using task_management.Contexts;
using task_management.Repositories;
using Microsoft.Extensions.Caching.StackExchangeRedis;


namespace task_management
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Configuração do Entity Framework
            services.AddDbContext<TaskManagementContext>(options =>
                options.UseInMemoryDatabase("TaskManagementDb")
            );

            // Injeção de Dependência
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<ITarefaRepository, TarefaRepository>();

            // Configuração do Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Task Management API",
                    Version = "v1"
                });
            });

            // Configurações do Controllers e JSON
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler =
                        System.Text.Json.Serialization.ReferenceHandler.Preserve;
                });

            // Autenticação JWT (Básico)
            var key = Encoding.ASCII.GetBytes("ChaveSecretaDeSegurancaMuitoLongaESecurity");

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            // Configuração do Redis
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "localhost:6379"; // Configuração padrão do Redis
                options.InstanceName = "TaskManagementCache_";
            });

            // Registrar serviços de cache
            services.AddScoped<IUsuarioCacheService, UsuarioCacheService>();
            services.AddScoped<ITarefaCacheService, TarefaCacheService>();
        }

        public void Configure(IApplicationBuilder app)
        {
            // Middleware para desenvolvimento
            app.UseDeveloperExceptionPage();

            // Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Management API v1")
            );

            // Roteamento e endpoints
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}