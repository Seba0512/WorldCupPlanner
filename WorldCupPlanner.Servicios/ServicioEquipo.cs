using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.AccesoADatos.Repositorios;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.Servicios
{
    public class ServicioEquipo
    {
        private readonly IRepositorio<Equipo> _repositorio;

        public ServicioEquipo()
        {
            _repositorio = new RepositorioEquipoEnMemoria();
        }

        public ServicioEquipo(IRepositorio<Equipo> repositorio)
        {
            _repositorio = repositorio;
        }

        public void AgregarEquipo(Equipo equipo)
        {
            if (!equipo.EsValido())
                throw new EntidadInvalidaExcepcion("El equipo no es valido.");
            if (_repositorio.Buscar(e => e.Nombre == equipo.Nombre).Count > 0)
                throw new EntidadYaExistenteExcepcion("Ya existe un equipo con ese nombre.");
            _repositorio.Agregar(equipo);
        }

        public void EditarEquipo(Equipo equipo, string nombreOriginal)
        {
            if (!equipo.EsValido())
                throw new EntidadInvalidaExcepcion("El equipo no es valido.");
            if (_repositorio.Buscar(e => e.Nombre == equipo.Nombre && e.Nombre != nombreOriginal).Count > 0)
                throw new EntidadYaExistenteExcepcion("Ya existe un equipo con ese nombre.");
            var existente = _repositorio.Buscar(e => e.Nombre == nombreOriginal).FirstOrDefault();
            if (existente == null)
                throw new EntidadInvalidaExcepcion("El equipo no existe.");
            existente.Nombre = equipo.Nombre;
            existente.Confederacion = equipo.Confederacion;
            existente.RankingFIFA = equipo.RankingFIFA;
            _repositorio.Actualizar(existente);
        }

        public void EliminarEquipo(Equipo equipo)
        {
            _repositorio.Eliminar(equipo);
        }

        public List<Equipo> ObtenerEquipos()
        {
            return _repositorio.Buscar(_ => true);
        }
    }
}
