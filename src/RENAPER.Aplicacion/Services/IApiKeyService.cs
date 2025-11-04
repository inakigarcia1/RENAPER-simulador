namespace RENAPER.Aplicacion.Services;

public interface IApiKeyService
{
    Task<bool> ValidarApiKeyAsync(string apiKey);
    Task DescontarSolicitudAsync(string apiKey);
    Task<string> CrearApiKeyAsync(string mail, int solicitudesSemanales);
    Task<bool> AmpliarSolicitudesAsync(string mail);
}

