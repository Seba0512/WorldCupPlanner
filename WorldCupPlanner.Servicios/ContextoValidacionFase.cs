using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Servicios;

public class ContextoValidacionFase
{
    public int CantidadEsperada { get; set; }
    public FaseTorneo FaseEsperada { get; set; }
    public string FaseAGenerar { get; set; } = string.Empty;
    public string FaseAnterior { get; set; } = string.Empty;
    public string? FaseAnteriorEsperada { get; set; }
}
