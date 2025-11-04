using Microsoft.AspNetCore.Mvc;
using RENAPER.Api.Attributes;
using RENAPER.Aplicacion.Services;
using RENAPER.Dominio;

namespace RENAPER.Api.Controllers;

[ApiController]
[Route("api/personas")]
public class PersonasController : ControllerBase
{
    private readonly IPersonaService _personaService;
    private readonly IApiKeyService _apiKeyService;

    public PersonasController(IPersonaService personaService, IApiKeyService apiKeyService)
    {
        _personaService = personaService;
        _apiKeyService = apiKeyService;
    }

    [HttpGet("por-cuil/{cuil}")]
    [RequiresApiKey]
    public async Task<ActionResult<Persona>> ObtenerPorCuil(string cuil)
    {
        if (!Request.Headers.TryGetValue("X-API-Key", out var apiKeyHeader))
        {
            return Unauthorized(new { error = "API Key requerida. Envíe la cabecera X-API-Key" });
        }

        var apiKey = apiKeyHeader.ToString();
        var isValid = await _apiKeyService.ValidarApiKeyAsync(apiKey);

        if (!isValid)
        {
            return Unauthorized(new { error = "API Key inválida o sin solicitudes disponibles" });
        }

        var persona = await _personaService.ObtenerPorCuilAsync(cuil);

        if (persona == null)
        {
            return NotFound(new { error = $"No se encontró una persona con CUIL: {cuil}" });
        }

        await _apiKeyService.DescontarSolicitudAsync(apiKey);

        return Ok(persona);
    }
}

