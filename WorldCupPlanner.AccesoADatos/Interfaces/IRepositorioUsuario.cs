using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.AccesoADatos.Interfaces;

public interface IRepositorioUsuario : IRepositorio<Usuario>
{
    Usuario? ObtenerPorEmail(string email);
}
