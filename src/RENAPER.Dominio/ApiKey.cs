namespace RENAPER.Dominio;

public class ApiKey
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Mail { get; set; } = string.Empty;
    public int SolicitudesSemanales { get; set; }
    public int SolicitudesUsadas { get; set; }
    public DateTime FechaInicioSemana { get; set; }
    public bool Activa { get; set; } = true;
}

