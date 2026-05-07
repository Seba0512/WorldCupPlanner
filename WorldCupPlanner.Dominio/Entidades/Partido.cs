using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Dominio.Entidades
{
    public class Partido
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public Estadio? Estadio { get; set; }
        public Equipo? EquipoLocal { get; set; }
        public Equipo? EquipoVisitante { get; set; }
        public FaseTorneo Fase { get; set; }
        public int? GolesLocal { get; set; }
        public int? GolesVisitante { get; set; }
        public Grupo? Grupo { get; set; }

        public bool EsValido()
        {
            return EquipoLocal != null && EquipoVisitante != null && EquipoLocal != EquipoVisitante && Estadio != null;
        }

        public bool EstaJugado()
        {
            return GolesLocal.HasValue && GolesVisitante.HasValue;
        }

        public Equipo? Ganador()
        {
            if (!EstaJugado()) return null;
            if (GolesLocal > GolesVisitante) return EquipoLocal;
            if (GolesVisitante > GolesLocal) return EquipoVisitante;
            return null;
        }

        public bool EsEmpate()
        {
            return EstaJugado() && GolesLocal == GolesVisitante;
        }
    }
}
