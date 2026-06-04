using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.AccesoADatos.Interfaces;

public interface IRepositorioNotificacion : IRepositorio<Notificacion>
{
    List<Notificacion> ObtenerPorUsuario(int usuarioId);
}