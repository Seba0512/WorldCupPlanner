using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.AccesoADatos.Repositorios;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.Servicios
{
    public class ServicioEquipo : IRepositorio<Equipo>
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

        private static readonly Dictionary<Confederacion, int> CuposPorConfederacion = new()
        {
            { Confederacion.UEFA,     16 },
            { Confederacion.CONMEBOL,  7 },
            { Confederacion.CONCACAF,  7 },
            { Confederacion.CAF,       9 },
            { Confederacion.AFC,       8 },
            { Confederacion.OFC,       1 },
        };

        public void Agregar(Equipo equipo)
        {
            ValidarEquipo(equipo);
            ValidarNombreUnico(equipo.Nombre);
            ValidarCupoConfederacion(equipo);
            _repositorio.Agregar(equipo);
        }

        public void EditarEquipo(Equipo equipo, string nombreOriginal)
        {
            ValidarEquipo(equipo);
            ValidarNombreUnico(equipo.Nombre, excluir: nombreOriginal);

            var existente = _repositorio.Buscar(e => e.Nombre == nombreOriginal).FirstOrDefault()
                ?? throw new EntidadInvalidaExcepcion("El equipo no existe.");

            existente.Nombre = equipo.Nombre;
            existente.Confederacion = equipo.Confederacion;
            existente.RankingFIFA = equipo.RankingFIFA;
            _repositorio.Actualizar(existente);
        }

        public void Actualizar(Equipo equipo)
        {
            if (equipo == null)
                throw new EntidadInvalidaExcepcion("El equipo no puede ser nulo.");
            var existente = _repositorio.Buscar(e => ReferenceEquals(e, equipo)).FirstOrDefault()
                ?? throw new EntidadInexistenteExcepcion("El equipo no existe en el sistema.");
            _repositorio.Actualizar(existente);
        }

        public void Eliminar(Equipo equipo) => _repositorio.Eliminar(equipo);

        public List<Equipo> Buscar(Func<Equipo, bool> predicado) => _repositorio.Buscar(predicado);

        private void ValidarEquipo(Equipo equipo)
        {
            if (!equipo.EsValido())
                throw new EntidadInvalidaExcepcion("El equipo no es valido.");
        }

        private void ValidarNombreUnico(string nombre, string? excluir = null)
        {
            if (_repositorio.Buscar(e => e.Nombre == nombre && e.Nombre != excluir).Count > 0)
                throw new EntidadYaExistenteExcepcion("Ya existe un equipo con ese nombre.");
        }

        private void ValidarCupoConfederacion(Equipo equipo)
        {
            var cupo = CuposPorConfederacion[equipo.Confederacion!.Value];
            var inscritos = _repositorio.Buscar(e => e.Confederacion == equipo.Confederacion).Count;
            if (inscritos >= cupo)
                throw new InvalidOperationException(
                    $"La confederación {equipo.Confederacion!.Value} ya alcanzó su cupo máximo de {cupo} equipos.");
        }
    }
}
