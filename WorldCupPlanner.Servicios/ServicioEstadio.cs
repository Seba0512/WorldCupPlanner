using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.AccesoADatos.Repositorios;
using  WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.Servicios;

public class ServicioEstadio
{
    private readonly IRepositorio<Estadio> _repositorioEstadio;
    private readonly ServicioAuditoria? _auditoria;

    public ServicioEstadio()
    {
        _repositorioEstadio = new RepositorioEstadioEnMemoria();
    }

    public ServicioEstadio(IRepositorio<Estadio> repositorioEstadio, ServicioAuditoria? auditoria = null)
    {
        _repositorioEstadio = repositorioEstadio;
        _auditoria = auditoria;
    }

    public void AgregarEstadio(Estadio estadio)
    {
        if (!estadio.EsValido())
            throw new EntidadInvalidaExcepcion("El estadio no es valido.");
        if (_repositorioEstadio.Buscar(e => e.Nombre == estadio.Nombre).Count > 0)
            throw new EntidadYaExistenteExcepcion("Ya existe un estadio con ese nombre.");
        _repositorioEstadio.Agregar(estadio);
        _auditoria?.Registrar("Sistema", "Alta", $"Estadio: {estadio.Nombre}, Ciudad: {estadio.Ciudad}, Capacidad: {estadio.Capacidad}");
    }

    public void EditarEstadio(string nombreOriginal, Estadio estadio)
    {
        if (!estadio.EsValido())
            throw new EntidadInvalidaExcepcion("El estadio no es valido.");
        var existente = _repositorioEstadio.Buscar(e => ReferenceEquals(e, estadio) || e.Nombre == nombreOriginal).FirstOrDefault();
        if (existente == null)
            throw new EntidadInvalidaExcepcion("El estadio no existe.");
        existente.Nombre = estadio.Nombre;
        existente.Ciudad = estadio.Ciudad;
        existente.Capacidad = estadio.Capacidad;
        existente.Descripcion = estadio.Descripcion;
        _repositorioEstadio.Actualizar(existente);
        _auditoria?.Registrar("Sistema", "Edicion", $"Original: {nombreOriginal} -> Nuevo: {estadio.Nombre}, Ciudad: {estadio.Ciudad}, Capacidad: {estadio.Capacidad}");
    }

    public void EliminarEstadio(Estadio estadio)
    {
        _repositorioEstadio.Eliminar(estadio);
        _auditoria?.Registrar("Sistema", "Baja", $"Estadio: {estadio.Nombre}, Ciudad: {estadio.Ciudad}");
    }

    public List<Estadio> ObtenerEstadios()
    {
        return _repositorioEstadio.Buscar(_ => true);
    }
}
