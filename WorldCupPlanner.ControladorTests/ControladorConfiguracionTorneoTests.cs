using WorldCupPlanner.Controladores;
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ControladorTests;

[TestClass]
public class ControladorConfiguracionTorneoTests
{
    [TestMethod]
    public void ObtenerConfiguracionActual_DevuelveConfiguracionDelServicio()
    {
        var servicio = new ServicioConfiguracionTorneo();
        var controlador = new ControladorConfiguracionTorneo(servicio);

        var configuracion = controlador.ObtenerConfiguracionActual();

        Assert.AreEqual(0, configuracion.SemillaFixture);
        Assert.AreEqual(0, configuracion.SemillaCompletar);
        Assert.AreEqual(3, configuracion.MaxPartidosPorDia);
    }

    [TestMethod]
    public void ModificarConfiguracion_DelegaEnServicioYMantieneComportamiento()
    {
        var servicio = new ServicioConfiguracionTorneo();
        var administrador = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        var controlador = CrearControlador(servicio, administrador);
        var administradorDto = UsuarioDTO.FromEntity(administrador);

        controlador.ModificarConfiguracion(administradorDto, new ConfiguracionTorneo
        {
            SemillaFixture = 12,
            SemillaCompletar = 4,
            SemillaCrucesFase = 8,
            SemillaSimulation = 16,
            MaxPartidosPorDia = 4,
            SeparacionEntreFechas = 2
        });

        var configuracion = servicio.ObtenerConfiguracionActual();
        Assert.AreEqual(12, configuracion.SemillaFixture);
        Assert.AreEqual(4, configuracion.SemillaCompletar);
        Assert.AreEqual(8, configuracion.SemillaCrucesFase);
        Assert.AreEqual(16, configuracion.SemillaSimulation);
        Assert.AreEqual(4, configuracion.MaxPartidosPorDia);
        Assert.AreEqual(2, configuracion.SeparacionEntreFechas);
    }

    [TestMethod]
    public void ModificarConfiguracion_ConUsuarioNoAdministrador_MantieneValidacionDelServicio()
    {
        var servicio = new ServicioConfiguracionTorneo();
        var editor = CrearUsuarioConRol(RolUsuario.Editor);
        var controlador = CrearControlador(servicio, editor);
        var editorDto = UsuarioDTO.FromEntity(editor);

        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            controlador.ModificarConfiguracion(editorDto, new ConfiguracionTorneo()));
    }

    [TestMethod]
    public void RegistrarFixtureGenerado_BloqueaConfiguracionYPermiteObtenerUsada()
    {
        var servicio = new ServicioConfiguracionTorneo();
        var controlador = new ControladorConfiguracionTorneo(servicio);
        var configuracionUsada = new ConfiguracionTorneo
        {
            SemillaFixture = 5,
            SemillaCompletar = 6
        };

        controlador.RegistrarFixtureGenerado(configuracionUsada);

        Assert.IsFalse(controlador.EsConfiguracionModificable);
        var obtenida = controlador.ObtenerConfiguracionUsadaParaFixture();
        Assert.AreEqual(5, obtenida.SemillaFixture);
        Assert.AreEqual(6, obtenida.SemillaCompletar);
    }

    [TestMethod]
    public void PuedeModificarConfiguracion_DelegaConsultaDelServicio()
    {
        var servicio = new ServicioConfiguracionTorneo();
        var administrador = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        var periodista = CrearUsuarioConRol(RolUsuario.Periodista);
        var controlador = CrearControlador(servicio, administrador, periodista);

        Assert.IsTrue(controlador.PuedeModificarConfiguracion(UsuarioDTO.FromEntity(administrador)));
        Assert.IsFalse(controlador.PuedeModificarConfiguracion(UsuarioDTO.FromEntity(periodista)));
        Assert.IsFalse(controlador.PuedeModificarConfiguracion(null));
    }

    private static ControladorConfiguracionTorneo CrearControlador(
        ServicioConfiguracionTorneo servicioConfiguracion,
        params Usuario[] usuarios)
    {
        var repositorio = new RepositorioUsuarioConfiguracionControladorFake();
        foreach (var usuario in usuarios)
            repositorio.Agregar(usuario);

        var auditoria = new ServicioAuditoria(new RepositorioAuditoriaConfiguracionControladorFake());
        var servicioUsuario = new ServicioUsuario(repositorio, auditoria);
        return new ControladorConfiguracionTorneo(servicioConfiguracion, servicioUsuario);
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

file sealed class RepositorioUsuarioConfiguracionControladorFake : IRepositorioUsuario
{
    private readonly List<Usuario> _usuarios = new();

    public void Agregar(Usuario usuario) => _usuarios.Add(usuario);
    public void Actualizar(Usuario usuario) { }
    public void Eliminar(Usuario usuario) => _usuarios.Remove(usuario);
    public List<Usuario> Buscar(Func<Usuario, bool> predicado) => _usuarios.Where(predicado).ToList();
    public Usuario? ObtenerPorEmail(string email) =>
        _usuarios.FirstOrDefault(u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
}

file sealed class RepositorioAuditoriaConfiguracionControladorFake : IRepositorioConId<Auditoria>
{
    private readonly List<Auditoria> _auditorias = new();
    private int _siguienteId;

    public int SiguienteId() => ++_siguienteId;
    public void Agregar(Auditoria auditoria) => _auditorias.Add(auditoria);
    public void Actualizar(Auditoria auditoria) { }
    public void Eliminar(Auditoria auditoria) => _auditorias.Remove(auditoria);
    public List<Auditoria> Buscar(Func<Auditoria, bool> predicado) => _auditorias.Where(predicado).ToList();
}

