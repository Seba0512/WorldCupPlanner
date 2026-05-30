using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.DominioTests;

[TestClass]
public class EstadioDTOTests
{
    [TestMethod]
    public void ToEntity_CopiaTodasLasPropiedades()
    {
        var dto = new EstadioDTO
        {
            Nombre = "Estadio Centenario",
            Ciudad = "Montevideo",
            Direccion = "Av. Dr. Américo Ricaldoni",
            Capacidad = 60000,
            Descripcion = "Estadio historico de Uruguay"
        };

        var estadio = dto.ToEntity();

        Assert.AreEqual(dto.Nombre, estadio.Nombre);
        Assert.AreEqual(dto.Ciudad, estadio.Ciudad);
        Assert.AreEqual(dto.Direccion, estadio.Direccion);
        Assert.AreEqual(dto.Capacidad, estadio.Capacidad);
        Assert.AreEqual(dto.Descripcion, estadio.Descripcion);
    }

    [TestMethod]
    public void FromEntity_CopiaTodasLasPropiedades()
    {
        var estadio = new Estadio
        {
            Nombre = "Lusail Stadium",
            Ciudad = "Lusail",
            Direccion = "Lusail",
            Capacidad = 88966,
            Descripcion = "Sede de partidos internacionales"
        };

        var dto = EstadioDTO.FromEntity(estadio);

        Assert.AreEqual(estadio.Nombre, dto.Nombre);
        Assert.AreEqual(estadio.Ciudad, dto.Ciudad);
        Assert.AreEqual(estadio.Direccion, dto.Direccion);
        Assert.AreEqual(estadio.Capacidad, dto.Capacidad);
        Assert.AreEqual(estadio.Descripcion, dto.Descripcion);
    }

    [TestMethod]
    public void Constructor_PropiedadesPorDefecto_SonValoresSeguros()
    {
        var dto = new EstadioDTO();

        Assert.AreEqual(string.Empty, dto.Nombre);
        Assert.AreEqual(string.Empty, dto.Ciudad);
        Assert.AreEqual(string.Empty, dto.Direccion);
        Assert.AreEqual(0, dto.Capacidad);
        Assert.IsNull(dto.Descripcion);
    }

    [TestMethod]
    public void ToEntity_ConValoresNulosOpcionales_LosConserva()
    {
        var dto = new EstadioDTO
        {
            Nombre = "Estadio Nacional",
            Ciudad = "Santiago",
            Direccion = null,
            Capacidad = 48000,
            Descripcion = null
        };

        var estadio = dto.ToEntity();

        Assert.IsNull(estadio.Direccion);
        Assert.IsNull(estadio.Descripcion);
    }

    [TestMethod]
    public void FromEntity_ConEntidadNula_LanzaExcepcion()
    {
        Assert.ThrowsException<ArgumentNullException>(() => EstadioDTO.FromEntity(null!));
    }
}
