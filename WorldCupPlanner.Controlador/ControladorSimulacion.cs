using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.Controladores;

public class ControladorSimulacion
{
    private readonly ServicioSimulacion _servicioSimulacion;
    private readonly ServicioOrquestadorSimulacion _orquestadorSimulacion;

    public ControladorSimulacion(
        ServicioSimulacion servicioSimulacion,
        ServicioOrquestadorSimulacion orquestadorSimulacion)
    {
        _servicioSimulacion = servicioSimulacion;
        _orquestadorSimulacion = orquestadorSimulacion;
    }

    public IReadOnlyList<MotorSimulacionDisponible> ObtenerMotoresDisponibles() =>
        _servicioSimulacion.ObtenerMotoresDisponibles();

    public string ObtenerNombreMotor(MotorSimulacion motor) =>
        _servicioSimulacion.ObtenerNombreMotor(motor);

    public void SimularYPersistirPartidos(
        List<Partido> partidos,
        ConfiguracionTorneo config,
        Usuario? usuarioActual,
        EstadoTorneo estadoTorneo) =>
        _orquestadorSimulacion.SimularYPersistirPartidos(partidos, config, usuarioActual, estadoTorneo);
}
