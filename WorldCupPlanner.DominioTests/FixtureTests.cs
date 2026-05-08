using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.DominioTests;

[TestClass]
public class FixtureTests
{
    [TestMethod]
    public void Fixture_DebeExponerTodosLosPartidosDeSusGrupos()
    {
        // Arrange
        var grupoA = new Grupo("A");
        var grupoB = new Grupo("B");

        grupoA.Partidos.Add(CrearPartido(1));
        grupoA.Partidos.Add(CrearPartido(2));
        grupoB.Partidos.Add(CrearPartido(3));

        var fixture = new Fixture();
        fixture.Grupos.Add(grupoA);
        fixture.Grupos.Add(grupoB);

        // Act
        var partidos = fixture.Partidos;

        // Assert
        Assert.AreEqual(3, partidos.Count);

        var idsObtenidos = partidos.Select(p => p.Id).ToList();
        var idsEsperados = new List<int> { 1, 2, 3 };

        CollectionAssert.AreEqual(idsEsperados, idsObtenidos);
    }

    [TestMethod]
    public void Fixture_SinGrupos_DebeRetornarListaVacia()
    {
        // Arrange
        var fixture = new Fixture();

        // Act
        var partidos = fixture.Partidos;

        // Assert
        Assert.AreEqual(0, partidos.Count);
    }

    private Partido CrearPartido(int id)
    {
        return new Partido
        {
            Id = id,
            Fecha = DateTime.Now,
            Estadio = new Estadio
            {
                Nombre = $"Estadio{id}",
                Ciudad = $"Ciudad{id}",
                Capacidad = 30000
            },
            EquipoLocal = new Equipo
            {
                Nombre = $"Local{id}",
                RankingFIFA = 1000,
                Confederacion = Confederacion.UEFA
            },
            EquipoVisitante = new Equipo
            {
                Nombre = $"Visitante{id}",
                RankingFIFA = 900,
                Confederacion = Confederacion.CONMEBOL
            },
            Fase = FaseTorneo.FaseDeGrupos
        };
    }
}
