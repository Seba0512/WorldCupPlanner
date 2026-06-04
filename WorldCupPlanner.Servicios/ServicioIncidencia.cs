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