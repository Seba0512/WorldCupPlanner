using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioEstadioEnMemoria : IRepositorio<Estadio>
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
    
    public List<Estadio> Buscar(Func<Estadio, bool> predicado)
    {
        var resultados = new List<Estadio>();

        foreach (var estadio in _estadios)
        {
            if (predicado(estadio))
            {
                resultados.Add(estadio);
            }
        }

        return resultados;
    }
}
