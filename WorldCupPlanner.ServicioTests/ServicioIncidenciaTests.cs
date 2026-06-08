using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public class ServicioIncidenciaTests
{
    private static Equipo CrearEquipo(string nombre = "Uruguay")
        => new Equipo { Nombre = nombre, Confederacion = Confederacion.CONMEBOL, RankingFIFA = 1000 };

    [TestMethod]
    public void AgregarTarjeta_CuandoTipoAmarilla_CreasTarjetaAmarilla()
    {
        // Arrange
        var repositorio = new RepositorioIncidenciaStub();
        var servicio = new ServicioIncidencia(repositorio);

        // Act
        servicio.AgregarTarjeta(1, TipoTarjeta.Amarilla, 30, CrearEquipo());

        // Assert
        var incidencias = repositorio.ObtenerPorPartido(1);
        Assert.AreEqual(1, incidencias.Count);
        Assert.IsInstanceOfType(incidencias[0], typeof(TarjetaAmarilla));
    }

    [TestMethod]
    public void AgregarTarjeta_CuandoTipoRoja_CreasTarjetaRoja()
    {
        // Arrange
        var repositorio = new RepositorioIncidenciaStub();
        var servicio = new ServicioIncidencia(repositorio);

        // Act
        servicio.AgregarTarjeta(1, TipoTarjeta.Roja, 45, CrearEquipo());

        // Assert
        var incidencias = repositorio.ObtenerPorPartido(1);
        Assert.AreEqual(1, incidencias.Count);
        Assert.IsInstanceOfType(incidencias[0], typeof(TarjetaRoja));
    }

    [TestMethod]
    public void AgregarTarjeta_CuandoMinutoCero_LanzaExcepcion()
    {
        // Arrange
        var repositorio = new RepositorioIncidenciaStub();
        var servicio = new ServicioIncidencia(repositorio);

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicio.AgregarTarjeta(1, TipoTarjeta.Amarilla, 0, CrearEquipo()));
    }

    [TestMethod]
    public void AgregarTarjeta_CuandoMinutoMayorA90_LanzaExcepcion()
    {
        // Arrange
        var repositorio = new RepositorioIncidenciaStub();
        var servicio = new ServicioIncidencia(repositorio);

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicio.AgregarTarjeta(1, TipoTarjeta.Roja, 91, CrearEquipo()));
    }

    [TestMethod]
    public void AgregarTarjeta_CuandoMinutoEsUno_NoLanzaExcepcion()
    {
        // Arrange
        var repositorio = new RepositorioIncidenciaStub();
        var servicio = new ServicioIncidencia(repositorio);

        // Act
        servicio.AgregarTarjeta(1, TipoTarjeta.Amarilla, 1, CrearEquipo());

        // Assert
        Assert.AreEqual(1, repositorio.ObtenerPorPartido(1).Count);
    }

    [TestMethod]
    public void AgregarTarjeta_CuandoMinutoEsNoventa_NoLanzaExcepcion()
    {
        // Arrange
        var repositorio = new RepositorioIncidenciaStub();
        var servicio = new ServicioIncidencia(repositorio);

        // Act
        servicio.AgregarTarjeta(1, TipoTarjeta.Roja, 90, CrearEquipo());

        // Assert
        Assert.AreEqual(1, repositorio.ObtenerPorPartido(1).Count);
    }

    [TestMethod]
    public void AgregarTarjeta_PersisteTodosLosDatos()
    {
        // Arrange
        var repositorio = new RepositorioIncidenciaStub();
        var servicio = new ServicioIncidencia(repositorio);
        var equipo = CrearEquipo("Brasil");

        // Act
        servicio.AgregarTarjeta(5, TipoTarjeta.Amarilla, 72, equipo);

        // Assert
        var incidencia = repositorio.ObtenerPorPartido(5).First();
        Assert.AreEqual(5, incidencia.PartidoId);
        Assert.AreEqual(72, incidencia.MinutoDelPartido);
        Assert.AreEqual("Brasil", incidencia.Equipo!.Nombre);
    }

    [TestMethod]
    public void ObtenerIncidencias_ConVariasIncidencias_RetornaSoloLasDelPartido()
    {
        // Arrange
        var repositorio = new RepositorioIncidenciaStub();
        var servicio = new ServicioIncidencia(repositorio);
        servicio.AgregarTarjeta(10, TipoTarjeta.Amarilla, 10, CrearEquipo());
        servicio.AgregarTarjeta(10, TipoTarjeta.Roja, 55, CrearEquipo());
        servicio.AgregarTarjeta(99, TipoTarjeta.Amarilla, 30, CrearEquipo());

        // Act
        var resultado = servicio.ObtenerIncidencias(10);

        // Assert
        Assert.AreEqual(2, resultado.Count);
        Assert.IsTrue(resultado.All(i => i.PartidoId == 10));
    }

    [TestMethod]
    public void EliminarIncidencia_CuandoExiste_LaEliminaDelRepositorio()
    {
        // Arrange
        var repositorio = new RepositorioIncidenciaStub();
        var servicio = new ServicioIncidencia(repositorio);
        servicio.AgregarTarjeta(1, TipoTarjeta.Amarilla, 20, CrearEquipo());
        var incidenciaId = repositorio.ObtenerPorPartido(1).First().Id;

        // Act
        servicio.EliminarIncidencia(incidenciaId);

        // Assert
        Assert.AreEqual(0, repositorio.ObtenerPorPartido(1).Count);
    }

    [TestMethod]
    public void EliminarIncidencia_CuandoNoExiste_LanzaExcepcion()
    {
        // Arrange
        var repositorio = new RepositorioIncidenciaStub();
        var servicio = new ServicioIncidencia(repositorio);

        // Act & Assert
        Assert.ThrowsException<EntidadInexistenteExcepcion>(() =>
            servicio.EliminarIncidencia(999));
    }
}

file sealed class RepositorioIncidenciaStub : IRepositorioIncidencia
{
    private readonly List<Incidencia> _incidencias = new();
    private int _siguienteId = 1;

    public void Agregar(Incidencia entidad)
    {
        entidad.Id = _siguienteId++;
        _incidencias.Add(entidad);
    }

    public void Actualizar(Incidencia entidad)
    {
        var indice = _incidencias.FindIndex(i => i.Id == entidad.Id);
        if (indice == -1)
            throw new EntidadInexistenteExcepcion("La incidencia no existe.");
        _incidencias[indice] = entidad;
    }

    public void Eliminar(Incidencia entidad)
    {
        var existente = _incidencias.FirstOrDefault(i => i.Id == entidad.Id)
            ?? throw new EntidadInexistenteExcepcion("La incidencia no existe.");
        _incidencias.Remove(existente);
    }

    public List<Incidencia> Buscar(Func<Incidencia, bool> predicado)
        => _incidencias.Where(predicado).ToList();

    public List<Incidencia> ObtenerPorPartido(int partidoId)
        => _incidencias.Where(i => i.PartidoId == partidoId).ToList();
}
