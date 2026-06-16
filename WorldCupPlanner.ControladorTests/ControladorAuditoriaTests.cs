using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Controladores;
using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ControladorTests;

[TestClass]
public class ControladorAuditoriaTests
{
    [TestMethod]
    public void ObtenerTodos_DevuelveRegistrosDelServicio()
    {
        var repositorio = new RepositorioAuditoriaControladorFake();
        var servicio = new ServicioAuditoria(repositorio);
        var controlador = new ControladorAuditoria(servicio);
        servicio.Registrar("admin@test.com", "ALTA", "detalle");

        var registros = controlador.ObtenerTodos();

        Assert.AreEqual(1, registros.Count);
        Assert.AreEqual("ALTA", registros[0].Accion);
        Assert.AreEqual("detalle", registros[0].Detalle);
    }

    [TestMethod]
    public void ObtenerTodos_ConSolicitanteNoAdministrador_MantieneValidacionDelServicio()
    {
        var servicio = new ServicioAuditoria(new RepositorioAuditoriaControladorFake());
        var controlador = new ControladorAuditoria(servicio);
        var editor = CrearUsuarioConRol(RolUsuario.Editor);

        Assert.ThrowsException<AccesoDenegadoExcepcion>(() =>
            controlador.ObtenerTodos(editor));
    }

    [TestMethod]
    public void ObtenerTodos_ConSolicitanteAdministrador_DevuelveRegistros()
    {
        var repositorio = new RepositorioAuditoriaControladorFake();
        var servicio = new ServicioAuditoria(repositorio);
        var controlador = new ControladorAuditoria(servicio);
        var administrador = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        servicio.Registrar("sistema", "ACCION", "detalle");

        var registros = controlador.ObtenerTodos(administrador);

        Assert.AreEqual(1, registros.Count);
    }

    [TestMethod]
    public void ObtenerTodosDTO_DevuelveAuditoriasParaUI()
    {
        var repositorio = new RepositorioAuditoriaControladorFake();
        var servicio = new ServicioAuditoria(repositorio);
        var controlador = new ControladorAuditoria(servicio);
        servicio.Registrar("admin@test.com", "ALTA", "detalle");

        var registros = controlador.ObtenerTodosDTO();

        Assert.AreEqual(1, registros.Count);
        Assert.IsInstanceOfType(registros[0], typeof(AuditoriaDTO));
        Assert.AreEqual("admin@test.com", registros[0].Usuario);
        Assert.AreEqual("ALTA", registros[0].Accion);
        Assert.AreEqual("detalle", registros[0].Detalle);
    }

    [TestMethod]
    public void ExportarComoTexto_DelegaFormatoDelServicio()
    {
        var servicio = new ServicioAuditoria(new RepositorioAuditoriaControladorFake());
        var controlador = new ControladorAuditoria(servicio);
        var registros = new List<Auditoria>
        {
            new(1, "2026-06-01T10:00:00.0000000Z", "admin@test.com", "ALTA", "Equipo Uruguay")
        };

        var texto = controlador.ExportarComoTexto(registros);

        StringAssert.Contains(texto, "2026-06-01T10:00:00.0000000Z");
        StringAssert.Contains(texto, "admin@test.com");
        StringAssert.Contains(texto, "ALTA");
        StringAssert.Contains(texto, "Equipo Uruguay");
    }

    [TestMethod]
    public void ExportarComoTexto_ConAuditoriaDTO_MantieneFormato()
    {
        var servicio = new ServicioAuditoria(new RepositorioAuditoriaControladorFake());
        var controlador = new ControladorAuditoria(servicio);
        var registros = new List<AuditoriaDTO>
        {
            new()
            {
                Id = 1,
                Timestamp = "2026-06-01T10:00:00.0000000Z",
                Usuario = "admin@test.com",
                Accion = "ALTA",
                Detalle = "Equipo Uruguay"
            }
        };

        var texto = controlador.ExportarComoTexto(registros);

        StringAssert.Contains(texto, "2026-06-01T10:00:00.0000000Z");
        StringAssert.Contains(texto, "admin@test.com");
        StringAssert.Contains(texto, "ALTA");
        StringAssert.Contains(texto, "Equipo Uruguay");
    }

    private static Usuario CrearUsuarioConRol(RolUsuario rol)
    {
        var usuario = new Usuario(new DatosUsuario
        {
            Nombre = "Usuario",
            Apellido = "Prueba",
            Email = $"{rol.ToString().ToLowerInvariant()}@test.com",
            FechaNacimiento = "01-01-1990"
        });
        usuario.AgregarRol(rol);
        return usuario;
    }
}

file sealed class RepositorioAuditoriaControladorFake : IRepositorioConId<Auditoria>
{
    private readonly List<Auditoria> _auditorias = new();
    private int _contador;

    public int SiguienteId() => ++_contador;

    public void Agregar(Auditoria auditoria) => _auditorias.Add(auditoria);

    public void Actualizar(Auditoria auditoria) { }

    public void Eliminar(Auditoria auditoria) => _auditorias.Remove(auditoria);

    public List<Auditoria> Buscar(Func<Auditoria, bool> predicado) =>
        _auditorias.Where(predicado).ToList();
}

