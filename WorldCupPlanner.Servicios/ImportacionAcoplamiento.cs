using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Servicios;

public class ServicioImportacionDesacoplada
{
    private const string EncabezadoEsperado = "Nombre,Confederacion,RankingFIFA";
    private const int ColumnasEsperadas = 3;
    private const int IndiceNombre = 0;
    private const int IndiceConfederacion = 1;
    private const int IndiceRanking = 2;

    // Único punto de contacto con ServicioEquipo: extrae el delegate y delega al núcleo desacoplado
    public ResultadoImportacion Importar(Stream csv, ServicioEquipo servicioEquipo)
    {
        return EjecutarImportacion(csv, servicioEquipo.Agregar);
    }

    private ResultadoImportacion EjecutarImportacion(Stream csv, Action<Equipo> persistir)
    {
        using var reader = new StreamReader(csv);
        var errores = new List<string>();

        if (!ValidarEncabezado(reader, errores))
            return new ResultadoImportacion(0, errores);

        var contexto = new ContextoLinea(errores, persistir);
        var exitosos = 0;
        var numeroLinea = 1;
        while (!reader.EndOfStream)
        {
            var linea = reader.ReadLine();
            numeroLinea++;
            if (string.IsNullOrWhiteSpace(linea)) continue;
            if (ProcesarLinea(linea, numeroLinea, contexto)) exitosos++;
        }

        return new ResultadoImportacion(exitosos, errores);
    }

    private static bool ValidarEncabezado(StreamReader reader, List<string> errores)
    {
        var encabezado = reader.ReadLine()?.Trim();
        if (encabezado == EncabezadoEsperado) return true;
        errores.Add($"Encabezado inválido. Se esperaba: {EncabezadoEsperado}");
        return false;
    }

    private static bool ProcesarLinea(string linea, int numeroLinea, ContextoLinea contexto)
    {
        var datos = ParsearColumnas(linea, numeroLinea, contexto.Errores);
        if (datos == null) return false;

        var equipo = ConstruirEquipo(datos, numeroLinea, contexto.Errores);
        if (equipo == null) return false;

        var error = Persistir(equipo, contexto.AccionPersistir);
        if (error == null) return true;

        contexto.Errores.Add($"Línea {numeroLinea} ({equipo.Nombre}): {error}");
        return false;
    }

    private static DatosEquipoCSV? ParsearColumnas(string linea, int numeroLinea, List<string> errores)
    {
        var columnas = linea.Split(',');
        if (columnas.Length != ColumnasEsperadas)
        {
            errores.Add($"Línea {numeroLinea}: se esperaban {ColumnasEsperadas} columnas.");
            return null;
        }
        return new DatosEquipoCSV(
            columnas[IndiceNombre].Trim(),
            columnas[IndiceConfederacion].Trim(),
            columnas[IndiceRanking].Trim()
        );
    }

    private static Equipo? ConstruirEquipo(DatosEquipoCSV datos, int numeroLinea, List<string> errores)
    {
        if (!Enum.TryParse<Confederacion>(datos.Confederacion, ignoreCase: true, out var confederacion))
        {
            errores.Add($"Línea {numeroLinea} ({datos.Nombre}): confederación inválida \"{datos.Confederacion}\".");
            return null;
        }

        if (!int.TryParse(datos.RankingFIFA, out var ranking))
        {
            errores.Add($"Línea {numeroLinea} ({datos.Nombre}): RankingFIFA debe ser un número entero.");
            return null;
        }

        return new Equipo { Nombre = datos.Nombre, Confederacion = confederacion, RankingFIFA = ranking };
    }

    private static string? Persistir(Equipo equipo, Action<Equipo> accion)
    {
        try { accion(equipo); return null; }
        catch (Exception ex) { return ex.Message; }
    }

    private record DatosEquipoCSV(string Nombre, string Confederacion, string RankingFIFA);

    private sealed class ContextoLinea
    {
        public List<string> Errores { get; }
        public Action<Equipo> AccionPersistir { get; }

        public ContextoLinea(List<string> errores, Action<Equipo> accionPersistir)
        {
            Errores = errores;
            AccionPersistir = accionPersistir;
        }
    }
}