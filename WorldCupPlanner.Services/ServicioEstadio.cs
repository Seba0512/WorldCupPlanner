using WorldCupPlanner.Domain;

namespace WorldCupPlanner.Services;

public class ServicioEstadio
{
    private readonly List<Estadio> _estadios = new();

    public void AgregarEstadio(Estadio estadio)
    {
        _estadios.Add(estadio);
    }

    public List<Estadio> ObtenerEstadios()
    {
        return _estadios;
    }
}