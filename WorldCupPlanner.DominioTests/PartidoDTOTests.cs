using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.DominioTests;

[TestClass]
public class PartidoDTOTests
{
    [TestMethod]
    public void ToEntity_CopiaTodasLasPropiedades()
    {
        var dto = CrearPartidoDTO();
        var grupo = new Grupo("A");

        var partido = dto.ToEntity(grupo: grupo);

        Assert.AreEqual(dto.Id, partido.Id);
        Assert.AreEqual(dto.Fecha, partido.Fecha);
        Assert.AreEqual(dto.Estadio?.Nombre, partido.Estadio?.Nombre);
        Assert.AreEqual(dto.EquipoLocal?.Nombre, partido.EquipoLocal?.Nombre);
        Assert.AreEqual(dto.EquipoVisitante?.Nombre, partido.EquipoVisitante?.Nombre);
        Assert.AreEqual(dto.Fase, partido.Fase);
        Assert.AreEqual(dto.GolesLocal, partido.GolesLocal);
        Assert.AreEqual(dto.GolesVisitante, partido.GolesVisitante);
        Assert.AreEqual(dto.GanadorPorPenales?.Nombre, partido.GanadorPorPenales?.Nombre);
        Assert.AreSame(grupo, partido.Grupo);
    }

    [TestMethod]
    public void ToEntity_SinGrupoExplicito_UsaGrupoEtiqueta()
    {
        var dto = CrearPartidoDTO();

        var partido = dto.ToEntity();

        Assert.IsNotNull(partido.Grupo);
        Assert.AreEqual(dto.GrupoEtiqueta, partido.Grupo.Etiqueta);
    }

    [TestMethod]
    public void FromEntity_CopiaTodasLasPropiedades()
    {
        var partido = CrearPartido();

        var dto = PartidoDTO.FromEntity(partido);

        Assert.AreEqual(partido.Id, dto.Id);
        Assert.AreEqual(partido.Fecha, dto.Fecha);
        Assert.AreEqual(partido.Estadio?.Nombre, dto.Estadio?.Nombre);
        Assert.AreEqual(partido.EquipoLocal?.Nombre, dto.EquipoLocal?.Nombre);
        Assert.AreEqual(partido.EquipoVisitante?.Nombre, dto.EquipoVisitante?.Nombre);
        Assert.AreEqual(partido.Fase, dto.Fase);
        Assert.AreEqual(partido.GolesLocal, dto.GolesLocal);
        Assert.AreEqual(partido.GolesVisitante, dto.GolesVisitante);
        Assert.AreEqual(partido.GanadorPorPenales?.Nombre, dto.GanadorPorPenales?.Nombre);
        Assert.AreEqual(partido.Grupo?.Etiqueta, dto.GrupoEtiqueta);
    }

    [TestMethod]
    public void Constructor_PropiedadesPorDefecto_SonValoresSeguros()
    {
        var dto = new PartidoDTO();

        Assert.AreEqual(0, dto.Id);
        Assert.AreEqual(default, dto.Fecha);
        Assert.IsNull(dto.Estadio);
        Assert.IsNull(dto.EquipoLocal);
        Assert.IsNull(dto.EquipoVisitante);
        Assert.AreEqual(default, dto.Fase);
        Assert.IsNull(dto.GolesLocal);
        Assert.IsNull(dto.GolesVisitante);
        Assert.IsNull(dto.GanadorPorPenales);
        Assert.IsNull(dto.GrupoEtiqueta);
    }

    [TestMethod]
    public void FromEntity_ConEntidadNula_LanzaExcepcion()
    {
        Assert.ThrowsException<ArgumentNullException>(() => PartidoDTO.FromEntity(null!));
    }

    private static PartidoDTO CrearPartidoDTO()
    {
        return PartidoDTO.FromEntity(CrearPartido());
    }

    private static Partido CrearPartido()
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

        return new Partido
        {
            Id = 5,
            Fecha = new DateTime(2026, 6, 3, 18, 0, 0),
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
    }
}
