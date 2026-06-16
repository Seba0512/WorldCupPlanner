using WorldCupPlanner.Dominio.DTO;
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

    public List<AuditoriaDTO> ObtenerTodosDTO(Usuario? solicitante = null) =>
        ObtenerTodos(solicitante).Select(AuditoriaDTO.FromEntity).ToList();

    public string ExportarComoTexto(List<Auditoria> registros) =>
        _servicioAuditoria.ExportarComoTexto(registros);

    public string ExportarComoTexto(List<AuditoriaDTO> registros) =>
        _servicioAuditoria.ExportarComoTexto(registros.Select(registro => registro.ToEntity()).ToList());
}
