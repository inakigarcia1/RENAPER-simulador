using RENAPER.Dominio;

namespace RENAPER.Aplicacion.Services;

public interface IPersonaService
{
    Task<Persona?> ObtenerPorCuilAsync(string cuil);
}

