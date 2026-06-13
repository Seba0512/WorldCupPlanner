using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.Controladores;

public class ControladorEquipo
{
    private readonly ServicioEquipo _servicioEquipo;

    public ControladorEquipo(ServicioEquipo servicioEquipo)
    {
        _servicioEquipo = servicioEquipo;
    }

    public List<Equipo> Listar() =>
        _servicioEquipo.Buscar(_ => true);

    public List<Equipo> Buscar(Func<Equipo, bool> predicado) =>
        _servicioEquipo.Buscar(predicado);

    public Equipo ObtenerParaEdicion(string nombre) =>
        _servicioEquipo.ClonarParaEdicion(nombre);

    public void Crear(NuevoEquipoDto equipo, Usuario? usuarioActual = null) =>
        _servicioEquipo.Crear(equipo, usuarioActual);

    public void Agregar(Equipo equipo, Usuario? usuarioActual = null) =>
        _servicioEquipo.Agregar(equipo, usuarioActual);

    public void Actualizar(Equipo equipo, string nombreOriginal, Usuario? usuarioActual = null) =>
        _servicioEquipo.EditarEquipo(equipo, nombreOriginal, usuarioActual);

    public void Eliminar(Equipo equipo, Usuario? usuarioActual = null) =>
        _servicioEquipo.Eliminar(equipo, usuarioActual);

    public List<Equipo> CompletarEquipos(int semilla, Usuario? usuarioActual = null) =>
        _servicioEquipo.CompletarEquipos(semilla, usuarioActual);

    public string ObtenerMensajeComplecion(int cantidadGenerados) =>
        _servicioEquipo.ObtenerMensajeComplecion(cantidadGenerados);

    public int ObtenerMaximoEquipos() =>
        _servicioEquipo.ObtenerMaximoEquipos();

    public IReadOnlyDictionary<Confederacion, int> ObtenerCuposMaximos() =>
        _servicioEquipo.ObtenerCuposMaximos();

    public Dictionary<Confederacion, (int registrados, int cupo)> ObtenerCuposConOcupacion(List<Equipo> equipos) =>
        _servicioEquipo.ObtenerCuposConOcupacion(equipos);

    public int ObtenerCantidadFaltante(int cantidadInscritos) =>
        _servicioEquipo.ObtenerCantidadFaltante(cantidadInscritos);

    public bool FixtureYaFueGenerado =>
        _servicioEquipo.FixtureYaFueGenerado;

    public void RegistrarFixtureGenerado(Fixture fixture) =>
        _servicioEquipo.RegistrarFixtureGenerado(fixture);

    public void LimpiarFixtureGenerado() =>
        _servicioEquipo.LimpiarFixtureGenerado();
}
