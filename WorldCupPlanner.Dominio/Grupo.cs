namespace WorldCupPlanner.Dominio;

public class Grupo
{
    private const char EtiquetaMinima = 'A';
    private const char EtiquetaMaxima = 'L';
    private const int CantidadMaximaEquipos = 4;
    private const int MaximoEquiposUefa = 2;
    private const int MaximoEquiposOtrasConfederaciones = 1;

    private static readonly Confederacion[] ConfederacionesNoUefa =
    {
        Confederacion.CONMEBOL,
        Confederacion.CONCACAF,
        Confederacion.CAF,
        Confederacion.AFC,
        Confederacion.OFC
    };

    public string Etiqueta { get; private set; }
    public List<Equipo> Equipos { get; private set; }
    public List<Partido> Partidos { get; private set; }

    public Grupo(string etiqueta, List<Equipo>? equipos = null)
    {
        ValidarEtiqueta(etiqueta);
        ValidarEquipos(equipos);

        Etiqueta = etiqueta;
        Equipos = equipos ?? new List<Equipo>();
        Partidos = new List<Partido>();
    }

    private void ValidarEtiqueta(string etiqueta)
    {
        if (string.IsNullOrEmpty(etiqueta))
            throw new ArgumentException("La etiqueta del grupo no puede estar vacía.", nameof(etiqueta));

        if (etiqueta.Length != 1)
            throw new ArgumentException("La etiqueta del grupo debe tener exactamente un carácter.", nameof(etiqueta));

        if (etiqueta[0] < EtiquetaMinima || etiqueta[0] > EtiquetaMaxima)
            throw new ArgumentException($"La etiqueta debe estar entre {EtiquetaMinima} y {EtiquetaMaxima}.", nameof(etiqueta));
    }

    private void ValidarEquipos(List<Equipo>? equipos)
    {
        if (equipos != null && equipos.Count > CantidadMaximaEquipos)
            throw new ArgumentException($"Un grupo no puede tener más de {CantidadMaximaEquipos} equipos.", nameof(equipos));
    }

    public bool EsValido()
    {
        return TieneCantidadCorrectaDeEquipos()
               && SonEquiposValidos()
               && CumpleRestriccionesConfederaciones();
    }

    private bool TieneCantidadCorrectaDeEquipos()
    {
        return Equipos.Count == CantidadMaximaEquipos;
    }

    private bool SonEquiposValidos()
    {
        return Equipos.All(e => e.EsValido());
    }

    private bool CumpleRestriccionesConfederaciones()
    {
        return CumpleRestriccionUefa() && CumpleRestriccionesPorOtrasConfederaciones();
    }

    private bool CumpleRestriccionUefa()
    {
        var conteoUefa = ObtenerConteoConfederacion(Confederacion.UEFA);
        return conteoUefa <= MaximoEquiposUefa;
    }

    private bool CumpleRestriccionesPorOtrasConfederaciones()
    {
        return ConfederacionesNoUefa.All(c =>
            ObtenerConteoConfederacion(c) <= MaximoEquiposOtrasConfederaciones);
    }

    private int ObtenerConteoConfederacion(Confederacion confederacion)
    {
        return Equipos.Count(e => e.Confederacion == confederacion);
    }
}