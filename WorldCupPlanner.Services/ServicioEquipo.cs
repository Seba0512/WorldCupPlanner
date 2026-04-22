using WorldCupPlanner.Domain;

namespace WorldCupPlanner.Services
{
    public class ServicioEquipo
    {
        private List<Equipo> _equipos = new List<Equipo>();

        public void AgregarEquipo(Equipo equipo)
        {
            if (!equipo.EsValido())
                throw new Exception("El equipo no es válido.");
            if (_equipos.Any(e => e.Nombre == equipo.Nombre))
                throw new Exception("Ya existe un equipo con ese nombre.");
            _equipos.Add(equipo);
        }

        public void EditarEquipo(Equipo equipo)
        {
            if (!equipo.EsValido())
                throw new Exception("El equipo no es válido.");
            if (_equipos.Any(e => e.Nombre == equipo.Nombre && e != equipo))
                throw new Exception("Ya existe un equipo con ese nombre.");
            var existente = _equipos.FirstOrDefault(e => e == equipo);
            if (existente == null)
                throw new Exception("El equipo no existe.");
            var index = _equipos.IndexOf(existente);
            _equipos[index] = equipo;
        }

        public void EliminarEquipo(Equipo equipo)
        {
            _equipos.Remove(equipo);
        }

        public List<Equipo> ObtenerEquipos()
        {
            return _equipos;
        }
    }
}