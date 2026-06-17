using System.Diagnostics.CodeAnalysis;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
[ExcludeFromCodeCoverage]
public class TestDoublesCoberturaTests
{
    [TestMethod]
    public void RepositorioAuditoriaFake_OperacionesCompletas_GestionanIdYColeccion()
    {
        // Arrange
        var repositorio = new RepositorioAuditoriaFake();
        var entrada = new Auditoria(repositorio.SiguienteId(), "usuario", "accion", "detalle");

        // Act
        repositorio.Agregar(entrada);
        repositorio.Actualizar(entrada);
        var encontradas = repositorio.Buscar(_ => true);
        repositorio.Eliminar(entrada);

        // Assert
        Assert.AreEqual(1, entrada.Id);
        Assert.AreEqual(1, encontradas.Count);
        Assert.AreEqual(0, repositorio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void RepositorioNotificacionStub_OperacionesCompletas_GestionanColeccion()
    {
        // Arrange
        var repositorio = new RepositorioNotificacionStub();
        var notificacion = new Notificacion { Mensaje = "Hola", UsuarioDestinatarioId = 7 };

        // Act
        repositorio.Agregar(notificacion);
        repositorio.Actualizar(notificacion);
        var porUsuario = repositorio.ObtenerPorUsuario(7);
        repositorio.Eliminar(notificacion);

        // Assert
        Assert.AreEqual(1, notificacion.Id);
        Assert.AreEqual(1, porUsuario.Count);
        Assert.AreEqual(0, repositorio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void RepositorioNotificacionStub_ActualizarInexistente_LanzaExcepcion()
    {
        // Arrange
        var repositorio = new RepositorioNotificacionStub();
        var notificacion = new Notificacion { Id = 999, Mensaje = "Inexistente" };

        // Act & Assert
        Assert.ThrowsException<EntidadInexistenteExcepcion>(() => repositorio.Actualizar(notificacion));
    }

    [TestMethod]
    public void RepositorioEquipoStub_ActualizarYEliminar_OperaSobreLaColeccion()
    {
        // Arrange
        var repositorio = new RepositorioEquipoStub();
        var equipo = new Equipo { Nombre = "Uruguay", RankingFIFA = 1500 };
        repositorio.Agregar(equipo);

        // Act
        repositorio.Actualizar(equipo);
        var antesDeEliminar = repositorio.Buscar(_ => true).Count;
        repositorio.Eliminar(equipo);

        // Assert
        Assert.AreEqual(1, antesDeEliminar);
        Assert.AreEqual(0, repositorio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void RepositorioEquipoStub_ActualizarInexistente_LanzaExcepcion()
    {
        // Arrange
        var repositorio = new RepositorioEquipoStub();
        var equipo = new Equipo { Nombre = "Inexistente", RankingFIFA = 1500 };

        // Act & Assert
        Assert.ThrowsException<EntidadInexistenteExcepcion>(() => repositorio.Actualizar(equipo));
    }

    [TestMethod]
    public void RepositorioPartidoStub_ActualizarYEliminar_OperaSobreLaColeccion()
    {
        // Arrange
        var repositorio = new RepositorioPartidoStub();
        var partido = new Partido { Id = 1 };
        repositorio.Agregar(partido);

        // Act
        repositorio.Actualizar(partido);
        var antesDeEliminar = repositorio.Buscar(_ => true).Count;
        repositorio.Eliminar(partido);

        // Assert
        Assert.AreEqual(1, antesDeEliminar);
        Assert.AreEqual(0, repositorio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void RepositorioPartidoStub_ActualizarInexistente_LanzaExcepcion()
    {
        // Arrange
        var repositorio = new RepositorioPartidoStub();
        var partido = new Partido { Id = 999 };

        // Act & Assert
        Assert.ThrowsException<EntidadInexistenteExcepcion>(() => repositorio.Actualizar(partido));
    }
}
