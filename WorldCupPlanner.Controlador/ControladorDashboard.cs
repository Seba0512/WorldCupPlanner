using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.Controladores;

public class ControladorDashboard
{
    private readonly ServicioDashboard _servicioDashboard;

    public ControladorDashboard(ServicioDashboard servicioDashboard)
    {
        _servicioDashboard = servicioDashboard;
    }

    public DatosDashboard ObtenerDatos() =>
        _servicioDashboard.ObtenerDatos();
}
