using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.DominioTests;

[TestClass]
public class ConfiguracionTorneoDTOTests
{
    [TestMethod]
    public void ToEntity_CopiaTodasLasPropiedades()
    {
        var dto = new ConfiguracionTorneoDTO
        {
            SemillaFixture = 12,
            SemillaCompletar = 34,
            SemillaCrucesFase = 56,
            SemillaSimulation = 78,
            FechaInicioTorneo = new DateTime(2026, 7, 15),
            MaxPartidosPorDia = 4,
            SeparacionEntreFechas = 2,
            HoraInicioPartidos = new TimeSpan(18, 30, 0),
            SeparacionHorasEntrePartidos = 5
        };

        var entidad = dto.ToEntity();

        Assert.AreEqual(dto.SemillaFixture, entidad.SemillaFixture);
        Assert.AreEqual(dto.SemillaCompletar, entidad.SemillaCompletar);
        Assert.AreEqual(dto.SemillaCrucesFase, entidad.SemillaCrucesFase);
        Assert.AreEqual(dto.SemillaSimulation, entidad.SemillaSimulation);
        Assert.AreEqual(dto.FechaInicioTorneo, entidad.FechaInicioTorneo);
        Assert.AreEqual(dto.MaxPartidosPorDia, entidad.MaxPartidosPorDia);
        Assert.AreEqual(dto.SeparacionEntreFechas, entidad.SeparacionEntreFechas);
        Assert.AreEqual(dto.HoraInicioPartidos, entidad.HoraInicioPartidos);
        Assert.AreEqual(dto.SeparacionHorasEntrePartidos, entidad.SeparacionHorasEntrePartidos);
    }

    [TestMethod]
    public void FromEntity_CopiaTodasLasPropiedades()
    {
        var entidad = new ConfiguracionTorneo
        {
            SemillaFixture = 91,
            SemillaCompletar = 82,
            SemillaCrucesFase = 73,
            SemillaSimulation = 64,
            FechaInicioTorneo = new DateTime(2026, 8, 20),
            MaxPartidosPorDia = 5,
            SeparacionEntreFechas = 4,
            HoraInicioPartidos = new TimeSpan(15, 45, 0),
            SeparacionHorasEntrePartidos = 6
        };

        var dto = ConfiguracionTorneoDTO.FromEntity(entidad);

        Assert.AreEqual(entidad.SemillaFixture, dto.SemillaFixture);
        Assert.AreEqual(entidad.SemillaCompletar, dto.SemillaCompletar);
        Assert.AreEqual(entidad.SemillaCrucesFase, dto.SemillaCrucesFase);
        Assert.AreEqual(entidad.SemillaSimulation, dto.SemillaSimulation);
        Assert.AreEqual(entidad.FechaInicioTorneo, dto.FechaInicioTorneo);
        Assert.AreEqual(entidad.MaxPartidosPorDia, dto.MaxPartidosPorDia);
        Assert.AreEqual(entidad.SeparacionEntreFechas, dto.SeparacionEntreFechas);
        Assert.AreEqual(entidad.HoraInicioPartidos, dto.HoraInicioPartidos);
        Assert.AreEqual(entidad.SeparacionHorasEntrePartidos, dto.SeparacionHorasEntrePartidos);
    }

    [TestMethod]
    public void FromEntity_ConEntidadNula_LanzaExcepcion()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            ConfiguracionTorneoDTO.FromEntity(null!));
    }
}
