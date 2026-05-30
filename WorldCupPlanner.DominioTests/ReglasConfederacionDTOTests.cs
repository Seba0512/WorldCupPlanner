using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.DominioTests;

[TestClass]
public class ReglasConfederacionDTOTests
{
    [TestMethod]
    public void FromEntity_CopiaTodosLosCuposDeConfederacion()
    {
        var dto = ReglasConfederacionDTO.FromEntity();

        Assert.AreEqual(ReglasConfederacion.CuposPorConfederacion.Count, dto.CuposPorConfederacion.Count);

        foreach (var cupo in ReglasConfederacion.CuposPorConfederacion)
        {
            Assert.IsTrue(dto.CuposPorConfederacion.Any(dtoCupo =>
                dtoCupo.Confederacion == cupo.Key &&
                dtoCupo.Cupos == cupo.Value));
        }
    }

    [TestMethod]
    public void ToEntity_DevuelveDiccionarioConTodosLosCupos()
    {
        var dto = ReglasConfederacionDTO.FromEntity();

        var cupos = dto.ToEntity();

        Assert.AreEqual(16, cupos[Confederacion.UEFA]);
        Assert.AreEqual(7, cupos[Confederacion.CONMEBOL]);
        Assert.AreEqual(7, cupos[Confederacion.CONCACAF]);
        Assert.AreEqual(9, cupos[Confederacion.CAF]);
        Assert.AreEqual(8, cupos[Confederacion.AFC]);
        Assert.AreEqual(1, cupos[Confederacion.OFC]);
    }

    [TestMethod]
    public void Constructor_PropiedadesPorDefecto_SonValoresSeguros()
    {
        var dto = new ReglasConfederacionDTO();

        Assert.IsNotNull(dto.CuposPorConfederacion);
        Assert.AreEqual(0, dto.CuposPorConfederacion.Count);
    }

    [TestMethod]
    public void CupoConfederacionDTO_Propiedades_SePuedenTransportar()
    {
        var dto = new CupoConfederacionDTO
        {
            Confederacion = Confederacion.AFC,
            Cupos = 8
        };

        Assert.AreEqual(Confederacion.AFC, dto.Confederacion);
        Assert.AreEqual(8, dto.Cupos);
    }
}
