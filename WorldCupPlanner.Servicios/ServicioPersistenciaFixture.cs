using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Servicios;

public class ServicioPersistenciaFixture
{
    private readonly IRepositorio<Grupo> _repositorioGrupo;
    private readonly IRepositorio<Partido> _repositorioPartido;

    public ServicioPersistenciaFixture(
        IRepositorio<Grupo> repositorioGrupo,
        IRepositorio<Partido> repositorioPartido)
    {
        _repositorioGrupo = repositorioGrupo;
        _repositorioPartido = repositorioPartido;
    }

    public void GuardarFixture(Fixture fixture)
    {
        GuardarGrupos(fixture.Grupos);
        GuardarPartidos(fixture.Partidos);
    }

    public void GuardarPartidos(IEnumerable<Partido> partidos)
    {
        var existentesPorId = _repositorioPartido.Buscar(_ => true)
            .ToDictionary(partido => partido.Id);

        foreach (var partido in partidos)
        {
            if (existentesPorId.TryGetValue(partido.Id, out var existente))
            {
                CopiarDatosPartido(partido, existente);
                _repositorioPartido.Actualizar(existente);
                continue;
            }

            _repositorioPartido.Agregar(partido);
            existentesPorId[partido.Id] = partido;
        }
    }

    private void GuardarGrupos(IEnumerable<Grupo> grupos)
    {
        var etiquetasExistentes = _repositorioGrupo.Buscar(_ => true)
            .Select(grupo => grupo.Etiqueta)
            .ToHashSet();

        foreach (var grupo in grupos)
        {
            if (etiquetasExistentes.Contains(grupo.Etiqueta))
            {
                continue;
            }

            _repositorioGrupo.Agregar(grupo);
            etiquetasExistentes.Add(grupo.Etiqueta);
        }
    }

    public void LimpiarFixture()
    {
        var partidos = _repositorioPartido.Buscar(_ => true);
        foreach (var partido in partidos)
            _repositorioPartido.Eliminar(partido);

        var grupos = _repositorioGrupo.Buscar(_ => true);
        foreach (var grupo in grupos)
            _repositorioGrupo.Eliminar(grupo);
    }

    public void CargarEnEstadoTorneo(EstadoTorneo estadoTorneo)
    {
        var grupos = _repositorioGrupo.Buscar(_ => true);
        if (!grupos.Any()) return;

        estadoTorneo.Fixture = new Fixture { Grupos = grupos };
        estadoTorneo.PartidosEliminatorios = _repositorioPartido
            .Buscar(p => p.Fase != FaseTorneo.FaseDeGrupos);
    }

    private static void CopiarDatosPartido(Partido origen, Partido destino)
    {
        destino.Fecha = origen.Fecha;
        destino.Estadio = origen.Estadio;
        destino.EquipoLocal = origen.EquipoLocal;
        destino.EquipoVisitante = origen.EquipoVisitante;
        destino.Fase = origen.Fase;
        destino.GolesLocal = origen.GolesLocal;
        destino.GolesVisitante = origen.GolesVisitante;
        destino.Grupo = origen.Grupo;
        destino.GanadorPorPenales = origen.GanadorPorPenales;
    }
}
