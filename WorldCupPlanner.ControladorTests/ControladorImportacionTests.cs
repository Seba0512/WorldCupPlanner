using System.Text;
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Controladores;
using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ControladorTests;

[TestClass]
public class ControladorImportacionTests
{
    [TestMethod]
    public void Importar_CsvValido_DelegaEnServicioImportacionYServicioEquipo()
    {
        var repositorioEquipo = new RepositorioEquipoControladorImportacionFake();
        var controlador = CrearControlador(repositorioEquipo);
        using var csv = CrearCsv("""
            Nombre,Confederacion,RankingFIFA
            Uruguay,CONMEBOL,1800
            Francia,UEFA,1900
            """);

        var resultado = controlador.Importar(csv, solicitante: null);

        Assert.AreEqual(2, resultado.Exitosos);
        Assert.AreEqual(0, resultado.Errores.Count);
        Assert.AreEqual(2, repositorioEquipo.Buscar(_ => true).Count);
        Assert.IsTrue(repositorioEquipo.Buscar(e => e.Nombre == "Uruguay").Any());
        Assert.IsTrue(repositorioEquipo.Buscar(e => e.Nombre == "Francia").Any());
    }

    [TestMethod]
    public void Importar_CsvInvalido_DevuelveErroresDelServicioSinAgregarEquipos()
    {
        var repositorioEquipo = new RepositorioEquipoControladorImportacionFake();
        var controlador = CrearControlador(repositorioEquipo);
        using var csv = CrearCsv("""
            Nombre,Confederacion
            Uruguay,CONMEBOL
            """);

        var resultado = controlador.Importar(csv, solicitante: null);

        Assert.AreEqual(0, resultado.Exitosos);
        Assert.AreEqual(1, resultado.Errores.Count);
        Assert.AreEqual(0, repositorioEquipo.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void Importar_ConSolicitanteSinPermisos_PropagaValidacionDelServicio()
    {
        var controlador = CrearControlador(new RepositorioEquipoControladorImportacionFake());
        using var csv = CrearCsv("""
            Nombre,Confederacion,RankingFIFA
            Uruguay,CONMEBOL,1800
            """);
        var usuarioSinRol = CrearUsuarioSinRol();

        Assert.ThrowsException<AccesoDenegadoExcepcion>(() =>
            controlador.Importar(csv, UsuarioDTO.FromEntity(usuarioSinRol)));
    }

    [TestMethod]
    public void Importar_ConSolicitanteAdministrador_RegistraImportacionCorrectamente()
    {
        var repositorioEquipo = new RepositorioEquipoControladorImportacionFake();
        var controlador = CrearControlador(repositorioEquipo);
        using var csv = CrearCsv("""
            Nombre,Confederacion,RankingFIFA
            Brasil,CONMEBOL,1950
            """);
        var administrador = CrearAdministrador();

        var resultado = controlador.Importar(csv, UsuarioDTO.FromEntity(administrador));

        Assert.AreEqual(1, resultado.Exitosos);
        Assert.AreEqual(1, repositorioEquipo.Buscar(_ => true).Count);
    }

    private static ControladorImportacion CrearControlador(IRepositorio<Equipo> repositorioEquipo)
    {
        var repositorioUsuario = new RepositorioUsuarioControladorImportacionFake();
        repositorioUsuario.Agregar(CrearUsuarioSinRol());
        repositorioUsuario.Agregar(CrearAdministrador());
        var auditoria = new ServicioAuditoria(new RepositorioAuditoriaFake());
        var servicioUsuario = new ServicioUsuario(repositorioUsuario, auditoria);
        var servicioEquipo = new ServicioEquipo(repositorioEquipo);
        var servicioImportacion = new ServicioImportacionLegible();
        return new ControladorImportacion(servicioImportacion, servicioEquipo, servicioUsuario);
    }

    private static MemoryStream CrearCsv(string contenido) =>
        new(Encoding.UTF8.GetBytes(contenido.Replace("\r", string.Empty).Trim()));

    private static Usuario CrearAdministrador()
    {
        var usuario = new Usuario(new DatosUsuario
        {
            Nombre = "Admin",
            Apellido = "Prueba",
            Email = "admin@prueba.com",
            FechaNacimiento = "01-01-1990"
        });
        usuario.AgregarRol(RolUsuario.AdministradorDelSistema);
        return usuario;
    }

    private static Usuario CrearUsuarioSinRol() => new(new DatosUsuario
    {
        Nombre = "Usuario",
        Apellido = "Prueba",
        Email = "usuario@prueba.com",
        FechaNacimiento = "01-01-1990"
    });
}

file sealed class RepositorioUsuarioControladorImportacionFake : IRepositorioUsuario
{
    private readonly List<Usuario> _usuarios = new();

    public void Agregar(Usuario usuario) => _usuarios.Add(usuario);
    public void Actualizar(Usuario usuario) { }
    public void Eliminar(Usuario usuario) => _usuarios.Remove(usuario);
    public List<Usuario> Buscar(Func<Usuario, bool> predicado) => _usuarios.Where(predicado).ToList();
    public Usuario? ObtenerPorEmail(string email) =>
        _usuarios.FirstOrDefault(u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
}



file sealed class RepositorioEquipoControladorImportacionFake : IRepositorio<Equipo>
{
    private readonly List<Equipo> _equipos = new();

    public void Agregar(Equipo equipo)
    {
        equipo.Id = _equipos.Count + 1;
        _equipos.Add(equipo);
    }

    public void Actualizar(Equipo equipo)
    {
        var indice = _equipos.FindIndex(e =>
            ReferenceEquals(e, equipo) ||
            string.Equals(e.Nombre, equipo.Nombre, StringComparison.OrdinalIgnoreCase));
        if (indice >= 0)
            _equipos[indice] = equipo;
    }

    public void Eliminar(Equipo equipo) => _equipos.Remove(equipo);

    public List<Equipo> Buscar(Func<Equipo, bool> predicado) =>
        _equipos.Where(predicado).ToList();
}

