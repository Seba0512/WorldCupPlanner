using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.Servicios;

public class ServicioAuditoria
{
    private readonly IRepositorioConId<Auditoria> _repo;

    public ServicioAuditoria(IRepositorioConId<Auditoria> repo)
    {
        _repo = repo;
    }

    public void Registrar(string usuario, string accion, string detalle = "")
    {
        var entry = new Auditoria(_repo.SiguienteId(), usuario, accion, detalle);
        _repo.Agregar(entry);
    }
}