using WorldCupPlanner.Domain;

namespace WorldCupPlanner.Services;

public class ServicioEstadio
{
    private readonly List<Estadio> _estadios = new();

    public void AgregarEstadio(Estadio estadio)
    {
        if (!estadio.EsValido())
            throw new Exception("El estadio no es válido.");
        if (_estadios.Any(e => e.Nombre == estadio.Nombre))
            throw new Exception("Ya existe un estadio con ese nombre.");
        _estadios.Add(estadio);
    }

    public List<Estadio> ObtenerEstadios()
    {
        return _estadios;
    }
}