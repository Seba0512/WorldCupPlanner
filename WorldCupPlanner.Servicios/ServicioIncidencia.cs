using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.Servicios;

public class ServicioIncidencia
{
    private readonly IRepositorioIncidencia _repositorio;

    private const int MinutoMinimo = 1;
    private const int MinutoMaximo = 90;

    public ServicioIncidencia(IRepositorioIncidencia repositorio)
    {
        _repositorio = repositorio;
    }

    public Equipo ResolverEquipoPorRol(Partido partido, string rolEquipo)
    {
        return rolEquipo == "Local" ? partido.EquipoLocal! : partido.EquipoVisitante!;
    }

    public void AgregarTarjeta(int partidoId, TipoTarjeta tipo, int minuto, Equipo equipo)
    {
        ValidarMinuto(minuto);
        Incidencia tarjeta = tipo == TipoTarjeta.Amarilla
            ? new TarjetaAmarilla { MinutoDelPartido = minuto, Equipo = equipo, PartidoId = partidoId }
            : new TarjetaRoja    { MinutoDelPartido = minuto, Equipo = equipo, PartidoId = partidoId };
        _repositorio.Agregar(tarjeta);
    }

    public List<Incidencia> ObtenerIncidencias(int partidoId)
        => _repositorio.ObtenerPorPartido(partidoId);

    public void PersistirIncidenciasDePartido(Partido partido)
    {
        var nuevas = partido.Incidencias
            .Where(i => i.Id == 0 && i.PartidoId == partido.Id)
            .ToList();

        if (!nuevas.Any() || ObtenerIncidencias(partido.Id).Any())
            return;

        foreach (var incidencia in nuevas)
        {
            if (incidencia.Equipo == null)
                continue;

            var tipo = incidencia is TarjetaRoja ? TipoTarjeta.Roja : TipoTarjeta.Amarilla;
            AgregarTarjeta(partido.Id, tipo, incidencia.MinutoDelPartido, incidencia.Equipo);
        }
    }

    public void EliminarIncidencia(int incidenciaId)
    {
        var incidencia = _repositorio.Buscar(i => i.Id == incidenciaId).FirstOrDefault()
            ?? throw new EntidadInexistenteExcepcion("La incidencia no existe.");
        _repositorio.Eliminar(incidencia);
    }

    private static void ValidarMinuto(int minuto)
    {
        if (minuto < MinutoMinimo || minuto > MinutoMaximo)
            throw new EntidadInvalidaExcepcion($"El minuto debe estar entre {MinutoMinimo} y {MinutoMaximo}.");
    }
}