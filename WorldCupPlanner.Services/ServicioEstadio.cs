using WorldCupPlanner.DataAccess.Repositorios;
using WorldCupPlanner.Domain;

namespace WorldCupPlanner.Services;

public class ServicioEstadio
{
    private readonly RepositorioEstadioEnMemoria _repositorioEstadio;

    public ServicioEstadio()
    {
        _repositorioEstadio = new RepositorioEstadioEnMemoria();
    }

    public ServicioEstadio(RepositorioEstadioEnMemoria repositorioEstadio)
    {
        _repositorioEstadio = repositorioEstadio;
    }

    public void AgregarEstadio(Estadio estadio)
    {
        if (!estadio.EsValido())
            throw new Exception("El estadio no es valido.");
        if (_repositorioEstadio.ObtenerTodos().Any(e => e.Nombre == estadio.Nombre))
            throw new Exception("Ya existe un estadio con ese nombre.");
        _repositorioEstadio.Agregar(estadio);
    }

    public void EditarEstadio(Estadio estadio)
    {
        var existente = _repositorioEstadio.ObtenerTodos().FirstOrDefault(e => e == estadio);
        if (existente == null)
            throw new Exception("El estadio no existe.");
        _repositorioEstadio.Actualizar(estadio);
    }

    public void EliminarEstadio(Estadio estadio)
    {
        _repositorioEstadio.Eliminar(estadio);
    }

    public List<Estadio> ObtenerEstadios()
    {
        return _repositorioEstadio.ObtenerTodos();
    }
}
