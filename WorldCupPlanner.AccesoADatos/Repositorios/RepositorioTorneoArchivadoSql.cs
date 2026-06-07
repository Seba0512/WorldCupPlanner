using Microsoft.EntityFrameworkCore;
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioTorneoArchivadoSql : IRepositorioTorneoArchivado
{
    private readonly SqlContext _contexto;

    public RepositorioTorneoArchivadoSql(SqlContext contexto)
    {
        _contexto = contexto;
    }

    public void Guardar(TorneoArchivado torneoArchivado)
    {
        var existente = _contexto.TorneosArchivados.FirstOrDefault();
        if (existente == null)
        {
            _contexto.TorneosArchivados.Add(torneoArchivado);
        }
        else
        {
            existente.GruposJson = torneoArchivado.GruposJson;
            existente.PartidosEliminatoriosJson = torneoArchivado.PartidosEliminatoriosJson;
        }
        _contexto.SaveChanges();
    }

    public TorneoArchivado? Obtener()
    {
        return _contexto.TorneosArchivados.AsNoTracking().FirstOrDefault();
    }
}
