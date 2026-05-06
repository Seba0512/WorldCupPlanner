using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using WorldCupPlanner.AccesoADatos.Repositorios;
using  WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public class RepositoriosEnMemoriaTests
{
    [TestMethod]
    public void RepositorioEquipoEnMemoria_AgregarEquipo_GuardaEquipo()
    {
        var repositorio = new RepositorioEquipoEnMemoria();
        var equipo = CrearEquipoValido("Uruguay");

        repositorio.Agregar(equipo);

        Assert.AreEqual(1, repositorio.ObtenerTodos().Count);
        Assert.AreEqual(equipo, repositorio.ObtenerTodos().First());
    }

    [TestMethod]
    public void RepositorioEquipoEnMemoria_EliminarEquipo_EliminaEquipo()
    {
        var repositorio = new RepositorioEquipoEnMemoria();
        var equipo = CrearEquipoValido("Uruguay");
        repositorio.Agregar(equipo);

        repositorio.Eliminar(equipo);

        Assert.AreEqual(0, repositorio.ObtenerTodos().Count);
    }

    [TestMethod]
    public void RepositorioEstadioEnMemoria_ActualizarEstadio_ModificaEstadio()
    {
        var repositorio = new RepositorioEstadioEnMemoria();
        var estadio = CrearEstadioValido("Estadio Centenario");
        repositorio.Agregar(estadio);
        estadio.Nombre = "Estadio Olimpico";

        repositorio.Actualizar(estadio);

        Assert.AreEqual("Estadio Olimpico", repositorio.ObtenerTodos().First().Nombre);
    }

    [TestMethod]
    public void RepositorioGrupoEnMemoria_AgregarGrupo_GuardaGrupo()
    {
        var repositorio = new RepositorioGrupoEnMemoria();
        var grupo = new Grupo("A");

        repositorio.Agregar(grupo);

        Assert.AreEqual(1, repositorio.ObtenerTodos().Count);
        Assert.AreEqual(grupo, repositorio.ObtenerTodos().First());
    }

    [TestMethod]
    public void RepositorioPartidoEnMemoria_AgregarPartido_GuardaPartido()
    {
        var repositorio = new RepositorioPartidoEnMemoria();
        var partido = CrearPartidoValido();

        repositorio.Agregar(partido);

        Assert.AreEqual(1, repositorio.ObtenerTodos().Count);
        Assert.AreEqual(partido, repositorio.ObtenerTodos().First());
    }

    [TestMethod]
    public void RepositorioUsuarioEnMemoria_AgregarUsuario_GuardaUsuario()
    {
        var repositorio = new RepositorioUsuarioEnMemoria();
        var usuario = CrearUsuarioValido("juan@example.com");

        repositorio.Agregar(usuario);

        Assert.AreEqual(1, repositorio.ObtenerTodos().Count);
        Assert.AreSame(usuario, repositorio.ObtenerTodos().First());
    }

    [TestMethod]
    public void RepositorioUsuarioEnMemoria_AgregarVariosUsuarios_GuardaTodos()
    {
        var repositorio = new RepositorioUsuarioEnMemoria();
        var usuario1 = CrearUsuarioValido("juan@example.com");
        var usuario2 = CrearUsuarioValido("maria@example.com");
        var usuario3 = CrearUsuarioValido("pedro@example.com");

        repositorio.Agregar(usuario1);
        repositorio.Agregar(usuario2);
        repositorio.Agregar(usuario3);

        Assert.AreEqual(3, repositorio.ObtenerTodos().Count);
    }

    [TestMethod]
    public void RepositorioUsuarioEnMemoria_ActualizarUsuarioExistente_ModificaUsuario()
    {
        var repositorio = new RepositorioUsuarioEnMemoria();
        var usuario = CrearUsuarioValido("juan@example.com");
        repositorio.Agregar(usuario);
        usuario.AgregarRol(RolUsuario.AdministradorDelSistema);

        repositorio.Actualizar(usuario);

        Assert.IsTrue(repositorio.ObtenerTodos().First().TieneRol(RolUsuario.AdministradorDelSistema));
    }

    [TestMethod]
    public void RepositorioUsuarioEnMemoria_ActualizarUsuarioNoExistente_LanzaExcepcion()
    {
        var repositorio = new RepositorioUsuarioEnMemoria();
        var usuario = CrearUsuarioValido("juan@example.com");

        Assert.ThrowsException<InvalidOperationException>(() => repositorio.Actualizar(usuario));
    }

    [TestMethod]
    public void RepositorioUsuarioEnMemoria_EliminarUsuarioExistente_EliminaUsuario()
    {
        var repositorio = new RepositorioUsuarioEnMemoria();
        var usuario = CrearUsuarioValido("juan@example.com");
        repositorio.Agregar(usuario);

        repositorio.Eliminar(usuario);

        Assert.AreEqual(0, repositorio.ObtenerTodos().Count);
    }

    [TestMethod]
    public void RepositorioUsuarioEnMemoria_EliminarUsuarioNoExistente_NoLanzaExcepcion()
    {
        var repositorio = new RepositorioUsuarioEnMemoria();
        var usuario = CrearUsuarioValido("juan@example.com");

        repositorio.Eliminar(usuario);

        Assert.AreEqual(0, repositorio.ObtenerTodos().Count);
    }

    [TestMethod]
    public void RepositorioUsuarioEnMemoria_ObtenerTodosSinUsuarios_DevuelveListaVacia()
    {
        var repositorio = new RepositorioUsuarioEnMemoria();

        var usuarios = repositorio.ObtenerTodos();

        Assert.AreEqual(0, usuarios.Count);
    }

    [TestMethod]
    public void RepositorioUsuarioEnMemoria_ObtenerTodos_DevuelveCopia()
    {
        var repositorio = new RepositorioUsuarioEnMemoria();
        var usuario = CrearUsuarioValido("juan@example.com");
        repositorio.Agregar(usuario);

        var usuarios1 = repositorio.ObtenerTodos();
        var usuarios2 = repositorio.ObtenerTodos();

        Assert.AreNotSame(usuarios1, usuarios2);
        Assert.AreEqual(usuarios1.Count, usuarios2.Count);
    }

    [TestMethod]
    public void RepositorioUsuarioEnMemoria_ObtenerPorEmailExistente_DevuelveUsuario()
    {
        var repositorio = new RepositorioUsuarioEnMemoria();
        var usuario = CrearUsuarioValido("juan@example.com");
        repositorio.Agregar(usuario);

        var usuarioEncontrado = repositorio.ObtenerPorEmail("juan@example.com");

        Assert.IsNotNull(usuarioEncontrado);
        Assert.AreSame(usuario, usuarioEncontrado);
    }

    [TestMethod]
    public void RepositorioUsuarioEnMemoria_ObtenerPorEmailNoExistente_DevuelveNull()
    {
        var repositorio = new RepositorioUsuarioEnMemoria();

        var usuarioEncontrado = repositorio.ObtenerPorEmail("noexiste@example.com");

        Assert.IsNull(usuarioEncontrado);
    }

    [TestMethod]
    public void RepositorioUsuarioEnMemoria_ObtenerPorEmailNoDistingueMayusculas_DevuelveUsuario()
    {
        var repositorio = new RepositorioUsuarioEnMemoria();
        var usuario = CrearUsuarioValido("Juan@Example.com");
        repositorio.Agregar(usuario);

        var usuarioEncontrado = repositorio.ObtenerPorEmail("juan@example.com");

        Assert.IsNotNull(usuarioEncontrado);
        Assert.AreSame(usuario, usuarioEncontrado);
    }

    private Equipo CrearEquipoValido(string nombre)
    {
        return new Equipo
        {
            Nombre = nombre,
            RankingFIFA = 1000,
            Confederacion = Confederacion.CONMEBOL
        };
    }

    private Estadio CrearEstadioValido(string nombre)
    {
        return new Estadio
        {
            Nombre = nombre,
            Ciudad = "Montevideo",
            Capacidad = 60000
        };
    }

    private Partido CrearPartidoValido()
    {
        return new Partido
        {
            Id = 1,
            Fecha = DateTime.Now,
            Estadio = CrearEstadioValido("Estadio Centenario"),
            EquipoLocal = CrearEquipoValido("Uruguay"),
            EquipoVisitante = CrearEquipoValido("Argentina"),
            Fase = FaseTorneo.FaseDeGrupos,
            Grupo = new Grupo("A")
        };
    }

    private Usuario CrearUsuarioValido(string email)
    {
        return new Usuario("Juan", "Pérez", email, new DateTime(1990, 1, 1));
    }
}
