using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Controladores;
using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ControladorTests;

[TestClass]
public class ControladorUsuarioTests
{
    [TestMethod]
    public void Listar_DevuelveUsuariosDTO()
    {
        var repositorio = new RepositorioUsuarioControladorFake();
        repositorio.Agregar(CrearUsuario("uno@mail.com", RolUsuario.Editor));
        repositorio.Agregar(CrearUsuario("dos@mail.com", RolUsuario.Periodista));
        var controlador = CrearControlador(repositorio);

        var usuarios = controlador.Listar();

        Assert.AreEqual(2, usuarios.Count);
        Assert.IsInstanceOfType(usuarios[0], typeof(UsuarioDTO));
        Assert.IsTrue(usuarios.Any(u => u.Email == "uno@mail.com"));
    }

    [TestMethod]
    public void ObtenerPorEmail_DevuelveUsuarioDTO()
    {
        var repositorio = new RepositorioUsuarioControladorFake();
        repositorio.Agregar(CrearUsuario("usuario@mail.com", RolUsuario.Editor));
        var controlador = CrearControlador(repositorio);

        var usuario = controlador.ObtenerPorEmail("usuario@mail.com");

        Assert.IsNotNull(usuario);
        Assert.AreEqual("usuario@mail.com", usuario.Email);
        Assert.IsInstanceOfType(usuario, typeof(UsuarioDTO));
    }

    [TestMethod]
    public void Crear_DelegaEnServicio()
    {
        var repositorio = new RepositorioUsuarioControladorFake();
        var controlador = CrearControlador(repositorio);

        controlador.Crear(new NuevoUsuarioDto
        {
            Nombre = "Nuevo",
            Apellido = "Usuario",
            Email = "nuevo@mail.com",
            FechaNacimiento = new DateTime(1990, 1, 1),
            Contrasenia = "Passw0rd!",
            ConfirmarContrasenia = "Passw0rd!",
            Roles = new[] { RolUsuario.Editor }
        });

        var creado = repositorio.ObtenerPorEmail("nuevo@mail.com");
        Assert.IsNotNull(creado);
        Assert.IsTrue(creado.VerificarContrasenia("Passw0rd!"));
    }

    [TestMethod]
    public void Actualizar_DelegaEnServicio()
    {
        var repositorio = new RepositorioUsuarioControladorFake();
        repositorio.Agregar(CrearUsuario("usuario@mail.com", RolUsuario.Editor));
        var controlador = CrearControlador(repositorio);

        controlador.Actualizar("usuario@mail.com", new ActualizacionUsuario
        {
            Nombre = "Editado",
            Apellido = "Nuevo",
            Email = "editado@mail.com",
            FechaNacimiento = "02-02-1991"
        });

        var actualizado = repositorio.ObtenerPorEmail("editado@mail.com");
        Assert.IsNotNull(actualizado);
        Assert.AreEqual("Editado", actualizado.Nombre);
        Assert.AreEqual("Nuevo", actualizado.Apellido);
    }

    [TestMethod]
    public void Eliminar_DelegaEnServicio()
    {
        var repositorio = new RepositorioUsuarioControladorFake();
        repositorio.Agregar(CrearUsuario("admin@mail.com", RolUsuario.AdministradorDelSistema));
        repositorio.Agregar(CrearUsuario("editor@mail.com", RolUsuario.Editor));
        var controlador = CrearControlador(repositorio);

        controlador.Eliminar("editor@mail.com");

        Assert.IsNull(repositorio.ObtenerPorEmail("editor@mail.com"));
    }

    [TestMethod]
    public void Login_DevuelveUsuarioDTO()
    {
        var repositorio = new RepositorioUsuarioControladorFake();
        var usuario = CrearUsuario("login@mail.com", RolUsuario.Editor);
        usuario.EstablecerContrasenia("Passw0rd!");
        repositorio.Agregar(usuario);
        var controlador = CrearControlador(repositorio);

        var dto = controlador.Login("login@mail.com", "Passw0rd!");

        Assert.AreEqual("login@mail.com", dto.Email);
        Assert.IsInstanceOfType(dto, typeof(UsuarioDTO));
    }

    private static ControladorUsuario CrearControlador(IRepositorioUsuario repositorio)
    {
        var auditoria = new ServicioAuditoria(new RepositorioAuditoriaUsuarioControladorFake());
        return new ControladorUsuario(new ServicioUsuario(repositorio, auditoria));
    }

    private static Usuario CrearUsuario(string email, RolUsuario rol)
    {
        var usuario = new Usuario(new DatosUsuario
        {
            Nombre = "Nombre",
            Apellido = "Apellido",
            Email = email,
            FechaNacimiento = "01-01-1990"
        });
        usuario.AgregarRol(rol);
        return usuario;
    }
}

file sealed class RepositorioUsuarioControladorFake : IRepositorioUsuario, IRepositorioConClaveEditable<Usuario>
{
    private readonly List<Usuario> _usuarios = new();

    public void Agregar(Usuario usuario) => _usuarios.Add(usuario);

    public void Actualizar(Usuario usuario)
    {
        var indice = _usuarios.FindIndex(u => ReferenceEquals(u, usuario) || u.Id == usuario.Id && usuario.Id > 0);
        if (indice >= 0)
            _usuarios[indice] = usuario;
    }

    public void ActualizarClave(string claveOriginal, Usuario usuario)
    {
        var indice = _usuarios.FindIndex(u =>
            string.Equals(u.Email, claveOriginal, StringComparison.OrdinalIgnoreCase));
        if (indice >= 0)
            _usuarios[indice] = usuario;
    }

    public void Eliminar(Usuario usuario) => _usuarios.Remove(usuario);

    public List<Usuario> Buscar(Func<Usuario, bool> predicado) =>
        _usuarios.Where(predicado).ToList();

    public Usuario? ObtenerPorEmail(string email) =>
        _usuarios.FirstOrDefault(u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
}

file sealed class RepositorioAuditoriaUsuarioControladorFake : IRepositorioConId<Auditoria>
{
    private readonly List<Auditoria> _auditorias = new();
    private int _siguienteId;

    public int SiguienteId() => ++_siguienteId;
    public void Agregar(Auditoria auditoria) => _auditorias.Add(auditoria);
    public void Actualizar(Auditoria auditoria) { }
    public void Eliminar(Auditoria auditoria) => _auditorias.Remove(auditoria);
    public List<Auditoria> Buscar(Func<Auditoria, bool> predicado) =>
        _auditorias.Where(predicado).ToList();
}

