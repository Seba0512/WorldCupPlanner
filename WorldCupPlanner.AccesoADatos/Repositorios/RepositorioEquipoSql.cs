using Microsoft.EntityFrameworkCore;
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioEquipoSql : IRepositorio<Equipo>, IRepositorioConClaveEditable<Equipo>
{
    private readonly SqlContext _contexto;

    public RepositorioEquipoSql(SqlContext contexto)
    {
        _contexto = contexto;
    }

    public void Agregar(Equipo equipo)
    {
        AsegurarRankingInicial(equipo);
        _contexto.Equipos.Add(equipo);
        _contexto.SaveChanges();
    }

    public void Actualizar(Equipo equipo)
    {
        AsegurarRankingInicial(equipo);
        var existe = _contexto.Equipos.Any(e => e.Id == equipo.Id);
        if (!existe)
            throw new EntidadInexistenteExcepcion("El equipo no existe.");

        DesvincularInstanciaLocal(equipo.Id, equipo);
        _contexto.Equipos.Update(equipo);
        _contexto.SaveChanges();
    }

    public void ActualizarClave(string claveOriginal, Equipo equipo)
    {
        var existente = _contexto.Equipos.FirstOrDefault(e => e.Nombre == claveOriginal);
        if (existente == null)
            throw new EntidadInexistenteExcepcion("El equipo no existe.");

        existente.Nombre = equipo.Nombre;
        existente.Confederacion = equipo.Confederacion;
        existente.RankingFIFA = equipo.RankingFIFA;
        existente.RankingInicial = equipo.RankingInicial;
        existente.Bandera = equipo.Bandera;
        _contexto.SaveChanges();
    }

    public void Eliminar(Equipo equipo)
    {
        var existente = ObtenerEquipoPersistido(equipo);
        if (existente == null)
            throw new EntidadInexistenteExcepcion("El equipo no existe.");

        var incidencias = _contexto.Incidencias
            .Where(i => EF.Property<int>(i, "EquipoId") == existente.Id)
            .ToList();
        _contexto.Incidencias.RemoveRange(incidencias);

        _contexto.Equipos.Remove(existente);
        _contexto.SaveChanges();
    }

    public List<Equipo> Buscar(Func<Equipo, bool> predicado)
    {
        return _contexto.Equipos.AsNoTracking().Where(predicado).ToList();
    }

    private Equipo? ObtenerEquipoPersistido(Equipo equipo)
    {
        if (equipo.Id > 0)
            return _contexto.Equipos.FirstOrDefault(e => e.Id == equipo.Id);

        return _contexto.Equipos.FirstOrDefault(e => e.Nombre == equipo.Nombre);
    }

    private void DesvincularInstanciaLocal(int id, Equipo entidad)
    {
        var local = _contexto.Equipos.Local.FirstOrDefault(e => e.Id == id);
        if (local != null && !ReferenceEquals(local, entidad))
            _contexto.Entry(local).State = EntityState.Detached;
    }

    private static void AsegurarRankingInicial(Equipo equipo)
    {
        if (equipo.RankingInicial == 0)
            equipo.RankingInicial = equipo.RankingFIFA;
    }
}
