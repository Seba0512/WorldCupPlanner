using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.Servicios
{
    public class ServicioEquipo
    {
        private readonly IRepositorio<Equipo> _repositorio;
        private readonly IRepositorio<Partido>? _repositorioPartido;
        private readonly IRepositorio<Grupo>? _repositorioGrupo;
        private readonly ServicioAuditoria? _auditoria;
        private readonly ServicioAutorizacion _autorizacion;
        private Fixture? _fixtureGenerado;

        private static IReadOnlyDictionary<Confederacion, int> CuposPorConfederacion =>
            ReglasConfederacion.CuposPorConfederacion;

        public ServicioEquipo(
            IRepositorio<Equipo> repositorio,
            ServicioAuditoria? auditoria = null,
            ServicioAutorizacion? autorizacion = null,
            IRepositorio<Partido>? repositorioPartido = null,
            IRepositorio<Grupo>? repositorioGrupo = null)
        {
            _repositorio = repositorio;
            _repositorioPartido = repositorioPartido;
            _repositorioGrupo = repositorioGrupo;
            _auditoria = auditoria;
            _autorizacion = autorizacion ?? new ServicioAutorizacion();
        }

        private const int MaximoEquipos = 48;
        private const int RankingMinimoFicticio = 300;
        private const int RankingMaximoFicticioExclusivo = 2501;

        public void Crear(NuevoEquipoComando dto, Usuario? solicitante = null)
        {
            var equipo = new Equipo
            {
                Nombre = dto.Nombre,
                Confederacion = dto.Confederacion,
                RankingFIFA = dto.RankingFIFA,
                Bandera = dto.Bandera
            };
            Agregar(equipo, solicitante);
        }

        public void Agregar(Equipo equipo, Usuario? solicitante = null)
        {
            if (solicitante != null)
                _autorizacion.VerificarEsAdministrador(solicitante);
            AsegurarRankingInicial(equipo);
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
            AsegurarRankingInicial(equipo);
            ValidarEquipo(equipo);
            ValidarNombreUnico(equipo.Nombre, excluir: nombreOriginal);

            var existente = _repositorio.Buscar(e => e.Nombre == nombreOriginal).FirstOrDefault()
                ?? throw new EntidadInexistenteExcepcion("El equipo no existe.");

            ValidarEdicionPermitida(nombreOriginal);

            if (equipo.RankingFIFA != existente.RankingFIFA)
                equipo.RankingInicial = equipo.RankingFIFA;

            if (equipo.Confederacion != existente.Confederacion)
                ValidarCupoConfederacion(equipo);

            if (_repositorio is IRepositorioConClaveEditable<Equipo> repositorioConClaveEditable)
            {
                repositorioConClaveEditable.ActualizarClave(nombreOriginal, equipo);
            }
            else if (!string.Equals(nombreOriginal, equipo.Nombre, StringComparison.Ordinal))
            {
                _repositorio.Eliminar(existente);
                _repositorio.Agregar(equipo);
            }
            else
            {
                existente.Confederacion = equipo.Confederacion;
                existente.RankingFIFA = equipo.RankingFIFA;
                existente.RankingInicial = equipo.RankingInicial;
                existente.Bandera = equipo.Bandera;
                _repositorio.Actualizar(existente);
            }

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

        public string ObtenerMensajeComplecion(int cantidadGenerados) =>
            cantidadGenerados > 0
                ? $"Se generaron {cantidadGenerados} equipos ficticios correctamente."
                : $"Ya hay {MaximoEquipos} equipos registrados.";

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
                    RankingInicial = ranking,
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
            ValidarEliminacionPermitida(equipo);

            _repositorio.Eliminar(equipo);
            _auditoria?.Registrar(
                solicitante != null ? $"{solicitante.Nombre} {solicitante.Apellido}" : "Sistema",
                "Baja",
                $"Equipo: {equipo.Nombre}, Confederacion: {equipo.Confederacion}");
        }

        public bool FixtureYaFueGenerado => FixturePersistidoExiste() || _fixtureGenerado != null;

        public void RegistrarFixtureGenerado(Fixture fixture)
        {
            _fixtureGenerado = fixture;
        }

        public void LimpiarFixtureGenerado()
        {
            _fixtureGenerado = null;
        }

        public List<Equipo> Buscar(Func<Equipo, bool> predicado) => _repositorio.Buscar(predicado);

        public Equipo ClonarParaEdicion(string nombre)
        {
            var original = Buscar(e => e.Nombre == nombre).First();
            return new Equipo
            {
                Id             = original.Id,
                Nombre         = original.Nombre,
                Confederacion  = original.Confederacion,
                RankingFIFA    = original.RankingFIFA,
                RankingInicial = original.RankingInicial,
                Bandera        = original.Bandera
            };
        }

        public int ObtenerMaximoEquipos() => MaximoEquipos;

        public IReadOnlyDictionary<Confederacion, int> ObtenerCuposMaximos() =>
            ReglasConfederacion.CuposPorConfederacion;

        public Dictionary<Confederacion, (int registrados, int cupo)> ObtenerCuposConOcupacion(List<Equipo> equipos) =>
            ObtenerCuposMaximos().ToDictionary(
                kvp => kvp.Key,
                kvp => (equipos.Count(e => e.Confederacion == kvp.Key), kvp.Value));

        public int ObtenerCantidadFaltante(int inscritos) =>
            Math.Max(0, MaximoEquipos - inscritos);

        private void ValidarEquipo(Equipo equipo)
        {
            if (!equipo.EsValido())
                throw new EntidadInvalidaExcepcion("El equipo no es válido.");
        }

        private static void AsegurarRankingInicial(Equipo equipo)
        {
            if (equipo.RankingInicial == 0)
                equipo.RankingInicial = equipo.RankingFIFA;
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
            while (_repositorio.Buscar(e => string.Equals(e.Nombre, nombre, StringComparison.OrdinalIgnoreCase)).Count > 0)
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

        private void ValidarEliminacionPermitida(Equipo equipo)
        {
            if (EquipoParticipaEnFixturePersistido(equipo.Nombre))
            {
                throw new EntidadInvalidaExcepcion(
                    "No se pueden eliminar equipos porque el fixture ya fue generado.");
            }

            if (HayRepositoriosDeFixturePersistido())
                return;

            if (_fixtureGenerado != null)
                throw new EntidadInvalidaExcepcion(
                    "No se pueden eliminar equipos porque el fixture ya fue generado.");
        }

        private void ValidarEdicionPermitida(string nombreOriginal)
        {
            if (EquipoParticipaEnFixturePersistido(nombreOriginal) ||
                (_fixtureGenerado != null && EquipoParticipaEnFixture(nombreOriginal)))
                throw new EntidadInvalidaExcepcion(
                    "No se pueden editar equipos que participan en un fixture generado.");
        }

        private bool FixturePersistidoExiste()
        {
            return _repositorioGrupo?.Buscar(_ => true).Count > 0 ||
                   _repositorioPartido?.Buscar(_ => true).Count > 0;
        }

        private bool EquipoParticipaEnFixturePersistido(string nombreEquipo)
        {
            return _repositorioGrupo?.Buscar(g =>
                       g.Equipos.Any(e => EsMismoEquipo(e, nombreEquipo))).Count > 0 ||
                   _repositorioPartido?.Buscar(p =>
                       EsMismoEquipo(p.EquipoLocal, nombreEquipo) ||
                       EsMismoEquipo(p.EquipoVisitante, nombreEquipo) ||
                       EsMismoEquipo(p.GanadorPorPenales, nombreEquipo)).Count > 0;
        }

        private bool HayRepositoriosDeFixturePersistido()
        {
            return _repositorioGrupo != null || _repositorioPartido != null;
        }

        private bool EquipoParticipaEnFixture(string nombreEquipo)
        {
            if (_fixtureGenerado == null)
                return false;

            return _fixtureGenerado.Grupos
                       .SelectMany(g => g.Equipos)
                       .Any(e => EsMismoEquipo(e, nombreEquipo))
                   || _fixtureGenerado.Partidos
                       .Any(p => EsMismoEquipo(p.EquipoLocal, nombreEquipo)
                                 || EsMismoEquipo(p.EquipoVisitante, nombreEquipo));
        }

        private static bool EsMismoEquipo(Equipo? equipo, string nombreEquipo)
        {
            return equipo != null &&
                   string.Equals(equipo.Nombre, nombreEquipo, StringComparison.OrdinalIgnoreCase);
        }
    }
}
