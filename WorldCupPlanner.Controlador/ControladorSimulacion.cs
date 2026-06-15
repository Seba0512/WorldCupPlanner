using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.Controladores;

public class ControladorSimulacion
{
    private readonly ServicioSimulacion _servicioSimulacion;
    private readonly ServicioOrquestadorSimulacion _orquestadorSimulacion;
    private readonly ServicioUsuario? _servicioUsuario;

    public ControladorSimulacion(
        ServicioSimulacion servicioSimulacion,
        ServicioOrquestadorSimulacion orquestadorSimulacion,
        ServicioUsuario? servicioUsuario = null)
    {
        _servicioSimulacion = servicioSimulacion;
        _orquestadorSimulacion = orquestadorSimulacion;
        _servicioUsuario = servicioUsuario;
    }

    public IReadOnlyList<MotorSimulacionDisponible> ObtenerMotoresDisponibles() =>
        _servicioSimulacion.ObtenerMotoresDisponibles();

    public string ObtenerNombreMotor(MotorSimulacion motor) =>
        _servicioSimulacion.ObtenerNombreMotor(motor);

    public void SimularYPersistirPartidos(
        List<Partido> partidos,
        ConfiguracionTorneo config,
        UsuarioDTO? usuarioActual,
        EstadoTorneo estadoTorneo) =>
        _orquestadorSimulacion.SimularYPersistirPartidos(partidos, config, ResolverUsuario(usuarioActual), estadoTorneo);

    private Usuario? ResolverUsuario(UsuarioDTO? usuarioActual)
    {
        if (usuarioActual == null)
            return null;

        if (_servicioUsuario == null)
            throw new InvalidOperationException("El servicio de usuario no fue configurado.");

        return _servicioUsuario.ObtenerPorEmail(usuarioActual.Email)
               ?? throw new EntidadInexistenteExcepcion($"No existe un usuario con el email '{usuarioActual.Email}'.");
    }
}
