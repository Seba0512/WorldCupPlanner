using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.UI.Helpers;

public static class EtapasTorneoHelper
{
    public record EtapaTorneo(FaseTorneo Fase, string Etiqueta);

    public static readonly List<EtapaTorneo> EtapasDisponibles = new()
    {
        new(FaseTorneo.FaseDeGrupos,              "Fase de grupos"),
        new(FaseTorneo.PrimeraRondaEliminatoria,  "16avos"),
        new(FaseTorneo.Octavos,                   "8avos"),
        new(FaseTorneo.Cuartos,                   "4tos"),
        new(FaseTorneo.Semifinal,                 "Semifinal"),
        new(FaseTorneo.Final,                     "Final"),
    };
}
