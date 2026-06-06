using Microsoft.EntityFrameworkCore;
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioConfiguracionTorneoSql : IRepositorioConfiguracionTorneo
{
    private const int IdConfiguracionActual = 1;
    private const int IdConfiguracionParaFixture = 2;

    private readonly SqlContext _contexto;

    public RepositorioConfiguracionTorneoSql(SqlContext contexto)
    {
        _contexto = contexto;
    }

    public ConfiguracionTorneo ObtenerConfiguracionActual()
    {
        return _contexto.ConfiguracionesTorneo
            .AsNoTracking()
            .FirstOrDefault(c => c.Id == IdConfiguracionActual)
            ?? new ConfiguracionTorneo();
    }

    public void GuardarConfiguracionActual(ConfiguracionTorneo configuracion)
    {
        GuardarConfiguracion(configuracion, IdConfiguracionActual);
    }

    public ConfiguracionTorneo? ObtenerConfiguracionUsadaParaFixture()
    {
        return _contexto.ConfiguracionesTorneo
            .AsNoTracking()
            .FirstOrDefault(c => c.Id == IdConfiguracionParaFixture);
    }

    public void GuardarConfiguracionUsadaParaFixture(ConfiguracionTorneo configuracion)
    {
        GuardarConfiguracion(configuracion, IdConfiguracionParaFixture);
    }

    private void GuardarConfiguracion(ConfiguracionTorneo configuracion, int id)
    {
        var entidad = new ConfiguracionTorneo
        {
            Id = id,
            SemillaFixture = configuracion.SemillaFixture,
            SemillaCompletar = configuracion.SemillaCompletar,
            SemillaCrucesFase = configuracion.SemillaCrucesFase,
            SemillaSimulation = configuracion.SemillaSimulation,
            FechaInicioTorneo = configuracion.FechaInicioTorneo,
            MaxPartidosPorDia = configuracion.MaxPartidosPorDia,
            SeparacionEntreFechas = configuracion.SeparacionEntreFechas,
            HoraInicioPartidos = configuracion.HoraInicioPartidos,
            SeparacionHorasEntrePartidos = configuracion.SeparacionHorasEntrePartidos
        };

        var local = _contexto.ConfiguracionesTorneo.Local.FirstOrDefault(c => c.Id == id);
        if (local != null)
            _contexto.Entry(local).State = EntityState.Detached;

        var existe = _contexto.ConfiguracionesTorneo.AsNoTracking().Any(c => c.Id == id);

        if (existe)
            _contexto.ConfiguracionesTorneo.Update(entidad);
        else
            _contexto.ConfiguracionesTorneo.Add(entidad);

        _contexto.SaveChanges();
    }
}
