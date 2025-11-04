using Microsoft.EntityFrameworkCore;
using RENAPER.Dominio;
using RENAPER.Dominio.Data;

namespace RENAPER.Aplicacion.Services;

public class ApiKeyService : IApiKeyService
{
    private readonly RenaperDbContext _context;

    public ApiKeyService(RenaperDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ValidarApiKeyAsync(string apiKey)
    {
        var apiKeyEntity = await _context.ApiKeys
            .FirstOrDefaultAsync(k => k.Key == apiKey && k.Activa);

        if (apiKeyEntity == null)
            return false;

        // Verificar si es una nueva semana y resetear contador
        var ahora = DateTime.UtcNow;
        var inicioSemanaActual = ahora.Date.AddDays(-(int)ahora.DayOfWeek);

        if (apiKeyEntity.FechaInicioSemana.Date >= inicioSemanaActual)
            return apiKeyEntity.SolicitudesUsadas < apiKeyEntity.SolicitudesSemanales;

        apiKeyEntity.FechaInicioSemana = inicioSemanaActual;
        apiKeyEntity.SolicitudesUsadas = 0;
        await _context.SaveChangesAsync();

        // Verificar si tiene solicitudes disponibles
        return apiKeyEntity.SolicitudesUsadas < apiKeyEntity.SolicitudesSemanales;
    }

    public async Task DescontarSolicitudAsync(string apiKey)
    {
        var apiKeyEntity = await _context.ApiKeys
            .FirstOrDefaultAsync(k => k.Key == apiKey && k.Activa);

        if (apiKeyEntity != null)
        {
            apiKeyEntity.SolicitudesUsadas++;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<string> CrearApiKeyAsync(string mail, int solicitudesSemanales)
    {
        var nuevaApiKey = new ApiKey
        {
            Key = Guid.NewGuid().ToString("N"),
            Mail = mail,
            SolicitudesSemanales = solicitudesSemanales,
            SolicitudesUsadas = 0,
            FechaInicioSemana = DateTime.UtcNow.Date.AddDays(-(int)DateTime.UtcNow.DayOfWeek),
            Activa = true
        };

        _context.ApiKeys.Add(nuevaApiKey);
        await _context.SaveChangesAsync();

        return nuevaApiKey.Key;
    }

    public async Task<bool> AmpliarSolicitudesAsync(string mail)
    {
        var apiKey = await _context.ApiKeys
            .FirstOrDefaultAsync(k => k.Mail == mail && k.Activa);

        if (apiKey == null)
            return false;

        apiKey.SolicitudesSemanales += 10;
        await _context.SaveChangesAsync();

        return true;
    }
}

