using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.Servicios
{
    public class ServicioEquipo
    {
        private readonly IRepositorio<Equipo> _repositorio;
        private readonly ServicioAuditoria? _auditoria;
        private readonly ServicioAutorizacion _autorizacion;
        private Fixture? _fixtureGenerado;

        private static readonly Dictionary<Confederacion, int> CuposPorConfederacion = new()
        {
            { Confederacion.UEFA,     16 },
            { Confederacion.CONMEBOL,  7 },
            { Confederacion.CONCACAF,  7 },
            { Confederacion.CAF,       9 },
            { Confederacion.AFC,       8 },
            { Confederacion.OFC,       1 },
        };

        public ServicioEquipo(IRepositorio<Equipo> repositorio, ServicioAuditoria? auditoria = null, ServicioAutorizacion? autorizacion = null)
        {
            _repositorio = repositorio;
            _auditoria = auditoria;
            _autorizacion = autorizacion ?? new ServicioAutorizacion();
        }

        private const int MaximoEquipos = 48;
        private const int RankingMinimoFicticio = 300;
        private const int RankingMaximoFicticioExclusivo = 501;

        public void Agregar(Equipo equipo, Usuario? solicitante = null)
        {
            if (solicitante != null)
                _autorizacion.VerificarEsAdministrador(solicitante);
            ValidarEquipo(equipo);
            ValidarNombreUnico(equipo.Nombre);
            ValidarCupoConfederacion(equipo);
            _repositorio.Agregar(equipo);
            _auditoria?.Registrar("Sistema", "Alta", $"Equipo: {equipo.Nombre}, Confederacion: {equipo.Confederacion}, Ranking: {equipo.RankingFIFA}");
        }

        public void EditarEquipo(Equipo equipo, string nombreOriginal, Usuario? solicitante = null)
        {
            if (solicitante != null)
                _autorizacion.VerificarEsAdministrador(solicitante);
            ValidarEquipo(equipo);
            ValidarNombreUnico(equipo.Nombre, excluir: nombreOriginal);

            var existente = _repositorio.Buscar(e => e.Nombre == nombreOriginal).FirstOrDefault()
                ?? throw new EntidadInexistenteExcepcion("El equipo no existe.");

            if (equipo.Confederacion != existente.Confederacion)
                ValidarCupoConfederacion(equipo);

            existente.Nombre = equipo.Nombre;
            existente.Confederacion = equipo.Confederacion;
            existente.RankingFIFA = equipo.RankingFIFA;
            _repositorio.Actualizar(existente);
            _auditoria?.Registrar("Sistema", "Edicion", $"Original: {nombreOriginal} -> Nuevo: {equipo.Nombre}, Confederacion: {equipo.Confederacion}, Ranking: {equipo.RankingFIFA}");
        }

        public void Actualizar(Equipo equipo)
        {
            if (equipo == null)
                throw new EntidadInvalidaExcepcion("El equipo no puede ser nulo.");
            EditarEquipo(equipo, equipo.Nombre);
        }

        public List<Equipo> CompletarEquipos(int semilla, Usuario? solicitante = null)
        {
            if (solicitante != null)
                _autorizacion.VerificarEsAdministrador(solicitante);
            var existentes = _repositorio.Buscar(_ => true);
            if (existentes.Count >= MaximoEquipos)
                return [];

            var rng = new Random(semilla);
            var generados = new List<Equipo>();

            foreach (var (confederacion, cupo) in CuposPorConfederacion)
            {
                var inscritos = existentes.Count(e => e.Confederacion == confederacion);
                for (var i = inscritos; i < cupo; i++)
                {
                    var equipo = new Equipo
                    {
                        Nombre = GenerarNombre(confederacion),
                        RankingFIFA = rng.Next(RankingMinimoFicticio, RankingMaximoFicticioExclusivo),
                        Confederacion = confederacion
                    };
                    Agregar(equipo);
                    generados.Add(equipo);
                }
            }

            return generados;
        }

        public void Eliminar(Equipo equipo, Usuario? solicitante = null)
        {
            if (solicitante != null)
                _autorizacion.VerificarEsAdministrador(solicitante);
            ValidarEliminacionPermitida();

            _repositorio.Eliminar(equipo);
            _auditoria?.Registrar("Sistema", "Baja", $"Equipo: {equipo.Nombre}, Confederacion: {equipo.Confederacion}");
        }

        public bool FixtureYaFueGenerado => _fixtureGenerado != null;

        public void RegistrarFixtureGenerado(Fixture fixture)
        {
            _fixtureGenerado = fixture;
        }

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

        private string GenerarNombre(Confederacion confederacion)
        {
            var numero = 1;
            var nombre = $"{confederacion}_{numero:00}";
            while (_repositorio.Buscar(e => e.Nombre == nombre).Count > 0)
            {
                numero++;
                nombre = $"{confederacion}_{numero:00}";
            }
            return nombre;
        }

        private void ValidarCupoConfederacion(Equipo equipo)
        {
            var cupo = CuposPorConfederacion[equipo.Confederacion!.Value];
            var inscritos = _repositorio.Buscar(e => e.Confederacion == equipo.Confederacion).Count;
            if (inscritos >= cupo)
                throw new InvalidOperationException(
                    $"La confederación {equipo.Confederacion!.Value} ya alcanzó su cupo máximo de {cupo} equipos.");
        }

        private void ValidarEliminacionPermitida()
        {
            if (_fixtureGenerado != null)
                throw new EntidadInvalidaExcepcion(
                    "No se pueden eliminar equipos porque el fixture ya fue generado.");
        }
    }
}