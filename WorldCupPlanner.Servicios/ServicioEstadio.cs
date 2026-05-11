using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.Servicios;

public class ServicioEstadio
{
    private readonly IRepositorio<Estadio> _repositorioEstadio;
    private readonly ServicioAuditoria? _auditoria;
    private readonly ServicioAutorizacion _autorizacion;
    private Fixture? _fixtureGenerado;

    public ServicioEstadio(IRepositorio<Estadio> repositorioEstadio, ServicioAuditoria? auditoria = null, ServicioAutorizacion? autorizacion = null)
    {
        _repositorioEstadio = repositorioEstadio;
        _auditoria = auditoria;
        _autorizacion = autorizacion ?? new ServicioAutorizacion();
    }

    public void AgregarEstadio(Estadio estadio, Usuario? solicitante = null)
    {
        if (solicitante != null)
            _autorizacion.VerificarEsAdministrador(solicitante);
        if (!estadio.EsValido())
            throw new EntidadInvalidaExcepcion("El estadio no es valido.");
        if (_repositorioEstadio.Buscar(e => NombresIguales(e.Nombre, estadio.Nombre)).Count > 0)
            throw new EntidadYaExistenteExcepcion("Ya existe un estadio con ese nombre.");
        _repositorioEstadio.Agregar(estadio);
        _auditoria?.Registrar("Sistema", "Alta", $"Estadio: {estadio.Nombre}, Ciudad: {estadio.Ciudad}, Capacidad: {estadio.Capacidad}");
    }

    public void EditarEstadio(string nombreOriginal, Estadio estadio, Usuario? solicitante = null)
    {
        if (solicitante != null)
            _autorizacion.VerificarEsAdministrador(solicitante);
        if (!estadio.EsValido())
            throw new EntidadInvalidaExcepcion("El estadio no es valido.");
        var existente = _repositorioEstadio.Buscar(e => ReferenceEquals(e, estadio) || NombresIguales(e.Nombre, nombreOriginal)).FirstOrDefault();
        if (existente == null)
            throw new EntidadInvalidaExcepcion("El estadio no existe.");
        var conflicto = _repositorioEstadio.Buscar(e => NombresIguales(e.Nombre, estadio.Nombre) && !ReferenceEquals(e, existente)).FirstOrDefault();
        if (conflicto != null)
            throw new EntidadYaExistenteExcepcion("Ya existe un estadio con ese nombre.");
        existente.Nombre = estadio.Nombre;
        existente.Ciudad = estadio.Ciudad;
        existente.Capacidad = estadio.Capacidad;
        existente.Descripcion = estadio.Descripcion;
        _repositorioEstadio.Actualizar(existente);
        _auditoria?.Registrar("Sistema", "Edicion", $"Original: {nombreOriginal} -> Nuevo: {estadio.Nombre}, Ciudad: {estadio.Ciudad}, Capacidad: {estadio.Capacidad}");
    }

    public void EliminarEstadio(Estadio estadio, Usuario? solicitante = null)
    {
        if (solicitante != null)
            _autorizacion.VerificarEsAdministrador(solicitante);
        ValidarEstadioNoEstaEnFixture(estadio);

        _repositorioEstadio.Eliminar(estadio);
        _auditoria?.Registrar("Sistema", "Baja", $"Estadio: {estadio.Nombre}, Ciudad: {estadio.Ciudad}");
    }

    public void RegistrarFixtureGenerado(Fixture fixture)
    {
        _fixtureGenerado = fixture;
    }

    public List<Estadio> ObtenerEstadios()
    {
        return _repositorioEstadio.Buscar(_ => true);
    }

    private void ValidarEstadioNoEstaEnFixture(Estadio estadio)
    {
        if (_fixtureGenerado == null)
        {
            return;
        }

        if (_fixtureGenerado.Partidos.Any(p => p.Estadio?.Nombre == estadio.Nombre))
        {
            throw new EntidadInvalidaExcepcion(
                "No se puede eliminar un estadio que existe en un fixture generado.");
        }
    }

    private static bool NombresIguales(string a, string b) =>
        string.Equals(a.Trim(), b.Trim(), StringComparison.OrdinalIgnoreCase);
}
