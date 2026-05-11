using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioPartidoEnMemoria : IRepositorio<Partido>
{
    private readonly List<Partido> _partidos = new();

    public void Agregar(Partido partido)
    {
        _partidos.Add(partido);
    }

    public void Actualizar(Partido partido)
    {
        var indice = _partidos.FindIndex(p => ReferenceEquals(p, partido));

        if (indice == -1)
        {
            throw new InvalidOperationException("El partido no existe.");
        }

        _partidos[indice] = partido;
    }

    public void Eliminar(Partido partido)
    {
        _partidos.Remove(partido);
    }

    public List<Partido> Buscar(Func<Partido, bool> predicado)
    {
        var resultados = new List<Partido>();

        foreach (var partido in _partidos)
        {
            if (predicado(partido))
            {
                resultados.Add(partido);
            }
        }

        return resultados;
    }
}
