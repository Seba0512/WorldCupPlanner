using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.DominioTests;

[TestClass]
public class EquipoTests
{
    private const int MaxLongitudBandera = 2_800_000;

    [TestMethod]
    public void Equipo_ConNombreVacio_NoEsValido()
    {
        // Arrange
        var equipo = new Equipo();
        equipo.Nombre = "";

        // Act
        var resultado = equipo.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Equipo_ConNombreMasDe60Caracteres_NoEsValido()
    {
        // Arrange
        var equipo = new Equipo();
        equipo.Nombre = new string('A', 61);

        // Act
        var resultado = equipo.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Equipo_ConRankingFIFAMenorA300_NoEsValido()
    {
        // Arrange
        var equipo = new Equipo();
        equipo.Nombre = "Barcelona";
        equipo.RankingFIFA = 299;

        // Act
        var resultado = equipo.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Equipo_ConRankingFIFAMayorA2500_NoEsValido()
    {
        // Arrange
        var equipo = new Equipo();
        equipo.Nombre = "Barcelona";
        equipo.RankingFIFA = 2501;

        // Act
        var resultado = equipo.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }
    
    [TestMethod]
    public void Equipo_SinConfederacion_NoEsValido()
    {
        // Arrange
        var equipo = new Equipo();
        equipo.Nombre = "Barcelona";
        equipo.RankingFIFA = 500;
        equipo.Confederacion = null;

        // Act
        var resultado = equipo.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Equipo_ConDatosValidos_EsValido()
    {
        // Arrange
        var equipo = new Equipo
        {
            Nombre = "Uruguay",
            RankingFIFA = 1000,
            Confederacion = Confederacion.CONMEBOL
        };

        // Act
        var resultado = equipo.EsValido();

        // Assert
        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void Equipo_ConNombreSoloEspacios_NoEsValido()
    {
        // Arrange
        var equipo = new Equipo
        {
            Nombre = "   ",
            RankingFIFA = 500,
            Confederacion = Confederacion.UEFA
        };

        // Act
        var resultado = equipo.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Equipo_ConBanderaVacia_EsValido()
    {
        // Arrange
        var equipo = new Equipo
        {
            Nombre = "Uruguay",
            RankingFIFA = 1000,
            Confederacion = Confederacion.CONMEBOL,
            Bandera = ""
        };

        // Act
        var resultado = equipo.EsValido();

        // Assert
        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void Equipo_ConBanderaEnFormatoDataUriBase64_EsValido()
    {
        // Arrange
        var equipo = new Equipo
        {
            Nombre = "Uruguay",
            RankingFIFA = 1000,
            Confederacion = Confederacion.CONMEBOL,
            Bandera = "data:image/svg+xml;base64,dXJ1Z3VheQ=="
        };

        // Act
        var resultado = equipo.EsValido();

        // Assert
        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void Equipo_ConBanderaQueNoEsDataUri_NoEsValido()
    {
        // Arrange
        var equipo = new Equipo
        {
            Nombre = "Uruguay",
            RankingFIFA = 1000,
            Confederacion = Confederacion.CONMEBOL,
            Bandera = "uruguay.png"
        };

        // Act
        var resultado = equipo.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Equipo_ConBanderaConPrefijoDataPeroSinMarcadorBase64_NoEsValido()
    {
        // Arrange
        var equipo = new Equipo
        {
            Nombre = "Uruguay",
            RankingFIFA = 1000,
            Confederacion = Confederacion.CONMEBOL,
            Bandera = "data:image/svg+xml,dXJ1Z3VheQ=="
        };

        // Act
        var resultado = equipo.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Equipo_ConBanderaQueSuperaElTamanoMaximo_NoEsValido()
    {
        // Arrange — Data URI válido en formato pero excede el tamaño máximo permitido
        var equipo = new Equipo
        {
            Nombre = "Uruguay",
            RankingFIFA = 1000,
            Confederacion = Confederacion.CONMEBOL,
            Bandera = "data:image/svg+xml;base64," + new string('A', MaxLongitudBandera)
        };

        // Act
        var resultado = equipo.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Equipo_ConBanderaEnElLimiteDeTamano_EsValido()
    {
        // Arrange — el campo Bandera queda exactamente en el tamaño máximo permitido
        var prefijo = "data:image/svg+xml;base64,";
        var equipo = new Equipo
        {
            Nombre = "Uruguay",
            RankingFIFA = 1000,
            Confederacion = Confederacion.CONMEBOL,
            Bandera = prefijo + new string('A', MaxLongitudBandera - prefijo.Length)
        };

        // Act
        var resultado = equipo.EsValido();

        // Assert
        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void Equipo_ConBanderaPng_EsValido()
    {
        // Arrange
        var equipo = new Equipo
        {
            Nombre = "Uruguay",
            RankingFIFA = 1000,
            Confederacion = Confederacion.CONMEBOL,
            Bandera = "data:image/png;base64,dXJ1Z3VheQ=="
        };

        // Act
        var resultado = equipo.EsValido();

        // Assert
        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void Equipo_ConBanderaJpeg_EsValido()
    {
        // Arrange
        var equipo = new Equipo
        {
            Nombre = "Uruguay",
            RankingFIFA = 1000,
            Confederacion = Confederacion.CONMEBOL,
            Bandera = "data:image/jpeg;base64,dXJ1Z3VheQ=="
        };

        // Act
        var resultado = equipo.EsValido();

        // Assert
        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void Equipo_ConBanderaWebp_EsValido()
    {
        // Arrange
        var equipo = new Equipo
        {
            Nombre = "Uruguay",
            RankingFIFA = 1000,
            Confederacion = Confederacion.CONMEBOL,
            Bandera = "data:image/webp;base64,dXJ1Z3VheQ=="
        };

        // Act
        var resultado = equipo.EsValido();

        // Assert
        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void Equipo_ConBanderaDeFormatoNoPermitido_NoEsValido()
    {
        // Arrange — bmp no está en la lista de formatos aceptados
        var equipo = new Equipo
        {
            Nombre = "Uruguay",
            RankingFIFA = 1000,
            Confederacion = Confederacion.CONMEBOL,
            Bandera = "data:image/bmp;base64,dXJ1Z3VheQ=="
        };

        // Act
        var resultado = equipo.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }
}
