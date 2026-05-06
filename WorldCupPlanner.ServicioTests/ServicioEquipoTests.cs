using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public class ServicioEquipoTests
{
    private ServicioEquipo _servicio;

    [TestInitialize]
    public void Setup()
    {
        _servicio = new ServicioEquipo();
    }

    [TestMethod]
    public void AgregarEquipo_EquipoValido_SeAgregaCorrectamente()
    {
        // Arrange
        var equipo = new Equipo
        {
            Nombre = "Uruguay",
            RankingFIFA = 1000,
            Confederacion = Confederacion.CONMEBOL
        };

        // Act
        _servicio.AgregarEquipo(equipo);

        // Assert
        Assert.AreEqual(1, _servicio.ObtenerEquipos().Count);
    }

    [TestMethod]
    public void AgregarEquipo_NombreDuplicado_LanzaExcepcion()
    {
        // Arrange
        var equipo1 = new Equipo
        {
            Nombre = "Uruguay",
            RankingFIFA = 1000,
            Confederacion = Confederacion.CONMEBOL
        };
        var equipo2 = new Equipo
        {
            Nombre = "Uruguay",
            RankingFIFA = 800,
            Confederacion = Confederacion.CONMEBOL
        };

        // Act
        _servicio.AgregarEquipo(equipo1);

        // Assert
        Assert.ThrowsException<EntidadYaExistenteExcepcion>(() => _servicio.AgregarEquipo(equipo2));
    }
    
    [TestMethod]
    public void AgregarEquipo_EquipoInvalido_LanzaExcepcion()
    {
        // Arrange
        var equipo = new Equipo
        {
            Nombre = "",
            RankingFIFA = 1000,
            Confederacion = Confederacion.CONMEBOL
        };

        // Act y1 Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() => _servicio.AgregarEquipo(equipo));
    }
    
    [TestMethod]
    public void EliminarEquipo_EquipoExistente_SeEliminaCorrectamente()
    {
        // Arrange
        var equipo = new Equipo
        {
            Nombre = "Uruguay",
            RankingFIFA = 1000,
            Confederacion = Confederacion.CONMEBOL
        };
        _servicio.AgregarEquipo(equipo);

        // Act
        _servicio.EliminarEquipo(equipo);

        // Assert
        Assert.AreEqual(0, _servicio.ObtenerEquipos().Count);
    }
    
    [TestMethod]
    public void EditarEquipo_EquipoInvalido_LanzaExcepcion()
    {
        // Arrange
        var equipo = new Equipo
        {
            Nombre = "Uruguay",
            RankingFIFA = 1000,
            Confederacion = Confederacion.CONMEBOL
        };
        _servicio.AgregarEquipo(equipo);

        // Act
        equipo.Nombre = "";

        // Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() => _servicio.EditarEquipo(equipo, "Uruguay"));
    }
    
    [TestMethod]
    public void EditarEquipo_NombreDuplicado_LanzaExcepcion()
    {
        // Arrange
        var equipo1 = new Equipo
        {
            Nombre = "Uruguay",
            RankingFIFA = 1000,
            Confederacion = Confederacion.CONMEBOL
        };
        var equipo2 = new Equipo
        {
            Nombre = "Argentina",
            RankingFIFA = 800,
            Confederacion = Confederacion.CONMEBOL
        };
        _servicio.AgregarEquipo(equipo1);
        _servicio.AgregarEquipo(equipo2);

        // Act
        equipo2.Nombre = "Uruguay";

        // Assert
        Assert.ThrowsException<EntidadYaExistenteExcepcion>(() => _servicio.EditarEquipo(equipo2, "Argentina"));
    }
}
