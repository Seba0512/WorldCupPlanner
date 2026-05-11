using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.Servicios;

public class ServicioAuditoria
{
    private readonly IRepositorioConId<Auditoria> _repositorio;

    public ServicioAuditoria(IRepositorioConId<Auditoria> repo)
    {
        _repositorio = repo;
    }

    public void Registrar(string usuario, string accion, string detalle = "")
    {
        var registro = new Auditoria(_repositorio.SiguienteId(), usuario, accion, detalle);
        _repositorio.Agregar(registro);
    }
}