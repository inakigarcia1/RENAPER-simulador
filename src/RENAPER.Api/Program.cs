
using Microsoft.EntityFrameworkCore;
using RENAPER.Api.Filtros;
using RENAPER.Aplicacion.Services;
using RENAPER.Dominio.Data;

namespace RENAPER.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("RenderInternal");
            
            builder.Services.AddDbContext<RenaperDbContext>(options =>
                options.UseNpgsql(connectionString));

            builder.Services.AddScoped<IPersonaService, PersonaService>();
            builder.Services.AddScoped<IApiKeyService, ApiKeyService>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.OperationFilter<ApiKeyHeader>();
            });
			
			builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy =>
        {
            policy.AllowAnyOrigin() // Allows requests from any origin
                  .AllowAnyHeader()   // Allows any headers to be sent
                  .AllowAnyMethod();  // Allows any HTTP methods (GET, POST, PUT, DELETE, etc.)
        });
});

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
			app.UseCors("AllowAllOrigins"); // Apply the named CORS policy

            app.UseAuthorization();

            app.MapControllers();

            // Aplicar migraciones automáticamente (solo para desarrollo)
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<RenaperDbContext>();
                db.Database.EnsureCreated();
            }

            app.Run();
        }
    }
}
