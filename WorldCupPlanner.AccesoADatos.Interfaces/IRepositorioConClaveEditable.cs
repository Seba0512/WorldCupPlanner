namespace WorldCupPlanner.AccesoADatos.Interfaces;

public interface IRepositorioConClaveEditable<T>
{
    void ActualizarClave(string claveOriginal, T entidad);
}
