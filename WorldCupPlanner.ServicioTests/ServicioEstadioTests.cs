using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.AccesoADatos.Repositorios;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

internal class RepositorioAuditoriaEstadioFake : IRepositorioConId<Auditoria>
{
    private int _nextId = 1;
    public List<Auditoria> Entradas { get; } = [];

    public int SiguienteId() => _nextId++;
    public void Agregar(Auditoria entidad) => Entradas.Add(entidad);
    public void Actualizar(Auditoria entidad) { }
    public void Eliminar(Auditoria entidad) => Entradas.Remove(entidad);
    public List<Auditoria> Buscar(Func<Auditoria, bool> predicado) => Entradas.Where(predicado).ToList();
}

[TestClass]
public class ServicioEstadioTests
{
    private ServicioEstadio _servicio;

    [TestInitialize]
    public void Setup()
    {
        _servicio = new ServicioEstadio(new RepositorioEstadioEnMemoria());
    }

    private static (ServicioEstadio servicio, RepositorioAuditoriaEstadioFake repoAuditoria) CrearServicioConAuditoria()
    {
        var repoAuditoria = new RepositorioAuditoriaEstadioFake();
        var auditoria = new ServicioAuditoria(repoAuditoria);
        var servicio = new ServicioEstadio(new RepositorioEstadioEnMemoria(), auditoria);
        return (servicio, repoAuditoria);
    }

    private static Estadio CrearEstadio(string nombre = "Estadio Centenario", string ciudad = "Montevideo", int capacidad = 60000) => new()
    {
        Nombre = nombre,
        Ciudad = ciudad,
        Capacidad = capacidad
    };

    [TestMethod]
    public void AgregarEstadio_EstadioValido_SeAgregaCorrectamente()
    {
        _servicio.AgregarEstadio(CrearEstadio());

        Assert.AreEqual(1, _servicio.ObtenerEstadios().Count);
    }

    [TestMethod]
    public void AgregarEstadio_NombreDuplicado_LanzaExcepcion()
    {
        _servicio.AgregarEstadio(CrearEstadio());

        Assert.ThrowsException<EntidadYaExistenteExcepcion>(() => _servicio.AgregarEstadio(CrearEstadio(capacidad: 40000)));
    }

    [TestMethod]
    public void AgregarEstadio_EstadioInvalido_LanzaExcepcion()
    {
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() => _servicio.AgregarEstadio(CrearEstadio(nombre: "")));
    }

    [TestMethod]
    public void EliminarEstadio_EstadioExistente_SeEliminaCorrectamente()
    {
        var estadio = CrearEstadio();
        _servicio.AgregarEstadio(estadio);

        _servicio.EliminarEstadio(estadio);

        Assert.AreEqual(0, _servicio.ObtenerEstadios().Count);
    }

    [TestMethod]
    public void EliminarEstadio_EstadioUsadoEnFixture_DebeLanzarExcepcion()
    {
        var estadio = CrearEstadio();
        var fixture = CrearFixtureConEstadio(estadio);
        _servicio.AgregarEstadio(estadio);
        _servicio.RegistrarFixtureGenerado(fixture);

        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            _servicio.EliminarEstadio(estadio));
    }

    [TestMethod]
    public void EditarEstadio_EstadioValido_SeEditaCorrectamente()
    {
        var estadio = CrearEstadio();
        _servicio.AgregarEstadio(estadio);
        string nombreOriginal = estadio.Nombre;
        estadio.Nombre = "Estadio Olimpico";

        _servicio.EditarEstadio(nombreOriginal, estadio);

        Assert.AreEqual("Estadio Olimpico", _servicio.ObtenerEstadios().First().Nombre);
    }

    [TestMethod]
    public void EditarEstadio_EstadioNoExistente_LanzaExcepcion()
    {
        var estadio = CrearEstadio();

        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            _servicio.EditarEstadio("EstadioQueNoExiste", estadio));
    }

    [TestMethod]
    public void EditarEstadio_NombreYaExisteEnOtroEstadio_LanzaExcepcion()
    {
        // Arrange
        _servicio.AgregarEstadio(CrearEstadio("Estadio Centenario"));
        _servicio.AgregarEstadio(CrearEstadio("Estadio Olimpico"));
        var estadioActualizado = CrearEstadio("Estadio Olimpico");

        // Act & Assert
        Assert.ThrowsException<EntidadYaExistenteExcepcion>(() =>
            _servicio.EditarEstadio("Estadio Centenario", estadioActualizado));
    }

    [TestMethod]
    public void EditarEstadio_MismoNombreSinCambios_SeEditaCorrectamente()
    {
        // Arrange
        _servicio.AgregarEstadio(CrearEstadio("Estadio Centenario", capacidad: 60000));
        var estadioActualizado = CrearEstadio("Estadio Centenario", ciudad: "Montevideo Centro", capacidad: 65000);

        // Act
        _servicio.EditarEstadio("Estadio Centenario", estadioActualizado);

        // Assert
        var resultado = _servicio.ObtenerEstadios().First();
        Assert.AreEqual("Estadio Centenario", resultado.Nombre);
        Assert.AreEqual(65000, resultado.Capacidad);
    }

    [TestMethod]
    public void EditarEstadio_NombreIgualDiferenteCaso_LanzaExcepcion()
    {
        // Arrange
        _servicio.AgregarEstadio(CrearEstadio("Centenario"));
        _servicio.AgregarEstadio(CrearEstadio("Olimpico"));
        var estadioActualizado = CrearEstadio("CENTENARIO");

        // Act & Assert
        Assert.ThrowsException<EntidadYaExistenteExcepcion>(() =>
            _servicio.EditarEstadio("Olimpico", estadioActualizado));
    }

    [TestMethod]
    public void EditarEstadio_NombreConEspaciosExtraIgualAExistente_LanzaExcepcion()
    {
        // Arrange
        _servicio.AgregarEstadio(CrearEstadio("Centenario"));
        _servicio.AgregarEstadio(CrearEstadio("Olimpico"));
        var estadioActualizado = CrearEstadio(" centenario ");

        // Act & Assert
        Assert.ThrowsException<EntidadYaExistenteExcepcion>(() =>
            _servicio.EditarEstadio("Olimpico", estadioActualizado));
    }

    [TestMethod]
    public void AgregarEstadio_NombreIgualDiferenteCaso_LanzaExcepcion()
    {
        // Arrange
        _servicio.AgregarEstadio(CrearEstadio("Centenario"));

        // Act & Assert
        Assert.ThrowsException<EntidadYaExistenteExcepcion>(() =>
            _servicio.AgregarEstadio(CrearEstadio("CENTENARIO")));
    }

    [TestMethod]
    public void AgregarEstadio_NombreConEspaciosExtraIgualAExistente_LanzaExcepcion()
    {
        // Arrange
        _servicio.AgregarEstadio(CrearEstadio("Centenario"));

        // Act & Assert
        Assert.ThrowsException<EntidadYaExistenteExcepcion>(() =>
            _servicio.AgregarEstadio(CrearEstadio(" centenario ")));
    }

    [TestMethod]
    public void EditarEstadio_ConNuevoObjetoYNombreOriginal_SeEditaCorrectamente()
    {
        // Cubre la rama del predicado donde ReferenceEquals es false
        // pero e.Nombre == nombreOriginal es true (busca por nombre, no por referencia)
        var estadio = CrearEstadio();
        _servicio.AgregarEstadio(estadio);

        var estadioActualizado = new Estadio
        {
            Nombre = "Estadio Olimpico",
            Ciudad = "Buenos Aires",
            Capacidad = 70000
        };

        _servicio.EditarEstadio("Estadio Centenario", estadioActualizado);

        Assert.AreEqual("Estadio Olimpico", _servicio.ObtenerEstadios().First().Nombre);
    }

    // --- Auditoría ---

    [TestMethod]
    public void AgregarEstadio_EstadioValido_RegistraEntradaDeAuditoria()
    {
        var (servicio, repoAuditoria) = CrearServicioConAuditoria();

        servicio.AgregarEstadio(CrearEstadio());

        Assert.AreEqual(1, repoAuditoria.Entradas.Count);
        Assert.AreEqual("Alta", repoAuditoria.Entradas[0].Accion);
        StringAssert.Contains(repoAuditoria.Entradas[0].Detalle, "Estadio Centenario");
    }

    [TestMethod]
    public void AgregarEstadio_EstadioInvalido_NoRegistraAuditoria()
    {
        var (servicio, repoAuditoria) = CrearServicioConAuditoria();

        Assert.ThrowsException<EntidadInvalidaExcepcion>(() => servicio.AgregarEstadio(CrearEstadio(nombre: "")));

        Assert.AreEqual(0, repoAuditoria.Entradas.Count);
    }

    [TestMethod]
    public void EditarEstadio_EstadioValido_RegistraEntradaDeAuditoria()
    {
        var (servicio, repoAuditoria) = CrearServicioConAuditoria();
        var estadio = CrearEstadio();
        servicio.AgregarEstadio(estadio);
        repoAuditoria.Entradas.Clear();

        estadio.Nombre = "Estadio Olimpico";
        servicio.EditarEstadio("Estadio Centenario", estadio);

        Assert.AreEqual(1, repoAuditoria.Entradas.Count);
        Assert.AreEqual("Edicion", repoAuditoria.Entradas[0].Accion);
        StringAssert.Contains(repoAuditoria.Entradas[0].Detalle, "Estadio Centenario");
        StringAssert.Contains(repoAuditoria.Entradas[0].Detalle, "Estadio Olimpico");
    }

    [TestMethod]
    public void EditarEstadio_EstadioInvalido_NoRegistraAuditoria()
    {
        var (servicio, repoAuditoria) = CrearServicioConAuditoria();
        servicio.AgregarEstadio(CrearEstadio());
        repoAuditoria.Entradas.Clear();

        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicio.EditarEstadio("Estadio Centenario", CrearEstadio(nombre: "")));

        Assert.AreEqual(0, repoAuditoria.Entradas.Count);
    }

    [TestMethod]
    public void EliminarEstadio_EstadioExistente_RegistraEntradaDeAuditoria()
    {
        var (servicio, repoAuditoria) = CrearServicioConAuditoria();
        var estadio = CrearEstadio();
        servicio.AgregarEstadio(estadio);
        repoAuditoria.Entradas.Clear();

        servicio.EliminarEstadio(estadio);

        Assert.AreEqual(1, repoAuditoria.Entradas.Count);
        Assert.AreEqual("Baja", repoAuditoria.Entradas[0].Accion);
        StringAssert.Contains(repoAuditoria.Entradas[0].Detalle, "Estadio Centenario");
    }

    [TestMethod]
    public void AgregarEstadio_SinAuditoria_NoLanzaExcepcion()
    {
        var servicio = new ServicioEstadio(new RepositorioEstadioEnMemoria());

        servicio.AgregarEstadio(CrearEstadio());

        Assert.AreEqual(1, servicio.ObtenerEstadios().Count);
    }

    [TestMethod]
    public void Auditoria_RegistraUsuarioSistema()
    {
        var (servicio, repoAuditoria) = CrearServicioConAuditoria();

        servicio.AgregarEstadio(CrearEstadio());

        Assert.AreEqual("Sistema", repoAuditoria.Entradas[0].Usuario);
    }

    private static Fixture CrearFixtureConEstadio(Estadio estadio)
    {
        var equipoLocal = new Equipo
        {
            Nombre = "Uruguay",
            RankingFIFA = 1000,
            Confederacion = Confederacion.CONMEBOL
        };
        var equipoVisitante = new Equipo
        {
            Nombre = "Francia",
            RankingFIFA = 1000,
            Confederacion = Confederacion.UEFA
        };
        var grupo = new Grupo("A", new List<Equipo>
        {
            equipoLocal,
            equipoVisitante,
            new Equipo
            {
                Nombre = "Marruecos",
                RankingFIFA = 1000,
                Confederacion = Confederacion.CAF
            },
            new Equipo
            {
                Nombre = "Japon",
                RankingFIFA = 1000,
                Confederacion = Confederacion.AFC
            }
        });

        grupo.Partidos.Add(new Partido
        {
            Id = 1,
            Fecha = new DateTime(2026, 6, 1),
            Estadio = estadio,
            EquipoLocal = equipoLocal,
            EquipoVisitante = equipoVisitante,
            Fase = FaseTorneo.FaseDeGrupos,
            Grupo = grupo
        });

        return new Fixture
        {
            Grupos = { grupo }
        };
    }

    // --- Autorización ---

    [TestMethod]
    public void AgregarEstadio_ConSolicitanteAdministrador_AgregaCorrectamente()
    {
        // Arrange
        var admin = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        var estadio = CrearEstadio();

        // Act
        _servicio.AgregarEstadio(estadio, solicitante: admin);

        // Assert
        Assert.AreEqual(1, _servicio.ObtenerEstadios().Count);
    }

    [TestMethod]
    public void AgregarEstadio_ConSolicitanteEditor_LanzaAccesoDenegado()
    {
        // Arrange
        var editor = CrearUsuarioConRol(RolUsuario.Editor);
        var estadio = CrearEstadio();

        // Act & Assert
        Assert.ThrowsException<AccesoDenegadoExcepcion>(() =>
            _servicio.AgregarEstadio(estadio, solicitante: editor));
    }

    [TestMethod]
    public void AgregarEstadio_ConSolicitanteSinRoles_LanzaAccesoDenegado()
    {
        // Arrange
        var sinRoles = CrearUsuarioSinRoles();
        var estadio = CrearEstadio();

        // Act & Assert
        Assert.ThrowsException<AccesoDenegadoExcepcion>(() =>
            _servicio.AgregarEstadio(estadio, solicitante: sinRoles));
    }

    [TestMethod]
    public void EditarEstadio_ConSolicitanteEditor_LanzaAccesoDenegado()
    {
        // Arrange
        var editor = CrearUsuarioConRol(RolUsuario.Editor);
        _servicio.AgregarEstadio(CrearEstadio());
        var actualizado = CrearEstadio("Estadio Nuevo");

        // Act & Assert
        Assert.ThrowsException<AccesoDenegadoExcepcion>(() =>
            _servicio.EditarEstadio("Estadio Centenario", actualizado, solicitante: editor));
    }

    [TestMethod]
    public void EditarEstadio_ConSolicitanteAdministrador_EditaCorrectamente()
    {
        // Arrange
        var admin = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        _servicio.AgregarEstadio(CrearEstadio());
        var actualizado = CrearEstadio("Estadio Nuevo");

        // Act
        _servicio.EditarEstadio("Estadio Centenario", actualizado, solicitante: admin);

        // Assert
        Assert.AreEqual("Estadio Nuevo", _servicio.ObtenerEstadios().First().Nombre);
    }

    [TestMethod]
    public void EliminarEstadio_ConSolicitanteEditor_LanzaAccesoDenegado()
    {
        // Arrange
        var editor = CrearUsuarioConRol(RolUsuario.Editor);
        var estadio = CrearEstadio();
        _servicio.AgregarEstadio(estadio);

        // Act & Assert
        Assert.ThrowsException<AccesoDenegadoExcepcion>(() =>
            _servicio.EliminarEstadio(estadio, solicitante: editor));
    }

    [TestMethod]
    public void EliminarEstadio_ConSolicitanteAdministrador_EliminaCorrectamente()
    {
        // Arrange
        var admin = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        var estadio = CrearEstadio();
        _servicio.AgregarEstadio(estadio);

        // Act
        _servicio.EliminarEstadio(estadio, solicitante: admin);

        // Assert
        Assert.AreEqual(0, _servicio.ObtenerEstadios().Count);
    }

    [TestMethod]
    public void AgregarEstadio_ConAuditoriaYSolicitante_RegistraUsuarioEnAuditoria()
    {
        // Arrange
        var (servicio, repoAuditoria) = CrearServicioConAuditoria();
        var admin = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);

        // Act
        servicio.AgregarEstadio(CrearEstadio(), solicitante: admin);

        // Assert
        Assert.AreEqual("Test User", repoAuditoria.Entradas[0].Usuario);
    }

    [TestMethod]
    public void EditarEstadio_ConAuditoriaYSolicitante_RegistraUsuarioEnAuditoria()
    {
        // Arrange
        var (servicio, repoAuditoria) = CrearServicioConAuditoria();
        var admin = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        servicio.AgregarEstadio(CrearEstadio());
        repoAuditoria.Entradas.Clear();

        // Act
        servicio.EditarEstadio("Estadio Centenario", CrearEstadio("Estadio Nuevo"), solicitante: admin);

        // Assert
        Assert.AreEqual("Test User", repoAuditoria.Entradas[0].Usuario);
    }

    [TestMethod]
    public void EliminarEstadio_ConAuditoriaYSolicitante_RegistraUsuarioEnAuditoria()
    {
        // Arrange
        var (servicio, repoAuditoria) = CrearServicioConAuditoria();
        var admin = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        var estadio = CrearEstadio();
        servicio.AgregarEstadio(estadio);
        repoAuditoria.Entradas.Clear();

        // Act
        servicio.EliminarEstadio(estadio, solicitante: admin);

        // Assert
        Assert.AreEqual("Test User", repoAuditoria.Entradas[0].Usuario);
    }

    private static Usuario CrearUsuarioConRol(RolUsuario rol)
    {
        var usuario = CrearUsuarioSinRoles();
        usuario.AgregarRol(rol);
        return usuario;
    }

    private static Usuario CrearUsuarioSinRoles()
    {
        return new Usuario(new DatosUsuario
        {
            Nombre = "Test",
            Apellido = "User",
            Email = "test@example.com",
            FechaNacimiento = "01-01-1990"
        });
    }
}
