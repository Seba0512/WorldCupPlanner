using System.Globalization;
using System.Text;
using ClosedXML.Excel;
using CsvHelper;
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.Servicios;

public class ServicioExportacion
{
    private readonly IRepositorioConId<Auditoria> _repositorioAuditoria;
    private readonly EstadoTorneo _estadoTorneo;
    private readonly ServicioAuditoria _auditoria;

    public ServicioExportacion(
        IRepositorioConId<Auditoria> repositorioAuditoria,
        EstadoTorneo estadoTorneo,
        ServicioAuditoria auditoria)
    {
        _repositorioAuditoria = repositorioAuditoria;
        _estadoTorneo = estadoTorneo;
        _auditoria = auditoria;
    }

    public byte[] ExportarAuditoriasCsv(DateTime desde, DateTime hasta, string usuario)
    {
        var registros = ObtenerAuditoriasEnRango(desde, hasta);
        var nombreArchivo = $"auditoria_{FormatearFecha(desde)}_{FormatearFecha(hasta)}.csv";
        RegistrarExportacion(usuario, nombreArchivo);

        using var memory = new MemoryStream();
        using var writer = new StreamWriter(memory, Encoding.UTF8);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        EscribirEncabezadosAuditoria(csv);
        foreach (var r in registros)
            EscribirFilaAuditoria(csv, r);

        writer.Flush();
        return memory.ToArray();
    }

    public byte[] ExportarAuditoriasXlsx(DateTime desde, DateTime hasta, string usuario)
    {
        var registros = ObtenerAuditoriasEnRango(desde, hasta);
        var nombreArchivo = $"auditoria_{FormatearFecha(desde)}_{FormatearFecha(hasta)}.xlsx";
        RegistrarExportacion(usuario, nombreArchivo);

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Auditoría");

        ws.Cell(1, 1).Value = "Id";
        ws.Cell(1, 2).Value = "Timestamp";
        ws.Cell(1, 3).Value = "Usuario";
        ws.Cell(1, 4).Value = "Accion";
        ws.Cell(1, 5).Value = "Detalle";
        AplicarEstiloEncabezado(ws.Row(1));

        for (var i = 0; i < registros.Count; i++)
        {
            var fila = i + 2;
            var r = registros[i];
            ws.Cell(fila, 1).Value = r.Id;
            ws.Cell(fila, 2).Value = r.Timestamp;
            ws.Cell(fila, 3).Value = r.Usuario;
            ws.Cell(fila, 4).Value = r.Accion;
            ws.Cell(fila, 5).Value = r.Detalle;
        }

        AplicarAnchosAuditoria(ws);
        using var memory = new MemoryStream();
        workbook.SaveAs(memory);
        return memory.ToArray();
    }

    public byte[] ExportarFixtureCsv(string usuario)
    {
        RegistrarExportacion(usuario, "fixture.csv");

        using var memory = new MemoryStream();
        using var writer = new StreamWriter(memory, Encoding.UTF8);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        EscribirEncabezadosFixture(csv);
        foreach (var partido in ObtenerTodosLosPartidos())
            EscribirFilaFixture(csv, partido);

        writer.Flush();
        return memory.ToArray();
    }

    public byte[] ExportarFixtureXlsx(string usuario)
    {
        RegistrarExportacion(usuario, "fixture.xlsx");

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Fixture");

        ws.Cell(1, 1).Value = "Id";
        ws.Cell(1, 2).Value = "Fecha";
        ws.Cell(1, 3).Value = "Grupo";
        ws.Cell(1, 4).Value = "Fase";
        ws.Cell(1, 5).Value = "Equipo Local";
        ws.Cell(1, 6).Value = "Goles Local";
        ws.Cell(1, 7).Value = "Goles Visitante";
        ws.Cell(1, 8).Value = "Equipo Visitante";
        ws.Cell(1, 9).Value = "Estadio";
        AplicarEstiloEncabezado(ws.Row(1));

        var partidos = ObtenerTodosLosPartidos();
        for (var i = 0; i < partidos.Count; i++)
        {
            var fila = i + 2;
            var p = partidos[i];
            ws.Cell(fila, 1).Value = p.Id;
            ws.Cell(fila, 2).Value = p.Fecha.ToString("dd/MM/yyyy HH:mm");
            ws.Cell(fila, 3).Value = p.Grupo?.Etiqueta ?? "";
            ws.Cell(fila, 4).Value = p.Fase.ToString();
            ws.Cell(fila, 5).Value = p.EquipoLocal?.Nombre ?? "";
            if (p.GolesLocal.HasValue) ws.Cell(fila, 6).Value = p.GolesLocal.Value;
            if (p.GolesVisitante.HasValue) ws.Cell(fila, 7).Value = p.GolesVisitante.Value;
            ws.Cell(fila, 8).Value = p.EquipoVisitante?.Nombre ?? "";
            ws.Cell(fila, 9).Value = p.Estadio?.Nombre ?? "";
        }

        AplicarAnchosFixture(ws);
        using var memory = new MemoryStream();
        workbook.SaveAs(memory);
        return memory.ToArray();
    }

    private List<Auditoria> ObtenerAuditoriasEnRango(DateTime desde, DateTime hasta)
    {
        ValidarRangoFechas(desde, hasta);

        return _repositorioAuditoria.Buscar(a =>
        {
            if (!DateTime.TryParse(a.Timestamp, null, DateTimeStyles.RoundtripKind, out var dt))
                return false;
            return dt.Date >= desde.Date && dt.Date <= hasta.Date;
        });
    }

    private static void ValidarRangoFechas(DateTime desde, DateTime hasta)
    {
        if (desde.Date > hasta.Date)
        {
            throw new EntidadInvalidaExcepcion(
                "La fecha desde no puede ser posterior a la fecha hasta.");
        }
    }

    private List<Partido> ObtenerTodosLosPartidos()
    {
        var partidos = new List<Partido>();
        if (_estadoTorneo.Fixture != null)
            partidos.AddRange(_estadoTorneo.Fixture.Grupos.SelectMany(g => g.Partidos));
        partidos.AddRange(_estadoTorneo.PartidosEliminatorios);
        return partidos;
    }

    private void RegistrarExportacion(string usuario, string nombreArchivo)
    {
        _auditoria.Registrar(usuario, "Exportacion", $"{usuario} descargó {nombreArchivo}");
    }

    private static void EscribirEncabezadosAuditoria(CsvWriter csv)
    {
        csv.WriteField("Id");
        csv.WriteField("Timestamp");
        csv.WriteField("Usuario");
        csv.WriteField("Accion");
        csv.WriteField("Detalle");
        csv.NextRecord();
    }

    private static void EscribirFilaAuditoria(CsvWriter csv, Auditoria r)
    {
        csv.WriteField(r.Id);
        csv.WriteField(r.Timestamp);
        csv.WriteField(r.Usuario);
        csv.WriteField(r.Accion);
        csv.WriteField(r.Detalle);
        csv.NextRecord();
    }

    private static void EscribirEncabezadosFixture(CsvWriter csv)
    {
        csv.WriteField("Id");
        csv.WriteField("Fecha");
        csv.WriteField("Grupo");
        csv.WriteField("Fase");
        csv.WriteField("Equipo Local");
        csv.WriteField("Goles Local");
        csv.WriteField("Goles Visitante");
        csv.WriteField("Equipo Visitante");
        csv.WriteField("Estadio");
        csv.NextRecord();
    }

    private static void EscribirFilaFixture(CsvWriter csv, Partido p)
    {
        csv.WriteField(p.Id);
        csv.WriteField(p.Fecha.ToString("dd/MM/yyyy HH:mm"));
        csv.WriteField(p.Grupo?.Etiqueta ?? "");
        csv.WriteField(p.Fase.ToString());
        csv.WriteField(p.EquipoLocal?.Nombre ?? "");
        csv.WriteField(p.GolesLocal?.ToString() ?? "");
        csv.WriteField(p.GolesVisitante?.ToString() ?? "");
        csv.WriteField(p.EquipoVisitante?.Nombre ?? "");
        csv.WriteField(p.Estadio?.Nombre ?? "");
        csv.NextRecord();
    }

    private static void AplicarEstiloEncabezado(IXLRow fila)
    {
        fila.Style.Font.Bold = true;
        fila.Style.Fill.BackgroundColor = XLColor.FromHtml("#2563EB");
        fila.Style.Font.FontColor = XLColor.White;
    }

    private static void AplicarAnchosAuditoria(IXLWorksheet ws)
    {
        ws.Column(1).Width = 10;
        ws.Column(2).Width = 30;
        ws.Column(3).Width = 24;
        ws.Column(4).Width = 22;
        ws.Column(5).Width = 60;
    }

    private static void AplicarAnchosFixture(IXLWorksheet ws)
    {
        ws.Column(1).Width = 10;
        ws.Column(2).Width = 20;
        ws.Column(3).Width = 12;
        ws.Column(4).Width = 24;
        ws.Column(5).Width = 24;
        ws.Column(6).Width = 14;
        ws.Column(7).Width = 18;
        ws.Column(8).Width = 24;
        ws.Column(9).Width = 24;
    }

    private static string FormatearFecha(DateTime dt) => dt.ToString("yyyyMMdd");
}
