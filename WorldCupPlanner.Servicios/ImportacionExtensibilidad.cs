using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Servicios;

public class ServicioImportacionExtensible
{
    // Agregar una nueva columna requerida solo implica añadirla aquí y parsearla en ProcesarLinea
    private static readonly IReadOnlyList<string> ColumnasRequeridas = ["Nombre", "Confederacion", "RankingFIFA"];

    public ResultadoImportacion Importar(Stream csv, ServicioEquipo servicioEquipo)
    {
        using var reader = new StreamReader(csv);
        var errores = new List<string>();

        var mapa = LeerEncabezado(reader, errores);
        if (mapa == null)
            return new ResultadoImportacion(0, errores);

        var contexto = new ContextoImportacion(mapa, servicioEquipo, errores);
        var numeroLinea = 1;
        while (!reader.EndOfStream)
        {
            var linea = reader.ReadLine();
            numeroLinea++;
            if (string.IsNullOrWhiteSpace(linea)) continue;
            ProcesarLinea(linea, numeroLinea, contexto);
        }

        return new ResultadoImportacion(contexto.Exitosos, contexto.Errores);
    }

    private static Dictionary<string, int>? LeerEncabezado(StreamReader reader, List<string> errores)
    {
        var encabezado = reader.ReadLine();
        if (string.IsNullOrWhiteSpace(encabezado))
        {
            errores.Add("El archivo está vacío o no tiene encabezado.");
            return null;
        }

        var mapa = encabezado.Split(',')
            .Select((nombre, indice) => (nombre.Trim(), indice))
            .ToDictionary(t => t.Item1, t => t.indice);

        var faltantes = ColumnasRequeridas.Where(c => !mapa.ContainsKey(c)).ToList();
        if (faltantes.Count > 0)
        {
            errores.Add($"Encabezado inválido. Columnas faltantes: {string.Join(", ", faltantes)}");
            return null;
        }

        return mapa;
    }

    private static void ProcesarLinea(string linea, int numeroLinea, ContextoImportacion contexto)
    {
        var columnas = linea.Split(',');
        var valores = contexto.Mapa.ToDictionary(kv => kv.Key, kv => ObtenerValor(columnas, kv.Value));
        var nombre = valores["Nombre"];

        if (!Enum.TryParse<Confederacion>(valores["Confederacion"], ignoreCase: true, out var confederacion))
        {
            contexto.Errores.Add($"Línea {numeroLinea} ({nombre}): confederación inválida \"{valores["Confederacion"]}\".");
            return;
        }

        if (!int.TryParse(valores["RankingFIFA"], out var ranking))
        {
            contexto.Errores.Add($"Línea {numeroLinea} ({nombre}): RankingFIFA debe ser un número entero.");
            return;
        }

        try
        {
            contexto.ServicioEquipo.Agregar(new Equipo { Nombre = nombre, Confederacion = confederacion, RankingFIFA = ranking });
            contexto.Exitosos++;
        }
        catch (Exception ex)
        {
            contexto.Errores.Add($"Línea {numeroLinea} ({nombre}): {ex.Message}");
        }
    }

    private static string ObtenerValor(string[] columnas, int indice) =>
        indice < columnas.Length ? columnas[indice].Trim() : string.Empty;

    private sealed class ContextoImportacion
    {
        public Dictionary<string, int> Mapa { get; }
        public ServicioEquipo ServicioEquipo { get; }
        public List<string> Errores { get; }
        public int Exitosos { get; set; }

        public ContextoImportacion(Dictionary<string, int> mapa, ServicioEquipo servicioEquipo, List<string> errores)
        {
            Mapa = mapa;
            ServicioEquipo = servicioEquipo;
            Errores = errores;
        }
    }
}