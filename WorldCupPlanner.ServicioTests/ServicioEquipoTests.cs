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

    [TestMethod]
    public void AgregarEquipo_CupoLlenoOFC_LanzaExcepcion()
    {
        // Arrange - OFC tiene cupo 1
        _servicio.AgregarEquipo(CrearEquipo("Nueva Zelanda", Confederacion.OFC));

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            _servicio.AgregarEquipo(CrearEquipo("Australia", Confederacion.OFC)));
    }

    [TestMethod]
    public void AgregarEquipo_UltimoEquipoDentroDelCupo_SeAgregaCorrectamente()
    {
        // Arrange - llenar CONMEBOL hasta el límite (7)
        for (var i = 1; i <= 7; i++)
            _servicio.AgregarEquipo(CrearEquipo($"Equipo CONMEBOL {i}", Confederacion.CONMEBOL));

        // Assert
        Assert.AreEqual(7, _servicio.ObtenerEquipos().Count);
    }

    [TestMethod]
    public void AgregarEquipo_CupoLlenoCONMEBOL_LanzaExcepcion()
    {
        // Arrange - llenar CONMEBOL (cupo 7) y luego intentar agregar uno más
        for (var i = 1; i <= 7; i++)
            _servicio.AgregarEquipo(CrearEquipo($"Equipo CONMEBOL {i}", Confederacion.CONMEBOL));

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            _servicio.AgregarEquipo(CrearEquipo("Equipo CONMEBOL 8", Confederacion.CONMEBOL)));
    }

    [TestMethod]
    public void AgregarEquipo_CupoLlenoEnUnaConfederacion_NoAfectaOtraConfederacion()
    {
        // Arrange - llenar OFC
        _servicio.AgregarEquipo(CrearEquipo("Nueva Zelanda", Confederacion.OFC));

        // Act - agregar en UEFA no debe verse afectado
        _servicio.AgregarEquipo(CrearEquipo("España", Confederacion.UEFA));

        // Assert
        Assert.AreEqual(2, _servicio.ObtenerEquipos().Count);
    }

    [TestMethod]
    public void AgregarEquipo_CupoLlenoUEFA_LanzaExcepcion()
    {
        // Arrange - llenar UEFA (cupo 16)
        for (var i = 1; i <= 16; i++)
            _servicio.AgregarEquipo(CrearEquipo($"Equipo UEFA {i}", Confederacion.UEFA));

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            _servicio.AgregarEquipo(CrearEquipo("Equipo UEFA 17", Confederacion.UEFA)));
    }

    // IRepositorio<Equipo>

    [TestMethod]
    public void Agregar_EquipoValido_SeAgregaAlRepositorio()
    {
        // Arrange
        var equipo = CrearEquipo("España", Confederacion.UEFA);

        // Act
        _servicio.Agregar(equipo);

        // Assert
        Assert.AreEqual(1, _servicio.ObtenerEquipos().Count);
    }

    [TestMethod]
    public void Agregar_EquipoInvalido_LanzaExcepcion()
    {
        // Arrange
        var equipo = new Equipo { Nombre = "", RankingFIFA = 1000, Confederacion = Confederacion.UEFA };

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() => _servicio.Agregar(equipo));
    }

    [TestMethod]
    public void Actualizar_EquipoExistente_PersisteCambios()
    {
        // Arrange
        var equipo = CrearEquipo("España", Confederacion.UEFA);
        _servicio.AgregarEquipo(equipo);
        equipo.RankingFIFA = 500;

        // Act
        _servicio.Actualizar(equipo);

        // Assert
        Assert.AreEqual(500, _servicio.Buscar(e => e.Nombre == "España").First().RankingFIFA);
    }

    [TestMethod]
    public void Actualizar_EquipoNoExistente_LanzaExcepcion()
    {
        // Arrange
        var equipo = CrearEquipo("España", Confederacion.UEFA);

        // Act & Assert
        Assert.ThrowsException<EntidadInexistenteExcepcion>(() => _servicio.Actualizar(equipo));
    }

    [TestMethod]
    public void Actualizar_EquipoNulo_LanzaExcepcion()
    {
        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() => _servicio.Actualizar(null!));
    }

    [TestMethod]
    public void Eliminar_EquipoExistente_SeEliminaDelRepositorio()
    {
        // Arrange
        var equipo = CrearEquipo("España", Confederacion.UEFA);
        _servicio.AgregarEquipo(equipo);

        // Act
        _servicio.Eliminar(equipo);

        // Assert
        Assert.AreEqual(0, _servicio.ObtenerEquipos().Count);
    }

    [TestMethod]
    public void Buscar_ConPredicadoCoincidente_DevuelveEquiposCorrespondientes()
    {
        // Arrange
        _servicio.AgregarEquipo(CrearEquipo("España", Confederacion.UEFA));
        _servicio.AgregarEquipo(CrearEquipo("Francia", Confederacion.UEFA));
        _servicio.AgregarEquipo(CrearEquipo("Uruguay", Confederacion.CONMEBOL));

        // Act
        var resultado = _servicio.Buscar(e => e.Confederacion == Confederacion.UEFA);

        // Assert
        Assert.AreEqual(2, resultado.Count);
    }

    [TestMethod]
    public void Buscar_SinCoincidencias_DevuelveListaVacia()
    {
        // Arrange
        _servicio.AgregarEquipo(CrearEquipo("Uruguay", Confederacion.CONMEBOL));

        // Act
        var resultado = _servicio.Buscar(e => e.Confederacion == Confederacion.OFC);

        // Assert
        Assert.AreEqual(0, resultado.Count);
    }

    private static Equipo CrearEquipo(string nombre, Confederacion confederacion) => new()
    {
        Nombre = nombre,
        RankingFIFA = 1000,
        Confederacion = confederacion
    };
}
