using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.Controladores;

public class ControladorAuditoria
{
    private readonly ServicioAuditoria _servicioAuditoria;

    public ControladorAuditoria(ServicioAuditoria servicioAuditoria)
    {
        _servicioAuditoria = servicioAuditoria;
    }

    public List<Auditoria> ObtenerTodos(Usuario? solicitante = null) =>
        _servicioAuditoria.ObtenerTodos(solicitante);

    public string ExportarComoTexto(List<Auditoria> registros) =>
        _servicioAuditoria.ExportarComoTexto(registros);
}
