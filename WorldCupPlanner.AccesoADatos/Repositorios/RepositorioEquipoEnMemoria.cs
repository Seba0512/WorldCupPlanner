using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioEquipoEnMemoria : IRepositorio<Equipo>
{
    private readonly List<Equipo> _equipos = new();

    public void Agregar(Equipo equipo)
    {
        _equipos.Add(equipo);
    }

    public void Actualizar(Equipo equipo)
    {
        var indice = _equipos.FindIndex(e => ReferenceEquals(e, equipo));

        if (indice == -1)
        {
            throw new EntidadInexistenteExcepcion("El equipo no existe.");
        }

        _equipos[indice] = equipo;
    }

    public void Eliminar(Equipo equipo)
    {
        _equipos.Remove(equipo);
    }

    public List<Equipo> Buscar(Func<Equipo, bool> predicado)
    {
        var resultados = new List<Equipo>();

        foreach (var equipo in _equipos)
        {
            if (predicado(equipo))
            {
                resultados.Add(equipo);
            }
        }

        return resultados;
    }
}
