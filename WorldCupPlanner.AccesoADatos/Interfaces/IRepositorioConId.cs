using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public interface IRepositorioConId<T> : IRepositorio<T>
{
    int SiguienteId();
}