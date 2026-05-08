using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.AccesoADatos.Repositorios;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.Servicios;

public class ServicioPartido
{
    private readonly IRepositorio<Partido> _repositorioPartido;
    private bool _edicionPartidosFaseDeGruposBloqueada;

    public ServicioPartido()
    {
        _repositorioPartido = new RepositorioPartidoEnMemoria();
    }

    public ServicioPartido(IRepositorio<Partido> repositorioPartido)
    {
        _repositorioPartido = repositorioPartido;
    }

    public void EditarFecha(int idPartido, DateTime nuevaFecha)
    {
        if (nuevaFecha == default)
        {
            throw new EntidadInvalidaExcepcion("La fecha del partido es obligatoria.");
        }

        var partido = ObtenerPartidoPorId(idPartido);
        ValidarPuedeEditarPartido(partido);

        partido.Fecha = nuevaFecha;
        _repositorioPartido.Actualizar(partido);
    }

    public void EditarEstadio(int idPartido, Estadio nuevoEstadio)
    {
        if (!nuevoEstadio.EsValido())
        {
            throw new EntidadInvalidaExcepcion("El estadio del partido no es valido.");
        }

        var partido = ObtenerPartidoPorId(idPartido);
        ValidarPuedeEditarPartido(partido);

        partido.Estadio = nuevoEstadio;
        _repositorioPartido.Actualizar(partido);
    }

    public void CargarResultado(int idPartido, int golesLocal, int golesVisitante)
    {
        ValidarGoles(golesLocal, golesVisitante);

        var partido = ObtenerPartidoPorId(idPartido);
        ValidarPuedeEditarPartido(partido);

        partido.GolesLocal = golesLocal;
        partido.GolesVisitante = golesVisitante;
        _repositorioPartido.Actualizar(partido);
    }

    public void BloquearEdicionPartidosDeFaseDeGrupos()
    {
        _edicionPartidosFaseDeGruposBloqueada = true;
    }

    public List<Partido> ObtenerPartidos()
    {
        return _repositorioPartido.Buscar(_ => true);
    }

    private Partido ObtenerPartidoPorId(int idPartido)
    {
        var partido = _repositorioPartido.Buscar(p => p.Id == idPartido).FirstOrDefault();

        if (partido == null)
        {
            throw new EntidadInexistenteExcepcion("El partido no existe.");
        }

        return partido;
    }

    private void ValidarPuedeEditarPartido(Partido partido)
    {
        if (_edicionPartidosFaseDeGruposBloqueada && partido.Fase == FaseTorneo.FaseDeGrupos)
        {
            throw new EntidadInvalidaExcepcion(
                "No se pueden editar partidos de fase de grupos una vez generados los cruces.");
        }
    }

    private void ValidarGoles(int golesLocal, int golesVisitante)
    {
        if (golesLocal < 0 || golesVisitante < 0)
        {
            throw new EntidadInvalidaExcepcion("Los goles del partido no pueden ser negativos.");
        }
    }
}
