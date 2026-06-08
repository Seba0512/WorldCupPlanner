using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioIncidenciaEnMemoria : IRepositorioIncidencia
{
    private readonly List<Incidencia> _incidencias = new();
    private int _siguienteId = 1;

    public void Agregar(Incidencia entidad)
    {
        entidad.Id = _siguienteId++;
        _incidencias.Add(entidad);
    }

    public void Actualizar(Incidencia entidad)
    {
        var indice = _incidencias.FindIndex(i => i.Id == entidad.Id);
        if (indice == -1)
            throw new EntidadInexistenteExcepcion("La incidencia no existe.");
        _incidencias[indice] = entidad;
    }

    public void Eliminar(Incidencia entidad)
    {
        var existente = _incidencias.FirstOrDefault(i => i.Id == entidad.Id)
            ?? throw new EntidadInexistenteExcepcion("La incidencia no existe.");
        _incidencias.Remove(existente);
    }

    public List<Incidencia> Buscar(Func<Incidencia, bool> predicado)
        => _incidencias.Where(predicado).ToList();

    public List<Incidencia> ObtenerPorPartido(int partidoId)
        => _incidencias.Where(i => i.PartidoId == partidoId).ToList();
}
