using WorldCupPlanner.DataAccess.Repositorios;
using WorldCupPlanner.Domain;

namespace WorldCupPlanner.Services
{
    public class ServicioEquipo
    {
        private readonly RepositorioEquipoEnMemoria _repositorioEquipo;

        public ServicioEquipo()
        {
            _repositorioEquipo = new RepositorioEquipoEnMemoria();
        }

        public ServicioEquipo(RepositorioEquipoEnMemoria repositorioEquipo)
        {
            _repositorioEquipo = repositorioEquipo;
        }

        public void AgregarEquipo(Equipo equipo)
        {
            if (!equipo.EsValido())
                throw new Exception("El equipo no es valido.");
            if (_repositorioEquipo.ObtenerTodos().Any(e => e.Nombre == equipo.Nombre))
                throw new Exception("Ya existe un equipo con ese nombre.");
            _repositorioEquipo.Agregar(equipo);
        }

        public void EditarEquipo(Equipo equipo)
        {
            if (!equipo.EsValido())
                throw new Exception("El equipo no es valido.");
            if (_repositorioEquipo.ObtenerTodos().Any(e => e.Nombre == equipo.Nombre && e != equipo))
                throw new Exception("Ya existe un equipo con ese nombre.");
            var existente = _repositorioEquipo.ObtenerTodos().FirstOrDefault(e => e == equipo);
            if (existente == null)
                throw new Exception("El equipo no existe.");
            _repositorioEquipo.Actualizar(equipo);
        }

        public void EliminarEquipo(Equipo equipo)
        {
            _repositorioEquipo.Eliminar(equipo);
        }

        public List<Equipo> ObtenerEquipos()
        {
            return _repositorioEquipo.ObtenerTodos();
        }
    }
}
