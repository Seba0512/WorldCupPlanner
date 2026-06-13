using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Controladores;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public class ControladorNotificacionTests
{
    [TestMethod]
    public void ObtenerNoLeidas_DevuelvePendientesDelUsuario()
    {
        var repositorio = new RepositorioNotificacionControladorFake();
        var controlador = CrearControlador(repositorio);
        var periodista = CrearUsuario(1);
        repositorio.Agregar(new Notificacion { UsuarioDestinatarioId = periodista.Id, EsLeida = false });
        repositorio.Agregar(new Notificacion { UsuarioDestinatarioId = periodista.Id, EsLeida = true });
        repositorio.Agregar(new Notificacion { UsuarioDestinatarioId = 99, EsLeida = false });

        var notificaciones = controlador.ObtenerNoLeidas(periodista);

        Assert.AreEqual(1, notificaciones.Count);
        Assert.AreEqual(periodista.Id, notificaciones[0].UsuarioDestinatarioId);
        Assert.IsFalse(notificaciones[0].EsLeida);
    }

    [TestMethod]
    public void MarcarComoLeida_DelegaEnServicio()
    {
        var repositorio = new RepositorioNotificacionControladorFake();
        var controlador = CrearControlador(repositorio);
        var periodista = CrearUsuario(2);
        var notificacion = new Notificacion { UsuarioDestinatarioId = periodista.Id, EsLeida = false };
        repositorio.Agregar(notificacion);

        controlador.MarcarComoLeida(notificacion.Id, periodista);

        Assert.IsTrue(repositorio.ObtenerPorUsuario(periodista.Id).Single().EsLeida);
    }

    [TestMethod]
    public void MarcarComoLeida_CuandoNoPerteneceAlUsuario_MantieneValidacionDelServicio()
    {
        var repositorio = new RepositorioNotificacionControladorFake();
        var controlador = CrearControlador(repositorio);
        var periodista = CrearUsuario(3);
        var otroPeriodista = CrearUsuario(4);
        var notificacion = new Notificacion { UsuarioDestinatarioId = periodista.Id, EsLeida = false };
        repositorio.Agregar(notificacion);

        Assert.ThrowsException<AccesoDenegadoExcepcion>(() =>
            controlador.MarcarComoLeida(notificacion.Id, otroPeriodista));
    }

    [TestMethod]
    public void MarcarTodasComoLeidas_MarcaTodasLasPendientesDelUsuario()
    {
        var repositorio = new RepositorioNotificacionControladorFake();
        var controlador = CrearControlador(repositorio);
        var periodista = CrearUsuario(5);
        repositorio.Agregar(new Notificacion { UsuarioDestinatarioId = periodista.Id, EsLeida = false });
        repositorio.Agregar(new Notificacion { UsuarioDestinatarioId = periodista.Id, EsLeida = false });
        repositorio.Agregar(new Notificacion { UsuarioDestinatarioId = 99, EsLeida = false });

        controlador.MarcarTodasComoLeidas(periodista);

        Assert.AreEqual(0, controlador.ObtenerNoLeidas(periodista).Count);
        Assert.AreEqual(1, repositorio.ObtenerPorUsuario(99).Count);
        Assert.IsFalse(repositorio.ObtenerPorUsuario(99).Single().EsLeida);
    }

    private static ControladorNotificacion CrearControlador(IRepositorioNotificacion repositorio)
    {
        var servicio = new ServicioNotificacion(repositorio);
        return new ControladorNotificacion(servicio);
    }

    private static Usuario CrearUsuario(int id)
    {
        var usuario = new Usuario(new DatosUsuario
        {
            Nombre = "Periodista",
            Apellido = id.ToString(),
            Email = $"periodista{id}@test.com",
            FechaNacimiento = "01-01-1990"
        });
        usuario.Id = id;
        usuario.AgregarRol(RolUsuario.Periodista);
        return usuario;
    }
}

file sealed class RepositorioNotificacionControladorFake : IRepositorioNotificacion
{
    private readonly List<Notificacion> _notificaciones = new();
    private int _siguienteId = 1;

    public void Agregar(Notificacion notificacion)
    {
        notificacion.Id = _siguienteId++;
        _notificaciones.Add(notificacion);
    }

    public void Actualizar(Notificacion notificacion)
    {
        var indice = _notificaciones.FindIndex(n => n.Id == notificacion.Id);
        if (indice == -1)
            throw new EntidadInexistenteExcepcion("La notificacion no existe.");
        _notificaciones[indice] = notificacion;
    }

    public void Eliminar(Notificacion notificacion) => _notificaciones.Remove(notificacion);

    public List<Notificacion> Buscar(Func<Notificacion, bool> predicado) =>
        _notificaciones.Where(predicado).ToList();

    public List<Notificacion> ObtenerPorUsuario(int usuarioId) =>
        _notificaciones.Where(n => n.UsuarioDestinatarioId == usuarioId).ToList();
}
