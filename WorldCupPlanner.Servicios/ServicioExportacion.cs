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
    private readonly ServicioConsultasTorneo _consultasTorneo;

    public ServicioExportacion(
        IRepositorioConId<Auditoria> repositorioAuditoria,
        EstadoTorneo estadoTorneo,
        ServicioAuditoria auditoria,
        ServicioConsultasTorneo consultasTorneo)
    {
        _repositorioAuditoria = repositorioAuditoria;
        _estadoTorneo = estadoTorneo;
        _auditoria = auditoria;
        _consultasTorneo = consultasTorneo;
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
        RegistrarExportacion(usuario, $"auditoria_{FormatearFecha(desde)}_{FormatearFecha(hasta)}.xlsx");

        using var workbook = new XLWorkbook();
        var hoja = workbook.Worksheets.Add("Auditoría");
        EscribirEncabezadosAuditoriaXlsx(hoja);
        EscribirFilasAuditoriaXlsx(hoja, registros);
        AplicarAnchosAuditoria(hoja);

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
        foreach (var partido in _consultasTorneo.ObtenerTodosLosPartidos(_estadoTorneo))
            EscribirFilaFixture(csv, partido);

        writer.Flush();
        return memory.ToArray();
    }

    public byte[] ExportarFixtureXlsx(string usuario)
    {
        RegistrarExportacion(usuario, "fixture.xlsx");
        var partidos = _consultasTorneo.ObtenerTodosLosPartidos(_estadoTorneo);

        using var workbook = new XLWorkbook();
        var hoja = workbook.Worksheets.Add("Fixture");
        EscribirEncabezadosFixtureXlsx(hoja);
        EscribirFilasFixtureXlsx(hoja, partidos);
        AplicarAnchosFixture(hoja);

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

    private static void EscribirEncabezadosAuditoriaXlsx(IXLWorksheet hoja)
    {
        hoja.Cell(1, 1).Value = "Id";
        hoja.Cell(1, 2).Value = "Timestamp";
        hoja.Cell(1, 3).Value = "Usuario";
        hoja.Cell(1, 4).Value = "Accion";
        hoja.Cell(1, 5).Value = "Detalle";
        AplicarEstiloEncabezado(hoja.Row(1));
    }

    private static void EscribirFilasAuditoriaXlsx(IXLWorksheet hoja, List<Auditoria> registros)
    {
        for (var i = 0; i < registros.Count; i++)
        {
            var fila = i + 2;
            var registro = registros[i];
            hoja.Cell(fila, 1).Value = registro.Id;
            hoja.Cell(fila, 2).Value = registro.Timestamp;
            hoja.Cell(fila, 3).Value = registro.Usuario;
            hoja.Cell(fila, 4).Value = registro.Accion;
            hoja.Cell(fila, 5).Value = registro.Detalle;
        }
    }

    private static void EscribirEncabezadosFixtureXlsx(IXLWorksheet hoja)
    {
        hoja.Cell(1, 1).Value = "Id";
        hoja.Cell(1, 2).Value = "Fecha";
        hoja.Cell(1, 3).Value = "Grupo";
        hoja.Cell(1, 4).Value = "Fase";
        hoja.Cell(1, 5).Value = "Equipo Local";
        hoja.Cell(1, 6).Value = "Goles Local";
        hoja.Cell(1, 7).Value = "Goles Visitante";
        hoja.Cell(1, 8).Value = "Equipo Visitante";
        hoja.Cell(1, 9).Value = "Estadio";
        AplicarEstiloEncabezado(hoja.Row(1));
    }

    private static void EscribirFilasFixtureXlsx(IXLWorksheet hoja, List<Partido> partidos)
    {
        for (var i = 0; i < partidos.Count; i++)
        {
            var fila = i + 2;
            var partido = partidos[i];
            hoja.Cell(fila, 1).Value = partido.Id;
            hoja.Cell(fila, 2).Value = partido.Fecha.ToString("dd/MM/yyyy HH:mm");
            hoja.Cell(fila, 3).Value = partido.Grupo?.Etiqueta ?? "";
            hoja.Cell(fila, 4).Value = partido.Fase.ToString();
            hoja.Cell(fila, 5).Value = partido.EquipoLocal?.Nombre ?? "";
            if (partido.GolesLocal.HasValue) hoja.Cell(fila, 6).Value = partido.GolesLocal.Value;
            if (partido.GolesVisitante.HasValue) hoja.Cell(fila, 7).Value = partido.GolesVisitante.Value;
            hoja.Cell(fila, 8).Value = partido.EquipoVisitante?.Nombre ?? "";
            hoja.Cell(fila, 9).Value = partido.Estadio?.Nombre ?? "";
        }
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
