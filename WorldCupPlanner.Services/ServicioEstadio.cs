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

    public void EditarEstadio(Estadio estadio)
    {
        var existente = _estadios.FirstOrDefault(e => e.Nombre == estadio.Nombre);
        if (existente == null)
            throw new Exception("El estadio no existe.");
        var index = _estadios.IndexOf(existente);
        _estadios[index] = estadio;
    }

    public void EliminarEstadio(Estadio estadio)
    {
        _estadios.Remove(estadio);
    }

    public List<Estadio> ObtenerEstadios()
    {
        return _estadios;
    }
}