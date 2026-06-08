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
        _auditoria?.Registrar(
            solicitante != null ? $"{solicitante.Nombre} {solicitante.Apellido}" : "Sistema",
            "Alta",
            $"Estadio: {estadio.Nombre}, Ciudad: {estadio.Ciudad}, Capacidad: {estadio.Capacidad}");
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

        ValidarEdicionPermitida(existente);

        var conflicto = _repositorioEstadio.Buscar(e =>
            NombresIguales(e.Nombre, estadio.Nombre) &&
            !ReferenceEquals(e, existente) &&
            !NombresIguales(e.Nombre, nombreOriginal)).FirstOrDefault();
        if (conflicto != null)
            throw new EntidadYaExistenteExcepcion("Ya existe un estadio con ese nombre.");

        if (_repositorioEstadio is IRepositorioConClaveEditable<Estadio> repositorioConClaveEditable)
        {
            repositorioConClaveEditable.ActualizarClave(nombreOriginal, estadio);
        }
        else if (!string.Equals(nombreOriginal, estadio.Nombre, StringComparison.Ordinal))
        {
            _repositorioEstadio.Eliminar(existente);
            _repositorioEstadio.Agregar(estadio);
        }
        else
        {
            existente.Ciudad = estadio.Ciudad;
            existente.Capacidad = estadio.Capacidad;
            existente.Descripcion = estadio.Descripcion;
            _repositorioEstadio.Actualizar(existente);
        }

        _auditoria?.Registrar(
            solicitante != null ? $"{solicitante.Nombre} {solicitante.Apellido}" : "Sistema",
            "Edicion",
            $"Original: {nombreOriginal} -> Nuevo: {estadio.Nombre}, Ciudad: {estadio.Ciudad}, Capacidad: {estadio.Capacidad}");
    }

    public void EliminarEstadio(Estadio estadio, Usuario? solicitante = null)
    {
        if (solicitante != null)
            _autorizacion.VerificarEsAdministrador(solicitante);
        ValidarEstadioNoEstaEnFixture(estadio);

        _repositorioEstadio.Eliminar(estadio);
        _auditoria?.Registrar(
            solicitante != null ? $"{solicitante.Nombre} {solicitante.Apellido}" : "Sistema",
            "Baja",
            $"Estadio: {estadio.Nombre}, Ciudad: {estadio.Ciudad}");
    }

    public void RegistrarFixtureGenerado(Fixture fixture)
    {
        _fixtureGenerado = fixture;
    }

    public void LimpiarFixtureGenerado()
    {
        _fixtureGenerado = null;
    }

    public List<Estadio> ObtenerEstadios()
    {
        return _repositorioEstadio.Buscar(_ => true);
    }

    private void ValidarEstadioNoEstaEnFixture(Estadio estadio)
    {
        if (EstadioEstaEnFixture(estadio))
        {
            throw new EntidadInvalidaExcepcion(
                "No se puede eliminar un estadio que existe en un fixture generado.");
        }
    }

    private void ValidarEdicionPermitida(Estadio estadio)
    {
        if (EstadioEstaEnFixture(estadio))
        {
            throw new EntidadInvalidaExcepcion(
                "No se puede editar un estadio que existe en un fixture generado.");
        }
    }

    private bool EstadioEstaEnFixture(Estadio estadio)
    {
        if (_fixtureGenerado == null)
        {
            return false;
        }

        return _fixtureGenerado.Partidos.Any(p => EsMismoEstadio(p.Estadio, estadio));
    }

    private static bool EsMismoEstadio(Estadio? estadioEnFixture, Estadio estadio)
    {
        if (estadioEnFixture == null)
        {
            return false;
        }

        if (estadioEnFixture.Id > 0 && estadio.Id > 0)
        {
            return estadioEnFixture.Id == estadio.Id;
        }

        return NombresIguales(estadioEnFixture.Nombre, estadio.Nombre);
    }

    private static bool NombresIguales(string a, string b) =>
        string.Equals(a.Trim(), b.Trim(), StringComparison.OrdinalIgnoreCase);
}
