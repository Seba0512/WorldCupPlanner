using WorldCupPlanner.AccesoADatos.Interfaces;
using  WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioGrupoEnMemoria : IRepositorio<Grupo>
{
    private readonly List<Grupo> _grupos = new();

    public void Agregar(Grupo grupo)
    {
        _grupos.Add(grupo);
    }

    public void Actualizar(Grupo grupo)
    {
        var indice = _grupos.FindIndex(g => ReferenceEquals(g, grupo));

        if (indice == -1)
        {
            throw new InvalidOperationException("El grupo no existe.");
        }

        _grupos[indice] = grupo;
    }

    public void Eliminar(Grupo grupo)
    {
        _grupos.Remove(grupo);
    }

    public List<Grupo> Buscar(Func<Grupo, bool> predicado)
    {
        var resultados = new List<Grupo>();

        foreach (var grupo in _grupos)
        {
            if (predicado(grupo))
            {
                resultados.Add(grupo);
            }
        }

        return resultados;
    }
}
