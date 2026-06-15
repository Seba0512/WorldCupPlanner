using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.Controladores;

public class ControladorExportacion
{
    private readonly ServicioExportacion _servicioExportacion;

    public ControladorExportacion(ServicioExportacion servicioExportacion)
    {
        _servicioExportacion = servicioExportacion;
    }

    public byte[] ExportarAuditoriasCsv(DateTime desde, DateTime hasta, string usuario) =>
        _servicioExportacion.ExportarAuditoriasCsv(desde, hasta, usuario);

    public byte[] ExportarAuditoriasXlsx(DateTime desde, DateTime hasta, string usuario) =>
        _servicioExportacion.ExportarAuditoriasXlsx(desde, hasta, usuario);

    public byte[] ExportarFixtureCsv(string usuario) =>
        _servicioExportacion.ExportarFixtureCsv(usuario);

    public byte[] ExportarFixtureXlsx(string usuario) =>
        _servicioExportacion.ExportarFixtureXlsx(usuario);
}
