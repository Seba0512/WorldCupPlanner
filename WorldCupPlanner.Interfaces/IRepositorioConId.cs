namespace WorldCupPlanner.AccesoADatos.Interfaces;

public interface IRepositorioConId<T> : IRepositorio<T>
{
    int SiguienteId();
}
