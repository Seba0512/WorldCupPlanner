using System.Diagnostics.CodeAnalysis;
using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.ControladorTests;

[TestClass]
[ExcludeFromCodeCoverage]
public class FakesRepositoriosCoberturaTests
{
    [TestMethod]
    public void RepositorioTorneoArchivadoControladorFaseFake_GuardarYObtener_DevuelveLoGuardado()
    {
        // Arrange
        var repositorio = new RepositorioTorneoArchivadoControladorFaseFake();
        var torneo = new TorneoArchivado { Id = 1, GruposJson = "[]", PartidosEliminatoriosJson = "[]" };

        // Act
        Assert.IsNull(repositorio.Obtener());
        repositorio.Guardar(torneo);

        // Assert
        Assert.AreSame(torneo, repositorio.Obtener());
    }

    [TestMethod]
    public void RepositorioGrupoControladorFaseFake_AgregarActualizarEliminarBuscar_OperaSobreLaColeccion()
    {
        // Arrange
        var repositorio = new RepositorioGrupoControladorFaseFake();
        var grupo = new Grupo("A");

        // Act
        repositorio.Agregar(grupo);
        repositorio.Actualizar(grupo);
        var encontrados = repositorio.Buscar(_ => true);
        repositorio.Eliminar(grupo);

        // Assert
        Assert.AreEqual(1, encontrados.Count);
        Assert.AreEqual(0, repositorio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void RepositorioPartidoControladorFaseFake_ActualizarYEliminar_NoAlteranLaBusquedaIndebidamente()
    {
        // Arrange
        var repositorio = new RepositorioPartidoControladorFaseFake();
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
    public void RepositorioEstadioControladorFaseFake_ActualizarYEliminar_OperaSobreLaColeccion()
    {
        // Arrange
        var repositorio = new RepositorioEstadioControladorFaseFake();
        var estadio = FabricaEntidades.CrearEstadio("Estadio");
        repositorio.Agregar(estadio);

        // Act
        repositorio.Actualizar(estadio);
        var antesDeEliminar = repositorio.Buscar(_ => true).Count;
        repositorio.Eliminar(estadio);

        // Assert
        Assert.AreEqual(1, antesDeEliminar);
        Assert.AreEqual(0, repositorio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void RepositorioAuditoriaFake_OperacionesCompletas_GestionanIdYColeccion()
    {
        // Arrange
        var repositorio = new RepositorioAuditoriaFake();
        var original = new Auditoria(repositorio.SiguienteId(), "usuario", "accion", "detalle");
        var actualizada = new Auditoria(original.Id, "usuario", "accion", "detalle actualizado");
        var inexistente = new Auditoria(999, "usuario", "accion", "detalle");

        // Act
        repositorio.Agregar(original);
        repositorio.Actualizar(actualizada);
        repositorio.Actualizar(inexistente);
        var encontradas = repositorio.Buscar(a => a.Id == original.Id);
        repositorio.Eliminar(actualizada);

        // Assert
        Assert.AreEqual(1, original.Id);
        Assert.AreSame(actualizada, encontradas.Single());
        Assert.AreEqual(0, repositorio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void RepositorioGenericoControladorFixtureFake_Actualizar_AgregaCuandoNoExisteYReemplazaCuandoExiste()
    {
        // Arrange
        var repositorio = new RepositorioGenericoControladorFixtureFake<Equipo>();
        var existente = FabricaEntidades.CrearEquipo("Existente");
        var nuevo = FabricaEntidades.CrearEquipo("Nuevo");
        repositorio.Agregar(existente);

        // Act
        repositorio.Actualizar(nuevo);
        repositorio.Actualizar(existente);
        var todos = repositorio.Buscar(_ => true);
        repositorio.Eliminar(existente);

        // Assert
        Assert.AreEqual(2, todos.Count);
        Assert.AreEqual(1, repositorio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void RepositorioDashboardFake_ActualizarYEliminar_OperaSobreLaColeccion()
    {
        // Arrange
        var repositorio = new RepositorioDashboardFake<Equipo>();
        var equipo = FabricaEntidades.CrearEquipo("Equipo");
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
    public void RepositorioGenericoControladorSimulacionFake_ActualizarYEliminar_OperaSobreLaColeccion()
    {
        // Arrange
        var repositorio = new RepositorioGenericoControladorSimulacionFake<Equipo>();
        var equipo = FabricaEntidades.CrearEquipo("Equipo");
        repositorio.Agregar(equipo);

        // Act
        repositorio.Actualizar(equipo);
        var antesDeEliminar = repositorio.Buscar(_ => true).Count;
        repositorio.Eliminar(equipo);

        // Assert
        Assert.AreEqual(1, antesDeEliminar);
        Assert.AreEqual(0, repositorio.Buscar(_ => true).Count);
    }
}
