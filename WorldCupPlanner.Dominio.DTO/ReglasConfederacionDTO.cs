using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Dominio.DTO;

public class ReglasConfederacionDTO
{
    public List<CupoConfederacionDTO> CuposPorConfederacion { get; set; } = [];

    public static ReglasConfederacionDTO FromEntity()
    {
        return new ReglasConfederacionDTO
        {
            CuposPorConfederacion = ReglasConfederacion.CuposPorConfederacion
                .Select(cupo => new CupoConfederacionDTO
                {
                    Confederacion = cupo.Key,
                    Cupos = cupo.Value
                })
                .ToList()
        };
    }
}
