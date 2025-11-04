using Microsoft.EntityFrameworkCore;
using RENAPER.Dominio;
using RENAPER.Dominio.Data;

namespace RENAPER.Aplicacion.Services;

public class PersonaService : IPersonaService
{
    private readonly RenaperDbContext _context;

    public PersonaService(RenaperDbContext context)
    {
        _context = context;
    }

    public async Task<Persona?> ObtenerPorCuilAsync(string cuil)
    {
        return await _context.Personas
            .FirstOrDefaultAsync(p => p.CUIL == cuil);
    }
}

