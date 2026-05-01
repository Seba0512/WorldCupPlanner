using WorldCupPlanner.Domain;
using WorldCupPlanner.Services;

namespace WorldCupPlanner.Tests;

[TestClass]
public class ServicioClasificacionTests
{
    [TestMethod]
    public void CalcularTabla_ConResultadosDeGrupo_CalculaPuntosCorrectamente()
    {
        // Arrange
        var servicioClasificacion = new ServicioClasificacion();

        var equipo1 = CrearEquipo("Equipo1");
        var equipo2 = CrearEquipo("Equipo2");
        var equipo3 = CrearEquipo("Equipo3");
        var equipo4 = CrearEquipo("Equipo4");

        var grupo = new Grupo("A", new List<Equipo>
        {
            equipo1,
            equipo2,
            equipo3,
            equipo4
        });

        grupo.Partidos.Add(CrearPartido(equipo1, equipo4, 2, 0)); // Equipo1 gana
        grupo.Partidos.Add(CrearPartido(equipo2, equipo3, 1, 1)); // Empate
        grupo.Partidos.Add(CrearPartido(equipo1, equipo3, 0, 0)); // Empate
        grupo.Partidos.Add(CrearPartido(equipo2, equipo4, 3, 1)); // Equipo2 gana
        grupo.Partidos.Add(CrearPartido(equipo1, equipo2, 1, 0)); // Equipo1 gana
        grupo.Partidos.Add(CrearPartido(equipo3, equipo4, 2, 1)); // Equipo3 gana

        // Act
        var tabla = servicioClasificacion.CalcularTabla(grupo);

        // Assert
        Assert.AreEqual(7, ObtenerPuntos(tabla, equipo1));
        Assert.AreEqual(4, ObtenerPuntos(tabla, equipo2));
        Assert.AreEqual(5, ObtenerPuntos(tabla, equipo3));
        Assert.AreEqual(0, ObtenerPuntos(tabla, equipo4));
    }

    private Equipo CrearEquipo(string nombre)
    {
        return new Equipo
        {
            Nombre = nombre,
            RankingFIFA = 1000,
            Confederacion = Confederacion.UEFA
        };
    }

    private Partido CrearPartido(Equipo local, Equipo visitante, int golesLocal, int golesVisitante)
    {
        return new Partido
        {
            EquipoLocal = local,
            EquipoVisitante = visitante,
            GolesLocal = golesLocal,
            GolesVisitante = golesVisitante,
            Fase = FaseTorneo.FaseDeGrupos,
            Estadio = new Estadio
            {
                Nombre = "Estadio",
                Ciudad = "Ciudad",
                Capacidad = 30000
            },
            Fecha = DateTime.Now
        };
    }

    private int ObtenerPuntos(List<PosicionEquipo> tabla, Equipo equipo)
    {
        return tabla.Single(p => p.Equipo == equipo).Puntos;
    }
}