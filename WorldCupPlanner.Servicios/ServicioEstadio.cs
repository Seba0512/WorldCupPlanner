using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.AccesoADatos.Repositorios;
using  WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.Servicios;

public class ServicioEstadio
{
    private readonly IRepositorio<Estadio> _repositorioEstadio;

    public ServicioEstadio()
    {
        _repositorioEstadio = new RepositorioEstadioEnMemoria();
    }

    public ServicioEstadio(IRepositorio<Estadio> repositorioEstadio)
    {
        _repositorioEstadio = repositorioEstadio;
    }

    public void AgregarEstadio(Estadio estadio)
    {
        if (!estadio.EsValido())
            throw new EntidadInvalidaExcepcion("El estadio no es valido.");
        if (_repositorioEstadio.Buscar(e => e.Nombre == estadio.Nombre).Count > 0)
            throw new EntidadYaExistenteExcepcion("Ya existe un estadio con ese nombre.");
        _repositorioEstadio.Agregar(estadio);
    }

    public void EditarEstadio(Estadio estadio)
    {
        var existente = _repositorioEstadio.Buscar(e => e == estadio).FirstOrDefault();
        if (existente == null)
            throw new EntidadInvalidaExcepcion("El estadio no existe.");
        _repositorioEstadio.Actualizar(estadio);
    }

    public void EliminarEstadio(Estadio estadio)
    {
        _repositorioEstadio.Eliminar(estadio);
    }

    public List<Estadio> ObtenerEstadios()
    {
        return _repositorioEstadio.Buscar(_ => true);
    }
}
