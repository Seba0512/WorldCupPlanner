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

        private static IReadOnlyDictionary<Confederacion, int> CuposPorConfederacion =>
            ReglasConfederacion.CuposPorConfederacion;

        public ServicioEquipo(IRepositorio<Equipo> repositorio, ServicioAuditoria? auditoria = null, ServicioAutorizacion? autorizacion = null)
        {
            _repositorio = repositorio;
            _auditoria = auditoria;
            _autorizacion = autorizacion ?? new ServicioAutorizacion();
        }

        private const int MaximoEquipos = 48;
        private const int RankingMinimoFicticio = 300;
        private const int RankingMaximoFicticioExclusivo = 2501;

        public void Agregar(Equipo equipo, Usuario? solicitante = null)
        {
            if (solicitante != null)
                _autorizacion.VerificarEsAdministrador(solicitante);
            ValidarEquipo(equipo);
            ValidarNombreUnico(equipo.Nombre);
            ValidarCupoConfederacion(equipo);
            _repositorio.Agregar(equipo);
            _auditoria?.Registrar(
                solicitante != null ? $"{solicitante.Nombre} {solicitante.Apellido}" : "Sistema",
                "Alta",
                $"Equipo: {equipo.Nombre}, Confederacion: {equipo.Confederacion}, Ranking: {equipo.RankingFIFA}");
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
            _auditoria?.Registrar(
                solicitante != null ? $"{solicitante.Nombre} {solicitante.Apellido}" : "Sistema",
                "Edicion",
                $"Original: {nombreOriginal} -> Nuevo: {equipo.Nombre}, Confederacion: {equipo.Confederacion}, Ranking: {equipo.RankingFIFA}");
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
            ValidarSemillaNoNegativa(semilla);
            var existentes = _repositorio.Buscar(_ => true);
            if (existentes.Count >= MaximoEquipos)
                return [];

            var rng = new Random(semilla);
            var generados = new List<Equipo>();
            _auditoria?.Registrar(
                solicitante != null ? $"{solicitante.Nombre} {solicitante.Apellido}" : "Sistema",
                "Generación automática de equipos",
                $"Semilla utilizada: {semilla}");

            foreach (var (confederacion, cupo) in CuposPorConfederacion)
            {
                var inscritos = existentes.Count(e => e.Confederacion == confederacion);
                var generadosEnConfederacion = GenerarEquiposParaConfederacion(confederacion, cupo, inscritos, rng);
                generados.AddRange(generadosEnConfederacion);
                RegistrarAuditoriaConfederacion(confederacion, generadosEnConfederacion, solicitante);
            }

            return generados;
        }

        private void ValidarSemillaNoNegativa(int semilla)
        {
            if (semilla < 0)
                throw new EntidadInvalidaExcepcion("La semilla de generación automática debe ser mayor o igual a cero.");
        }

        private List<Equipo> GenerarEquiposParaConfederacion(
            Confederacion confederacion,
            int cupo,
            int inscritos,
            Random rng)
        {
            var generados = new List<Equipo>();
            for (var i = inscritos; i < cupo; i++)
            {
                var ranking = Math.Clamp(
                    rng.Next(RankingMinimoFicticio, RankingMaximoFicticioExclusivo),
                    RankingMinimoFicticio,
                    RankingMaximoFicticioExclusivo - 1);
                var equipo = new Equipo
                {
                    Nombre = GenerarNombre(confederacion),
                    RankingFIFA = ranking,
                    Confederacion = confederacion
                };
                Agregar(equipo);
                generados.Add(equipo);
            }
            return generados;
        }

        private void RegistrarAuditoriaConfederacion(
            Confederacion confederacion,
            List<Equipo> generados,
            Usuario? solicitante)
        {
            if (generados.Count == 0) return;
            _auditoria?.Registrar(
                solicitante != null ? $"{solicitante.Nombre} {solicitante.Apellido}" : "Sistema",
                "Generación automática de equipos",
                $"Confederacion: {confederacion}, Generados: {generados.Count}, " +
                $"RankingMin: {generados.Min(e => e.RankingFIFA)}, " +
                $"RankingMax: {generados.Max(e => e.RankingFIFA)}");
        }

        public void Eliminar(Equipo equipo, Usuario? solicitante = null)
        {
            if (solicitante != null)
                _autorizacion.VerificarEsAdministrador(solicitante);
            ValidarEliminacionPermitida();

            _repositorio.Eliminar(equipo);
            _auditoria?.Registrar(
                solicitante != null ? $"{solicitante.Nombre} {solicitante.Apellido}" : "Sistema",
                "Baja",
                $"Equipo: {equipo.Nombre}, Confederacion: {equipo.Confederacion}");
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
            if (_repositorio.Buscar(e =>
                    string.Equals(e.Nombre, nombre, StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(e.Nombre, excluir, StringComparison.OrdinalIgnoreCase)).Count > 0)
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
                throw new EntidadInvalidaExcepcion(
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
