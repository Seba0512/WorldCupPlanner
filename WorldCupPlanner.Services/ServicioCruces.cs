using WorldCupPlanner.Domain;

namespace WorldCupPlanner.Services;

public class ServicioCruces
{
    public List<Partido> GenerarPrimeraRondaEliminatoria(
        ClasificadosFaseGrupos clasificados,
        List<Estadio> estadios,
        ConfiguracionTorneo configuracionTorneo)
    {
        var equiposClasificados = clasificados.Todos;
        var partidos = new List<Partido>();

        for (int i = 0; i < equiposClasificados.Count; i += 2)
        {
            partidos.Add(new Partido
            {
                Id = i / 2 + 1,
                EquipoLocal = equiposClasificados[i],
                EquipoVisitante = equiposClasificados[i + 1],
                Estadio = estadios[(i / 2) % estadios.Count],
                Fecha = configuracionTorneo.FechaInicioTorneo,
                Fase = FaseTorneo.PrimeraRondaEliminatoria
            });
        }

        return partidos;
    }
}