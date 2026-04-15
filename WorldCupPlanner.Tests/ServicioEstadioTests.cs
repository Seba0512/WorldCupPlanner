using WorldCupPlanner.Domain;
using WorldCupPlanner.Services;

[TestClass]
public class ServicioEstadioTests
{
    private ServicioEstadio _servicio;

    [TestInitialize]
    public void Setup()
    {
        _servicio = new ServicioEstadio();
    }

    [TestMethod]
    public void AgregarEstadio_EstadioValido_SeAgregaCorrectamente()
    {
        // Arrange
        var estadio = new Estadio
        {
            Nombre = "Estadio Centenario",
            Ciudad = "Montevideo",
            Capacidad = 60000
        };

        // Act
        _servicio.AgregarEstadio(estadio);

        // Assert
        Assert.AreEqual(1, _servicio.ObtenerEstadios().Count);
    }
    
    [TestMethod]
    public void AgregarEstadio_NombreDuplicado_LanzaExcepcion()
    {
        // Arrange
        var estadio1 = new Estadio
        {
            Nombre = "Estadio Centenario",
            Ciudad = "Montevideo",
            Capacidad = 60000
        };
        var estadio2 = new Estadio
        {
            Nombre = "Estadio Centenario",
            Ciudad = "Montevideo",
            Capacidad = 40000
        };

        // Act
        _servicio.AgregarEstadio(estadio1);

        // Assert
        Assert.ThrowsException<Exception>(() => _servicio.AgregarEstadio(estadio2));
    }
    
    [TestMethod]
    public void AgregarEstadio_EstadioInvalido_LanzaExcepcion()
    {
        // Arrange
        var estadio = new Estadio
        {
            Nombre = "",
            Ciudad = "Montevideo",
            Capacidad = 60000
        };

        // Act yAssert
        Assert.ThrowsException<Exception>(() => _servicio.AgregarEstadio(estadio));
    }
    
    [TestMethod]
    public void EliminarEstadio_EstadioExistente_SeEliminaCorrectamente()
    {
        // Arrange
        var estadio = new Estadio
        {
            Nombre = "Estadio Centenario",
            Ciudad = "Montevideo",
            Capacidad = 60000
        };
        _servicio.AgregarEstadio(estadio);

        // Act
        _servicio.EliminarEstadio(estadio);

        // Assert
        Assert.AreEqual(0, _servicio.ObtenerEstadios().Count);
    }
}