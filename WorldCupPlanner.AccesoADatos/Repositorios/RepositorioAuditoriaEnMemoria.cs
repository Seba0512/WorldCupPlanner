using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioAuditoriaEnMemoria : IRepositorioConId<Auditoria>
{
    private readonly List<Auditoria> _registros = new();
    private int _contador;

    public int SiguienteId() => ++_contador;
    
    public void Agregar(Auditoria entidad)
    {
        _registros.Add(entidad);
    }

    public void Actualizar(Auditoria entidad)
    {
        var indice = _registros.FindIndex(e => e.Id == entidad.Id);

        if (indice == -1)
        {
            throw new EntidadInexistenteExcepcion("El log de auditoría no existe.");
        }

        _registros[indice] = entidad;
    }

    public void Eliminar(Auditoria entidad)
    {
            _registros.Remove(entidad);
    }

    public List<Auditoria> Buscar(Func<Auditoria, bool> predicado)
    {
        var resultados = new List<Auditoria>();

        foreach (var log in _registros)
        {
            if (predicado(log))
            {
                resultados.Add(log);
            }
        }

        return resultados;
    }
}
