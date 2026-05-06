using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;

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
            throw new InvalidOperationException("El equipo no existe.");
        }

        _equipos[indice] = equipo;
    }

    public void Eliminar(Equipo equipo)
    {
        _equipos.Remove(equipo);
    }

    public List<Equipo> Buscar(Func<Equipo, bool> predicado)
    {
        return _equipos.Where(predicado).ToList();
    }
}
