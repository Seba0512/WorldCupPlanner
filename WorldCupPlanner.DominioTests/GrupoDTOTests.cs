using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.DominioTests;

[TestClass]
public class GrupoDTOTests
{
    [TestMethod]
    public void ToEntity_CopiaEtiquetaEquiposYPartidos()
    {
        var dto = CrearGrupoDTOConPartido();

        var grupo = dto.ToEntity();

        Assert.AreEqual("A", grupo.Etiqueta);
        Assert.AreEqual(2, grupo.Equipos.Count);
        Assert.AreEqual(1, grupo.Partidos.Count);
        Assert.AreEqual("Uruguay", grupo.Equipos[0].Nombre);
        Assert.AreSame(grupo, grupo.Partidos[0].Grupo);
    }

    [TestMethod]
    public void FromEntity_CopiaEtiquetaEquiposYPartidos()
    {
        var grupo = CrearGrupoConPartido();

        var dto = GrupoDTO.FromEntity(grupo);

        Assert.AreEqual(grupo.Etiqueta, dto.Etiqueta);
        Assert.AreEqual(2, dto.Equipos.Count);
        Assert.AreEqual(1, dto.Partidos.Count);
        Assert.AreEqual("Argentina", dto.Equipos[1].Nombre);
        Assert.AreEqual("A", dto.Partidos[0].GrupoEtiqueta);
    }

    [TestMethod]
    public void Constructor_PropiedadesPorDefecto_SonValoresSeguros()
    {
        var dto = new GrupoDTO();

        Assert.AreEqual(string.Empty, dto.Etiqueta);
        Assert.IsNotNull(dto.Equipos);
        Assert.IsNotNull(dto.Partidos);
        Assert.AreEqual(0, dto.Equipos.Count);
        Assert.AreEqual(0, dto.Partidos.Count);
    }

    [TestMethod]
    public void FromEntity_ConEntidadNula_LanzaExcepcion()
    {
        Assert.ThrowsException<ArgumentNullException>(() => GrupoDTO.FromEntity(null!));
    }

    private static GrupoDTO CrearGrupoDTOConPartido()
    {
        return GrupoDTO.FromEntity(CrearGrupoConPartido());
    }

    private static Grupo CrearGrupoConPartido()
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
        grupo.Partidos.Add(new Partido
        {
            Id = 1,
            Fecha = new DateTime(2026, 6, 1),
            EquipoLocal = uruguay,
            EquipoVisitante = argentina,
            Fase = FaseTorneo.FaseDeGrupos,
            Grupo = grupo
        });

        return grupo;
    }
}
