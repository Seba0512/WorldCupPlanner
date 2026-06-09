using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.UI.Helpers;

public static class FaseTorneoExtensions
{
    public static string ToEtiqueta(this FaseTorneo fase) => fase switch
    {
        FaseTorneo.FaseDeGrupos             => "Fase de Grupos",
        FaseTorneo.PrimeraRondaEliminatoria => "Primera Ronda",
        FaseTorneo.Octavos                  => "Octavos de Final",
        FaseTorneo.Cuartos                  => "Cuartos de Final",
        FaseTorneo.Semifinal                => "Semifinal",
        FaseTorneo.TercerPuesto             => "Tercer Puesto",
        FaseTorneo.Final                    => "Final",
        _                                   => fase.ToString()
    };
}
