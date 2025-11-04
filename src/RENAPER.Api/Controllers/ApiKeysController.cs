using Microsoft.AspNetCore.Mvc;
using RENAPER.Aplicacion.Services;
using System.ComponentModel.DataAnnotations;

namespace RENAPER.Api.Controllers;

[ApiController]
[Route("api/keys")]
public class ApiKeysController : ControllerBase
{
    private readonly IApiKeyService _apiKeyService;

    public ApiKeysController(IApiKeyService apiKeyService)
    {
        _apiKeyService = apiKeyService;
    }

    [HttpPost]
    public async Task<ActionResult> CrearApiKey([FromBody] CrearApiKeyRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Mail))
        {
            return BadRequest(new { error = "El mail es requerido" });
        }

        if (request.SolicitudesSemanales <= 0)
        {
            return BadRequest(new { error = "Las solicitudes semanales deben ser mayor a 0" });
        }

        if (!new EmailAddressAttribute().IsValid(request.Mail))
        {
            return BadRequest(new { error = "El formato del mail no es válido" });
        }

        var apiKey = await _apiKeyService.CrearApiKeyAsync(request.Mail, request.SolicitudesSemanales);

        return Ok(new { apiKey, mail = request.Mail, solicitudesSemanales = request.SolicitudesSemanales });
    }

    [HttpPost("ampliar")]
    public async Task<ActionResult> AmpliarSolicitudes([FromBody] AmpliarSolicitudesRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Mail))
        {
            return BadRequest(new { error = "El mail es requerido" });
        }

        if (!new EmailAddressAttribute().IsValid(request.Mail))
        {
            return BadRequest(new { error = "El formato del mail no es válido" });
        }

        var resultado = await _apiKeyService.AmpliarSolicitudesAsync(request.Mail);

        if (!resultado)
        {
            return NotFound(new { error = $"No se encontró una API key activa para el mail: {request.Mail}" });
        }

        return Ok(new { mensaje = "Se agregaron 10 solicitudes semanales exitosamente" });
    }
}

public class CrearApiKeyRequest
{
    public string Mail { get; set; } = string.Empty;
    public int SolicitudesSemanales { get; set; }
}

public class AmpliarSolicitudesRequest
{
    public string Mail { get; set; } = string.Empty;
}

