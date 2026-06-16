using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Servicios;

public class ServicioImportacionLegible
{
    private const char SeparadorCSV = ',';
    private const string EncabezadoEsperadoBase = "Nombre,Confederacion,RankingFIFA";
    private const string EncabezadoConBandera = "Nombre,Confederacion,RankingFIFA,Bandera";
    private const int CantidadDeColumnasBase = 3;
    private const int CantidadDeColumnasConBandera = 4;
    private const int IndiceNombre = 0;
    private const int IndiceConfederacion = 1;
    private const int IndiceRankingFIFA = 2;
    private const int IndiceBandera = 3;

    private readonly ServicioAuditoria? _auditoria;
    private readonly ServicioAutorizacion _autorizacion;
    private readonly IDescargadorContenido? _descargador;

    public ServicioImportacionLegible(
        ServicioAuditoria? auditoria = null,
        ServicioAutorizacion? autorizacion = null,
        IDescargadorContenido? descargador = null)
    {
        _auditoria = auditoria;
        _autorizacion = autorizacion ?? new ServicioAutorizacion();
        _descargador = descargador;
    }

    public ResultadoImportacion Importar(Stream csv, ServicioEquipo servicioEquipo, Usuario? solicitante = null)
    {
        if (solicitante != null)
            _autorizacion.VerificarEsEditorOAdministrador(solicitante);

        using var lector = new StreamReader(csv);
        var errores = new List<string>();

        if (!EncabezadoEsValido(lector, errores, out var incluyeBandera))
            return new ResultadoImportacion(0, errores);

        var exitosos = ContarImportacionesExitosas(lector, servicioEquipo, errores, incluyeBandera, _descargador);

        var nombreUsuario = solicitante?.Email ?? "Sistema";
        _auditoria?.Registrar(nombreUsuario, "Importacion",
            $"Equipos importados: {exitosos}, Errores: {errores.Count}");

        return new ResultadoImportacion(exitosos, errores);
    }

    private static bool EncabezadoEsValido(StreamReader lector, List<string> errores, out bool incluyeBandera)
    {
        incluyeBandera = false;
        var primeraLinea = lector.ReadLine()?.Trim();
        if (primeraLinea == EncabezadoConBandera)
        {
            incluyeBandera = true;
            return true;
        }
        if (primeraLinea == EncabezadoEsperadoBase)
            return true;
        errores.Add($"Encabezado inválido. Se esperaba: {EncabezadoEsperadoBase} o {EncabezadoConBandera}");
        return false;
    }

    private static int ContarImportacionesExitosas(
        StreamReader lector,
        ServicioEquipo servicioEquipo,
        List<string> errores,
        bool incluyeBandera,
        IDescargadorContenido? descargador)
    {
        var exitosos = 0;
        var numeroDeLinea = 1;
        while (!lector.EndOfStream)
        {
            var lineaActual = lector.ReadLine();
            numeroDeLinea++;
            if (string.IsNullOrWhiteSpace(lineaActual)) continue;

            var equipo = ParsearEquipo(lineaActual, new ContextoDeLinea(numeroDeLinea, errores), incluyeBandera, descargador);
            if (equipo != null && AgregarAlServicio(equipo, servicioEquipo, errores))
                exitosos++;
        }
        return exitosos;
    }

    private static Equipo? ParsearEquipo(
        string lineaCSV,
        ContextoDeLinea contexto,
        bool incluyeBandera,
        IDescargadorContenido? descargador)
    {
        // Split con máximo de columnas para preservar comas internas en Data URIs.
        // Con encabezado Bandera: se aceptan 3 o 4 columnas por fila (Bandera es opcional).
        var splitMaximo = incluyeBandera ? CantidadDeColumnasConBandera : CantidadDeColumnasBase;
        var columnas = lineaCSV.Split(SeparadorCSV, splitMaximo);
        if (columnas.Length < CantidadDeColumnasBase)
        {
            contexto.Errores.Add($"Línea {contexto.NumeroDeLinea}: se esperaban al menos {CantidadDeColumnasBase} columnas.");
            return null;
        }

        var nombre = columnas[IndiceNombre].Trim();
        if (string.IsNullOrEmpty(nombre))
        {
            contexto.Errores.Add($"Línea {contexto.NumeroDeLinea}: Nombre no puede estar vacío.");
            return null;
        }

        var confederacion = ParsearConfederacion(columnas[IndiceConfederacion].Trim(), contexto, nombre);
        if (confederacion == null) return null;

        var rankingFIFA = ParsearRankingFIFA(columnas[IndiceRankingFIFA].Trim(), contexto, nombre);
        if (rankingFIFA == null) return null;

        var bandera = string.Empty;
        if (incluyeBandera && columnas.Length == CantidadDeColumnasConBandera)
        {
            try
            {
                bandera = NormalizadorBanderaCsv.Normalizar(columnas[IndiceBandera].Trim(), descargador);
            }
            catch (Exception ex)
            {
                contexto.Errores.Add($"Línea {contexto.NumeroDeLinea} ({nombre}): Bandera inválida — {ex.Message}");
                return null;
            }
        }

        return new Equipo { Nombre = nombre, Confederacion = confederacion.Value, RankingFIFA = rankingFIFA.Value, Bandera = bandera };
    }

    private static Confederacion? ParsearConfederacion(string texto, ContextoDeLinea contexto, string nombreEquipo)
    {
        if (Enum.TryParse<Confederacion>(texto, ignoreCase: true, out var confederacion))
            return confederacion;

        contexto.Errores.Add($"Línea {contexto.NumeroDeLinea} ({nombreEquipo}): confederación inválida \"{texto}\".");
        return null;
    }

    private static int? ParsearRankingFIFA(string texto, ContextoDeLinea contexto, string nombreEquipo)
    {
        if (int.TryParse(texto, out var ranking))
            return ranking;

        contexto.Errores.Add($"Línea {contexto.NumeroDeLinea} ({nombreEquipo}): RankingFIFA debe ser un número entero.");
        return null;
    }

    private static bool AgregarAlServicio(Equipo equipo, ServicioEquipo servicioEquipo, List<string> errores)
    {
        try
        {
            servicioEquipo.Agregar(equipo);
            return true;
        }
        catch (Exception ex)
        {
            errores.Add($"Equipo \"{equipo.Nombre}\": {ex.Message}");
            return false;
        }
    }

    private record ContextoDeLinea(int NumeroDeLinea, List<string> Errores);
}
