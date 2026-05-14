using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Servicios;

public class ServicioImportacionLegible
{
    private const char SeparadorCSV = ',';
    private const string EncabezadoEsperado = "Nombre,Confederacion,RankingFIFA";
    private const int CantidadDeColumnasEsperada = 3;
    private const int IndiceNombre = 0;
    private const int IndiceConfederacion = 1;
    private const int IndiceRankingFIFA = 2;

    private readonly ServicioAuditoria? _auditoria;
    private readonly ServicioAutorizacion _autorizacion;

    public ServicioImportacionLegible(ServicioAuditoria? auditoria = null, ServicioAutorizacion? autorizacion = null)
    {
        _auditoria = auditoria;
        _autorizacion = autorizacion ?? new ServicioAutorizacion();
    }

    public ResultadoImportacion Importar(Stream csv, ServicioEquipo servicioEquipo, Usuario? solicitante = null)
    {
        if (solicitante != null)
            _autorizacion.VerificarEsEditorOAdministrador(solicitante);

        using var lector = new StreamReader(csv);
        var errores = new List<string>();

        if (!EncabezadoEsValido(lector, errores))
            return new ResultadoImportacion(0, errores);

        var exitosos = ContarImportacionesExitosas(lector, servicioEquipo, errores);

        var nombreUsuario = solicitante?.Email ?? "Sistema";
        _auditoria?.Registrar(nombreUsuario, "Importacion",
            $"Equipos importados: {exitosos}, Errores: {errores.Count}");

        return new ResultadoImportacion(exitosos, errores);
    }

    private static bool EncabezadoEsValido(StreamReader lector, List<string> errores)
    {
        var primeraLinea = lector.ReadLine()?.Trim();
        if (primeraLinea == EncabezadoEsperado) return true;
        errores.Add($"Encabezado inválido. Se esperaba: {EncabezadoEsperado}");
        return false;
    }

    private static int ContarImportacionesExitosas(StreamReader lector, ServicioEquipo servicioEquipo, List<string> errores)
    {
        var exitosos = 0;
        var numeroDeLinea = 1;
        while (!lector.EndOfStream)
        {
            var lineaActual = lector.ReadLine();
            numeroDeLinea++;
            if (string.IsNullOrWhiteSpace(lineaActual)) continue;

            var equipo = ParsearEquipo(lineaActual, new ContextoDeLinea(numeroDeLinea, errores));
            if (equipo != null && AgregarAlServicio(equipo, servicioEquipo, errores))
                exitosos++;
        }
        return exitosos;
    }

    private static Equipo? ParsearEquipo(string lineaCSV, ContextoDeLinea contexto)
    {
        var columnas = lineaCSV.Split(SeparadorCSV);
        if (columnas.Length != CantidadDeColumnasEsperada)
        {
            contexto.Errores.Add($"Línea {contexto.NumeroDeLinea}: se esperaban {CantidadDeColumnasEsperada} columnas.");
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

        return new Equipo { Nombre = nombre, Confederacion = confederacion.Value, RankingFIFA = rankingFIFA.Value };
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