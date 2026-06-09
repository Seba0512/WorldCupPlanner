using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.Servicios;

public class ServicioAuditoria
{
    private readonly IRepositorioConId<Auditoria> _repo;
    private readonly ServicioAutorizacion _autorizacion;

    public ServicioAuditoria(IRepositorioConId<Auditoria> repo, ServicioAutorizacion? autorizacion = null)
    {
        _repo = repo;
        _autorizacion = autorizacion ?? new ServicioAutorizacion();
    }

    public void Registrar(string usuario, string accion, string detalle = "")
    {
        var entry = new Auditoria(usuario, accion, detalle);
        _repo.Agregar(entry);
    }

    public List<Auditoria> ObtenerTodos(Usuario? solicitante = null)
    {
        if (solicitante != null)
            _autorizacion.VerificarEsAdministrador(solicitante);
        return _repo.Buscar(_ => true).OrderByDescending(a => a.Timestamp).ToList();
    }

    public string ExportarComoTexto(List<Auditoria> registros)
    {
        var lineas = registros.Select(r => $"{r.Timestamp} | {r.Usuario} | {r.Accion} | {r.Detalle}");
        return string.Join("\n", lineas);
    }
}