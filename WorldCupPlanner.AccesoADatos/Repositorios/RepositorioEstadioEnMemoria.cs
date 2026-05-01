using WorldCupPlanner.Dominio;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioEstadioEnMemoria
{
    private readonly List<Estadio> _estadios = new();

    public void Agregar(Estadio estadio)
    {
        _estadios.Add(estadio);
    }

    public void Actualizar(Estadio estadio)
    {
        var indice = _estadios.FindIndex(e => ReferenceEquals(e, estadio));

        if (indice == -1)
        {
            throw new InvalidOperationException("El estadio no existe.");
        }

        _estadios[indice] = estadio;
    }

    public void Eliminar(Estadio estadio)
    {
        _estadios.Remove(estadio);
    }

    public List<Estadio> ObtenerTodos()
    {
        return _estadios;
    }
}
