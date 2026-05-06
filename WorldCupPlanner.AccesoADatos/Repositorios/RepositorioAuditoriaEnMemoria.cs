using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioAuditoriaEnMemoria : IRepositorioConId<Auditoria>
{
    private readonly List<Auditoria> _logs = new();
    private int _contador;

    public int SiguienteId() => ++_contador;
    
    public void Agregar(Auditoria entidad)
    {
        _logs.Add(entidad);
    }

    public void Actualizar(Auditoria entidad)
    {
        var indice = _logs.FindIndex(e => e.Id == entidad.Id);

        if (indice == -1)
        {
            throw new EntidadInexistenteExcepcion("El log de auditoría no existe.");
        }

        _logs[indice] = entidad;
    }

    public void Eliminar(Auditoria entidad)
    {
            _logs.Remove(entidad);
    }

    public List<Auditoria> Buscar(Func<Auditoria, bool> predicado)
    {
        return _logs.Where(predicado).ToList();
    }
}
