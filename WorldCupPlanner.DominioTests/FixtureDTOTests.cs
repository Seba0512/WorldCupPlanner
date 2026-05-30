using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.DominioTests;

[TestClass]
public class FixtureDTOTests
{
    [TestMethod]
    public void FromEntity_CopiaGruposEquiposYPartidos()
    {
        var fixture = CrearFixtureConPartidoJugado();

        var dto = FixtureDTO.FromEntity(fixture);

        Assert.AreEqual(1, dto.Grupos.Count);
        Assert.AreEqual("A", dto.Grupos[0].Etiqueta);
        Assert.AreEqual(2, dto.Grupos[0].Equipos.Count);
        Assert.AreEqual(1, dto.Grupos[0].Partidos.Count);
        Assert.AreEqual("Uruguay", dto.Grupos[0].Partidos[0].EquipoLocal?.Nombre);
        Assert.AreEqual("Argentina", dto.Grupos[0].Partidos[0].EquipoVisitante?.Nombre);
        Assert.AreEqual("Uruguay", dto.Grupos[0].Partidos[0].GanadorPorPenales?.Nombre);
    }

    [TestMethod]
    public void ToEntity_CopiaGruposEquiposYPartidos()
    {
        var dto = CrearFixtureDTOConPartidoJugado();

        var fixture = dto.ToEntity();

        Assert.AreEqual(1, fixture.Grupos.Count);
        Assert.AreEqual("A", fixture.Grupos[0].Etiqueta);
        Assert.AreEqual(2, fixture.Grupos[0].Equipos.Count);
        Assert.AreEqual(1, fixture.Grupos[0].Partidos.Count);
        Assert.AreEqual(10, fixture.Grupos[0].Partidos[0].Id);
        Assert.AreEqual(2, fixture.Grupos[0].Partidos[0].GolesLocal);
        Assert.AreEqual(2, fixture.Grupos[0].Partidos[0].GolesVisitante);
        Assert.AreEqual("Uruguay", fixture.Grupos[0].Partidos[0].GanadorPorPenales?.Nombre);
    }

    [TestMethod]
    public void ToEntity_ReutilizaInstanciasDeEquiposEntreGrupoYPartidos()
    {
        var dto = CrearFixtureDTOConPartidoJugado();

        var fixture = dto.ToEntity();

        var grupo = fixture.Grupos[0];
        var partido = grupo.Partidos[0];

        Assert.AreSame(grupo.Equipos[0], partido.EquipoLocal);
        Assert.AreSame(grupo.Equipos[1], partido.EquipoVisitante);
        Assert.AreSame(grupo.Equipos[0], partido.GanadorPorPenales);
        Assert.AreSame(grupo, partido.Grupo);
    }

    [TestMethod]
    public void Partidos_DevuelveLosPartidosDeTodosLosGrupos()
    {
        var dto = new FixtureDTO
        {
            Grupos =
            [
                new GrupoDTO
                {
                    Etiqueta = "A",
                    Partidos =
                    [
                        new PartidoDTO { Id = 1 },
                        new PartidoDTO { Id = 2 }
                    ]
                },
                new GrupoDTO
                {
                    Etiqueta = "B",
                    Partidos =
                    [
                        new PartidoDTO { Id = 3 }
                    ]
                }
            ]
        };

        var ids = dto.Partidos.Select(partido => partido.Id).ToList();

        CollectionAssert.AreEqual(new List<int> { 1, 2, 3 }, ids);
    }

    [TestMethod]
    public void Constructor_PropiedadesPorDefecto_SonValoresSeguros()
    {
        var dto = new FixtureDTO();

        Assert.IsNotNull(dto.Grupos);
        Assert.AreEqual(0, dto.Grupos.Count);
        Assert.AreEqual(0, dto.Partidos.Count);
    }

    [TestMethod]
    public void FromEntity_ConEntidadNula_LanzaExcepcion()
    {
        Assert.ThrowsException<ArgumentNullException>(() => FixtureDTO.FromEntity(null!));
    }

    [TestMethod]
    public void GrupoDTO_FromEntity_ConEntidadNula_LanzaExcepcion()
    {
        Assert.ThrowsException<ArgumentNullException>(() => GrupoDTO.FromEntity(null!));
    }

    [TestMethod]
    public void PartidoDTO_FromEntity_ConEntidadNula_LanzaExcepcion()
    {
        Assert.ThrowsException<ArgumentNullException>(() => PartidoDTO.FromEntity(null!));
    }

    [TestMethod]
    public void PartidoDTO_ToEntity_ConReferenciasNulas_LasConserva()
    {
        var dto = new PartidoDTO
        {
            Id = 4,
            Fecha = new DateTime(2026, 6, 1),
            Fase = FaseTorneo.FaseDeGrupos
        };

        var partido = dto.ToEntity();

        Assert.IsNull(partido.Estadio);
        Assert.IsNull(partido.EquipoLocal);
        Assert.IsNull(partido.EquipoVisitante);
        Assert.IsNull(partido.GanadorPorPenales);
        Assert.IsNull(partido.Grupo);
    }

    private static Fixture CrearFixtureConPartidoJugado()
    {
        var uruguay = new Equipo
        {
            Nombre = "Uruguay",
            RankingFIFA = 1800,
            Confederacion = Confederacion.CONMEBOL
        };
        var argentina = new Equipo
        {
            Nombre = "Argentina",
            RankingFIFA = 1900,
            Confederacion = Confederacion.CONMEBOL
        };
        var grupo = new Grupo("A", [uruguay, argentina]);
        var partido = new Partido
        {
            Id = 10,
            Fecha = new DateTime(2026, 6, 1, 14, 0, 0),
            Estadio = new Estadio
            {
                Nombre = "Estadio Centenario",
                Ciudad = "Montevideo",
                Capacidad = 60000
            },
            EquipoLocal = uruguay,
            EquipoVisitante = argentina,
            Fase = FaseTorneo.FaseDeGrupos,
            GolesLocal = 2,
            GolesVisitante = 2,
            GanadorPorPenales = uruguay,
            Grupo = grupo
        };
        grupo.Partidos.Add(partido);

        return new Fixture
        {
            Grupos = [grupo]
        };
    }

    private static FixtureDTO CrearFixtureDTOConPartidoJugado()
    {
        return FixtureDTO.FromEntity(CrearFixtureConPartidoJugado());
    }
}
