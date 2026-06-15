using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.Controladores;

public class ControladorFaseEliminatoria
{
    private readonly ServicioOrquestadorFaseEliminatoria _orquestadorFaseEliminatoria;

    public ControladorFaseEliminatoria(ServicioOrquestadorFaseEliminatoria orquestadorFaseEliminatoria)
    {
        _orquestadorFaseEliminatoria = orquestadorFaseEliminatoria;
    }

    public void GenerarFase(FaseTorneo fase, EstadoTorneo estadoTorneo) =>
        _orquestadorFaseEliminatoria.GenerarFase(fase, estadoTorneo);
}
