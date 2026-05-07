# Codigo completo por proyecto

Generado desde los proyectos incluidos actualmente en WorldCupPlanner.sln.

## WorldCupPlanner.Dominio

### Auditoria.cs

Ruta: WorldCupPlanner.Dominio\Entidades\Auditoria.cs

~~~csharp
namespace WorldCupPlanner.Dominio.Entidades;

public class Auditoria
{
    public int Id { get; private set; }
    public string Timestamp { get; private set; }
    public string Usuario { get; private set; }
    public string Accion { get; private set; }
    public string Detalle { get; private set; }

    public Auditoria(int id, string usuario, string accion, string detalle)
    {
        Id = id;
        Timestamp = DateTime.UtcNow.ToString("o"); // ISO-8601
        Usuario = usuario;
        Accion = accion;
        Detalle = detalle;
    }
}
~~~

### ClasificadosFaseGrupos.cs

Ruta: WorldCupPlanner.Dominio\Entidades\ClasificadosFaseGrupos.cs

~~~csharp
namespace WorldCupPlanner.Dominio.Entidades;

public class ClasificadosFaseGrupos
{
    public List<Equipo> Primeros { get; set; } = new List<Equipo>();
    public List<Equipo> Segundos { get; set; } = new List<Equipo>();
    public List<Equipo> MejoresTerceros { get; set; } = new List<Equipo>();
    public Dictionary<Equipo, Grupo> GruposDeOrigen { get; set; } = new Dictionary<Equipo, Grupo>();

    public List<Equipo> Todos
    {
        get
        {
            return Primeros
                .Concat(Segundos)
                .Concat(MejoresTerceros)
                .ToList();
        }
    }

    public Grupo? ObtenerGrupoDeOrigen(Equipo equipo)
    {
        return GruposDeOrigen.GetValueOrDefault(equipo);
    }
}
~~~

### ConfiguracionTorneo.cs

Ruta: WorldCupPlanner.Dominio\Entidades\ConfiguracionTorneo.cs

~~~csharp
namespace WorldCupPlanner.Dominio.Entidades;

public class ConfiguracionTorneo
{
    private const int AnioInicioPorDefecto = 2026;
    private const int MesInicioPorDefecto = 6;
    private const int DiaInicioPorDefecto = 1;
    private const int MaxPartidosPorDiaPorDefecto = 3;
    private const int SeparacionEntreFechasPorDefecto = 3;
    private const int HoraInicioPorDefecto = 14;
    private const int MinutoInicioPorDefecto = 0;
    private const int SeparacionHorasEntrePartidosPorDefecto = 4;

    public int SemillaFixture { get; set; }
    public int SemillaCompletar { get; set; }
    public int SemillaCrucesFase { get; set; }
    public int SemillaSimulation { get; set; }

    public DateTime FechaInicioTorneo { get; set; } =
        new DateTime(AnioInicioPorDefecto, MesInicioPorDefecto, DiaInicioPorDefecto);

    public int MaxPartidosPorDia { get; set; } = MaxPartidosPorDiaPorDefecto;
    public int SeparacionEntreFechas { get; set; } = SeparacionEntreFechasPorDefecto;

    public TimeSpan HoraInicioPartidos { get; set; } =
        new TimeSpan(HoraInicioPorDefecto, MinutoInicioPorDefecto, 0);

    public int SeparacionHorasEntrePartidos { get; set; } =
        SeparacionHorasEntrePartidosPorDefecto;
}

~~~

### DatosUsuario.cs

Ruta: WorldCupPlanner.Dominio\Entidades\DatosUsuario.cs

~~~csharp
namespace WorldCupPlanner.Dominio.Entidades;

public record DatosUsuario
{
    public string Nombre { get; init; } = string.Empty;
    public string Apellido { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FechaNacimiento { get; init; } = string.Empty;
}
~~~

### Equipo.cs

Ruta: WorldCupPlanner.Dominio\Entidades\Equipo.cs

~~~csharp
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Dominio.Entidades
{
	public class Equipo
	{

		public string Nombre { get; set; } = ""; //arranca vacio por defecto
		public int RankingFIFA { get; set; }
		public Confederacion? Confederacion { get; set; } //puede ser nula

		public bool EsValido()
		{
			return !string.IsNullOrWhiteSpace(Nombre) && Nombre.Length <= 60 && RankingFIFA > 299 &&
			       RankingFIFA < 2501 && Confederacion.HasValue;

		}
	}
}

~~~

### Estadio.cs

Ruta: WorldCupPlanner.Dominio\Entidades\Estadio.cs

~~~csharp
namespace WorldCupPlanner.Dominio.Entidades
{
public class Estadio
{
    public string Nombre { get; set; } = "";
    public string Ciudad { get; set; } = "";    
    public string? Direccion { get; set; } = "";
    public int Capacidad { get; set; }
    public string? Descripcion { get; set; }
    
    public bool EsValido()
    {        
        return !string.IsNullOrWhiteSpace(Nombre) && Nombre.Length <= 80 && !string.IsNullOrWhiteSpace(Ciudad) && Ciudad.Length <= 60 && Capacidad >= 20000 && (Descripcion == null || Descripcion.Length <= 400);
    }
}
}
~~~

### Fixture.cs

Ruta: WorldCupPlanner.Dominio\Entidades\Fixture.cs

~~~csharp
namespace WorldCupPlanner.Dominio.Entidades;

public class Fixture
{
    public List<Grupo> Grupos { get; set; } = new List<Grupo>();

    public List<Partido> Partidos
    {
        get
        {
            return Grupos
                .SelectMany(g => g.Partidos)
                .ToList();
        }
    }
}
~~~

### Grupo.cs

Ruta: WorldCupPlanner.Dominio\Entidades\Grupo.cs

~~~csharp
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Dominio.Entidades;

public class Grupo
{
    private const char EtiquetaMinima = 'A';
    private const char EtiquetaMaxima = 'L';
    private const int CantidadMaximaEquipos = 4;
    private const int MaximoEquiposUefa = 2;
    private const int MaximoEquiposOtrasConfederaciones = 1;

    private static readonly Confederacion[] ConfederacionesNoUefa =
    {
        Confederacion.CONMEBOL,
        Confederacion.CONCACAF,
        Confederacion.CAF,
        Confederacion.AFC,
        Confederacion.OFC
    };

    public string Etiqueta { get; private set; }
    public List<Equipo> Equipos { get; private set; }
    public List<Partido> Partidos { get; private set; }

    public Grupo(string etiqueta, List<Equipo>? equipos = null)
    {
        ValidarEtiqueta(etiqueta);
        ValidarEquipos(equipos);

        Etiqueta = etiqueta;
        Equipos = equipos ?? new List<Equipo>();
        Partidos = new List<Partido>();
    }

    private void ValidarEtiqueta(string etiqueta)
    {
        if (string.IsNullOrEmpty(etiqueta))
            throw new ArgumentException("La etiqueta del grupo no puede estar vacía.", nameof(etiqueta));

        if (etiqueta.Length != 1)
            throw new ArgumentException("La etiqueta del grupo debe tener exactamente un carácter.", nameof(etiqueta));

        if (etiqueta[0] < EtiquetaMinima || etiqueta[0] > EtiquetaMaxima)
            throw new ArgumentException($"La etiqueta debe estar entre {EtiquetaMinima} y {EtiquetaMaxima}.", nameof(etiqueta));
    }

    private void ValidarEquipos(List<Equipo>? equipos)
    {
        if (equipos != null && equipos.Count > CantidadMaximaEquipos)
            throw new ArgumentException($"Un grupo no puede tener más de {CantidadMaximaEquipos} equipos.", nameof(equipos));
    }

    public bool EsValido()
    {
        return TieneCantidadCorrectaDeEquipos()
               && SonEquiposValidos()
               && CumpleRestriccionesConfederaciones();
    }

    private bool TieneCantidadCorrectaDeEquipos()
    {
        return Equipos.Count == CantidadMaximaEquipos;
    }

    private bool SonEquiposValidos()
    {
        return Equipos.All(e => e.EsValido());
    }

    private bool CumpleRestriccionesConfederaciones()
    {
        return CumpleRestriccionUefa() && CumpleRestriccionesPorOtrasConfederaciones();
    }

    private bool CumpleRestriccionUefa()
    {
        var conteoUefa = ObtenerConteoConfederacion(Confederacion.UEFA);
        return conteoUefa <= MaximoEquiposUefa;
    }

    private bool CumpleRestriccionesPorOtrasConfederaciones()
    {
        return ConfederacionesNoUefa.All(c =>
            ObtenerConteoConfederacion(c) <= MaximoEquiposOtrasConfederaciones);
    }

    private int ObtenerConteoConfederacion(Confederacion confederacion)
    {
        return Equipos.Count(e => e.Confederacion == confederacion);
    }
}
~~~

### Partido.cs

Ruta: WorldCupPlanner.Dominio\Entidades\Partido.cs

~~~csharp
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
~~~

### PosicionEquipo.cs

Ruta: WorldCupPlanner.Dominio\Entidades\PosicionEquipo.cs

~~~csharp
namespace WorldCupPlanner.Dominio.Entidades;

public class PosicionEquipo
{
    public Equipo Equipo { get; set; } = null!;
    public Grupo Grupo { get; set; } = null!;
    public int Puntos { get; set; }
    public int GolesAFavor { get; set; }
    public int GolesEnContra { get; set; }

    public int DiferenciaGoles
    {
        get
        {
            return GolesAFavor - GolesEnContra;
        }
    }
}
~~~

### Usuario.cs

Ruta: WorldCupPlanner.Dominio\Entidades\Usuario.cs

~~~csharp
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Dominio.Entidades;

public class Usuario
{
    private const string PatronEmail = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";
    private const string PatronFormatoFecha = @"^\d{2}-\d{2}-\d{4}$";
    private const string PatronMayuscula = @"[A-Z]";
    private const string PatronMinuscula = @"[a-z]";
    private const string PatronNumero = @"[0-9]";
    private const string PatronEspecial = @"[@#$\.\!\&\*\-_\+=\?]";

    private string? _contraseniaCifrada;
    private HashSet<RolUsuario> _roles;

    public string Nombre { get; private set; }
    public string Apellido { get; private set; }
    public string Email { get; private set; }
    public DateTime FechaNacimiento { get; private set; }
    public string? ContraseniaCifrada => _contraseniaCifrada;

    public Usuario(DatosUsuario datos)
    {
        ValidarNombre(datos.Nombre);
        ValidarApellido(datos.Apellido);
        ValidarEmail(datos.Email);

        var fecha = ParsearFechaNacimiento(datos.FechaNacimiento);
        ValidarFechaNacimiento(fecha);

        Nombre = datos.Nombre;
        Apellido = datos.Apellido;
        Email = datos.Email;
        FechaNacimiento = fecha;
        _roles = new HashSet<RolUsuario>();
    }

    private static void ValidarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre no puede estar vacÃ­o.", nameof(nombre));
    }

    private static void ValidarApellido(string apellido)
    {
        if (string.IsNullOrWhiteSpace(apellido))
            throw new ArgumentException("El apellido no puede estar vacÃ­o.", nameof(apellido));
    }

    private static void ValidarEmail(string email)
    {
        ValidarEmailNoVacio(email);
        ValidarFormatoEmail(email);
    }

    private static void ValidarEmailNoVacio(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("El email no puede estar vacÃ­o.", nameof(email));
    }

    private static void ValidarFormatoEmail(string email)
    {
        if (!Regex.IsMatch(email, PatronEmail))
            throw new ArgumentException("El email no es vÃ¡lido.", nameof(email));
    }

    private static void ValidarFechaNacimientoNoVacia(string fechaNacimiento)
    {
        if (string.IsNullOrWhiteSpace(fechaNacimiento))
            throw new ArgumentException("La fecha de nacimiento no puede estar vacÃ­a.", nameof(fechaNacimiento));
    }

    private static void ValidarFormatoFechaNacimiento(string fechaNacimiento)
    {
        if (!Regex.IsMatch(fechaNacimiento, PatronFormatoFecha))
            throw new ArgumentException("La fecha de nacimiento debe tener el formato MM-DD-AAAA.", nameof(fechaNacimiento));
    }

    private static DateTime ParsearFechaNacimiento(string fechaNacimiento)
    {
        const string formato = "MM-dd-yyyy";

        ValidarFechaNacimientoNoVacia(fechaNacimiento);
        ValidarFormatoFechaNacimiento(fechaNacimiento);

        if (!DateTime.TryParseExact(fechaNacimiento, formato,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out var fecha))
            throw new ArgumentException("La fecha de nacimiento no es una fecha vÃ¡lida.", nameof(fechaNacimiento));

        return fecha;
    }

    private static void ValidarFechaNacimiento(DateTime fechaNacimiento)
    {
        if (fechaNacimiento > DateTime.Now)
            throw new ArgumentException("La fecha de nacimiento no puede ser futura.", nameof(fechaNacimiento));
    }

    public void EstablecerContrasenia(string contrasenia)
    {
        ValidarContrasenia(contrasenia);
        _contraseniaCifrada = CifrarContrasenia(contrasenia);
    }

    private static void ValidarContrasenia(string contrasenia)
    {
        ValidarContraseniaNoVacia(contrasenia);
        ValidarLongitudMinima(contrasenia);
        ValidarTieneMayuscula(contrasenia);
        ValidarTieneMinuscula(contrasenia);
        ValidarTieneNumero(contrasenia);
        ValidarTieneCaracterEspecial(contrasenia);
    }

    private static void ValidarContraseniaNoVacia(string contrasenia)
    {
        if (string.IsNullOrWhiteSpace(contrasenia))
            throw new ArgumentException("La contrasenia no puede estar vacÃ­a.", nameof(contrasenia));
    }

    private static void ValidarLongitudMinima(string contrasenia)
    {
        if (contrasenia.Length < 8)
            throw new ArgumentException("La contrasenia debe tener al menos 8 caracteres.", nameof(contrasenia));
    }

    private static void ValidarTieneMayuscula(string contrasenia)
    {
        if (!Regex.IsMatch(contrasenia, PatronMayuscula))
            throw new ArgumentException("La contrasenia debe incluir al menos una letra mayÃºscula.", nameof(contrasenia));
    }

    private static void ValidarTieneMinuscula(string contrasenia)
    {
        if (!Regex.IsMatch(contrasenia, PatronMinuscula))
            throw new ArgumentException("La contrasenia debe incluir al menos una letra minÃºscula.", nameof(contrasenia));
    }

    private static void ValidarTieneNumero(string contrasenia)
    {
        if (!Regex.IsMatch(contrasenia, PatronNumero))
            throw new ArgumentException("La contrasenia debe incluir al menos un nÃºmero.", nameof(contrasenia));
    }

    private static void ValidarTieneCaracterEspecial(string contrasenia)
    {
        if (!Regex.IsMatch(contrasenia, PatronEspecial))
            throw new ArgumentException("La contrasenia debe incluir al menos un carÃ¡cter especial (@, #, $, ., !, &, *, -, _, +, =, ?).", nameof(contrasenia));
    }

    private static string CifrarContrasenia(string contrasenia)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(contrasenia));
        return Convert.ToBase64String(hashedBytes);
    }

    public bool VerificarContrasenia(string contrasenia)
    {
        if (string.IsNullOrEmpty(_contraseniaCifrada))
            return false;

        var contraseniaCifradaIngresada = CifrarContrasenia(contrasenia);
        return _contraseniaCifrada == contraseniaCifradaIngresada;
    }

    public void AgregarRol(RolUsuario rolUsuario)
    {
        _roles.Add(rolUsuario);
    }

    public void RemoverRol(RolUsuario rolUsuario)
    {
        _roles.Remove(rolUsuario);
    }

    public bool TieneRol(RolUsuario rolUsuario)
    {
        return _roles.Contains(rolUsuario);
    }

    public IEnumerable<RolUsuario> ObtenerRoles()
    {
        return _roles.AsEnumerable();
    }

    public void ActualizarNombre(string nuevoNombre)
    {
        ValidarNombre(nuevoNombre);
        Nombre = nuevoNombre;
    }

    public void ActualizarApellido(string nuevoApellido)
    {
        ValidarApellido(nuevoApellido);
        Apellido = nuevoApellido;
    }

    public void ActualizarEmail(string nuevoEmail)
    {
        ValidarEmail(nuevoEmail);
        Email = nuevoEmail;
    }

    public void ActualizarFechaNacimiento(string nuevaFecha)
    {
        var fecha = ParsearFechaNacimiento(nuevaFecha);
        ValidarFechaNacimiento(fecha);
        FechaNacimiento = fecha;
    }
}
~~~

### Confederacion.cs

Ruta: WorldCupPlanner.Dominio\Enumeraciones\Confederacion.cs

~~~csharp
namespace WorldCupPlanner.Dominio.Enumeraciones;

public enum Confederacion
{
    UEFA, CONMEBOL, CONCACAF, CAF, AFC, OFC
}
~~~

### FaseTorneo.cs

Ruta: WorldCupPlanner.Dominio\Enumeraciones\FaseTorneo.cs

~~~csharp
namespace WorldCupPlanner.Dominio.Enumeraciones;

public enum FaseTorneo
{
    FaseDeGrupos,
    PrimeraRondaEliminatoria,
    Octavos,
    Cuartos,
    Semifinal,
    Final
}
~~~

### RolUsuario.cs

Ruta: WorldCupPlanner.Dominio\Enumeraciones\RolUsuario.cs

~~~csharp
namespace WorldCupPlanner.Dominio.Enumeraciones;

public enum RolUsuario
{
    AdministradorDelSistema,
    Editor
}

~~~

### EntidadInexistenteExcepcion.cs

Ruta: WorldCupPlanner.Dominio\Excepciones\EntidadInexistenteExcepcion.cs

~~~csharp
namespace WorldCupPlanner.Dominio.Excepciones;

public class EntidadInexistenteExcepcion : Exception
{
    public EntidadInexistenteExcepcion(string mensaje) : base(mensaje)
    {
    }
}
~~~

### EntidadInvalidaExcepcion.cs

Ruta: WorldCupPlanner.Dominio\Excepciones\EntidadInvalidaExcepcion.cs

~~~csharp
namespace WorldCupPlanner.Dominio.Excepciones;

public class EntidadInvalidaExcepcion : Exception
{
        public EntidadInvalidaExcepcion(string mensaje) : base(mensaje)
        {
        }
}
~~~

### EntidadYaExistenteExcepcion.cs

Ruta: WorldCupPlanner.Dominio\Excepciones\EntidadYaExistenteExcepcion.cs

~~~csharp
namespace WorldCupPlanner.Dominio.Excepciones;

public class EntidadYaExistenteExcepcion : Exception
{  
    public EntidadYaExistenteExcepcion(string mensaje) : base(mensaje)
    {
    }
}
~~~

### WorldCupPlanner.Dominio.csproj

Ruta: WorldCupPlanner.Dominio\WorldCupPlanner.Dominio.csproj

~~~xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>


~~~

End of WorldCupPlanner.Dominio

## WorldCupPlanner.Servicios

### Normalizar.cs

Ruta: WorldCupPlanner.Servicios\Normalizar.cs

~~~csharp
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace WorldCupPlanner.Servicios;

public class Normalizar
{
    public static string NormalizarTexto(string texto)
    {
        var normalizado = texto.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var c in normalizado)
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        var sinTildes = sb.ToString().Normalize(NormalizationForm.FormC).ToLower();
        var sinEspeciales = Regex.Replace(sinTildes, @"[^a-z0-9 ]", " ");
        return Regex.Replace(sinEspeciales, @" +", " ").Trim();
    }
}
~~~

### ServicioAuditoria.cs

Ruta: WorldCupPlanner.Servicios\ServicioAuditoria.cs

~~~csharp
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.Servicios;

public class ServicioAuditoria
{
    private readonly IRepositorioConId<Auditoria> _repo;

    public ServicioAuditoria(IRepositorioConId<Auditoria> repo)
    {
        _repo = repo;
    }

    public void Registrar(string usuario, string accion, string detalle = "")
    {
        var entry = new Auditoria(_repo.SiguienteId(), usuario, accion, detalle);
        _repo.Agregar(entry);
    }
}
~~~

### ServicioClasificacion.cs

Ruta: WorldCupPlanner.Servicios\ServicioClasificacion.cs

~~~csharp
using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.Servicios;

public class ServicioClasificacion
{
    private const int PuntosPorVictoria = 3;
    private const int PuntosPorEmpate = 1;

    public List<PosicionEquipo> CalcularTabla(Grupo grupo)
    {
        return CalcularTabla(grupo, new ConfiguracionTorneo());
    }

    public List<PosicionEquipo> CalcularTabla(Grupo grupo, ConfiguracionTorneo configuracionTorneo)
    {
        var posiciones = CrearPosicionesIniciales(grupo);

        foreach (var partido in grupo.Partidos)
        {
            if (partido.EstaJugado())
            {
                ActualizarPosicionesConPartido(posiciones, partido);
            }
        }

        return OrdenarPosicionesConDesempateDeterministico(
            posiciones.Values.ToList(),
            configuracionTorneo.SemillaCrucesFase
        );
    }

    public ClasificadosFaseGrupos ObtenerClasificadosAFaseEliminatoria(
        Fixture fixture,
        ConfiguracionTorneo configuracionTorneo)
    {
        var tablas = fixture.Grupos
            .Select(g => CalcularTabla(g, configuracionTorneo))
            .ToList();

        var primerosOrdenados = OrdenarPosicionesConDesempateDeterministico(
            tablas.Select(t => t[0]).ToList(),
            configuracionTorneo.SemillaCrucesFase
        );

        var segundosOrdenados = OrdenarPosicionesConDesempateDeterministico(
            tablas.Select(t => t[1]).ToList(),
            configuracionTorneo.SemillaCrucesFase
        );

        var mejoresTerceros = OrdenarPosicionesConDesempateDeterministico(
                tablas.Select(t => t[2]).ToList(),
                configuracionTorneo.SemillaCrucesFase
            )
            .Take(8)
            .ToList();

        var clasificados = new ClasificadosFaseGrupos
        {
            Primeros = primerosOrdenados.Select(p => p.Equipo).ToList(),
            Segundos = segundosOrdenados.Select(p => p.Equipo).ToList(),
            MejoresTerceros = mejoresTerceros.Select(p => p.Equipo).ToList()
        };

        foreach (var posicion in primerosOrdenados.Concat(segundosOrdenados).Concat(mejoresTerceros))
        {
            clasificados.GruposDeOrigen[posicion.Equipo] = posicion.Grupo;
        }

        return clasificados;
    }

    private static Dictionary<Equipo, PosicionEquipo> CrearPosicionesIniciales(Grupo grupo)
    {
        var posiciones = new Dictionary<Equipo, PosicionEquipo>();

        foreach (var equipo in grupo.Equipos)
        {
            posiciones[equipo] = new PosicionEquipo
            {
                Equipo = equipo,
                Grupo = grupo
            };
        }

        return posiciones;
    }

    private static bool PartidoTieneEquipos(Partido partido) =>
        partido.EquipoLocal is not null && partido.EquipoVisitante is not null;

    private static void ActualizarPosicionesConPartido(Dictionary<Equipo, PosicionEquipo> posiciones, Partido partido)
    {
        if (!PartidoTieneEquipos(partido)) return;

        var posicionLocal = posiciones[partido.EquipoLocal!];
        var posicionVisitante = posiciones[partido.EquipoVisitante!];

        int golesLocal = partido.GolesLocal!.Value;
        int golesVisitante = partido.GolesVisitante!.Value;

        posicionLocal.GolesAFavor += golesLocal;
        posicionLocal.GolesEnContra += golesVisitante;

        posicionVisitante.GolesAFavor += golesVisitante;
        posicionVisitante.GolesEnContra += golesLocal;

        ActualizarPuntos(posicionLocal, posicionVisitante, partido);
    }

    private static void ActualizarPuntos(
        PosicionEquipo posicionLocal,
        PosicionEquipo posicionVisitante,
        Partido partido)
    {
        int golesLocal = partido.GolesLocal!.Value;
        int golesVisitante = partido.GolesVisitante!.Value;

        if (golesLocal > golesVisitante)
        {
            posicionLocal.Puntos += PuntosPorVictoria;
        }
        else if (golesVisitante > golesLocal)
        {
            posicionVisitante.Puntos += PuntosPorVictoria;
        }
        else
        {
            posicionLocal.Puntos += PuntosPorEmpate;
            posicionVisitante.Puntos += PuntosPorEmpate;
        }
    }

    private static List<PosicionEquipo> AplicarDesempateDeterministico(
        List<PosicionEquipo> tablaOrdenada,
        int semillaCrucesFase)
    {
        var random = new Random(semillaCrucesFase);
        var tablaFinal = new List<PosicionEquipo>();

        var gruposEmpatados = tablaOrdenada
            .GroupBy(p => new
            {
                p.Puntos,
                p.DiferenciaGoles,
                p.GolesAFavor
            });

        foreach (var grupoEmpatado in gruposEmpatados)
        {
            var posiciones = grupoEmpatado.ToList();

            if (posiciones.Count > 1)
            {
                MezclarConFisherYates(posiciones, random);
            }

            tablaFinal.AddRange(posiciones);
        }

        return tablaFinal;
    }

    private static void MezclarConFisherYates(List<PosicionEquipo> posiciones, Random random)
    {
        for (int i = posiciones.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (posiciones[i], posiciones[j]) = (posiciones[j], posiciones[i]);
        }
    }

    private static List<PosicionEquipo> OrdenarPosicionesConDesempateDeterministico(
        List<PosicionEquipo> posiciones,
        int semillaCrucesFase)
    {
        var posicionesOrdenadas = posiciones
            .OrderByDescending(p => p.Puntos)
            .ThenByDescending(p => p.DiferenciaGoles)
            .ThenByDescending(p => p.GolesAFavor)
            .ToList();

        return AplicarDesempateDeterministico(posicionesOrdenadas, semillaCrucesFase);
    }
}
~~~

### ServicioCruces.cs

Ruta: WorldCupPlanner.Servicios\ServicioCruces.cs

~~~csharp
using  WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Servicios;

public class ServicioCruces
{
    public List<Partido> GenerarPrimeraRondaEliminatoria(
        ClasificadosFaseGrupos clasificados,
        List<Estadio> estadios,
        ConfiguracionTorneo configuracionTorneo)
    {
        var partidos = new List<Partido>();
        var random = new Random(configuracionTorneo.SemillaCrucesFase);

        var primerosParaTerceros = MezclarEquiposConFisherYates(
            clasificados.Primeros.Take(8).ToList(),
            random);

        var terceros = MezclarEquiposConFisherYates(
            clasificados.MejoresTerceros.ToList(),
            random);

        AgregarCrucesPrimerosContraTerceros(
            partidos,
            primerosParaTerceros,
            terceros,
            clasificados,
            estadios,
            configuracionTorneo
        );

        var primerosRestantes = MezclarEquiposConFisherYates(
            clasificados.Primeros.Skip(8).Take(4).ToList(),
            random);

        var segundosDeMenorRendimiento = MezclarEquiposConFisherYates(
            clasificados.Segundos.Skip(8).Take(4).ToList(),
            random);

        AgregarCrucesPrimerosRestantesContraSegundosDeMenorRendimiento(
            partidos,
            primerosRestantes,
            segundosDeMenorRendimiento,
            clasificados,
            estadios,
            configuracionTorneo
        );

        var segundosRestantes = MezclarEquiposConFisherYates(
            clasificados.Segundos.Take(8).ToList(),
            random);

        AgregarCrucesSegundosRestantesMinimos(
            partidos,
            segundosRestantes,
            clasificados,
            estadios,
            configuracionTorneo
        );

        return partidos;
    }

    private void AgregarCrucesPrimerosContraTerceros(
        List<Partido> partidos,
        List<Equipo> primeros,
        List<Equipo> terceros,
        ClasificadosFaseGrupos clasificados,
        List<Estadio> estadios,
        ConfiguracionTorneo configuracionTorneo)
    {
        ReordenarVisitantesParaEvitarMismoGrupo(primeros, terceros, clasificados);

        for (int i = 0; i < primeros.Count; i++)
        {
            partidos.Add(CrearPartidoDePrimeraRonda(
                partidos.Count + 1,
                primeros[i],
                terceros[i],
                estadios[partidos.Count % estadios.Count],
                configuracionTorneo));
        }
    }

    private void AgregarCrucesPrimerosRestantesContraSegundosDeMenorRendimiento(
        List<Partido> partidos,
        List<Equipo> primerosRestantes,
        List<Equipo> segundosDeMenorRendimiento,
        ClasificadosFaseGrupos clasificados,
        List<Estadio> estadios,
        ConfiguracionTorneo configuracionTorneo)
    {
        ReordenarVisitantesParaEvitarMismoGrupo(
            primerosRestantes,
            segundosDeMenorRendimiento,
            clasificados);

        for (int i = 0; i < primerosRestantes.Count; i++)
        {
            partidos.Add(CrearPartidoDePrimeraRonda(
                partidos.Count + 1,
                primerosRestantes[i],
                segundosDeMenorRendimiento[i],
                estadios[partidos.Count % estadios.Count],
                configuracionTorneo));
        }
    }

    private void AgregarCrucesSegundosRestantesMinimos(
        List<Partido> partidos,
        List<Equipo> segundosRestantes,
        ClasificadosFaseGrupos clasificados,
        List<Estadio> estadios,
        ConfiguracionTorneo configuracionTorneo)
    {
        ReordenarEquiposParaEvitarCrucesInternosDelMismoGrupo(segundosRestantes, clasificados);

        for (int i = 0; i < segundosRestantes.Count; i += 2)
        {
            partidos.Add(CrearPartidoDePrimeraRonda(
                partidos.Count + 1,
                segundosRestantes[i],
                segundosRestantes[i + 1],
                estadios[partidos.Count % estadios.Count],
                configuracionTorneo));
        }
    }

    private Partido CrearPartidoDePrimeraRonda(
        int id,
        Equipo equipoLocal,
        Equipo equipoVisitante,
        Estadio estadio,
        ConfiguracionTorneo configuracionTorneo)
    {
        return new Partido
        {
            Id = id,
            EquipoLocal = equipoLocal,
            EquipoVisitante = equipoVisitante,
            Estadio = estadio,
            Fecha = configuracionTorneo.FechaInicioTorneo,
            Fase = FaseTorneo.PrimeraRondaEliminatoria
        };
    }

    private List<Equipo> MezclarEquiposConFisherYates(List<Equipo> equipos, Random random)
    {
        for (int i = equipos.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (equipos[i], equipos[j]) = (equipos[j], equipos[i]);
        }

        return equipos;
    }

    private void ReordenarVisitantesParaEvitarMismoGrupo(
        List<Equipo> locales,
        List<Equipo> visitantes,
        ClasificadosFaseGrupos clasificados)
    {
        for (int i = 0; i < locales.Count; i++)
        {
            if (!PertenecenAlMismoGrupo(locales[i], visitantes[i], clasificados))
            {
                continue;
            }

            var indiceAlternativo = BuscarVisitanteAlternativo(locales, visitantes, clasificados, i);

            if (indiceAlternativo.HasValue)
            {
                (visitantes[i], visitantes[indiceAlternativo.Value]) =
                    (visitantes[indiceAlternativo.Value], visitantes[i]);
            }
        }
    }

    private int? BuscarVisitanteAlternativo(
        List<Equipo> locales,
        List<Equipo> visitantes,
        ClasificadosFaseGrupos clasificados,
        int indiceActual)
    {
        for (int i = 0; i < visitantes.Count; i++)
        {
            if (i == indiceActual)
            {
                continue;
            }

            var evitaCruceActual = !PertenecenAlMismoGrupo(
                locales[indiceActual],
                visitantes[i],
                clasificados);

            var noRompeCruceFuturo = !PertenecenAlMismoGrupo(
                locales[i],
                visitantes[indiceActual],
                clasificados);

            if (evitaCruceActual && noRompeCruceFuturo)
            {
                return i;
            }
        }

        return null;
    }

    private void ReordenarEquiposParaEvitarCrucesInternosDelMismoGrupo(
        List<Equipo> equipos,
        ClasificadosFaseGrupos clasificados)
    {
        for (int i = 0; i < equipos.Count; i += 2)
        {
            if (!PertenecenAlMismoGrupo(equipos[i], equipos[i + 1], clasificados))
            {
                continue;
            }

            var indiceAlternativo = BuscarRivalAlternativo(equipos, clasificados, i);

            if (indiceAlternativo.HasValue)
            {
                (equipos[i + 1], equipos[indiceAlternativo.Value]) =
                    (equipos[indiceAlternativo.Value], equipos[i + 1]);
            }
        }
    }

    private int? BuscarRivalAlternativo(
        List<Equipo> equipos,
        ClasificadosFaseGrupos clasificados,
        int indiceLocal)
    {
        var indiceVisitante = indiceLocal + 1;

        for (int i = 0; i < equipos.Count; i++)
        {
            if (i == indiceLocal || i == indiceVisitante)
            {
                continue;
            }

            var indiceCompanero = ObtenerIndiceCompaneroDeCruce(i);

            var evitaCruceActual = !PertenecenAlMismoGrupo(
                equipos[indiceLocal],
                equipos[i],
                clasificados);

            var noRompeCruceAlternativo = !PertenecenAlMismoGrupo(
                equipos[indiceCompanero],
                equipos[indiceVisitante],
                clasificados);

            if (evitaCruceActual && noRompeCruceAlternativo)
            {
                return i;
            }
        }

        return null;
    }

    private int ObtenerIndiceCompaneroDeCruce(int indice)
    {
        return indice % 2 == 0 ? indice + 1 : indice - 1;
    }

    private bool PertenecenAlMismoGrupo(
        Equipo equipo1,
        Equipo equipo2,
        ClasificadosFaseGrupos clasificados)
    {
        var grupo1 = clasificados.ObtenerGrupoDeOrigen(equipo1);
        var grupo2 = clasificados.ObtenerGrupoDeOrigen(equipo2);

        return grupo1 != null && ReferenceEquals(grupo1, grupo2);
    }
    
    public List<Partido> GenerarOctavos(
        List<Partido> partidosPrimeraRonda,
        List<Estadio> estadios,
        ConfiguracionTorneo configuracionTorneo)
    {
        if (partidosPrimeraRonda.Any(p => !p.EstaJugado()))
        {
            throw new InvalidOperationException("No se pueden generar octavos si hay partidos de primera ronda sin jugar.");
        }

        var ganadores = partidosPrimeraRonda
            .Select(p => p.Ganador())
            .ToList();

        var partidos = new List<Partido>();
        var proximoId = partidosPrimeraRonda.Max(p => p.Id) + 1;

        for (int i = 0; i < ganadores.Count; i += 2)
        {
            partidos.Add(new Partido
            {
                Id = proximoId,
                EquipoLocal = ganadores[i],
                EquipoVisitante = ganadores[i + 1],
                Estadio = estadios[partidos.Count % estadios.Count],
                Fecha = configuracionTorneo.FechaInicioTorneo.AddDays(configuracionTorneo.SeparacionEntreFechas),
                Fase = FaseTorneo.Octavos
            });

            proximoId++;
        }

        return partidos;
    }
}
~~~

### ServicioEquipo.cs

Ruta: WorldCupPlanner.Servicios\ServicioEquipo.cs

~~~csharp
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
~~~

### ServicioEstadio.cs

Ruta: WorldCupPlanner.Servicios\ServicioEstadio.cs

~~~csharp
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.AccesoADatos.Repositorios;
using  WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.Servicios;

public class ServicioEstadio
{
    private readonly IRepositorio<Estadio> _repositorioEstadio;

    public ServicioEstadio()
    {
        _repositorioEstadio = new RepositorioEstadioEnMemoria();
    }

    public ServicioEstadio(IRepositorio<Estadio> repositorioEstadio)
    {
        _repositorioEstadio = repositorioEstadio;
    }

    public void AgregarEstadio(Estadio estadio)
    {
        if (!estadio.EsValido())
            throw new EntidadInvalidaExcepcion("El estadio no es valido.");
        if (_repositorioEstadio.Buscar(e => e.Nombre == estadio.Nombre).Count > 0)
            throw new EntidadYaExistenteExcepcion("Ya existe un estadio con ese nombre.");
        _repositorioEstadio.Agregar(estadio);
    }

    public void EditarEstadio(string nombreOriginal, Estadio estadio)
    {
        if (!estadio.EsValido())
            throw new EntidadInvalidaExcepcion("El estadio no es valido.");
        var existente = _repositorioEstadio.Buscar(e => ReferenceEquals(e, estadio) || e.Nombre == nombreOriginal).FirstOrDefault();
        if (existente == null)
            throw new EntidadInvalidaExcepcion("El estadio no existe.");
        existente.Nombre = estadio.Nombre;
        existente.Ciudad = estadio.Ciudad;
        existente.Capacidad = estadio.Capacidad;
        existente.Descripcion = estadio.Descripcion;
        _repositorioEstadio.Actualizar(existente);
    }

    public void EliminarEstadio(Estadio estadio)
    {
        _repositorioEstadio.Eliminar(estadio);
    }

    public List<Estadio> ObtenerEstadios()
    {
        return _repositorioEstadio.Buscar(_ => true);
    }
}
~~~

### ServicioFixture.cs

Ruta: WorldCupPlanner.Servicios\ServicioFixture.cs

~~~csharp
using  WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Servicios;

public class ServicioFixture
{
    private const int CantidadExactaEquipos = 48;
    private const int CantidadMinimaEstadios = 4;
    private const int CantidadGrupos = 12;
    private const int CantidadEquiposPorGrupo = 4;
    private const int MaximoEquiposUefaPorGrupo = 2;
    private const int MaximoEquiposOtrasConfederacionesPorGrupo = 1;

    private const int IndiceJornadaUno = 0;
    private const int IndiceJornadaDos = 1;
    private const int IndiceJornadaTres = 2;
    private const int CantidadJornadasPorGrupo = 3;

    private const int EquipoUno = 0;
    private const int EquipoDos = 1;
    private const int EquipoTres = 2;
    private const int EquipoCuatro = 3;

    private const char PrimeraEtiquetaGrupo = 'A';
    private const char UltimaEtiquetaGrupo = 'L';

    public Fixture GenerarFixture(List<Equipo> equipos, List<Estadio> estadios, ConfiguracionTorneo configuracionTorneo)
    {
        ValidarCantidadEquipos(equipos);
        ValidarCantidadEstadios(estadios);

        return CrearFixtureConDistribucionRoundRobin(equipos, estadios, configuracionTorneo);
    }

    private void ValidarCantidadEquipos(List<Equipo> equipos)
    {
        ValidarCantidadExactaDeEquipos(equipos);
        ValidarTodosLosEquiposSonValidos(equipos);
    }

    private void ValidarCantidadExactaDeEquipos(List<Equipo> equipos)
    {
        if (equipos.Count < CantidadExactaEquipos)
        {
            throw new InvalidOperationException("No se puede generar el fixture con menos de 48 equipos.");
        }

        if (equipos.Count > CantidadExactaEquipos)
        {
            throw new InvalidOperationException("No se puede generar el fixture con más de 48 equipos.");
        }
    }

    private void ValidarTodosLosEquiposSonValidos(List<Equipo> equipos)
    {
        if (equipos.Any(e => !e.EsValido()))
        {
            throw new InvalidOperationException("No se puede generar el fixture con equipos inválidos.");
        }
    }

    private void ValidarCantidadEstadios(List<Estadio> estadios)
    {
        ValidarCantidadMinimaDeEstadios(estadios);
        ValidarTodosLosEstadiosSonValidos(estadios);
    }

    private void ValidarCantidadMinimaDeEstadios(List<Estadio> estadios)
    {
        if (estadios.Count < CantidadMinimaEstadios)
        {
            throw new InvalidOperationException("No se puede generar el fixture con menos de 4 estadios.");
        }
    }

    private void ValidarTodosLosEstadiosSonValidos(List<Estadio> estadios)
    {
        if (estadios.Any(e => !e.EsValido()))
        {
            throw new InvalidOperationException("No se puede generar el fixture con estadios inválidos.");
        }
    }

    private Fixture CrearFixtureConDistribucionRoundRobin(
        List<Equipo> equipos,
        List<Estadio> estadios,
        ConfiguracionTorneo configuracionTorneo)
    {
        var fixture = CrearFixtureVacioConGrupos();
        var equiposOrdenados = OrdenarEquiposParaFixture(equipos, configuracionTorneo.SemillaFixture);

        for (int i = 0; i < equiposOrdenados.Count; i++)
        {
            AsignarEquipoAUnGrupoValido(fixture, equiposOrdenados[i], i);
        }

        GenerarPartidosDeGrupos(fixture, configuracionTorneo, estadios);

        return fixture;
    }

    private List<Equipo> OrdenarEquiposParaFixture(List<Equipo> equipos, int semillaFixture)
    {
        var random = new Random(semillaFixture);
        var equiposOrdenados = new List<Equipo>();

        foreach (var grupoDeRanking in ObtenerGruposPorRankingDescendente(equipos))
        {
            var equiposMezclados = MezclarEquiposConFisherYates(grupoDeRanking.ToList(), random);
            equiposOrdenados.AddRange(equiposMezclados);
        }

        return equiposOrdenados;
    }

    private IEnumerable<IGrouping<int, Equipo>> ObtenerGruposPorRankingDescendente(List<Equipo> equipos)
    {
        return equipos
            .GroupBy(e => e.RankingFIFA)
            .OrderByDescending(g => g.Key);
    }

    private List<Equipo> MezclarEquiposConFisherYates(List<Equipo> equipos, Random random)
    {
        for (int i = equipos.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (equipos[i], equipos[j]) = (equipos[j], equipos[i]);
        }

        return equipos;
    }

    private Fixture CrearFixtureVacioConGrupos()
    {
        var fixture = new Fixture();

        for (char etiqueta = PrimeraEtiquetaGrupo; etiqueta <= UltimaEtiquetaGrupo; etiqueta++)
        {
            fixture.Grupos.Add(new Grupo(etiqueta.ToString()));
        }

        return fixture;
    }

    private void AsignarEquipoAUnGrupoValido(Fixture fixture, Equipo equipo, int indiceEquipo)
    {
        for (int intento = 0; intento < CantidadGrupos; intento++)
        {
            var grupo = ObtenerGrupoCandidato(fixture, indiceEquipo, intento);

            if (PuedeAsignarseEquipoAGrupo(equipo, grupo))
            {
                grupo.Equipos.Add(equipo);
                return;
            }
        }

        throw new InvalidOperationException(
            "No se pudo asignar el equipo a ningún grupo respetando las restricciones de confederación.");
    }

    private Grupo ObtenerGrupoCandidato(Fixture fixture, int indiceEquipo, int intento)
    {
        int indiceGrupo = (indiceEquipo + intento) % CantidadGrupos;
        return fixture.Grupos[indiceGrupo];
    }

    private bool PuedeAsignarseEquipoAGrupo(Equipo equipo, Grupo grupo)
    {
        if (grupo.Equipos.Count >= CantidadEquiposPorGrupo)
        {
            return false;
        }

        if (!equipo.EsValido())
        {
            return false;
        }

        return CumpleRestriccionDeConfederacion(equipo, grupo);
    }

    private bool CumpleRestriccionDeConfederacion(Equipo equipo, Grupo grupo)
    {
        var confederacion = equipo.Confederacion!.Value;
        int cantidadMismaConfederacion = grupo.Equipos.Count(e => e.Confederacion == confederacion);

        if (confederacion == Confederacion.UEFA)
        {
            return cantidadMismaConfederacion < MaximoEquiposUefaPorGrupo;
        }

        return cantidadMismaConfederacion < MaximoEquiposOtrasConfederacionesPorGrupo;
    }

    private void GenerarPartidosDeGrupos(
        Fixture fixture,
        ConfiguracionTorneo configuracionTorneo,
        List<Estadio> estadios)
    {
        var estadiosOrdenados = OrdenarEstadiosPorNombreNormalizado(estadios);
        var indicePartidoGlobal = 0;

        for (int indiceGrupo = 0; indiceGrupo < fixture.Grupos.Count; indiceGrupo++)
        {
            var grupo = fixture.Grupos[indiceGrupo];
            GenerarPartidosDelGrupo(grupo, configuracionTorneo, estadiosOrdenados, indiceGrupo, ref indicePartidoGlobal);
        }
    }

    private void GenerarPartidosDelGrupo(
        Grupo grupo,
        ConfiguracionTorneo configuracionTorneo,
        List<Estadio> estadiosOrdenados,
        int indiceGrupo,
        ref int indicePartidoGlobal)
    {
        var equipos = grupo.Equipos;
        var fechaBaseGrupo = ObtenerFechaBaseDelGrupo(configuracionTorneo, indiceGrupo);

        var (fechaJornada1Partido1, fechaJornada1Partido2) = ObtenerFechasJornada1(configuracionTorneo, fechaBaseGrupo);
        var (fechaJornada2Partido1, fechaJornada2Partido2) = ObtenerFechasJornada2(configuracionTorneo, fechaBaseGrupo);
        var fechaJornada3 = ObtenerFechasJornada3(configuracionTorneo, fechaBaseGrupo);

        AgregarPartidoDeGrupo(grupo, equipos, EquipoUno, EquipoCuatro, fechaJornada1Partido1, estadiosOrdenados, ref indicePartidoGlobal);
        AgregarPartidoDeGrupo(grupo, equipos, EquipoDos, EquipoTres, fechaJornada1Partido2, estadiosOrdenados, ref indicePartidoGlobal);

        AgregarPartidoDeGrupo(grupo, equipos, EquipoUno, EquipoTres, fechaJornada2Partido1, estadiosOrdenados, ref indicePartidoGlobal);
        AgregarPartidoDeGrupo(grupo, equipos, EquipoDos, EquipoCuatro, fechaJornada2Partido2, estadiosOrdenados, ref indicePartidoGlobal);

        AgregarPartidoDeGrupo(grupo, equipos, EquipoUno, EquipoDos, fechaJornada3, estadiosOrdenados, ref indicePartidoGlobal);
        AgregarPartidoDeGrupo(grupo, equipos, EquipoTres, EquipoCuatro, fechaJornada3, estadiosOrdenados, ref indicePartidoGlobal);
    }

    private void AgregarPartidoDeGrupo(
        Grupo grupo,
        List<Equipo> equipos,
        int indiceEquipoLocal,
        int indiceEquipoVisitante,
        DateTime fecha,
        List<Estadio> estadiosOrdenados,
        ref int indicePartidoGlobal)
    {
        var estadio = ObtenerEstadioParaPartido(estadiosOrdenados, indicePartidoGlobal);
        int idPartido = ObtenerSiguienteIdPartido(indicePartidoGlobal);
        var partido = CrearPartidoDeGrupoConId(
            grupo,
            equipos[indiceEquipoLocal],
            equipos[indiceEquipoVisitante],
            fecha,
            estadio,
            idPartido
        );

        grupo.Partidos.Add(partido);
        indicePartidoGlobal++;
    }

    private DateTime ObtenerFechaBaseDelGrupo(ConfiguracionTorneo configuracionTorneo, int indiceGrupo)
    {
        return configuracionTorneo.FechaInicioTorneo
            .AddDays(indiceGrupo * configuracionTorneo.SeparacionEntreFechas * CantidadJornadasPorGrupo);
    }

    private DateTime ObtenerFechaDeJornadaDelGrupo(DateTime fechaBase, ConfiguracionTorneo configuracionTorneo, int indiceJornada)
    {
        return fechaBase.Date
            .Add(configuracionTorneo.HoraInicioPartidos)
            .AddDays(configuracionTorneo.SeparacionEntreFechas * indiceJornada);
    }


    private DateTime ObtenerFechaSegundoPartidoDeJornada(ConfiguracionTorneo configuracionTorneo, DateTime fechaPrimerPartido)
    {
        return fechaPrimerPartido.AddHours(configuracionTorneo.SeparacionHorasEntrePartidos);
    }

    private (DateTime Partido1, DateTime Partido2) ObtenerFechasJornada1(ConfiguracionTorneo configuracionTorneo, DateTime fechaBaseGrupo)
    {
        var fechaPartido1 = ObtenerFechaDeJornadaDelGrupo(fechaBaseGrupo, configuracionTorneo, IndiceJornadaUno);
        var fechaPartido2 = ObtenerFechaSegundoPartidoDeJornada(configuracionTorneo, fechaPartido1);
        return (fechaPartido1, fechaPartido2);
    }

    private (DateTime Partido1, DateTime Partido2) ObtenerFechasJornada2(ConfiguracionTorneo configuracionTorneo, DateTime fechaBaseGrupo)
    {
        var fechaPartido1 = ObtenerFechaDeJornadaDelGrupo(fechaBaseGrupo, configuracionTorneo, IndiceJornadaDos);
        var fechaPartido2 = ObtenerFechaSegundoPartidoDeJornada(configuracionTorneo, fechaPartido1);
        return (fechaPartido1, fechaPartido2);
    }

    private DateTime ObtenerFechasJornada3(ConfiguracionTorneo configuracionTorneo, DateTime fechaBaseGrupo)
    {
        return ObtenerFechaDeJornadaDelGrupo(fechaBaseGrupo, configuracionTorneo, IndiceJornadaTres);
    }

    private List<Estadio> OrdenarEstadiosPorNombreNormalizado(List<Estadio> estadios)
    {
        return estadios
            .OrderBy(e => Normalizar.NormalizarTexto(e.Nombre))
            .ToList();
    }

    private Estadio ObtenerEstadioParaPartido(List<Estadio> estadiosOrdenados, int indicePartidoGlobal)
    {
        return estadiosOrdenados[indicePartidoGlobal % estadiosOrdenados.Count];
    }

    private int ObtenerSiguienteIdPartido(int indicePartidoGlobal)
    {
        return indicePartidoGlobal + 1;
    }

    private Partido CrearPartidoDeGrupoConId(
        Grupo grupo,
        Equipo local,
        Equipo visitante,
        DateTime fecha,
        Estadio estadio,
        int id)
    {
        return new Partido
        {
            Id = id,
            Fecha = fecha,
            Estadio = estadio,
            EquipoLocal = local,
            EquipoVisitante = visitante,
            Grupo = grupo,
            Fase = FaseTorneo.FaseDeGrupos
        };
    }
}
~~~

### ServicioUsuario.cs

Ruta: WorldCupPlanner.Servicios\ServicioUsuario.cs

~~~csharp
using WorldCupPlanner.AccesoADatos.Repositorios;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.Servicios;

public class ServicioUsuario
{
    private readonly RepositorioUsuarioEnMemoria _repositorio;

    public ServicioUsuario(RepositorioUsuarioEnMemoria repositorio)
    {
        _repositorio = repositorio;
    }

    public void AgregarUsuario(Usuario usuario)
    {
        ValidarEmailUnico(usuario.Email);
        _repositorio.Agregar(usuario);
    }
    
    public void ActualizarUsuario(Usuario usuario, string? nombre = null, string? apellido = null,
        string? email = null, string? fechaNacimiento = null)
    {
        var usuarioActual = _repositorio.Buscar(u => ReferenceEquals(u, usuario))
            .FirstOrDefault();

        if (usuarioActual == null)
        {
            throw new EntidadInvalidaExcepcion("El usuario no existe en el sistema.");
        }

        if (email != null && !email.Equals(usuarioActual.Email, StringComparison.OrdinalIgnoreCase))
        {
            ValidarEmailUnico(email);
            usuarioActual.ActualizarEmail(email);
        }

        if (nombre != null)
        {
            usuarioActual.ActualizarNombre(nombre);
        }

        if (apellido != null)
        {
            usuarioActual.ActualizarApellido(apellido);
        }

        if (fechaNacimiento != null)
        {
            usuarioActual.ActualizarFechaNacimiento(fechaNacimiento);
        }

        _repositorio.Actualizar(usuarioActual);
    }

    public void EliminarUsuario(Usuario usuario)
    {
        _repositorio.Eliminar(usuario);
    }

    public List<Usuario> ObtenerTodosLosUsuarios()
    {
        return _repositorio.Buscar(_ => true);
    }

    public Usuario? ObtenerPorEmail(string email)
    {
        return _repositorio.ObtenerPorEmail(email);
    }

    private void ValidarEmailUnico(string email)
    {
        var usuarioExistente = _repositorio.ObtenerPorEmail(email);
        if (usuarioExistente != null)
        {
            throw new InvalidOperationException($"El email '{email}' ya está registrado en el sistema.");
        }
    }

    public void GenerarContraseniaDefecto(Usuario usuario, int seed = 0)
    {
        var contraseniaDefecto = $"TempPass@{2026 + seed}";
        usuario.EstablecerContrasenia(contraseniaDefecto);
    }

    public void ReiniciarContrasenia(Usuario usuario, Usuario administrador)
    {
        if (!administrador.TieneRol(RolUsuario.AdministradorDelSistema))
        {
            throw new InvalidOperationException("Solo un administrador puede reiniciar contraseñas.");
        }

        GenerarContraseniaDefecto(usuario);
    }

    public void CambiarContrasenia(Usuario usuario, string contraseniaAnterior, string nuevaContrasenia)
    {
        if (!usuario.VerificarContrasenia(contraseniaAnterior))
        {
            throw new InvalidOperationException("La contraseña actual es incorrecta.");
        }

        usuario.EstablecerContrasenia(nuevaContrasenia);
    }

    public Usuario Login(string email, string contrasenia)
    {
        var usuario = _repositorio.ObtenerPorEmail(email);
        if (usuario == null)
        {
            throw new InvalidOperationException("Email o contraseña incorrectos.");
        }

        if (!usuario.VerificarContrasenia(contrasenia))
        {
            throw new InvalidOperationException("Email o contraseña incorrectos.");
        }

        return usuario;
    }
}

~~~

### WorldCupPlanner.Servicios.csproj

Ruta: WorldCupPlanner.Servicios\WorldCupPlanner.Servicios.csproj

~~~xml
<Project Sdk="Microsoft.NET.Sdk">

  <ItemGroup>
    <ProjectReference Include="..\WorldCupPlanner.Dominio\WorldCupPlanner.Dominio.csproj" />
    <ProjectReference Include="..\WorldCupPlanner.AccesoADatos\WorldCupPlanner.AccesoADatos.csproj" />
  </ItemGroup>

  <ItemGroup>
    <Folder Include="Implementaciones\" />
    <Folder Include="Interfaces\" />
  </ItemGroup>

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
~~~

End of WorldCupPlanner.Servicios

## WorldCupPlanner.ServicioTests

### ConfiguracionTorneoTests.cs

Ruta: WorldCupPlanner.ServicioTests\ConfiguracionTorneoTests.cs

~~~csharp
using  WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public class ConfiguracionTorneoTests
{
    [TestMethod]
    public void ConfiguracionTorneo_DebeTenerValoresPorDefecto()
    {
        // Arrange & Act
        var configuracionTorneo = new ConfiguracionTorneo();

        // Assert
        Assert.AreEqual(new DateTime(2026, 6, 1), configuracionTorneo.FechaInicioTorneo);
        Assert.AreEqual(3, configuracionTorneo.MaxPartidosPorDia);
        Assert.AreEqual(3, configuracionTorneo.SeparacionEntreFechas);
        Assert.AreEqual(new TimeSpan(14, 0, 0), configuracionTorneo.HoraInicioPartidos);
        Assert.AreEqual(4, configuracionTorneo.SeparacionHorasEntrePartidos);
    }
}
~~~

### NormalizarTest.cs

Ruta: WorldCupPlanner.ServicioTests\NormalizarTest.cs

~~~csharp
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public class NormalizarTests
{
    [TestMethod]
    public void Normalizar_TextoConMayusculas_RetornaMinusculas()
    {
        // Arrange
        var texto = "URUGUAY";

        // Act
        var resultado = Normalizar.NormalizarTexto(texto);

        // Assert
        Assert.AreEqual("uruguay", resultado);
    }

    [TestMethod]
    public void Normalizar_TextoConTildes_EliminaTildes()
    {
        // Arrange
        var texto = "ParÃ­s";

        // Act
        var resultado = Normalizar.NormalizarTexto(texto);

        // Assert
        Assert.AreEqual("paris", resultado);
    }

    [TestMethod]
    public void Normalizar_TextoConCaracteresEspeciales_ReemplazaPorEspacio()
    {
        // Arrange
        var texto = "estadio#central";

        // Act
        var resultado = Normalizar.NormalizarTexto(texto);

        // Assert
        Assert.AreEqual("estadio central", resultado);
    }

    [TestMethod]
    public void Normalizar_TextoConEspaciosMultiples_ColapsaEspacios()
    {
        // Arrange
        var texto = "estadio   central";

        // Act
        var resultado = Normalizar.NormalizarTexto(texto);

        // Assert
        Assert.AreEqual("estadio central", resultado);
    }

    [TestMethod]
    public void Normalizar_TextoConEspaciosAlRededor_EliminaEspacios()
    {
        // Arrange
        var texto = "  uruguay  ";

        // Act
        var resultado = Normalizar.NormalizarTexto(texto);

        // Assert
        Assert.AreEqual("uruguay", resultado);
    }
}
~~~

### ServicioClasificacionTests.cs

Ruta: WorldCupPlanner.ServicioTests\ServicioClasificacionTests.cs

~~~csharp
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public class ServicioClasificacionTests
{
    [TestMethod]
    public void CalcularTabla_ConResultadosDeGrupo_CalculaPuntosCorrectamente()
    {
        // Arrange
        var servicioClasificacion = new ServicioClasificacion();

        var equipo1 = CrearEquipo("Equipo1");
        var equipo2 = CrearEquipo("Equipo2");
        var equipo3 = CrearEquipo("Equipo3");
        var equipo4 = CrearEquipo("Equipo4");

        var grupo = new Grupo("A", new List<Equipo>
        {
            equipo1,
            equipo2,
            equipo3,
            equipo4
        });

        grupo.Partidos.Add(CrearPartido(equipo1, equipo4, 2, 0)); // Equipo1 gana
        grupo.Partidos.Add(CrearPartido(equipo2, equipo3, 1, 1)); // Empate
        grupo.Partidos.Add(CrearPartido(equipo1, equipo3, 0, 0)); // Empate
        grupo.Partidos.Add(CrearPartido(equipo2, equipo4, 3, 1)); // Equipo2 gana
        grupo.Partidos.Add(CrearPartido(equipo1, equipo2, 1, 0)); // Equipo1 gana
        grupo.Partidos.Add(CrearPartido(equipo3, equipo4, 2, 1)); // Equipo3 gana

        // Act
        var tabla = servicioClasificacion.CalcularTabla(grupo);

        // Assert
        Assert.AreEqual(7, ObtenerPuntos(tabla, equipo1));
        Assert.AreEqual(4, ObtenerPuntos(tabla, equipo2));
        Assert.AreEqual(5, ObtenerPuntos(tabla, equipo3));
        Assert.AreEqual(0, ObtenerPuntos(tabla, equipo4));
    }

    private Equipo CrearEquipo(string nombre)
    {
        return CrearEquipo(nombre, Confederacion.UEFA);
    }

    private Equipo CrearEquipo(string nombre, Confederacion confederacion)
    {
        return new Equipo
        {
            Nombre = nombre,
            RankingFIFA = 1000,
            Confederacion = confederacion
        };
    }

    private Partido CrearPartido(Equipo local, Equipo visitante, int golesLocal, int golesVisitante)
    {
        return new Partido
        {
            EquipoLocal = local,
            EquipoVisitante = visitante,
            GolesLocal = golesLocal,
            GolesVisitante = golesVisitante,
            Fase = FaseTorneo.FaseDeGrupos,
            Estadio = new Estadio
            {
                Nombre = "Estadio",
                Ciudad = "Ciudad",
                Capacidad = 30000
            },
            Fecha = DateTime.Now
        };
    }

    private int ObtenerPuntos(List<PosicionEquipo> tabla, Equipo equipo)
    {
        return tabla.Single(p => p.Equipo == equipo).Puntos;
    }

    [TestMethod]
    public void CalcularTabla_ConResultadosDeGrupo_OrdenaPorPuntosDescendente()
    {
        // Arrange
        var servicioClasificacion = new ServicioClasificacion();

        var equipo1 = CrearEquipo("Equipo1");
        var equipo2 = CrearEquipo("Equipo2");
        var equipo3 = CrearEquipo("Equipo3");
        var equipo4 = CrearEquipo("Equipo4");

        var grupo = new Grupo("A", new List<Equipo>
        {
            equipo1,
            equipo2,
            equipo3,
            equipo4
        });

        grupo.Partidos.Add(CrearPartido(equipo1, equipo4, 2, 0)); // Equipo1 gana: 3 pts
        grupo.Partidos.Add(CrearPartido(equipo2, equipo3, 1, 1)); // Equipo2 y Equipo3: 1 pt
        grupo.Partidos.Add(CrearPartido(equipo1, equipo3, 0, 0)); // Equipo1 y Equipo3: +1 pt
        grupo.Partidos.Add(CrearPartido(equipo2, equipo4, 3, 1)); // Equipo2 gana: +3 pts
        grupo.Partidos.Add(CrearPartido(equipo1, equipo2, 1, 0)); // Equipo1 gana: +3 pts
        grupo.Partidos.Add(CrearPartido(equipo3, equipo4, 2, 1)); // Equipo3 gana: +3 pts

        // Act
        var tabla = servicioClasificacion.CalcularTabla(grupo);

        // Assert
        var ordenEsperado = new List<Equipo>
        {
            equipo1, // 7 puntos
            equipo3, // 5 puntos
            equipo2, // 4 puntos
            equipo4  // 0 puntos
        };

        var ordenObtenido = tabla.Select(p => p.Equipo).ToList();

        CollectionAssert.AreEqual(ordenEsperado, ordenObtenido);
    }

    [TestMethod]
    public void CalcularTabla_ConMismosPuntos_OrdenaPorDiferenciaDeGolesDescendente()
    {
        // Arrange
        var servicioClasificacion = new ServicioClasificacion();

        var equipo1 = CrearEquipo("Equipo1");
        var equipo2 = CrearEquipo("Equipo2");
        var equipo3 = CrearEquipo("Equipo3");
        var equipo4 = CrearEquipo("Equipo4");

        var grupo = new Grupo("A", new List<Equipo>
        {
            equipo1,
            equipo2,
            equipo3,
            equipo4
        });

        grupo.Partidos.Add(CrearPartido(equipo1, equipo2, 0, 3)); // Equipo2 gana por 3
        grupo.Partidos.Add(CrearPartido(equipo3, equipo4, 2, 0)); // Equipo3 gana por 2
        grupo.Partidos.Add(CrearPartido(equipo1, equipo3, 1, 0)); // Equipo1 gana por 1
        grupo.Partidos.Add(CrearPartido(equipo2, equipo4, 0, 1)); // Equipo4 gana por 1
        grupo.Partidos.Add(CrearPartido(equipo1, equipo4, 0, 0)); // Empate
        grupo.Partidos.Add(CrearPartido(equipo2, equipo3, 0, 0)); // Empate

        // Act
        var tabla = servicioClasificacion.CalcularTabla(grupo);

        // Assert
        var ordenEsperado = new List<Equipo>
        {
            equipo2, // 4 puntos, diferencia +2
            equipo3, // 4 puntos, diferencia +1
            equipo4, // 4 puntos, diferencia -1
            equipo1  // 4 puntos, diferencia -2
        };

        var ordenObtenido = tabla.Select(p => p.Equipo).ToList();

        CollectionAssert.AreEqual(ordenEsperado, ordenObtenido);
    }

    [TestMethod]
    public void CalcularTabla_ConMismosPuntosYMismaDiferencia_OrdenaPorGolesAFavorDescendente()
    {
        // Arrange
        var servicioClasificacion = new ServicioClasificacion();

        var equipo1 = CrearEquipo("Equipo1");
        var equipo2 = CrearEquipo("Equipo2");
        var equipo3 = CrearEquipo("Equipo3");
        var equipo4 = CrearEquipo("Equipo4");

        var grupo = new Grupo("A", new List<Equipo>
        {
            equipo1,
            equipo2,
            equipo3,
            equipo4
        });

        grupo.Partidos.Add(CrearPartido(equipo1, equipo2, 4, 3)); // Equipo1 gana
        grupo.Partidos.Add(CrearPartido(equipo3, equipo4, 6, 7)); // Equipo4 gana
        grupo.Partidos.Add(CrearPartido(equipo1, equipo3, 2, 3)); // Equipo3 gana
        grupo.Partidos.Add(CrearPartido(equipo2, equipo4, 5, 4)); // Equipo2 gana
        grupo.Partidos.Add(CrearPartido(equipo1, equipo4, 0, 0)); // Empate
        grupo.Partidos.Add(CrearPartido(equipo2, equipo3, 1, 1)); // Empate

        // Act
        var tabla = servicioClasificacion.CalcularTabla(grupo);

        // Assert
        var ordenEsperado = new List<Equipo>
        {
            equipo4, // 4 puntos, diferencia 0, 11 goles a favor
            equipo3, // 4 puntos, diferencia 0, 10 goles a favor
            equipo2, // 4 puntos, diferencia 0, 9 goles a favor
            equipo1  // 4 puntos, diferencia 0, 6 goles a favor
        };

        var ordenObtenido = tabla.Select(p => p.Equipo).ToList();

        CollectionAssert.AreEqual(ordenEsperado, ordenObtenido);
    }

    [TestMethod]
    public void CalcularTabla_ConEmpateTotalYMismaSemilla_DebeGenerarMismoDesempate()
    {
        // Arrange
        var servicioClasificacion = new ServicioClasificacion();
        var grupo = CrearGrupoConEmpateTotal();

        var configuracionTorneo1 = new ConfiguracionTorneo { SemillaCrucesFase = 7 };
        var configuracionTorneo2 = new ConfiguracionTorneo { SemillaCrucesFase = 7 };

        // Act
        var tabla1 = servicioClasificacion.CalcularTabla(grupo, configuracionTorneo1);
        var tabla2 = servicioClasificacion.CalcularTabla(grupo, configuracionTorneo2);

        // Assert
        var orden1 = tabla1.Select(p => p.Equipo.Nombre).ToList();
        var orden2 = tabla2.Select(p => p.Equipo.Nombre).ToList();

        CollectionAssert.AreEqual(orden1, orden2);
    }

    [TestMethod]
    public void CalcularTabla_ConEmpateTotalYDistintasSemillas_DebePermitirDesempateDistinto()
    {
        // Arrange
        var servicioClasificacion = new ServicioClasificacion();
        var grupo = CrearGrupoConEmpateTotal();

        var configuracionReferencia = new ConfiguracionTorneo { SemillaCrucesFase = 1 };

        // Act
        var tablaReferencia = servicioClasificacion.CalcularTabla(grupo, configuracionReferencia);
        var ordenReferencia = tablaReferencia.Select(p => p.Equipo.Nombre).ToList();

        var existeOrdenDistinto = false;

        for (int semilla = 2; semilla <= 20; semilla++)
        {
            var configuracionTorneo = new ConfiguracionTorneo { SemillaCrucesFase = semilla };
            var tabla = servicioClasificacion.CalcularTabla(grupo, configuracionTorneo);
            var orden = tabla.Select(p => p.Equipo.Nombre).ToList();

            if (!orden.SequenceEqual(ordenReferencia))
            {
                existeOrdenDistinto = true;
                break;
            }
        }

        // Assert
        Assert.IsTrue(existeOrdenDistinto);
    }

    private Grupo CrearGrupoConEmpateTotal()
    {
        var equipo1 = CrearEquipo("Equipo1");
        var equipo2 = CrearEquipo("Equipo2");
        var equipo3 = CrearEquipo("Equipo3");
        var equipo4 = CrearEquipo("Equipo4");

        var grupo = new Grupo("A", new List<Equipo>
        {
            equipo1,
            equipo2,
            equipo3,
            equipo4
        });

        grupo.Partidos.Add(CrearPartido(equipo1, equipo2, 0, 0));
        grupo.Partidos.Add(CrearPartido(equipo3, equipo4, 0, 0));
        grupo.Partidos.Add(CrearPartido(equipo1, equipo3, 0, 0));
        grupo.Partidos.Add(CrearPartido(equipo2, equipo4, 0, 0));
        grupo.Partidos.Add(CrearPartido(equipo1, equipo4, 0, 0));
        grupo.Partidos.Add(CrearPartido(equipo2, equipo3, 0, 0));

        return grupo;
    }

    private Fixture CrearFixtureConDoceGruposResueltos()
    {
        var fixture = new Fixture();

        for (char etiqueta = 'A'; etiqueta <= 'L'; etiqueta++)
        {
            fixture.Grupos.Add(CrearGrupoResuelto(etiqueta.ToString()));
        }

        return fixture;
    }

    private Grupo CrearGrupoResuelto(string etiqueta)
    {
        var equipo1 = CrearEquipo($"{etiqueta}1", Confederacion.UEFA);
        var equipo2 = CrearEquipo($"{etiqueta}2", Confederacion.CONMEBOL);
        var equipo3 = CrearEquipo($"{etiqueta}3", Confederacion.AFC);
        var equipo4 = CrearEquipo($"{etiqueta}4", Confederacion.CAF);

        var grupo = new Grupo(etiqueta, new List<Equipo>
        {
            equipo1,
            equipo2,
            equipo3,
            equipo4
        });

        grupo.Partidos.Add(CrearPartido(equipo1, equipo2, 3, 0));
        grupo.Partidos.Add(CrearPartido(equipo1, equipo3, 3, 0));
        grupo.Partidos.Add(CrearPartido(equipo1, equipo4, 3, 0));
        grupo.Partidos.Add(CrearPartido(equipo2, equipo3, 2, 0));
        grupo.Partidos.Add(CrearPartido(equipo2, equipo4, 2, 0));
        grupo.Partidos.Add(CrearPartido(equipo3, equipo4, 1, 0));

        return grupo;
    }

    [TestMethod]
    public void ObtenerClasificadosAFaseEliminatoria_DebeTomarPrimerosSegundosYMejoresTerceros()
    {
        // Arrange
        var servicioClasificacion = new ServicioClasificacion();
        var fixture = CrearFixtureConDoceGruposResueltos();
        var configuracionTorneo = new ConfiguracionTorneo { SemillaCrucesFase = 1 };

        // Act
        var clasificados = servicioClasificacion.ObtenerClasificadosAFaseEliminatoria(
            fixture,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(12, clasificados.Primeros.Count);
        Assert.AreEqual(12, clasificados.Segundos.Count);
        Assert.AreEqual(8, clasificados.MejoresTerceros.Count);
        Assert.AreEqual(32, clasificados.Todos.Count);

        Assert.IsTrue(clasificados.Primeros.All(e => e.Nombre.EndsWith("1")));
        Assert.IsTrue(clasificados.Segundos.All(e => e.Nombre.EndsWith("2")));
    }

    [TestMethod]
    public void ObtenerClasificadosAFaseEliminatoria_ConTercerosTotalmenteEmpatadosYDistintasSemillas_DebePermitirMejoresTercerosDistintos()
    {
        // Arrange
        var servicioClasificacion = new ServicioClasificacion();
        var fixture = CrearFixtureConDoceGruposResueltos();

        var configuracionReferencia = new ConfiguracionTorneo { SemillaCrucesFase = 1 };

        // Act
        var clasificadosReferencia = servicioClasificacion.ObtenerClasificadosAFaseEliminatoria(
            fixture,
            configuracionReferencia
        );

        var mejoresTercerosReferencia = clasificadosReferencia.MejoresTerceros
            .Select(e => e.Nombre)
            .ToList();

        var existeSeleccionDistinta = false;

        for (int semilla = 2; semilla <= 20; semilla++)
        {
            var configuracionTorneo = new ConfiguracionTorneo { SemillaCrucesFase = semilla };

            var clasificados = servicioClasificacion.ObtenerClasificadosAFaseEliminatoria(
                fixture,
                configuracionTorneo
            );

            var mejoresTerceros = clasificados.MejoresTerceros
                .Select(e => e.Nombre)
                .ToList();

            if (!mejoresTerceros.SequenceEqual(mejoresTercerosReferencia))
            {
                existeSeleccionDistinta = true;
                break;
            }
        }

        // Assert
        Assert.IsTrue(existeSeleccionDistinta);
    }

    [TestMethod]
    public void ObtenerClasificadosAFaseEliminatoria_DebeOrdenarPrimerosPorRendimientoGlobal()
    {
        // Arrange
        var servicioClasificacion = new ServicioClasificacion();
        var fixture = CrearFixtureConPrimerGrupoDeMenorRendimiento();
        var configuracionTorneo = new ConfiguracionTorneo { SemillaCrucesFase = 1 };

        var primeroGrupoA = fixture.Grupos.Single(g => g.Etiqueta == "A").Equipos[0];
        var primeroGrupoB = fixture.Grupos.Single(g => g.Etiqueta == "B").Equipos[0];

        // Act
        var clasificados = servicioClasificacion.ObtenerClasificadosAFaseEliminatoria(
            fixture,
            configuracionTorneo
        );

        // Assert
        Assert.IsTrue(
            clasificados.Primeros.IndexOf(primeroGrupoB) < clasificados.Primeros.IndexOf(primeroGrupoA)
        );
    }

    [TestMethod]
    public void ObtenerClasificadosAFaseEliminatoria_DebeOrdenarSegundosPorRendimientoGlobal()
    {
        // Arrange
        var servicioClasificacion = new ServicioClasificacion();
        var fixture = CrearFixtureConPrimerGrupoDeMenorRendimiento();
        var configuracionTorneo = new ConfiguracionTorneo { SemillaCrucesFase = 1 };

        var segundoGrupoA = fixture.Grupos.Single(g => g.Etiqueta == "A").Equipos[1];
        var segundoGrupoB = fixture.Grupos.Single(g => g.Etiqueta == "B").Equipos[1];

        // Act
        var clasificados = servicioClasificacion.ObtenerClasificadosAFaseEliminatoria(
            fixture,
            configuracionTorneo
        );

        // Assert
        Assert.IsTrue(
            clasificados.Segundos.IndexOf(segundoGrupoB) < clasificados.Segundos.IndexOf(segundoGrupoA)
        );
    }

    [TestMethod]
    public void ObtenerClasificadosAFaseEliminatoria_DebeGuardarGrupoDeOrigenDeCadaClasificado()
    {
        // Arrange
        var servicioClasificacion = new ServicioClasificacion();
        var fixture = CrearFixtureConDoceGruposResueltos();
        var configuracionTorneo = new ConfiguracionTorneo { SemillaCrucesFase = 1 };

        var grupoA = fixture.Grupos.Single(g => g.Etiqueta == "A");
        var primerClasificadoGrupoA = grupoA.Equipos[0];
        var segundoClasificadoGrupoA = grupoA.Equipos[1];

        // Act
        var clasificados = servicioClasificacion.ObtenerClasificadosAFaseEliminatoria(
            fixture,
            configuracionTorneo
        );

        // Assert
        Assert.AreSame(grupoA, clasificados.ObtenerGrupoDeOrigen(primerClasificadoGrupoA));
        Assert.AreSame(grupoA, clasificados.ObtenerGrupoDeOrigen(segundoClasificadoGrupoA));
    }

    private Fixture CrearFixtureConPrimerGrupoDeMenorRendimiento()
    {
        var fixture = new Fixture();
        fixture.Grupos.Add(CrearGrupoConPrimerYSegundoDeMenorRendimiento("A"));

        for (char etiqueta = 'B'; etiqueta <= 'L'; etiqueta++)
        {
            fixture.Grupos.Add(CrearGrupoResuelto(etiqueta.ToString()));
        }

        return fixture;
    }

    private Grupo CrearGrupoConPrimerYSegundoDeMenorRendimiento(string etiqueta)
    {
        var equipo1 = CrearEquipo($"{etiqueta}1", Confederacion.UEFA);
        var equipo2 = CrearEquipo($"{etiqueta}2", Confederacion.CONMEBOL);
        var equipo3 = CrearEquipo($"{etiqueta}3", Confederacion.AFC);
        var equipo4 = CrearEquipo($"{etiqueta}4", Confederacion.CAF);

        var grupo = new Grupo(etiqueta, new List<Equipo>
        {
            equipo1,
            equipo2,
            equipo3,
            equipo4
        });

        grupo.Partidos.Add(CrearPartido(equipo1, equipo2, 1, 1));
        grupo.Partidos.Add(CrearPartido(equipo1, equipo3, 2, 0));
        grupo.Partidos.Add(CrearPartido(equipo1, equipo4, 2, 0));
        grupo.Partidos.Add(CrearPartido(equipo2, equipo3, 1, 0));
        grupo.Partidos.Add(CrearPartido(equipo2, equipo4, 0, 0));
        grupo.Partidos.Add(CrearPartido(equipo3, equipo4, 1, 0));

        return grupo;
    }
}
~~~

### ServicioCrucesTests.cs

Ruta: WorldCupPlanner.ServicioTests\ServicioCrucesTests.cs

~~~csharp
using  WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public class ServicioCrucesTests
{
    [TestMethod]
    public void GenerarPrimeraRondaEliminatoria_Con32Clasificados_DebeGenerar16Partidos()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var clasificados = CrearClasificadosCon32Equipos();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCrucesFase = 1
        };

        // Act
        var partidos = servicioCruces.GenerarPrimeraRondaEliminatoria(
            clasificados,
            estadios,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(16, partidos.Count);
        Assert.IsTrue(partidos.All(p => p.Fase == FaseTorneo.PrimeraRondaEliminatoria));
    }

    private ClasificadosFaseGrupos CrearClasificadosCon32Equipos()
    {
        return new ClasificadosFaseGrupos
        {
            Primeros = CrearEquipos("Primero", 12),
            Segundos = CrearEquipos("Segundo", 12),
            MejoresTerceros = CrearEquipos("Tercero", 8)
        };
    }

    private List<Equipo> CrearEquipos(string prefijo, int cantidad)
    {
        var equipos = new List<Equipo>();

        for (int i = 1; i <= cantidad; i++)
        {
            equipos.Add(new Equipo
            {
                Nombre = $"{prefijo}{i}",
                RankingFIFA = 1000 + i,
                Confederacion = Confederacion.UEFA
            });
        }

        return equipos;
    }

    private List<Estadio> CrearEstadiosValidos(int cantidad)
    {
        var estadios = new List<Estadio>();

        for (int i = 1; i <= cantidad; i++)
        {
            estadios.Add(new Estadio
            {
                Nombre = $"Estadio{i}",
                Ciudad = $"Ciudad{i}",
                Capacidad = 30000
            });
        }

        return estadios;
    }
    
    [TestMethod]
    public void GenerarPrimeraRondaEliminatoria_DebeCruzarOchoPrimerosConOchoTerceros()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var clasificados = CrearClasificadosCon32Equipos();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCrucesFase = 1
        };

        // Act
        var partidos = servicioCruces.GenerarPrimeraRondaEliminatoria(
            clasificados,
            estadios,
            configuracionTorneo
        );

        // Assert
        var primerosOchoPartidos = partidos.Take(8).ToList();

        for (int i = 0; i < primerosOchoPartidos.Count; i++)
        {
            Assert.IsTrue(
                clasificados.Primeros.Take(8).Contains(primerosOchoPartidos[i].EquipoLocal)
            );

            Assert.IsTrue(
                clasificados.MejoresTerceros.Contains(primerosOchoPartidos[i].EquipoVisitante)
            );
        }
    }
    
    [TestMethod]
    public void GenerarPrimeraRondaEliminatoria_DebeCruzarPrimerosRestantesConSegundosDeMenorRendimiento()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var clasificados = CrearClasificadosCon32Equipos();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCrucesFase = 1
        };

        // Act
        var partidos = servicioCruces.GenerarPrimeraRondaEliminatoria(
            clasificados,
            estadios,
            configuracionTorneo
        );

        // Assert
        var partidosPrimerosRestantesContraSegundos = partidos
            .Skip(8)
            .Take(4)
            .ToList();

        var primerosRestantes = clasificados.Primeros
            .Skip(8)
            .Take(4)
            .ToList();

        var segundosDeMenorRendimiento = clasificados.Segundos
            .Skip(8)
            .Take(4)
            .ToList();

        var locales = partidosPrimerosRestantesContraSegundos
            .Select(p => p.EquipoLocal)
            .ToList();

        var visitantes = partidosPrimerosRestantesContraSegundos
            .Select(p => p.EquipoVisitante)
            .ToList();

        CollectionAssert.AreEquivalent(primerosRestantes, locales);
        CollectionAssert.AreEquivalent(segundosDeMenorRendimiento, visitantes);
    }
    
    [TestMethod]
    public void GenerarPrimeraRondaEliminatoria_DebeCruzarSegundosRestantesEntreSi()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var clasificados = CrearClasificadosCon32Equipos();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCrucesFase = 1
        };

        // Act
        var partidos = servicioCruces.GenerarPrimeraRondaEliminatoria(
            clasificados,
            estadios,
            configuracionTorneo
        );

        // Assert
        var partidosSegundosRestantes = partidos
            .Skip(12)
            .Take(4)
            .ToList();

        var segundosRestantes = clasificados.Segundos
            .Take(8)
            .ToList();

        for (int i = 0; i < partidosSegundosRestantes.Count; i++)
        {
            Assert.IsTrue(segundosRestantes.Contains(partidosSegundosRestantes[i].EquipoLocal));
            Assert.IsTrue(segundosRestantes.Contains(partidosSegundosRestantes[i].EquipoVisitante));
            Assert.AreNotEqual(
                partidosSegundosRestantes[i].EquipoLocal,
                partidosSegundosRestantes[i].EquipoVisitante
            );
        }

        var equiposUsados = partidosSegundosRestantes
            .SelectMany(p => new[] { p.EquipoLocal, p.EquipoVisitante })
            .ToList();

        CollectionAssert.AreEquivalent(segundosRestantes, equiposUsados);
    }

    [TestMethod]
    public void GenerarPrimeraRondaEliminatoria_ConMismaSemilla_DebeGenerarMismosCruces()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var clasificados = CrearClasificadosCon32Equipos();
        var estadios = CrearEstadiosValidos(4);

        var configuracionTorneo1 = new ConfiguracionTorneo { SemillaCrucesFase = 7 };
        var configuracionTorneo2 = new ConfiguracionTorneo { SemillaCrucesFase = 7 };

        // Act
        var partidos1 = servicioCruces.GenerarPrimeraRondaEliminatoria(
            clasificados,
            estadios,
            configuracionTorneo1
        );

        var partidos2 = servicioCruces.GenerarPrimeraRondaEliminatoria(
            clasificados,
            estadios,
            configuracionTorneo2
        );

        // Assert
        var cruces1 = ObtenerCruces(partidos1);
        var cruces2 = ObtenerCruces(partidos2);

        CollectionAssert.AreEqual(cruces1, cruces2);
    }

    [TestMethod]
    public void GenerarPrimeraRondaEliminatoria_ConDistintasSemillas_DebePermitirCrucesDistintos()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var clasificados = CrearClasificadosCon32Equipos();
        var estadios = CrearEstadiosValidos(4);

        var configuracionReferencia = new ConfiguracionTorneo { SemillaCrucesFase = 1 };

        // Act
        var partidosReferencia = servicioCruces.GenerarPrimeraRondaEliminatoria(
            clasificados,
            estadios,
            configuracionReferencia
        );

        var crucesReferencia = ObtenerCruces(partidosReferencia);
        var existeCruceDistinto = false;

        for (int semilla = 2; semilla <= 20; semilla++)
        {
            var configuracionTorneo = new ConfiguracionTorneo { SemillaCrucesFase = semilla };

            var partidos = servicioCruces.GenerarPrimeraRondaEliminatoria(
                clasificados,
                estadios,
                configuracionTorneo
            );

            var cruces = ObtenerCruces(partidos);

            if (!cruces.SequenceEqual(crucesReferencia))
            {
                existeCruceDistinto = true;
                break;
            }
        }

        // Assert
        Assert.IsTrue(existeCruceDistinto);
    }

    [TestMethod]
    public void GenerarPrimeraRondaEliminatoria_DebeEvitarCrucesDelMismoGrupoCuandoSeaPosible()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var clasificados = CrearClasificadosConGruposDeOrigen();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo { SemillaCrucesFase = 1 };

        // Act
        var partidos = servicioCruces.GenerarPrimeraRondaEliminatoria(
            clasificados,
            estadios,
            configuracionTorneo
        );

        // Assert
        foreach (var partido in partidos)
        {
            var grupoLocal = clasificados.ObtenerGrupoDeOrigen(partido.EquipoLocal!);
            var grupoVisitante = clasificados.ObtenerGrupoDeOrigen(partido.EquipoVisitante!);

            Assert.AreNotSame(grupoLocal, grupoVisitante);
        }
    }

    private List<string> ObtenerCruces(List<Partido> partidos)
    {
        return partidos
            .Select(p => $"{p.EquipoLocal.Nombre}-{p.EquipoVisitante.Nombre}")
            .ToList();
    }

    private ClasificadosFaseGrupos CrearClasificadosConGruposDeOrigen()
    {
        var clasificados = new ClasificadosFaseGrupos();
        var grupos = CrearGruposDeOrigen();

        clasificados.Primeros = CrearEquiposConOrigen("Primero", 12, grupos, clasificados);
        clasificados.Segundos = CrearEquiposConOrigen("Segundo", 12, grupos, clasificados);
        clasificados.MejoresTerceros = CrearEquiposConOrigen("Tercero", 8, grupos, clasificados);

        return clasificados;
    }

    private List<Grupo> CrearGruposDeOrigen()
    {
        var grupos = new List<Grupo>();

        for (char etiqueta = 'A'; etiqueta <= 'L'; etiqueta++)
        {
            grupos.Add(new Grupo(etiqueta.ToString()));
        }

        return grupos;
    }

    private List<Equipo> CrearEquiposConOrigen(
        string prefijo,
        int cantidad,
        List<Grupo> grupos,
        ClasificadosFaseGrupos clasificados)
    {
        var equipos = CrearEquipos(prefijo, cantidad);

        for (int i = 0; i < equipos.Count; i++)
        {
            clasificados.GruposDeOrigen[equipos[i]] = grupos[i];
        }

        return equipos;
    }
    
    private List<Partido> CrearPartidosPrimeraRondaJugados(int cantidad)
    {
        var partidos = new List<Partido>();
        var estadio = new Estadio
        {
            Nombre = "Estadio",
            Ciudad = "Ciudad",
            Capacidad = 30000
        };

        for (int i = 1; i <= cantidad; i++)
        {
            partidos.Add(new Partido
            {
                Id = i,
                EquipoLocal = new Equipo
                {
                    Nombre = $"Local{i}",
                    RankingFIFA = 1000 + i,
                    Confederacion = Confederacion.UEFA
                },
                EquipoVisitante = new Equipo
                {
                    Nombre = $"Visitante{i}",
                    RankingFIFA = 900 + i,
                    Confederacion = Confederacion.CONMEBOL
                },
                Estadio = estadio,
                Fecha = DateTime.Now,
                Fase = FaseTorneo.PrimeraRondaEliminatoria,
                GolesLocal = 2,
                GolesVisitante = 0
            });
        }

        return partidos;
    }
    
    [TestMethod]
    public void GenerarOctavos_Con16PartidosDePrimeraRondaJugados_DebeGenerar8Partidos()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var partidosPrimeraRonda = CrearPartidosPrimeraRondaJugados(16);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo
        {
            FechaInicioTorneo = new DateTime(2026, 6, 1),
            SeparacionEntreFechas = 3
        };

        // Act
        var octavos = servicioCruces.GenerarOctavos(
            partidosPrimeraRonda,
            estadios,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(8, octavos.Count);
        Assert.IsTrue(octavos.All(p => p.Fase == FaseTorneo.Octavos));
    }
    
    private List<Partido> CrearPartidosPrimeraRondaJugados()
    {
        var partidos = new List<Partido>();

        for (int i = 1; i <= 16; i++)
        {
            var equipoLocal = new Equipo
            {
                Nombre = $"Ganador{i}",
                RankingFIFA = 1000 + i,
                Confederacion = Confederacion.UEFA
            };

            var equipoVisitante = new Equipo
            {
                Nombre = $"Perdedor{i}",
                RankingFIFA = 800 + i,
                Confederacion = Confederacion.CONMEBOL
            };

            partidos.Add(new Partido
            {
                Id = i,
                EquipoLocal = equipoLocal,
                EquipoVisitante = equipoVisitante,
                GolesLocal = 2,
                GolesVisitante = 0,
                Estadio = new Estadio
                {
                    Nombre = $"Estadio{i}",
                    Ciudad = $"Ciudad{i}",
                    Capacidad = 30000
                },
                Fecha = DateTime.Now,
                Fase = FaseTorneo.PrimeraRondaEliminatoria
            });
        }

        return partidos;
    }
    
    [TestMethod]
    public void GenerarOctavos_DebeUsarGanadoresDePrimeraRonda()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var partidosPrimeraRonda = CrearPartidosPrimeraRondaJugados();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var octavos = servicioCruces.GenerarOctavos(
            partidosPrimeraRonda,
            estadios,
            configuracionTorneo
        );

        // Assert
        var ganadoresPrimeraRonda = partidosPrimeraRonda
            .Select(p => p.Ganador())
            .ToList();

        var equiposEnOctavos = octavos
            .SelectMany(p => new[] { p.EquipoLocal, p.EquipoVisitante })
            .ToList();

        CollectionAssert.AreEquivalent(ganadoresPrimeraRonda, equiposEnOctavos);
    }
    
    [TestMethod]
    public void GenerarOctavos_DebeCrearPartidosConFaseOctavos()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var partidosPrimeraRonda = CrearPartidosPrimeraRondaJugados();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var octavos = servicioCruces.GenerarOctavos(
            partidosPrimeraRonda,
            estadios,
            configuracionTorneo
        );

        // Assert
        Assert.IsTrue(octavos.All(p => p.Fase == FaseTorneo.Octavos));
    }
    
    [TestMethod]
    public void GenerarOctavos_DebeAsignarIdsIncrementalesLuegoDePrimeraRonda()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var partidosPrimeraRonda = CrearPartidosPrimeraRondaJugados();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var octavos = servicioCruces.GenerarOctavos(
            partidosPrimeraRonda,
            estadios,
            configuracionTorneo
        );

        // Assert
        var idsEsperados = Enumerable.Range(17, 8).ToList();
        var idsObtenidos = octavos.Select(p => p.Id).ToList();

        CollectionAssert.AreEqual(idsEsperados, idsObtenidos);
    }

    [TestMethod]
    public void GenerarOctavos_DebeAsignarEstadiosPorRotacion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var partidosPrimeraRonda = CrearPartidosPrimeraRondaJugados(16);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var octavos = servicioCruces.GenerarOctavos(
            partidosPrimeraRonda,
            estadios,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(estadios[0], octavos[0].Estadio);
        Assert.AreEqual(estadios[1], octavos[1].Estadio);
        Assert.AreEqual(estadios[2], octavos[2].Estadio);
        Assert.AreEqual(estadios[3], octavos[3].Estadio);
        Assert.AreEqual(estadios[0], octavos[4].Estadio);
    }
    
    [TestMethod]
    public void GenerarOctavos_ConPartidoPrimeraRondaNoJugado_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var partidosPrimeraRonda = CrearPartidosPrimeraRondaJugados(16);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        partidosPrimeraRonda[0].GolesLocal = null;
        partidosPrimeraRonda[0].GolesVisitante = null;

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            servicioCruces.GenerarOctavos(
                partidosPrimeraRonda,
                estadios,
                configuracionTorneo
            )
        );
    }
}
~~~

### ServicioEquipoTests.cs

Ruta: WorldCupPlanner.ServicioTests\ServicioEquipoTests.cs

~~~csharp
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public class ServicioEquipoTests
{
    private ServicioEquipo _servicio;

    [TestInitialize]
    public void Setup()
    {
        _servicio = new ServicioEquipo();
    }

    [TestMethod]
    public void AgregarEquipo_EquipoValido_SeAgregaCorrectamente()
    {
        // Arrange
        var equipo = new Equipo
        {
            Nombre = "Uruguay",
            RankingFIFA = 1000,
            Confederacion = Confederacion.CONMEBOL
        };

        // Act
        _servicio.AgregarEquipo(equipo);

        // Assert
        Assert.AreEqual(1, _servicio.ObtenerEquipos().Count);
    }

    [TestMethod]
    public void AgregarEquipo_NombreDuplicado_LanzaExcepcion()
    {
        // Arrange
        var equipo1 = new Equipo
        {
            Nombre = "Uruguay",
            RankingFIFA = 1000,
            Confederacion = Confederacion.CONMEBOL
        };
        var equipo2 = new Equipo
        {
            Nombre = "Uruguay",
            RankingFIFA = 800,
            Confederacion = Confederacion.CONMEBOL
        };

        // Act
        _servicio.AgregarEquipo(equipo1);

        // Assert
        Assert.ThrowsException<EntidadYaExistenteExcepcion>(() => _servicio.AgregarEquipo(equipo2));
    }
    
    [TestMethod]
    public void AgregarEquipo_EquipoInvalido_LanzaExcepcion()
    {
        // Arrange
        var equipo = new Equipo
        {
            Nombre = "",
            RankingFIFA = 1000,
            Confederacion = Confederacion.CONMEBOL
        };

        // Act y1 Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() => _servicio.AgregarEquipo(equipo));
    }
    
    [TestMethod]
    public void EliminarEquipo_EquipoExistente_SeEliminaCorrectamente()
    {
        // Arrange
        var equipo = new Equipo
        {
            Nombre = "Uruguay",
            RankingFIFA = 1000,
            Confederacion = Confederacion.CONMEBOL
        };
        _servicio.AgregarEquipo(equipo);

        // Act
        _servicio.EliminarEquipo(equipo);

        // Assert
        Assert.AreEqual(0, _servicio.ObtenerEquipos().Count);
    }
    
    [TestMethod]
    public void EditarEquipo_EquipoInvalido_LanzaExcepcion()
    {
        // Arrange
        var equipo = new Equipo
        {
            Nombre = "Uruguay",
            RankingFIFA = 1000,
            Confederacion = Confederacion.CONMEBOL
        };
        _servicio.AgregarEquipo(equipo);

        // Act
        equipo.Nombre = "";

        // Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() => _servicio.EditarEquipo(equipo, "Uruguay"));
    }
    
    [TestMethod]
    public void EditarEquipo_NombreDuplicado_LanzaExcepcion()
    {
        // Arrange
        var equipo1 = new Equipo
        {
            Nombre = "Uruguay",
            RankingFIFA = 1000,
            Confederacion = Confederacion.CONMEBOL
        };
        var equipo2 = new Equipo
        {
            Nombre = "Argentina",
            RankingFIFA = 800,
            Confederacion = Confederacion.CONMEBOL
        };
        _servicio.AgregarEquipo(equipo1);
        _servicio.AgregarEquipo(equipo2);

        // Act
        equipo2.Nombre = "Uruguay";

        // Assert
        Assert.ThrowsException<EntidadYaExistenteExcepcion>(() => _servicio.EditarEquipo(equipo2, "Argentina"));
    }
}
~~~

### ServicioEstadioTests.cs

Ruta: WorldCupPlanner.ServicioTests\ServicioEstadioTests.cs

~~~csharp
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public class ServicioEstadioTests
{
    private ServicioEstadio _servicio;

    [TestInitialize]
    public void Setup()
    {
        _servicio = new ServicioEstadio();
    }

    [TestMethod]
    public void AgregarEstadio_EstadioValido_SeAgregaCorrectamente()
    {
        // Arrange
        var estadio = new Estadio
        {
            Nombre = "Estadio Centenario",
            Ciudad = "Montevideo",
            Capacidad = 60000
        };

        // Act
        _servicio.AgregarEstadio(estadio);

        // Assert
        Assert.AreEqual(1, _servicio.ObtenerEstadios().Count);
    }
    
    [TestMethod]
    public void AgregarEstadio_NombreDuplicado_LanzaExcepcion()
    {
        // Arrange
        var estadio1 = new Estadio
        {
            Nombre = "Estadio Centenario",
            Ciudad = "Montevideo",
            Capacidad = 60000
        };
        var estadio2 = new Estadio
        {
            Nombre = "Estadio Centenario",
            Ciudad = "Montevideo",
            Capacidad = 40000
        };

        // Act
        _servicio.AgregarEstadio(estadio1);

        // Assert
        Assert.ThrowsException<EntidadYaExistenteExcepcion>(() => _servicio.AgregarEstadio(estadio2));
    }
    
    [TestMethod]
    public void AgregarEstadio_EstadioInvalido_LanzaExcepcion()
    {
        // Arrange
        var estadio = new Estadio
        {
            Nombre = "",
            Ciudad = "Montevideo",
            Capacidad = 60000
        };

        // Act yAssert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() => _servicio.AgregarEstadio(estadio));
    }
    
    [TestMethod]
public void EliminarEstadio_EstadioExistente_SeEliminaCorrectamente()
{
    // Arrange
    var estadio = new Estadio
    {
        Nombre = "Estadio Centenario",
        Ciudad = "Montevideo",
        Capacidad = 60000
    };
    _servicio.AgregarEstadio(estadio);

    // Act
    _servicio.EliminarEstadio(estadio);

    // Assert
    Assert.AreEqual(0, _servicio.ObtenerEstadios().Count);
}

    [TestMethod]
    public void EditarEstadio_EstadioValido_SeEditaCorrectamente()
    {
        // Arrange
        var estadio = new Estadio
        {
            Nombre = "Estadio Centenario",
            Ciudad = "Montevideo",
            Capacidad = 60000
        };
        _servicio.AgregarEstadio(estadio);

        // Act
        string nombreOriginal = estadio.Nombre;
        estadio.Nombre = "Estadio Olimpico";
        _servicio.EditarEstadio(nombreOriginal, estadio);

        // Assert
        Assert.AreEqual("Estadio Olimpico", _servicio.ObtenerEstadios().First().Nombre);
    }
}
~~~

### ServicioFixtureTests.cs

Ruta: WorldCupPlanner.ServicioTests\ServicioFixtureTests.cs

~~~csharp
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public class ServicioFixtureTests
{
    [TestMethod]
    public void GenerarFixture_ConMenosDe48Equipos_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();
        var equipos = new List<Equipo>();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo)
        );
    }

    [TestMethod]
    public void GenerarFixture_ConMenosDe4Estadios_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidos(48);
        var estadios = CrearEstadiosValidos(3);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo)
        );
    }

    [TestMethod]
    public void GenerarFixture_Con48EquiposY4Estadios_DebeGenerar12Grupos()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidos(48);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var fixture = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo);

        // Assert
        Assert.AreEqual(12, fixture.Grupos.Count);
    }

    [TestMethod]
    public void GenerarFixture_DebeCrearGruposConEtiquetasDeAToL()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidos(48);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var fixture = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo);

        // Assert
        var etiquetasEsperadas = new List<string>
        {
            "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L"
        };

        var etiquetasObtenidas = fixture.Grupos.Select(g => g.Etiqueta).ToList();

        CollectionAssert.AreEqual(etiquetasEsperadas, etiquetasObtenidas);
    }

    [TestMethod]
    public void GenerarFixture_DebeCrearGruposConCuatroEquiposCadaUno()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidos(48);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var fixture = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo);

        // Assert
        Assert.IsTrue(fixture.Grupos.All(g => g.Equipos.Count == 4));
    }

    [TestMethod]
    public void GenerarFixture_DebeDistribuirLosEquiposOrdenadosPorRankingEnRoundRobin()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var fixture = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo);

        // Assert
        var nombresGrupoA = fixture.Grupos[0].Equipos.Select(e => e.Nombre).ToList();
        var nombresGrupoB = fixture.Grupos[1].Equipos.Select(e => e.Nombre).ToList();

        CollectionAssert.AreEqual(
            new List<string> { "Equipo48", "Equipo36", "Equipo24", "Equipo12" },
            nombresGrupoA
        );

        CollectionAssert.AreEqual(
            new List<string> { "Equipo47", "Equipo35", "Equipo23", "Equipo11" },
            nombresGrupoB
        );
    }

    private List<Estadio> CrearEstadiosValidos(int cantidad)
    {
        var estadios = new List<Estadio>();

        for (int i = 1; i <= cantidad; i++)
        {
            estadios.Add(new Estadio
            {
                Nombre = $"Estadio{i}",
                Ciudad = $"Ciudad{i}",
                Capacidad = 30000
            });
        }
        return estadios;
    }

    private List<Equipo> CrearEquiposValidos(int cantidad)
    {
        var equipos = new List<Equipo>();
        var confederaciones = new[]
        {
            Confederacion.UEFA,
            Confederacion.CONMEBOL,
            Confederacion.CONCACAF,
            Confederacion.CAF,
            Confederacion.AFC,
            Confederacion.OFC
        };

        for (int i = 1; i <= cantidad; i++)
        {
            equipos.Add(new Equipo
            {
                Nombre = $"Equipo{i}",
                RankingFIFA = 500 + i,
                Confederacion = confederaciones[(i - 1) % confederaciones.Length]
            });
        }
        return equipos;
    }

    [TestMethod]
    public void GenerarFixture_ConEmpatesEnRanking_YDistintaSemilla_DebeGenerarDistribucionDistinta()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        // Cambiar algunos rankings para tener empates, permitiendo diferentes mezclas
        for (int i = 0; i < 12; i++)
        {
            equipos[i].RankingFIFA = 1000;
        }
        
        var estadios = CrearEstadiosValidos(4);

        var configuracionTorneo1 = new ConfiguracionTorneo { SemillaFixture = 1 };
        var configuracionTorneo2 = new ConfiguracionTorneo { SemillaFixture = 2 };

        // Act
        var fixture1 = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo1);
        var fixture2 = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo2);

        // Assert
        var ordenFixture1 = fixture1.Grupos
            .SelectMany(g => g.Equipos)
            .Select(e => e.Nombre)
            .ToList();

        var ordenFixture2 = fixture2.Grupos
            .SelectMany(g => g.Equipos)
            .Select(e => e.Nombre)
            .ToList();

        CollectionAssert.AreNotEqual(ordenFixture1, ordenFixture2);
    }
    
    [TestMethod]
    public void GenerarFixture_ConMismaSemilla_YEmpatesEnRanking_DebeGenerarMismaDistribucion()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        
        for (int i = 0; i < 12; i++)
        {
            equipos[i].RankingFIFA = 1000;
        }
        
        var estadios = CrearEstadiosValidos(4);

        var configuracionTorneo1 = new ConfiguracionTorneo { SemillaFixture = 7 };
        var configuracionTorneo2 = new ConfiguracionTorneo { SemillaFixture = 7 };

        // Act
        var fixture1 = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo1);
        var fixture2 = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo2);

        // Assert
        var ordenFixture1 = fixture1.Grupos
            .SelectMany(g => g.Equipos)
            .Select(e => e.Nombre)
            .ToList();

        var ordenFixture2 = fixture2.Grupos
            .SelectMany(g => g.Equipos)
            .Select(e => e.Nombre)
            .ToList();

        CollectionAssert.AreEqual(ordenFixture1, ordenFixture2);
    }
    
    private List<Equipo> CrearEquiposValidosSinConflictosDeConfederacion()
    {
        var confederacionesPorOrden = new[]
        {
            Confederacion.UEFA, Confederacion.UEFA, Confederacion.UEFA, Confederacion.UEFA,
            Confederacion.UEFA, Confederacion.UEFA, Confederacion.UEFA, Confederacion.UEFA,
            Confederacion.UEFA, Confederacion.UEFA, Confederacion.UEFA, Confederacion.UEFA,

            Confederacion.UEFA, Confederacion.UEFA, Confederacion.UEFA, Confederacion.UEFA,
            Confederacion.CONMEBOL, Confederacion.CONMEBOL, Confederacion.CONMEBOL,
            Confederacion.CONMEBOL, Confederacion.CONMEBOL, Confederacion.CONMEBOL,
            Confederacion.CONMEBOL, Confederacion.OFC,

            Confederacion.CONCACAF, Confederacion.CONCACAF, Confederacion.CONCACAF,
            Confederacion.CONCACAF, Confederacion.CONCACAF, Confederacion.CONCACAF,
            Confederacion.CONCACAF,

            Confederacion.CAF, Confederacion.CAF, Confederacion.CAF, Confederacion.CAF,
            Confederacion.CAF, Confederacion.CAF, Confederacion.CAF, Confederacion.CAF,
            Confederacion.CAF,

            Confederacion.AFC, Confederacion.AFC, Confederacion.AFC, Confederacion.AFC,
            Confederacion.AFC, Confederacion.AFC, Confederacion.AFC, Confederacion.AFC
        };

        var equipos = new List<Equipo>();

        for (int posicion = 0; posicion < confederacionesPorOrden.Length; posicion++)
        {
            int numeroEquipo = 48 - posicion;

            equipos.Add(new Equipo
            {
                Nombre = $"Equipo{numeroEquipo}",
                RankingFIFA = 500 + numeroEquipo,
                Confederacion = confederacionesPorOrden[posicion]
            });
        }

        return equipos;
    }
    
    private List<Equipo> CrearEquiposQueRequierenAjustePorConfederacion()
    {
        var confederacionesPorOrden = new List<Confederacion>
        {
            Confederacion.UEFA, Confederacion.UEFA, Confederacion.UEFA, Confederacion.UEFA,
            Confederacion.UEFA, Confederacion.UEFA, Confederacion.UEFA, Confederacion.UEFA,
            Confederacion.UEFA, Confederacion.UEFA, Confederacion.UEFA, Confederacion.UEFA,

            Confederacion.CONMEBOL,

            Confederacion.UEFA, Confederacion.UEFA, Confederacion.UEFA, Confederacion.UEFA,

            Confederacion.CONMEBOL, Confederacion.CONMEBOL, Confederacion.CONMEBOL,
            Confederacion.CONMEBOL, Confederacion.CONMEBOL,

            Confederacion.OFC,
            Confederacion.CONCACAF,

            Confederacion.CONMEBOL,

            Confederacion.CONCACAF, Confederacion.CONCACAF, Confederacion.CONCACAF,
            Confederacion.CONCACAF, Confederacion.CONCACAF, Confederacion.CONCACAF,

            Confederacion.CAF, Confederacion.CAF, Confederacion.CAF, Confederacion.CAF,
            Confederacion.CAF, Confederacion.CAF, Confederacion.CAF, Confederacion.CAF,
            Confederacion.CAF,

            Confederacion.AFC, Confederacion.AFC, Confederacion.AFC, Confederacion.AFC,
            Confederacion.AFC, Confederacion.AFC, Confederacion.AFC, Confederacion.AFC
        };

        var equipos = new List<Equipo>();

        for (int posicion = 0; posicion < confederacionesPorOrden.Count; posicion++)
        {
            int numeroEquipo = 48 - posicion;

            equipos.Add(new Equipo
            {
                Nombre = $"Equipo{numeroEquipo}",
                RankingFIFA = 500 + numeroEquipo,
                Confederacion = confederacionesPorOrden[posicion]
            });
        }

        return equipos;
    }
    
    [TestMethod]
    public void GenerarFixture_DebeCrearGruposValidosRespetandoRestriccionesDeConfederacion()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposQueRequierenAjustePorConfederacion();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo { SemillaFixture = 1 };

        // Act
        var fixture = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo);

        // Assert
        Assert.IsTrue(fixture.Grupos.All(g => g.EsValido()));
    }
    
    [TestMethod]
    public void GenerarFixture_DebeCrearSeisPartidosPorGrupo()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo { SemillaFixture = 1 };

        // Act
        var fixture = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo);

        // Assert
        Assert.IsTrue(fixture.Grupos.All(g => g.Partidos.Count == 6));
    }
    
    [TestMethod]
    public void GenerarFixture_DebeCrearPartidosDelGrupoSegunPatronDeJornadas()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo { SemillaFixture = 1 };

        // Act
        var fixture = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo);

        // Assert
        var grupo = fixture.Grupos[0];
        var equiposDelGrupo = grupo.Equipos;
        var partidos = grupo.Partidos;

        Assert.AreEqual(equiposDelGrupo[0], partidos[0].EquipoLocal);
        Assert.AreEqual(equiposDelGrupo[3], partidos[0].EquipoVisitante);

        Assert.AreEqual(equiposDelGrupo[1], partidos[1].EquipoLocal);
        Assert.AreEqual(equiposDelGrupo[2], partidos[1].EquipoVisitante);

        Assert.AreEqual(equiposDelGrupo[0], partidos[2].EquipoLocal);
        Assert.AreEqual(equiposDelGrupo[2], partidos[2].EquipoVisitante);

        Assert.AreEqual(equiposDelGrupo[1], partidos[3].EquipoLocal);
        Assert.AreEqual(equiposDelGrupo[3], partidos[3].EquipoVisitante);

        Assert.AreEqual(equiposDelGrupo[0], partidos[4].EquipoLocal);
        Assert.AreEqual(equiposDelGrupo[1], partidos[4].EquipoVisitante);

        Assert.AreEqual(equiposDelGrupo[2], partidos[5].EquipoLocal);
        Assert.AreEqual(equiposDelGrupo[3], partidos[5].EquipoVisitante);
    }
    
    [TestMethod]
    public void GenerarFixture_DebeAsignarHorariosSegunJornadasDelGrupo()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var estadios = CrearEstadiosValidos(4);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaFixture = 1,
            FechaInicioTorneo = new DateTime(2026, 6, 1),
            SeparacionEntreFechas = 3,
            HoraInicioPartidos = new TimeSpan(14, 0, 0),
            SeparacionHorasEntrePartidos = 4
        };

        // Act
        var fixture = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo);

        // Assert
        var partidos = fixture.Grupos[0].Partidos;

        Assert.AreEqual(new DateTime(2026, 6, 1, 14, 0, 0), partidos[0].Fecha);
        Assert.AreEqual(new DateTime(2026, 6, 1, 18, 0, 0), partidos[1].Fecha);

        Assert.AreEqual(new DateTime(2026, 6, 4, 14, 0, 0), partidos[2].Fecha);
        Assert.AreEqual(new DateTime(2026, 6, 4, 18, 0, 0), partidos[3].Fecha);

        Assert.AreEqual(new DateTime(2026, 6, 7, 14, 0, 0), partidos[4].Fecha);
        Assert.AreEqual(new DateTime(2026, 6, 7, 14, 0, 0), partidos[5].Fecha);
    }
    
    private List<Estadio> CrearEstadiosValidosDesordenadosParaRotacion()
    {
        return new List<Estadio>
        {
            new Estadio { Nombre = "Estadio3", Ciudad = "Ciudad3", Capacidad = 30000 },
            new Estadio { Nombre = "Estadio1", Ciudad = "Ciudad1", Capacidad = 30000 },
            new Estadio { Nombre = "Estadio4", Ciudad = "Ciudad4", Capacidad = 30000 },
            new Estadio { Nombre = "Estadio2", Ciudad = "Ciudad2", Capacidad = 30000 }
        };
    }
    
    [TestMethod]
    public void GenerarFixture_DebeAsignarEstadiosPorRotacionSegunNombreNormalizado()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var estadios = CrearEstadiosValidosDesordenadosParaRotacion();
        var configuracionTorneo = new ConfiguracionTorneo { SemillaFixture = 1 };

        // Act
        var fixture = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo);

        // Assert
        var partidos = fixture.Grupos.SelectMany(g => g.Partidos).ToList();

        Assert.AreEqual("Estadio1", partidos[0].Estadio.Nombre);
        Assert.AreEqual("Estadio2", partidos[1].Estadio.Nombre);
        Assert.AreEqual("Estadio3", partidos[2].Estadio.Nombre);
        Assert.AreEqual("Estadio4", partidos[3].Estadio.Nombre);
        Assert.AreEqual("Estadio1", partidos[4].Estadio.Nombre);
    }

    [TestMethod]
    public void GenerarFixture_NoDebeSuperarMaximoPartidosPorDia()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var estadios = CrearEstadiosValidos(4);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaFixture = 1,
            FechaInicioTorneo = new DateTime(2026, 6, 1),
            SeparacionEntreFechas = 3,
            HoraInicioPartidos = new TimeSpan(14, 0, 0),
            SeparacionHorasEntrePartidos = 4,
            MaxPartidosPorDia = 3
        };

        // Act
        var fixture = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo);

        // Assert
        var cantidadPartidosPorDia = fixture.Grupos
            .SelectMany(g => g.Partidos)
            .GroupBy(p => p.Fecha.Date)
            .Select(g => g.Count())
            .ToList();

        Assert.IsTrue(cantidadPartidosPorDia.All(cantidad => cantidad <= configuracionTorneo.MaxPartidosPorDia));
    }

    [TestMethod]
    public void GenerarFixture_DebeAsignarIdsIncrementalesALosPartidos()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo { SemillaFixture = 1 };

        // Act
        var fixture = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo);

        // Assert
        var partidos = fixture.Grupos
            .SelectMany(g => g.Partidos)
            .ToList();

        var idsEsperados = Enumerable.Range(1, 72).ToList();
        var idsObtenidos = partidos.Select(p => p.Id).ToList();

        Assert.AreEqual(72, partidos.Count);
        CollectionAssert.AreEqual(idsEsperados, idsObtenidos);
    }
    
    [TestMethod]
    public void GenerarFixture_ConMasDe48Equipos_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidos(49);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo)
        );
    }

    [TestMethod]
    public void GenerarFixture_ConEquipoInvalido_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidos(47);
        // Agregar un equipo inválido (sin confederación)
        equipos.Add(new Equipo
        {
            Nombre = "EquipoInvalido",
            RankingFIFA = 500,
            Confederacion = null
        });
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo)
        );
    }

    [TestMethod]
    public void GenerarFixture_ConEstadioInvalido_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidos(48);
        var estadios = CrearEstadiosValidos(3);
        // Agregar un estadio inválido (capacidad menor a 20000)
        estadios.Add(new Estadio
        {
            Nombre = "EstadioInvalido",
            Ciudad = "Ciudad",
            Capacidad = 10000
        });
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo)
        );
    }

    [TestMethod]
    public void GenerarFixture_DebeAsignarCadaEquipoUnaSolaVez()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var equiposSet = new HashSet<string>(equipos.Select(e => e.Nombre));
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var fixture = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo);

        // Assert - Total de 48 equipos asignados
        var todosLosEquiposEnFixture = fixture.Grupos.SelectMany(g => g.Equipos).ToList();
        Assert.AreEqual(48, todosLosEquiposEnFixture.Count);

        // Assert - No hay repetidos
        var equiposUnicos = new HashSet<string>(todosLosEquiposEnFixture.Select(e => e.Nombre));
        Assert.AreEqual(48, equiposUnicos.Count);

        // Assert - Todos los originales están en el fixture
        var equiposEnFixtureSet = new HashSet<string>(todosLosEquiposEnFixture.Select(e => e.Nombre));
        CollectionAssert.AreEquivalent(equiposSet.ToList(), equiposEnFixtureSet.ToList());
    }

    [TestMethod]
    public void GenerarFixture_CadaEquipoDebeJugarTresPartidosEnSuGrupo()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo { SemillaFixture = 1 };

        // Act
        var fixture = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo);

        // Assert
        foreach (var grupo in fixture.Grupos)
        {
            foreach (var equipo in grupo.Equipos)
            {
                var partidosDelEquipo = grupo.Partidos
                    .Where(p => p.EquipoLocal == equipo || p.EquipoVisitante == equipo)
                    .ToList();

                Assert.AreEqual(3, partidosDelEquipo.Count, 
                    $"El equipo {equipo.Nombre} en el grupo {grupo.Etiqueta} debe jugar exactamente 3 partidos");
            }
        }
    }

    [TestMethod]
    public void GenerarFixture_NoDebeRepetirEnfrentamientosDentroDelGrupo()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo { SemillaFixture = 1 };

        // Act
        var fixture = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo);

        // Assert
        foreach (var grupo in fixture.Grupos)
        {
            var partidosUnicos = new HashSet<string>();

            foreach (var partido in grupo.Partidos)
            {
                // Crear clave normalizada del enfrentamiento (sin importar orden)
                var clave = string.Concat(
                    new[] { partido.EquipoLocal.Nombre, partido.EquipoVisitante.Nombre }.OrderBy(x => x)
                );

                Assert.IsTrue(
                    partidosUnicos.Add(clave),
                    $"El enfrentamiento entre {partido.EquipoLocal.Nombre} y {partido.EquipoVisitante.Nombre} " +
                    $"aparece repetido en el grupo {grupo.Etiqueta}"
                );
            }
        }
    }

    [TestMethod]
    public void GenerarFixture_TodosLosPartidosGeneradosDebenSerDeFaseDeGrupos()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo { SemillaFixture = 1 };

        // Act
        var fixture = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo);

        // Assert
        foreach (var grupo in fixture.Grupos)
        {
            foreach (var partido in grupo.Partidos)
            {
                Assert.AreEqual(FaseTorneo.FaseDeGrupos, partido.Fase,
                    $"El partido entre {partido.EquipoLocal.Nombre} y {partido.EquipoVisitante.Nombre} " +
                    $"debe tener fase FaseDeGrupos");
            }
        }
    }

    [TestMethod]
    public void GenerarFixture_TodosLosPartidosDebenTenerGrupoAsignado()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo { SemillaFixture = 1 };

        // Act
        var fixture = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo);

        // Assert
        foreach (var grupo in fixture.Grupos)
        {
            foreach (var partido in grupo.Partidos)
            {
                Assert.IsNotNull(partido.Grupo,
                    $"El partido entre {partido.EquipoLocal.Nombre} y {partido.EquipoVisitante.Nombre} debe tener grupo asignado");
                
                Assert.AreEqual(grupo, partido.Grupo,
                    $"El partido debe pertenencer al grupo {grupo.Etiqueta}");
                
                var etiqueta = grupo.Etiqueta;
                Assert.IsTrue(etiqueta[0] >= 'A' && etiqueta[0] <= 'L',
                    $"La etiqueta del grupo '{etiqueta}' debe estar entre A y L");
            }
        }
    }

    [TestMethod]
    public void GenerarFixture_TodosLosPartidosDebenTenerEstadioAsignado()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo { SemillaFixture = 1 };

        // Act
        var fixture = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo);

        // Assert
        foreach (var grupo in fixture.Grupos)
        {
            foreach (var partido in grupo.Partidos)
            {
                Assert.IsNotNull(partido.Estadio,
                    $"El partido entre {partido.EquipoLocal.Nombre} y {partido.EquipoVisitante.Nombre} debe tener estadio asignado");
                
                Assert.IsTrue(estadios.Contains(partido.Estadio),
                    $"El estadio {partido.Estadio.Nombre} debe ser uno de los estadios disponibles");
                
                Assert.IsTrue(partido.Estadio.EsValido(),
                    $"El estadio {partido.Estadio.Nombre} asignado debe ser válido");
            }
        }
    }
}
~~~

### ServicioUsuarioTests.cs

Ruta: WorldCupPlanner.ServicioTests\ServicioUsuarioTests.cs

~~~csharp
using WorldCupPlanner.AccesoADatos.Repositorios;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public sealed class ServicioUsuarioTests
{
    private RepositorioUsuarioEnMemoria _repositorio = null!;
    private ServicioUsuario _servicio = null!;

    public ServicioUsuarioTests()
    {
        _repositorio = new RepositorioUsuarioEnMemoria();
        _servicio = new ServicioUsuario(_repositorio);
    }

    [TestMethod]
    public void AgregarUsuario_ConEmailValido_AgregarAlSistema()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");

        // Act
        _servicio.AgregarUsuario(usuario);

        // Assert
        var usuarios = _servicio.ObtenerTodosLosUsuarios();
        Assert.AreEqual(1, usuarios.Count);
        Assert.AreSame(usuario, usuarios[0]);
    }

    [TestMethod]
    public void AgregarUsuario_ConEmailDuplicado_LanzaExcepcion()
    {
        // Arrange
        var usuario1 = CrearUsuarioValido("juan@example.com");
        var usuario2 = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario1);

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => _servicio.AgregarUsuario(usuario2));
    }

    [TestMethod]
    public void AgregarUsuario_ConEmailDuplicadoDistintasMinusculas_LanzaExcepcion()
    {
        // Arrange
        var usuario1 = CrearUsuarioValido("Juan@Example.com");
        var usuario2 = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario1);

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => _servicio.AgregarUsuario(usuario2));
    }

    [TestMethod]
    public void ActualizarUsuario_ConUsuarioExistente_ActualizaEnSistema()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario);
        usuario.AgregarRol(RolUsuario.AdministradorDelSistema);

        // Act
        _servicio.ActualizarUsuario(usuario);

        // Assert
        var usuarios = _servicio.ObtenerTodosLosUsuarios();
        Assert.IsTrue(usuarios[0].TieneRol(RolUsuario.AdministradorDelSistema));
    }

    [TestMethod]
    public void ActualizarUsuario_ConNombreNuevo_ActualizaNombre()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario);

        // Act
        _servicio.ActualizarUsuario(usuario, nombre: "Carlos");

        // Assert
        var usuarioActualizado = _servicio.ObtenerTodosLosUsuarios()[0];
        Assert.AreEqual("Carlos", usuarioActualizado.Nombre);
    }

    [TestMethod]
    public void ActualizarUsuario_ConApellidoNuevo_ActualizaApellido()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario);

        // Act
        _servicio.ActualizarUsuario(usuario, apellido: "García");

        // Assert
        var usuarioActualizado = _servicio.ObtenerTodosLosUsuarios()[0];
        Assert.AreEqual("García", usuarioActualizado.Apellido);
    }

    [TestMethod]
    public void ActualizarUsuario_ConEmailNuevo_ActualizaEmail()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario);

        // Act
        _servicio.ActualizarUsuario(usuario, email: "newemail@example.com");

        // Assert
        var usuarioActualizado = _servicio.ObtenerTodosLosUsuarios()[0];
        Assert.AreEqual("newemail@example.com", usuarioActualizado.Email);
    }

    [TestMethod]
    public void ActualizarUsuario_ConEmailDuplicado_LanzaExcepcion()
    {
        // Arrange
        var usuario1 = CrearUsuarioValido("juan@example.com");
        var usuario2 = CrearUsuarioValido("maria@example.com");
        _servicio.AgregarUsuario(usuario1);
        _servicio.AgregarUsuario(usuario2);

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            _servicio.ActualizarUsuario(usuario2, email: "juan@example.com")
        );
    }

    [TestMethod]
    public void ActualizarUsuario_ConEmailDuplicadoDistintasMinusculas_LanzaExcepcion()
    {
        // Arrange
        var usuario1 = CrearUsuarioValido("Juan@Example.com");
        var usuario2 = CrearUsuarioValido("maria@example.com");
        _servicio.AgregarUsuario(usuario1);
        _servicio.AgregarUsuario(usuario2);

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            _servicio.ActualizarUsuario(usuario2, email: "juan@example.com")
        );
    }

    [TestMethod]
    public void ActualizarUsuario_ConFechaNacimientoNueva_ActualizaFecha()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario);

        // Act
        _servicio.ActualizarUsuario(usuario, fechaNacimiento: "05-15-1995");

        // Assert
        var usuarioActualizado = _servicio.ObtenerTodosLosUsuarios()[0];
        Assert.AreEqual(new DateTime(1995, 5, 15), usuarioActualizado.FechaNacimiento);
    }

    [TestMethod]
    public void ActualizarUsuario_ConMultiplesCampos_ActualizaTodos()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario);

        // Act
        _servicio.ActualizarUsuario(usuario, nombre: "Carlos", apellido: "García",
            email: "carlos@example.com", fechaNacimiento: "05-15-1995");

        // Assert
        var usuarioActualizado = _servicio.ObtenerTodosLosUsuarios()[0];
        Assert.AreEqual("Carlos", usuarioActualizado.Nombre);
        Assert.AreEqual("García", usuarioActualizado.Apellido);
        Assert.AreEqual("carlos@example.com", usuarioActualizado.Email);
        Assert.AreEqual(new DateTime(1995, 5, 15), usuarioActualizado.FechaNacimiento);
    }

    [TestMethod]
    public void ActualizarUsuario_ConUsuarioNoExistente_LanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() => _servicio.ActualizarUsuario(usuario));
    }

    [TestMethod]
    public void EliminarUsuario_ConUsuarioExistente_EliminaDelSistema()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario);

        // Act
        _servicio.EliminarUsuario(usuario);

        // Assert
        var usuarios = _servicio.ObtenerTodosLosUsuarios();
        Assert.AreEqual(0, usuarios.Count);
    }

    [TestMethod]
    public void ObtenerTodosLosUsuarios_SinUsuarios_DevuelveListaVacia()
    {
        // Act
        var usuarios = _servicio.ObtenerTodosLosUsuarios();

        // Assert
        Assert.AreEqual(0, usuarios.Count);
    }

    [TestMethod]
    public void ObtenerTodosLosUsuarios_ConVariosUsuarios_DevuelveTodos()
    {
        // Arrange
        var usuario1 = CrearUsuarioValido("juan@example.com");
        var usuario2 = CrearUsuarioValido("maria@example.com");
        var usuario3 = CrearUsuarioValido("pedro@example.com");
        _servicio.AgregarUsuario(usuario1);
        _servicio.AgregarUsuario(usuario2);
        _servicio.AgregarUsuario(usuario3);

        // Act
        var usuarios = _servicio.ObtenerTodosLosUsuarios();

        // Assert
        Assert.AreEqual(3, usuarios.Count);
    }

    [TestMethod]
    public void ObtenerPorEmail_ConEmailExistente_DevuelveUsuario()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario);

        // Act
        var usuarioEncontrado = _servicio.ObtenerPorEmail("juan@example.com");

        // Assert
        Assert.IsNotNull(usuarioEncontrado);
        Assert.AreSame(usuario, usuarioEncontrado);
    }

    [TestMethod]
    public void ObtenerPorEmail_ConEmailNoExistente_DevuelveNull()
    {
        // Act
        var usuarioEncontrado = _servicio.ObtenerPorEmail("noexiste@example.com");

        // Assert
        Assert.IsNull(usuarioEncontrado);
    }

    [TestMethod]
    public void GenerarContraseniaDefecto_AsignaContraseniaAlUsuario()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");

        // Act
        _servicio.GenerarContraseniaDefecto(usuario);

        // Assert
        Assert.IsNotNull(usuario.ContraseniaCifrada);
        Assert.IsTrue(usuario.VerificarContrasenia("TempPass@2026"));
    }

    [TestMethod]
    public void GenerarContraseniaDefecto_ConSeed_AsignaContraseniaConSeed()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");

        // Act
        _servicio.GenerarContraseniaDefecto(usuario, seed: 42);

        // Assert
        Assert.IsNotNull(usuario.ContraseniaCifrada);
        Assert.IsTrue(usuario.VerificarContrasenia("TempPass@2068"));
    }

    [TestMethod]
    public void ReiniciarContrasenia_ConAdministrador_ReiniciaContrasenia()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        var administrador = CrearUsuarioValido("admin@example.com");
        _servicio.AgregarUsuario(usuario);
        _servicio.AgregarUsuario(administrador);
        administrador.AgregarRol(RolUsuario.AdministradorDelSistema);

        // Primer login - establecer contraseña
        _servicio.GenerarContraseniaDefecto(usuario);

        // Act
        _servicio.ReiniciarContrasenia(usuario, administrador);

        // Assert
        Assert.IsTrue(usuario.VerificarContrasenia("TempPass@2026"));
    }

    [TestMethod]
    public void ReiniciarContrasenia_ConNoAdministrador_LanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        var noAdmin = CrearUsuarioValido("noadmin@example.com");
        _servicio.AgregarUsuario(usuario);
        _servicio.AgregarUsuario(noAdmin);
        noAdmin.AgregarRol(RolUsuario.Editor);

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => _servicio.ReiniciarContrasenia(usuario, noAdmin));
    }

    [TestMethod]
    public void CambiarContrasenia_ConContraseniaActualCorrecta_CambiaLaContrasenia()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        usuario.EstablecerContrasenia("ViejaPass@123");

        // Act
        _servicio.CambiarContrasenia(usuario, "ViejaPass@123", "NuevaPass@456");

        // Assert
        Assert.IsTrue(usuario.VerificarContrasenia("NuevaPass@456"));
        Assert.IsFalse(usuario.VerificarContrasenia("ViejaPass@123"));
    }

    [TestMethod]
    public void CambiarContrasenia_ConContraseniaActualIncorrecta_LanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        usuario.EstablecerContrasenia("ViejaPass@123");

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            _servicio.CambiarContrasenia(usuario, "IncorrectPass@999", "NuevaPass@456")
        );
    }

    [TestMethod]
    public void Login_ConEmailYContraseniaCorrectos_DevuelveUsuario()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        usuario.EstablecerContrasenia("Pass@12345");
        _servicio.AgregarUsuario(usuario);

        // Act
        var usuarioAutenticado = _servicio.Login("juan@example.com", "Pass@12345");

        // Assert
        Assert.IsNotNull(usuarioAutenticado);
        Assert.AreSame(usuario, usuarioAutenticado);
    }

    [TestMethod]
    public void Login_ConEmailNoExistente_LanzaExcepcion()
    {
        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            _servicio.Login("noexiste@example.com", "Pass@12345")
        );
    }

    [TestMethod]
    public void Login_ConContraseniaIncorrecta_LanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        usuario.EstablecerContrasenia("Pass@12345");
        _servicio.AgregarUsuario(usuario);

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            _servicio.Login("juan@example.com", "IncorrectPass@999")
        );
    }

    [TestMethod]
    public void Login_CaseSensitiveEmail_NoDistingueMayusculas()
    {
        // Arrange
        var usuario = CrearUsuarioValido("Juan@Example.com");
        usuario.EstablecerContrasenia("Pass@12345");
        _servicio.AgregarUsuario(usuario);

        // Act
        var usuarioAutenticado = _servicio.Login("juan@example.com", "Pass@12345");

        // Assert
        Assert.IsNotNull(usuarioAutenticado);
        Assert.AreSame(usuario, usuarioAutenticado);
    }

    [TestMethod]
    public void ActualizarUsuario_ConNombreVacio_LanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario);

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            _servicio.ActualizarUsuario(usuario, nombre: "")
        );
    }

    [TestMethod]
    public void ActualizarUsuario_ConApellidoVacio_LanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario);

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            _servicio.ActualizarUsuario(usuario, apellido: "")
        );
    }

    [TestMethod]
    public void ActualizarUsuario_ConEmailVacio_LanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario);

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            _servicio.ActualizarUsuario(usuario, email: "")
        );
    }

    [TestMethod]
    public void ActualizarUsuario_ConEmailInvalido_LanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario);

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            _servicio.ActualizarUsuario(usuario, email: "email_invalido")
        );
    }

    [TestMethod]
    public void ActualizarUsuario_ConEmailEspacios_LanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario);

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            _servicio.ActualizarUsuario(usuario, email: "   ")
        );
    }

    private static Usuario CrearUsuarioValido(string email)
    {
        return new Usuario(new DatosUsuario
        {
            Nombre = "Juan",
            Apellido = "Pérez",
            Email = email,
            FechaNacimiento = "01-01-1990"
        });
    }
}

~~~

### WorldCupPlanner.ServicioTests.csproj

Ruta: WorldCupPlanner.ServicioTests\WorldCupPlanner.ServicioTests.csproj

~~~xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>

    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="coverlet.collector" Version="6.0.0" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
    <PackageReference Include="MSTest.TestAdapter" Version="3.1.1" />
    <PackageReference Include="MSTest.TestFramework" Version="3.1.1" />
  </ItemGroup>

  <ItemGroup>
    <Using Include="Microsoft.VisualStudio.TestTools.UnitTesting" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\WorldCupPlanner.Dominio\WorldCupPlanner.Dominio.csproj" />
    <ProjectReference Include="..\WorldCupPlanner.Servicios\WorldCupPlanner.Servicios.csproj" />
    <ProjectReference Include="..\WorldCupPlanner.AccesoADatos\WorldCupPlanner.AccesoADatos.csproj" />
  </ItemGroup>

</Project>
~~~

End of WorldCupPlanner.ServicioTests

## WorldCupPlanner.UI

### _Imports.razor

Ruta: WorldCupPlanner.UI\_Imports.razor

~~~razor
@using System.Net.Http
@using Microsoft.AspNetCore.Authorization
@using Microsoft.AspNetCore.Components.Authorization
@using Microsoft.AspNetCore.Components.Forms
@using Microsoft.AspNetCore.Components.Routing
@using Microsoft.AspNetCore.Components.Web
@using Microsoft.AspNetCore.Components.Web.Virtualization
@using Microsoft.JSInterop
@using WorldCupPlanner.UI
@using WorldCupPlanner.UI.Shared
~~~

### App.razor

Ruta: WorldCupPlanner.UI\App.razor

~~~razor
<Router AppAssembly="@typeof(App).Assembly">
    <Found Context="routeData">
        <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
        <FocusOnNavigate RouteData="@routeData" Selector="h1" />
    </Found>
    <NotFound>
        <PageTitle>Not found</PageTitle>
        <LayoutView Layout="@typeof(MainLayout)">
            <p role="alert">Sorry, there's nothing at this address.</p>
        </LayoutView>
    </NotFound>
</Router>
~~~

### appsettings.Development.json

Ruta: WorldCupPlanner.UI\appsettings.Development.json

~~~json
{
  "DetailedErrors": true,
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
~~~

### appsettings.json

Ruta: WorldCupPlanner.UI\appsettings.json

~~~json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
~~~

### WeatherForecast.cs

Ruta: WorldCupPlanner.UI\Data\WeatherForecast.cs

~~~csharp
namespace WorldCupPlanner.UI.Data;

public class WeatherForecast
{
    public DateOnly Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary { get; set; }
}
~~~

### WeatherForecastService.cs

Ruta: WorldCupPlanner.UI\Data\WeatherForecastService.cs

~~~csharp
namespace WorldCupPlanner.UI.Data;

public class WeatherForecastService
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public Task<WeatherForecast[]> GetForecastAsync(DateOnly startDate)
    {
        return Task.FromResult(Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = startDate.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        }).ToArray());
    }
}
~~~

### _Host.cshtml

Ruta: WorldCupPlanner.UI\Pages\_Host.cshtml

~~~cshtml
@page "/"
@using Microsoft.AspNetCore.Components.Web
@namespace WorldCupPlanner.UI.Pages
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <base href="~/" />
    <link rel="stylesheet" href="css/bootstrap/bootstrap.min.css" />
    <link href="css/site.css" rel="stylesheet" />
    <link href="WorldCupPlanner.UI.styles.css" rel="stylesheet" />
    <link rel="icon" type="image/png" href="favicon.png"/>
    <component type="typeof(HeadOutlet)" render-mode="ServerPrerendered" />
</head>
<body>
    <component type="typeof(App)" render-mode="ServerPrerendered" />

    <div id="blazor-error-ui">
        <environment include="Staging,Production">
            An error has occurred. This application may no longer respond until reloaded.
        </environment>
        <environment include="Development">
            An unhandled exception has occurred. See browser dev tools for details.
        </environment>
        <a href="" class="reload">Reload</a>
        <a class="dismiss">🗙</a>
    </div>

    <script src="_framework/blazor.server.js"></script>
</body>
</html>
~~~

### CrearEquipo.razor

Ruta: WorldCupPlanner.UI\Pages\CrearEquipo.razor

~~~razor
@page "/equipos/crear"
@using WorldCupPlanner.Servicios
@using WorldCupPlanner.Dominio.Entidades
@using WorldCupPlanner.Dominio.Enumeraciones
@inject ServicioEquipo ServicioEquipo
@inject NavigationManager NavigationManager

<PageTitle>Crear Equipo</PageTitle>
<h1>Crear Equipo</h1>

@if (!string.IsNullOrEmpty(errorMensaje))
{
    <div class="alert alert-danger">@errorMensaje</div>
}

<div class="form-group">
    <label>Nombre</label>
    <input type="text" class="form-control" @bind="equipo.Nombre" />
</div>
<div class="form-group">
    <label>ConfederaciÃ³n</label>
    <select class="form-control" @bind="equipo.Confederacion">
        @foreach (var conf in Enum.GetValues<Confederacion>())
        {
            <option value="@conf">@conf</option>
        }
    </select>
</div>
<div class="form-group">
    <label>Ranking FIFA</label>
    <input type="number" class="form-control" @bind="equipo.RankingFIFA" />
</div>

<button type="button" class="btn btn-primary mt-2" @onclick="Guardar">Guardar</button>
<a href="/equipos" class="btn btn-secondary mt-2">Cancelar</a>

@code {
    private Equipo equipo = new();
    private string errorMensaje = string.Empty;

    private void Guardar()
    {
        try
        {
            ServicioEquipo.AgregarEquipo(equipo);
            NavigationManager.NavigateTo("/equipos");
        }
        catch (Exception ex)
        {
            errorMensaje = ex.Message;
        }
    }
}
~~~

### CrearEstadio.razor

Ruta: WorldCupPlanner.UI\Pages\CrearEstadio.razor

~~~razor
@page "/estadios/crear"
@using WorldCupPlanner.Servicios
@using WorldCupPlanner.Dominio.Entidades
@inject ServicioEstadio ServicioEstadio
@inject NavigationManager NavigationManager

<PageTitle>Crear Estadio</PageTitle>
<h1>Crear Estadio</h1>

@if (!string.IsNullOrEmpty(errorMensaje))
{
    <div class="alert alert-danger">@errorMensaje</div>
}

<div class="form-group">
    <label>Nombre</label>
    <input type="text" class="form-control" @bind="estadio.Nombre" />
</div>
<div class="form-group">
    <label>Ciudad</label>
    <input type="text" class="form-control" @bind="estadio.Ciudad" />
</div>
<div class="form-group">
    <label>Capacidad</label>
    <input type="number" class="form-control" @bind="estadio.Capacidad" />
</div>
<div class="form-group">
    <label>DescripciÃ³n (opcional)</label>
    <input type="text" class="form-control" @bind="estadio.Descripcion" />
</div>

<button type="button" class="btn btn-primary mt-2" @onclick="Guardar">Guardar</button>
<a href="/estadios" class="btn btn-secondary mt-2">Cancelar</a>

@code {
    private Estadio estadio = new();
    private string errorMensaje = string.Empty;

    private void Guardar()
    {
        try
        {
            ServicioEstadio.AgregarEstadio(estadio);
            NavigationManager.NavigateTo("/estadios");
        }
        catch (Exception ex)
        {
            errorMensaje = ex.Message;
        }
    }
}
~~~

### EditarEquipo.razor

Ruta: WorldCupPlanner.UI\Pages\EditarEquipo.razor

~~~razor
@page "/equipos/{Nombre}/editar"
@using WorldCupPlanner.Servicios
@using WorldCupPlanner.Dominio.Entidades
@using WorldCupPlanner.Dominio.Enumeraciones
@inject ServicioEquipo ServicioEquipo
@inject NavigationManager NavigationManager

<PageTitle>Editar Equipo</PageTitle>
<h1>Editar Equipo</h1>

@if (!string.IsNullOrEmpty(errorMensaje))
{
    <div class="alert alert-danger">@errorMensaje</div>
}

<div class="form-group">
    <label>Nombre</label>
    <input type="text" class="form-control" @bind="equipo.Nombre" />
</div>
<div class="form-group">
    <label>ConfederaciÃ³n</label>
    <select class="form-control" @bind="equipo.Confederacion">
        @foreach (var conf in Enum.GetValues<Confederacion>())
        {
            <option value="@conf">@conf</option>
        }
    </select>
</div>
<div class="form-group">
    <label>Ranking FIFA</label>
    <input type="number" class="form-control" @bind="equipo.RankingFIFA" />
</div>

<button type="button" class="btn btn-primary mt-2" @onclick="Guardar">Guardar</button>
<a href="/equipos" class="btn btn-secondary mt-2">Cancelar</a>

@code {
    [Parameter] public string Nombre { get; set; } = string.Empty;
    private Equipo equipo = new();
    private string errorMensaje = string.Empty;

    protected override void OnInitialized()
    {
        var original = ServicioEquipo.ObtenerEquipos().First(e => e.Nombre == Nombre);
        equipo = new Equipo
        {
            Nombre = original.Nombre,
            Confederacion = original.Confederacion,
            RankingFIFA = original.RankingFIFA
        };
    }

    private void Guardar()
    {
        try
        {
            ServicioEquipo.EditarEquipo(equipo, Nombre);
            NavigationManager.NavigateTo("/equipos");
        }
        catch (Exception ex)
        {
            errorMensaje = ex.Message;
        }
    }
}
~~~

### EditarEstadio.razor

Ruta: WorldCupPlanner.UI\Pages\EditarEstadio.razor

~~~razor
@page "/estadios/{Nombre}/editar"
@using WorldCupPlanner.Servicios
@using WorldCupPlanner.Dominio.Entidades
@inject ServicioEstadio ServicioEstadio
@inject NavigationManager NavigationManager

<PageTitle>Editar Estadio</PageTitle>
<h1>Editar Estadio</h1>

@if (!string.IsNullOrEmpty(errorMensaje))
{
    <div class="alert alert-danger">@errorMensaje</div>
}

<div class="form-group">
    <label>Nombre</label>
    <input type="text" class="form-control" @bind="estadio.Nombre" />
</div>
<div class="form-group">
    <label>Ciudad</label>
    <input type="text" class="form-control" @bind="estadio.Ciudad" />
</div>
<div class="form-group">
    <label>Capacidad</label>
    <input type="number" class="form-control" @bind="estadio.Capacidad" />
</div>
<div class="form-group">
    <label>DescripciÃ³n (opcional)</label>
    <input type="text" class="form-control" @bind="estadio.Descripcion" />
</div>

<button type="button" class="btn btn-primary mt-2" @onclick="Guardar">Guardar</button>
<a href="/estadios" class="btn btn-secondary mt-2">Cancelar</a>

@code {
    [Parameter] public string Nombre { get; set; } = string.Empty;
    private Estadio estadio = new();
    private string errorMensaje = string.Empty;

    protected override void OnInitialized()
    {
        var original = ServicioEstadio.ObtenerEstadios().First(e => e.Nombre == Nombre);
        estadio = new Estadio
        {
            Nombre = original.Nombre,
            Ciudad = original.Ciudad,
            Capacidad = original.Capacidad,
            Descripcion = original.Descripcion
        };
    }

    private void Guardar()
    {
        try
        {
            ServicioEstadio.EditarEstadio(Nombre, estadio);
            NavigationManager.NavigateTo("/estadios");
        }
        catch (Exception ex)
        {
            errorMensaje = ex.Message;
        }
    }
}
~~~

### Equipos.razor

Ruta: WorldCupPlanner.UI\Pages\Equipos.razor

~~~razor
@page "/equipos"
@using WorldCupPlanner.Servicios
@using WorldCupPlanner.Dominio.Entidades
@inject ServicioEquipo ServicioEquipo

<PageTitle>Equipos</PageTitle>

<h1>
    Equipos
    <a href="/equipos/crear" class="btn btn-success btn-sm" style="float: right; margin-right: 40px;">Agregar +</a>
</h1>

<table class="table">
    <thead>
    <tr>
        <th>Nombre</th>
        <th>ConfederaciÃ³n</th>
        <th>Ranking FIFA</th>
        <th>Acciones</th>
    </tr>
    </thead>
    <tbody>
    @foreach (var equipo in equipos)
    {
        <tr>
            <td>@equipo.Nombre</td>
            <td>@equipo.Confederacion</td>
            <td>@equipo.RankingFIFA</td>
            <td>
                <a href="/equipos/@equipo.Nombre/editar" class="btn btn-success btn-sm">Editar</a>
                <button class="btn btn-danger btn-sm" @onclick="(() => Eliminar(equipo))">Eliminar</button>
            </td>
        </tr>
    }
    </tbody>
</table>

@code {
    private List<Equipo> equipos = new();

    protected override void OnInitialized()
    {
        equipos = ServicioEquipo.ObtenerEquipos();
    }

    private void Eliminar(Equipo equipo)
    {
        ServicioEquipo.EliminarEquipo(equipo);
        equipos = ServicioEquipo.ObtenerEquipos();
    }
}
~~~

### Error.cshtml

Ruta: WorldCupPlanner.UI\Pages\Error.cshtml

~~~cshtml
@page
@model WorldCupPlanner.UI.Pages.ErrorModel

<!DOCTYPE html>
<html lang="en">

<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no" />
    <title>Error</title>
    <link href="~/css/bootstrap/bootstrap.min.css" rel="stylesheet" />
    <link href="~/css/site.css" rel="stylesheet" asp-append-version="true" />
</head>

<body>
    <div class="main">
        <div class="content px-4">
            <h1 class="text-danger">Error.</h1>
            <h2 class="text-danger">An error occurred while processing your request.</h2>

            @if (Model.ShowRequestId)
            {
                <p>
                    <strong>Request ID:</strong> <code>@Model.RequestId</code>
                </p>
            }

            <h3>Development Mode</h3>
            <p>
                Swapping to the <strong>Development</strong> environment displays detailed information about the error that occurred.
            </p>
            <p>
                <strong>The Development environment shouldn't be enabled for deployed applications.</strong>
                It can result in displaying sensitive information from exceptions to end users.
                For local debugging, enable the <strong>Development</strong> environment by setting the <strong>ASPNETCORE_ENVIRONMENT</strong> environment variable to <strong>Development</strong>
                and restarting the app.
            </p>
        </div>
    </div>
</body>

</html>
~~~

### Error.cshtml.cs

Ruta: WorldCupPlanner.UI\Pages\Error.cshtml.cs

~~~csharp
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WorldCupPlanner.UI.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel : PageModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    private readonly ILogger<ErrorModel> _logger;

    public ErrorModel(ILogger<ErrorModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
    }
}
~~~

### Estadios.razor

Ruta: WorldCupPlanner.UI\Pages\Estadios.razor

~~~razor
@page "/estadios"
@using WorldCupPlanner.Servicios
@using WorldCupPlanner.Dominio.Entidades
@inject ServicioEstadio ServicioEstadio

<PageTitle>Estadios</PageTitle>

<h1>
    Estadios
    <a href="/estadios/crear" class="btn btn-success btn-sm" style="float: right; margin-right: 40px;">Agregar +</a>
</h1>

<table class="table">
    <thead>
    <tr>
        <th>Nombre</th>
        <th>Ciudad</th>
        <th>Capacidad</th>
        <th>Acciones</th>
    </tr>
    </thead>
    <tbody>
    @foreach (var estadio in estadios)
    {
        <tr>
            <td>@estadio.Nombre</td>
            <td>@estadio.Ciudad</td>
            <td>@estadio.Capacidad</td>
            <td>
                <a href="/estadios/@estadio.Nombre/editar" class="btn btn-success btn-sm">Editar</a>
                <button class="btn btn-danger btn-sm" @onclick="(() => Eliminar(estadio))">Eliminar</button>
            </td>
        </tr>
    }
    </tbody>
</table>

@code {
    private List<Estadio> estadios = new();

    protected override void OnInitialized()
    {
        estadios = ServicioEstadio.ObtenerEstadios();
    }

    private void Eliminar(Estadio estadio)
    {
        ServicioEstadio.EliminarEstadio(estadio);
        estadios = ServicioEstadio.ObtenerEstadios();
    }
}
~~~

### Index.razor

Ruta: WorldCupPlanner.UI\Pages\Index.razor

~~~razor
@page "/"

<PageTitle>Index</PageTitle>

<h1>Hello, world!</h1>

Welcome to your new app.

<SurveyPrompt Title="How is Blazor working for you?" />
~~~

### Program.cs

Ruta: WorldCupPlanner.UI\Program.cs

~~~csharp
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using WorldCupPlanner.UI.Data;
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.AccesoADatos.Repositorios;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Servicios;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddSingleton<IRepositorioConId<Auditoria>, RepositorioAuditoriaEnMemoria>();
builder.Services.AddSingleton<IRepositorio<Equipo>, RepositorioEquipoEnMemoria>();
builder.Services.AddSingleton<IRepositorio<Estadio>, RepositorioEstadioEnMemoria>();
builder.Services.AddSingleton<ServicioAuditoria>();
builder.Services.AddSingleton<ServicioEstadio>();
builder.Services.AddSingleton<ServicioEquipo>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
~~~

### launchSettings.json

Ruta: WorldCupPlanner.UI\Properties\launchSettings.json

~~~json
{
  "iisSettings": {
    "windowsAuthentication": false,
    "anonymousAuthentication": true,
    "iisExpress": {
      "applicationUrl": "http://localhost:30895",
      "sslPort": 44322
    }
  },
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "applicationUrl": "http://localhost:5076",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "applicationUrl": "https://localhost:7129;http://localhost:5076",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "IIS Express": {
      "commandName": "IISExpress",
      "launchBrowser": true,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
~~~

### MainLayout.razor

Ruta: WorldCupPlanner.UI\Shared\MainLayout.razor

~~~razor
@inherits LayoutComponentBase

<PageTitle>WorldCupPlanner.UI</PageTitle>

<div class="page">
    <div class="sidebar">
        <NavMenu />
    </div>

    <main>
        <div class="top-row px-4">
            <a href="https://docs.microsoft.com/aspnet/" target="_blank">About</a>
        </div>

        <article class="content px-4">
            @Body
        </article>
    </main>
</div>
~~~

### MainLayout.razor.css

Ruta: WorldCupPlanner.UI\Shared\MainLayout.razor.css

~~~css
.page {
    position: relative;
    display: flex;
    flex-direction: column;
}

main {
    flex: 1;
}

.sidebar {
    background-image: linear-gradient(180deg, rgb(5, 39, 103) 0%, #3a0647 70%);
}

.top-row {
    background-color: #f7f7f7;
    border-bottom: 1px solid #d6d5d5;
    justify-content: flex-end;
    height: 3.5rem;
    display: flex;
    align-items: center;
}

    .top-row ::deep a, .top-row .btn-link {
        white-space: nowrap;
        margin-left: 1.5rem;
    }

    .top-row a:first-child {
        overflow: hidden;
        text-overflow: ellipsis;
    }

@media (max-width: 640.98px) {
    .top-row:not(.auth) {
        display: none;
    }

    .top-row.auth {
        justify-content: space-between;
    }

    .top-row a, .top-row .btn-link {
        margin-left: 0;
    }
}

@media (min-width: 641px) {
    .page {
        flex-direction: row;
    }

    .sidebar {
        width: 250px;
        height: 100vh;
        position: sticky;
        top: 0;
    }

    .top-row {
        position: sticky;
        top: 0;
        z-index: 1;
    }

    .top-row, article {
        padding-left: 2rem !important;
        padding-right: 1.5rem !important;
    }
}
~~~

### NavMenu.razor

Ruta: WorldCupPlanner.UI\Shared\NavMenu.razor

~~~razor
<div class="top-row ps-3 navbar navbar-dark">
    <div class="container-fluid">
        <a class="navbar-brand" href="">WorldCupPlanner.UI</a>
        <button title="Navigation menu" class="navbar-toggler" @onclick="ToggleNavMenu">
            <span class="navbar-toggler-icon"></span>
        </button>
    </div>
</div>

<div class="@NavMenuCssClass nav-scrollable" @onclick="ToggleNavMenu">
    <nav class="flex-column">
        <div class="nav-item px-3">
            <NavLink class="nav-link" href="" Match="NavLinkMatch.All">
                <span class="oi oi-home" aria-hidden="true"></span> Home
            </NavLink>
        </div>
        <div class="nav-item px-3">
            <NavLink class="nav-link" href="estadios">
                <span class="oi oi-map-marker" aria-hidden="true"></span> Estadios
            </NavLink>
        </div>
        <div class="nav-item px-3">
            <NavLink class="nav-link" href="equipos">
                <span class="oi oi-people" aria-hidden="true"></span> Equipos
            </NavLink>
        </div>
    </nav>
</div>

@code {
    private bool collapseNavMenu = true;

    private string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;

    private void ToggleNavMenu()
    {
        collapseNavMenu = !collapseNavMenu;
    }
}
~~~

### NavMenu.razor.css

Ruta: WorldCupPlanner.UI\Shared\NavMenu.razor.css

~~~css
.navbar-toggler {
    background-color: rgba(255, 255, 255, 0.1);
}

.top-row {
    height: 3.5rem;
    background-color: rgba(0,0,0,0.4);
}

.navbar-brand {
    font-size: 1.1rem;
}

.oi {
    width: 2rem;
    font-size: 1.1rem;
    vertical-align: text-top;
    top: -2px;
}

.nav-item {
    font-size: 0.9rem;
    padding-bottom: 0.5rem;
}

    .nav-item:first-of-type {
        padding-top: 1rem;
    }

    .nav-item:last-of-type {
        padding-bottom: 1rem;
    }

    .nav-item ::deep a {
        color: #d7d7d7;
        border-radius: 4px;
        height: 3rem;
        display: flex;
        align-items: center;
        line-height: 3rem;
    }

.nav-item ::deep a.active {
    background-color: rgba(255,255,255,0.25);
    color: white;
}

.nav-item ::deep a:hover {
    background-color: rgba(255,255,255,0.1);
    color: white;
}

@media (min-width: 641px) {
    .navbar-toggler {
        display: none;
    }

    .collapse {
        /* Never collapse the sidebar for wide screens */
        display: block;
    }
    
    .nav-scrollable {
        /* Allow sidebar to scroll for tall menus */
        height: calc(100vh - 3.5rem);
        overflow-y: auto;
    }
}
~~~

### SurveyPrompt.razor

Ruta: WorldCupPlanner.UI\Shared\SurveyPrompt.razor

~~~razor
<div class="alert alert-secondary mt-4">
    <span class="oi oi-pencil me-2" aria-hidden="true"></span>
    <strong>@Title</strong>

    <span class="text-nowrap">
        Please take our
        <a target="_blank" class="font-weight-bold link-dark" href="https://go.microsoft.com/fwlink/?linkid=2186158">brief survey</a>
    </span>
    and tell us what you think.
</div>

@code {
    // Demonstrates how a parent component can supply parameters
    [Parameter]
    public string? Title { get; set; }
}
~~~

### WorldCupPlanner.UI.csproj

Ruta: WorldCupPlanner.UI\WorldCupPlanner.UI.csproj

~~~xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\WorldCupPlanner.AccesoADatos\WorldCupPlanner.AccesoADatos.csproj" />
    <ProjectReference Include="..\WorldCupPlanner.Servicios\WorldCupPlanner.Servicios.csproj" />
    <ProjectReference Include="..\WorldCupPlanner.Dominio\WorldCupPlanner.Dominio.csproj" />
  </ItemGroup>

</Project>
~~~

### site.css

Ruta: WorldCupPlanner.UI\wwwroot\css\site.css

~~~css
@import url('open-iconic/font/css/open-iconic-bootstrap.min.css');

html, body {
    font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif;
}

h1:focus {
    outline: none;
}

a, .btn-link {
    color: #0071c1;
}

.btn-primary {
    color: #fff;
    background-color: #1b6ec2;
    border-color: #1861ac;
}

.btn:focus, .btn:active:focus, .btn-link.nav-link:focus, .form-control:focus, .form-check-input:focus {
  box-shadow: 0 0 0 0.1rem white, 0 0 0 0.25rem #258cfb;
}

.content {
    padding-top: 1.1rem;
}

.valid.modified:not([type=checkbox]) {
    outline: 1px solid #26b050;
}

.invalid {
    outline: 1px solid red;
}

.validation-message {
    color: red;
}

#blazor-error-ui {
    background: lightyellow;
    bottom: 0;
    box-shadow: 0 -1px 2px rgba(0, 0, 0, 0.2);
    display: none;
    left: 0;
    padding: 0.6rem 1.25rem 0.7rem 1.25rem;
    position: fixed;
    width: 100%;
    z-index: 1000;
}

    #blazor-error-ui .dismiss {
        cursor: pointer;
        position: absolute;
        right: 0.75rem;
        top: 0.5rem;
    }

.blazor-error-boundary {
    background: url(data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNTYiIGhlaWdodD0iNDkiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyIgeG1sbnM6eGxpbms9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkveGxpbmsiIG92ZXJmbG93PSJoaWRkZW4iPjxkZWZzPjxjbGlwUGF0aCBpZD0iY2xpcDAiPjxyZWN0IHg9IjIzNSIgeT0iNTEiIHdpZHRoPSI1NiIgaGVpZ2h0PSI0OSIvPjwvY2xpcFBhdGg+PC9kZWZzPjxnIGNsaXAtcGF0aD0idXJsKCNjbGlwMCkiIHRyYW5zZm9ybT0idHJhbnNsYXRlKC0yMzUgLTUxKSI+PHBhdGggZD0iTTI2My41MDYgNTFDMjY0LjcxNyA1MSAyNjUuODEzIDUxLjQ4MzcgMjY2LjYwNiA1Mi4yNjU4TDI2Ny4wNTIgNTIuNzk4NyAyNjcuNTM5IDUzLjYyODMgMjkwLjE4NSA5Mi4xODMxIDI5MC41NDUgOTIuNzk1IDI5MC42NTYgOTIuOTk2QzI5MC44NzcgOTMuNTEzIDI5MSA5NC4wODE1IDI5MSA5NC42NzgyIDI5MSA5Ny4wNjUxIDI4OS4wMzggOTkgMjg2LjYxNyA5OUwyNDAuMzgzIDk5QzIzNy45NjMgOTkgMjM2IDk3LjA2NTEgMjM2IDk0LjY3ODIgMjM2IDk0LjM3OTkgMjM2LjAzMSA5NC4wODg2IDIzNi4wODkgOTMuODA3MkwyMzYuMzM4IDkzLjAxNjIgMjM2Ljg1OCA5Mi4xMzE0IDI1OS40NzMgNTMuNjI5NCAyNTkuOTYxIDUyLjc5ODUgMjYwLjQwNyA1Mi4yNjU4QzI2MS4yIDUxLjQ4MzcgMjYyLjI5NiA1MSAyNjMuNTA2IDUxWk0yNjMuNTg2IDY2LjAxODNDMjYwLjczNyA2Ni4wMTgzIDI1OS4zMTMgNjcuMTI0NSAyNTkuMzEzIDY5LjMzNyAyNTkuMzEzIDY5LjYxMDIgMjU5LjMzMiA2OS44NjA4IDI1OS4zNzEgNzAuMDg4N0wyNjEuNzk1IDg0LjAxNjEgMjY1LjM4IDg0LjAxNjEgMjY3LjgyMSA2OS43NDc1QzI2Ny44NiA2OS43MzA5IDI2Ny44NzkgNjkuNTg3NyAyNjcuODc5IDY5LjMxNzkgMjY3Ljg3OSA2Ny4xMTgyIDI2Ni40NDggNjYuMDE4MyAyNjMuNTg2IDY2LjAxODNaTTI2My41NzYgODYuMDU0N0MyNjEuMDQ5IDg2LjA1NDcgMjU5Ljc4NiA4Ny4zMDA1IDI1OS43ODYgODkuNzkyMSAyNTkuNzg2IDkyLjI4MzcgMjYxLjA0OSA5My41Mjk1IDI2My41NzYgOTMuNTI5NSAyNjYuMTE2IDkzLjUyOTUgMjY3LjM4NyA5Mi4yODM3IDI2Ny4zODcgODkuNzkyMSAyNjcuMzg3IDg3LjMwMDUgMjY2LjExNiA4Ni4wNTQ3IDI2My41NzYgODYuMDU0N1oiIGZpbGw9IiNGRkU1MDAiIGZpbGwtcnVsZT0iZXZlbm9kZCIvPjwvZz48L3N2Zz4=) no-repeat 1rem/1.8rem, #b32121;
    padding: 1rem 1rem 1rem 3.7rem;
    color: white;
}

    .blazor-error-boundary::after {
        content: "An error has occurred."
    }
~~~

End of WorldCupPlanner.UI

## WorldCupPlanner.AccesoADatos

### IRepositorio.cs

Ruta: WorldCupPlanner.AccesoADatos\Interfaces\IRepositorio.cs

~~~csharp

namespace WorldCupPlanner.AccesoADatos.Interfaces;

public interface IRepositorio<T>
{
    void Agregar(T entidad);
    void Actualizar(T entidad);
    void Eliminar(T entidad);
    List<T> Buscar(Func<T,bool> predicado);
}
~~~

### IRepositorioConId.cs

Ruta: WorldCupPlanner.AccesoADatos\Interfaces\IRepositorioConId.cs

~~~csharp

namespace WorldCupPlanner.AccesoADatos.Interfaces;

public interface IRepositorioConId<T> : IRepositorio<T>
{
    int SiguienteId();
}
~~~

### RepositorioAuditoriaEnMemoria.cs

Ruta: WorldCupPlanner.AccesoADatos\Repositorios\RepositorioAuditoriaEnMemoria.cs

~~~csharp
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioAuditoriaEnMemoria : IRepositorioConId<Auditoria>
{
    private readonly List<Auditoria> _logs = new();
    private int _contador;

    public int SiguienteId() => ++_contador;
    
    public void Agregar(Auditoria entidad)
    {
        _logs.Add(entidad);
    }

    public void Actualizar(Auditoria entidad)
    {
        var indice = _logs.FindIndex(e => e.Id == entidad.Id);

        if (indice == -1)
        {
            throw new EntidadInexistenteExcepcion("El log de auditoría no existe.");
        }

        _logs[indice] = entidad;
    }

    public void Eliminar(Auditoria entidad)
    {
            _logs.Remove(entidad);
    }

    public List<Auditoria> Buscar(Func<Auditoria, bool> predicado)
    {
        return _logs.Where(predicado).ToList();
    }
}
~~~

### RepositorioEquipoEnMemoria.cs

Ruta: WorldCupPlanner.AccesoADatos\Repositorios\RepositorioEquipoEnMemoria.cs

~~~csharp
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioEquipoEnMemoria : IRepositorio<Equipo>
{
    private readonly List<Equipo> _equipos = new();

    public void Agregar(Equipo equipo)
    {
        _equipos.Add(equipo);
    }

    public void Actualizar(Equipo equipo)
    {
        var indice = _equipos.FindIndex(e => ReferenceEquals(e, equipo));

        if (indice == -1)
        {
            throw new InvalidOperationException("El equipo no existe.");
        }

        _equipos[indice] = equipo;
    }

    public void Eliminar(Equipo equipo)
    {
        _equipos.Remove(equipo);
    }

    public List<Equipo> Buscar(Func<Equipo, bool> predicado)
    {
        return _equipos.Where(predicado).ToList();
    }
}
~~~

### RepositorioEstadioEnMemoria.cs

Ruta: WorldCupPlanner.AccesoADatos\Repositorios\RepositorioEstadioEnMemoria.cs

~~~csharp
using WorldCupPlanner.AccesoADatos.Interfaces;
using  WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioEstadioEnMemoria : IRepositorio<Estadio>
{
    private readonly List<Estadio> _estadios = new();

    public void Agregar(Estadio estadio)
    {
        _estadios.Add(estadio);
    }

    public void Actualizar(Estadio estadio)
    {
        var indice = _estadios.FindIndex(e => ReferenceEquals(e, estadio));

        if (indice == -1)
        {
            throw new InvalidOperationException("El estadio no existe.");
        }

        _estadios[indice] = estadio;
    }

    public void Eliminar(Estadio estadio)
    {
        _estadios.Remove(estadio);
    }
    
    public List<Estadio> Buscar(Func<Estadio, bool> predicado)
    {
        return _estadios.Where(predicado).ToList();
    }
}
~~~

### RepositorioGrupoEnMemoria.cs

Ruta: WorldCupPlanner.AccesoADatos\Repositorios\RepositorioGrupoEnMemoria.cs

~~~csharp
using WorldCupPlanner.AccesoADatos.Interfaces;
using  WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioGrupoEnMemoria : IRepositorio<Grupo>
{
    private readonly List<Grupo> _grupos = new();

    public void Agregar(Grupo grupo)
    {
        _grupos.Add(grupo);
    }

    public void Actualizar(Grupo grupo)
    {
        var indice = _grupos.FindIndex(g => ReferenceEquals(g, grupo));

        if (indice == -1)
        {
            throw new InvalidOperationException("El grupo no existe.");
        }

        _grupos[indice] = grupo;
    }

    public void Eliminar(Grupo grupo)
    {
        _grupos.Remove(grupo);
    }

    public List<Grupo> Buscar(Func<Grupo, bool> predicado)
    {
        return _grupos.Where(predicado).ToList();
    }
}
~~~

### RepositorioPartidoEnMemoria.cs

Ruta: WorldCupPlanner.AccesoADatos\Repositorios\RepositorioPartidoEnMemoria.cs

~~~csharp
using WorldCupPlanner.AccesoADatos.Interfaces;
using  WorldCupPlanner.Dominio.Entidades;


namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioPartidoEnMemoria : IRepositorio<Partido>
{
    private readonly List<Partido> _partidos = new();

    public void Agregar(Partido partido)
    {
        _partidos.Add(partido);
    }

    public void Actualizar(Partido partido)
    {
        var indice = _partidos.FindIndex(p => ReferenceEquals(p, partido));

        if (indice == -1)
        {
            throw new InvalidOperationException("El partido no existe.");
        }

        _partidos[indice] = partido;
    }

    public void Eliminar(Partido partido)
    {
        _partidos.Remove(partido);
    }

    public List<Partido> Buscar(Func<Partido, bool> predicado)
    {
        return _partidos.Where(predicado).ToList();
    }
}
~~~

### RepositorioUsuarioEnMemoria.cs

Ruta: WorldCupPlanner.AccesoADatos\Repositorios\RepositorioUsuarioEnMemoria.cs

~~~csharp
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioUsuarioEnMemoria : IRepositorio<Usuario>
{
    private readonly List<Usuario> _usuarios = new();

    public void Agregar(Usuario usuario)
    {
        _usuarios.Add(usuario);
    }

    public void Actualizar(Usuario usuario)
    {
        var indice = _usuarios.FindIndex(u => ReferenceEquals(u, usuario));

        if (indice == -1)
        {
            throw new InvalidOperationException("El usuario no existe.");
        }

        _usuarios[indice] = usuario;
    }

    public void Eliminar(Usuario usuario)
    {
        _usuarios.Remove(usuario);
    }

    public List<Usuario> Buscar(Func<Usuario, bool> predicado)
    {
        return _usuarios.Where(predicado).ToList();
    }

    public Usuario? ObtenerPorEmail(string email)
    {
        return _usuarios.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }
}

~~~

### WorldCupPlanner.AccesoADatos.csproj

Ruta: WorldCupPlanner.AccesoADatos\WorldCupPlanner.AccesoADatos.csproj

~~~xml
<Project Sdk="Microsoft.NET.Sdk">

  <ItemGroup>
    <ProjectReference Include="..\WorldCupPlanner.Dominio\WorldCupPlanner.Dominio.csproj" />
  </ItemGroup>

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
~~~

End of WorldCupPlanner.AccesoADatos

## WorldCupPlanner.DominioTests

### EquipoTests.cs

Ruta: WorldCupPlanner.DominioTests\EquipoTests.cs

~~~csharp
using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.DominioTests;

[TestClass]
public class EquipoTests
{
    [TestMethod]
    public void Equipo_ConNombreVacio_NoEsValido()
    {
        // Arrange
        var equipo = new Equipo();
        equipo.Nombre = "";

        // Act
        var resultado = equipo.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Equipo_ConNombreMasDe60Caracteres_NoEsValido()
    {
        //Arrange
        var equipo = new Equipo();
        equipo.Nombre = new string('A', 61);
        
        //Act
        var resultado = equipo.EsValido();
        
        //Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Equipo_ConRankingFIFAMenorA300_NoEsValido()
    {
        //Arrange
        var equipo = new Equipo();
        equipo.Nombre = "Barcelona";
        equipo.RankingFIFA = 299;
        
        //Act
        var resultado = equipo.EsValido();
        
        //Assert
        Assert.IsFalse(resultado);
    }
    
    [TestMethod]
    public void Equipo_ConRankingFIFAMayorA2500_NoEsValido()
    {
        //Arrange
        var equipo = new Equipo();
        equipo.Nombre = "Barcelona";
        equipo.RankingFIFA = 2501;
        
        //Act
        var resultado = equipo.EsValido();
        
        //Assert
        Assert.IsFalse(resultado);
    }
    
    [TestMethod]
    public void Equipo_SinConfederacion_NoEsValido()
    {
        // Arrange
        var equipo = new Equipo();
        equipo.Nombre = "Barcelona";
        equipo.RankingFIFA = 500;
        equipo.Confederacion = null;

        // Act
        var resultado = equipo.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }
}
~~~

### EstadioTests.cs

Ruta: WorldCupPlanner.DominioTests\EstadioTests.cs

~~~csharp
using  WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.DominioTests;

[TestClass]
public class EstadioTests
{
    [TestMethod]
    public void Estadio_ConNombreVacio_NoEsValido()
    {
        // Arrange
        var estadio = new Estadio();
        estadio.Nombre = "";

        // Act
        var resultado = estadio.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }
    
    [TestMethod]
    public void Estadio_NombreExcede80Caracteres_EsInvalido()
    {
        // Arrange
        var estadio = new Estadio
        {
            Nombre = new string('A', 81),
            Ciudad = "Montevideo",
            Capacidad = 20000
        };
        // Act
        var resultado = estadio.EsValido();
        // Assert
        Assert.IsFalse(resultado);
    }
    
    [TestMethod]
    public void Estadio_CiudadVacia_EsInvalido()
    {
        // Arrange
        var estadio = new Estadio
        {
            Nombre = "Estadio Centenario",
            Ciudad = "",
            Capacidad = 20000
        };
        // Act
        var resultado = estadio.EsValido();
        // Assert
        Assert.IsFalse(resultado);
    }
    [TestMethod]
    public void Estadio_CiudadExcede60Caracteres_EsInvalido()
    {
        // Arrange
        var estadio = new Estadio
        {
            Nombre = "Estadio Centenaario",
            Ciudad = new string('A', 61),
            Capacidad = 20000
        };
        // Act
        var resultado = estadio.EsValido();
        // Assert
        Assert.IsFalse(resultado);
    }
    
    [TestMethod]
    public void Estadio_CapacidadMenorA20000_EsInvalido()
    {
        // Arrange
        var estadio = new Estadio
        {
            Nombre = "Estadio Centenario",
            Ciudad = "Montevideo",
            Capacidad = 19999
        };
        // Act
        var resultado = estadio.EsValido();
        // Assert
        Assert.IsFalse(resultado);
    }
    
    [TestMethod]
    public void Estadio_DescripcionExcede400Caracteres_EsInvalido()
    {
        // Arrange
        var estadio = new Estadio
        {
            Nombre = "Estadio Centenario",
            Ciudad = "Montevideo",
            Capacidad = 20000,
            Descripcion = new string('A', 401)
        };
        // Act
        var resultado = estadio.EsValido();
        // Assert
        Assert.IsFalse(resultado);
    }


    
}
~~~

### FixtureTests.cs

Ruta: WorldCupPlanner.DominioTests\FixtureTests.cs

~~~csharp
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.DominioTests;

[TestClass]
public class FixtureTests
{
    [TestMethod]
    public void Fixture_DebeExponerTodosLosPartidosDeSusGrupos()
    {
        // Arrange
        var grupoA = new Grupo("A");
        var grupoB = new Grupo("B");

        grupoA.Partidos.Add(CrearPartido(1));
        grupoA.Partidos.Add(CrearPartido(2));
        grupoB.Partidos.Add(CrearPartido(3));

        var fixture = new Fixture();
        fixture.Grupos.Add(grupoA);
        fixture.Grupos.Add(grupoB);

        // Act
        var partidos = fixture.Partidos;

        // Assert
        Assert.AreEqual(3, partidos.Count);

        var idsObtenidos = partidos.Select(p => p.Id).ToList();
        var idsEsperados = new List<int> { 1, 2, 3 };

        CollectionAssert.AreEqual(idsEsperados, idsObtenidos);
    }

    private Partido CrearPartido(int id)
    {
        return new Partido
        {
            Id = id,
            Fecha = DateTime.Now,
            Estadio = new Estadio
            {
                Nombre = $"Estadio{id}",
                Ciudad = $"Ciudad{id}",
                Capacidad = 30000
            },
            EquipoLocal = new Equipo
            {
                Nombre = $"Local{id}",
                RankingFIFA = 1000,
                Confederacion = Confederacion.UEFA
            },
            EquipoVisitante = new Equipo
            {
                Nombre = $"Visitante{id}",
                RankingFIFA = 900,
                Confederacion = Confederacion.CONMEBOL
            },
            Fase = FaseTorneo.FaseDeGrupos
        };
    }
}
~~~

### GrupoTests.cs

Ruta: WorldCupPlanner.DominioTests\GrupoTests.cs

~~~csharp
using  WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.DominioTests;

[TestClass]
public class GrupoTests
{
    [TestMethod]
    public void Grupo_ConEtiquetaVacia_DebeLanzarExcepcion()
    {
        // Arrange
        string etiquetaVacia = string.Empty;

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            new Grupo(etiquetaVacia)
        );
    }

    [TestMethod]
    public void Grupo_ConEtiquetaDeMasDeUnCaracter_DebeLanzarExcepcion()
    {
        // Arrange
        string etiquetaInvalida = "AB";

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            new Grupo(etiquetaInvalida)
        );
    }

    [TestMethod]
    public void Grupo_ConEtiquetaFueraDeRango_DebeLanzarExcepcion()
    {
        // Arrange
        string etiquetaFueraDeRango = "M";

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            new Grupo(etiquetaFueraDeRango)
        );
    }



    [TestMethod]
    public void Grupo_ConMasDeCuatroEquipos_DebeLanzarExcepcion()
    {
        // Arrange
        var equipos = new List<Equipo>
        {
            CrearEquipoValido("Equipo1", Confederacion.UEFA),
            CrearEquipoValido("Equipo2", Confederacion.UEFA),
            CrearEquipoValido("Equipo3", Confederacion.CONMEBOL),
            CrearEquipoValido("Equipo4", Confederacion.AFC),
            CrearEquipoValido("Equipo5", Confederacion.CAF)
        };

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            new Grupo("A", equipos)
        );
    }



    [TestMethod]
    public void Grupo_ConEtiquetaValida_YCuatroEquiposValidos_EsValido()
    {
        // Arrange
        var grupo = CrearGrupoValido("B");

        // Act
        var resultado = grupo.EsValido();

        // Assert
        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void Grupo_ConTresEquiposUEFA_NoEsValido()
    {
        // Arrange
        var equipos = new List<Equipo>
        {
            CrearEquipoValido("Equipo1", Confederacion.UEFA),
            CrearEquipoValido("Equipo2", Confederacion.UEFA),
            CrearEquipoValido("Equipo3", Confederacion.UEFA),
            CrearEquipoValido("Equipo4", Confederacion.CONMEBOL)
        };
        var grupo = new Grupo("C", equipos);

        // Act
        var resultado = grupo.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Grupo_ConDosEquiposConmebol_NoEsValido()
    {
        // Arrange
        var equipos = new List<Equipo>
        {
            CrearEquipoValido("Equipo1", Confederacion.UEFA),
            CrearEquipoValido("Equipo2", Confederacion.CONMEBOL),
            CrearEquipoValido("Equipo3", Confederacion.CONMEBOL),
            CrearEquipoValido("Equipo4", Confederacion.AFC)
        };
        var grupo = new Grupo("D", equipos);

        // Act
        var resultado = grupo.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Grupo_ConDosEquiposConcacaf_NoEsValido()
    {
        // Arrange
        var equipos = new List<Equipo>
        {
            CrearEquipoValido("Equipo1", Confederacion.UEFA),
            CrearEquipoValido("Equipo2", Confederacion.CONCACAF),
            CrearEquipoValido("Equipo3", Confederacion.CONCACAF),
            CrearEquipoValido("Equipo4", Confederacion.AFC)
        };
        var grupo = new Grupo("E", equipos);

        // Act
        var resultado = grupo.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Grupo_ConDosEquiposCAF_NoEsValido()
    {
        // Arrange
        var equipos = new List<Equipo>
        {
            CrearEquipoValido("Equipo1", Confederacion.UEFA),
            CrearEquipoValido("Equipo2", Confederacion.CAF),
            CrearEquipoValido("Equipo3", Confederacion.CAF),
            CrearEquipoValido("Equipo4", Confederacion.AFC)
        };
        var grupo = new Grupo("F", equipos);

        // Act
        var resultado = grupo.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Grupo_ConDosEquiposAFC_NoEsValido()
    {
        // Arrange
        var equipos = new List<Equipo>
        {
            CrearEquipoValido("Equipo1", Confederacion.UEFA),
            CrearEquipoValido("Equipo2", Confederacion.AFC),
            CrearEquipoValido("Equipo3", Confederacion.AFC),
            CrearEquipoValido("Equipo4", Confederacion.CONMEBOL)
        };
        var grupo = new Grupo("G", equipos);

        // Act
        var resultado = grupo.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Grupo_ConDosEquiposOFC_NoEsValido()
    {
        // Arrange
        var equipos = new List<Equipo>
        {
            CrearEquipoValido("Equipo1", Confederacion.UEFA),
            CrearEquipoValido("Equipo2", Confederacion.OFC),
            CrearEquipoValido("Equipo3", Confederacion.OFC),
            CrearEquipoValido("Equipo4", Confederacion.CONMEBOL)
        };
        var grupo = new Grupo("H", equipos);

        // Act
        var resultado = grupo.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Grupo_ConDosUEFAYUnoDeOtraConfederacion_EsValido()
    {
        // Arrange
        var equipos = new List<Equipo>
        {
            CrearEquipoValido("Equipo1", Confederacion.UEFA),
            CrearEquipoValido("Equipo2", Confederacion.UEFA),
            CrearEquipoValido("Equipo3", Confederacion.CONMEBOL),
            CrearEquipoValido("Equipo4", Confederacion.AFC)
        };
        var grupo = new Grupo("I", equipos);

        // Act
        var resultado = grupo.EsValido();

        // Assert
        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void Grupo_ConEquipoInvalido_NoEsValido()
    {
        // Arrange
        var equipos = new List<Equipo>
        {
            new Equipo { Nombre = "", RankingFIFA = 500, Confederacion = Confederacion.UEFA }
        };
        var grupo = new Grupo("A", equipos);

        // Act
        var resultado = grupo.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }
    
    [TestMethod]
    public void Grupo_ConMenosDeCuatroEquipos_NoEsValido()
    {
        // Arrange
        var equipos = new List<Equipo>
        {
            CrearEquipoValido("Equipo1", Confederacion.UEFA),
            CrearEquipoValido("Equipo2", Confederacion.CONMEBOL),
            CrearEquipoValido("Equipo3", Confederacion.AFC)
        };
        var grupo = new Grupo("J", equipos);

        // Act
        var resultado = grupo.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }



    private Grupo CrearGrupoValido(string etiqueta = "A")
    {
        var equipos = new List<Equipo>
        {
            CrearEquipoValido("Equipo1", Confederacion.UEFA),
            CrearEquipoValido("Equipo2", Confederacion.UEFA),
            CrearEquipoValido("Equipo3", Confederacion.CONMEBOL),
            CrearEquipoValido("Equipo4", Confederacion.AFC)
        };

        return new Grupo(etiqueta, equipos);
    }

    private Equipo CrearEquipoValido(string nombre, Confederacion confederacion)
    {
        return new Equipo { Nombre = nombre, RankingFIFA = 500, Confederacion = confederacion };
    }

}
~~~

### PartidoTests.cs

Ruta: WorldCupPlanner.DominioTests\PartidoTests.cs

~~~csharp
using  WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.DominioTests;

[TestClass]
public class PartidoTests
{
    [TestMethod]
    public void Partido_EquipoLocalIgualAVisitante_EsInvalido()
    {
        // Arrange
        var equipo = new Equipo
        {
            Nombre = "Uruguay",
            RankingFIFA = 1000,
            Confederacion = Confederacion.CONMEBOL
        };
        var partido = new Partido
        {
            Fecha = DateTime.Now,
            Estadio = new Estadio { Nombre = "Centenario", Ciudad = "Montevideo", Capacidad = 60000 },
            EquipoLocal = equipo,
            EquipoVisitante = equipo,
            Fase = FaseTorneo.FaseDeGrupos
        };

        // Act
        var resultado = partido.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }
    
    [TestMethod]
    public void Partido_SinEstadio_EsInvalido()
    {
        // Arrange
        var partido = new Partido
        {
            Fecha = DateTime.Now,
            EquipoLocal = new Equipo { Nombre = "Uruguay", RankingFIFA = 1000, Confederacion = Confederacion.CONMEBOL },
            EquipoVisitante = new Equipo { Nombre = "Argentina", RankingFIFA = 800, Confederacion = Confederacion.CONMEBOL },
            Fase = FaseTorneo.FaseDeGrupos
        };

        // Act
        var resultado = partido.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }
    
    [TestMethod]
    public void Partido_SinEquipoLocal_EsInvalido()
    {
        // Arrange
        var partido = new Partido
        {
            Fecha = DateTime.Now,
            Estadio = new Estadio { Nombre = "Centenario", Ciudad = "Montevideo", Capacidad = 60000 },
            EquipoVisitante = new Equipo { Nombre = "Argentina", RankingFIFA = 800, Confederacion = Confederacion.CONMEBOL },
            Fase = FaseTorneo.FaseDeGrupos
        };

        // Act
        var resultado = partido.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Partido_SinEquipoVisitante_EsInvalido()
    {
        // Arrange
        var partido = new Partido
        {
            Fecha = DateTime.Now,
            Estadio = new Estadio { Nombre = "Centenario", Ciudad = "Montevideo", Capacidad = 60000 },
            EquipoLocal = new Equipo { Nombre = "Uruguay", RankingFIFA = 1000, Confederacion = Confederacion.CONMEBOL },
            Fase = FaseTorneo.FaseDeGrupos
        };

        // Act
        var resultado = partido.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }
    [TestMethod]
    public void Partido_ConGoles_EstaJugado()
    {
        // Arrange
        var partido = new Partido
        {
            Fecha = DateTime.Now,
            Estadio = new Estadio { Nombre = "Centenario", Ciudad = "Montevideo", Capacidad = 60000 },
            EquipoLocal = new Equipo { Nombre = "Uruguay", RankingFIFA = 1000, Confederacion = Confederacion.CONMEBOL },
            EquipoVisitante = new Equipo { Nombre = "Argentina", RankingFIFA = 800, Confederacion = Confederacion.CONMEBOL },
            Fase = FaseTorneo.FaseDeGrupos,
            GolesLocal = 2,
            GolesVisitante = 1
        };

        // Act
        var resultado = partido.EstaJugado();

        // Assert
        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void Partido_GolesLocalMayor_GanaLocal()
    {
        // Arrange
        var equipoLocal = new Equipo { Nombre = "Uruguay", RankingFIFA = 1000, Confederacion = Confederacion.CONMEBOL };
        var partido = new Partido
        {
            Fecha = DateTime.Now,
            Estadio = new Estadio { Nombre = "Centenario", Ciudad = "Montevideo", Capacidad = 60000 },
            EquipoLocal = equipoLocal,
            EquipoVisitante = new Equipo { Nombre = "Argentina", RankingFIFA = 800, Confederacion = Confederacion.CONMEBOL },
            Fase = FaseTorneo.FaseDeGrupos,
            GolesLocal = 2,
            GolesVisitante = 1
        };

        // Act
        var resultado = partido.Ganador();

        // Assert
        Assert.AreEqual(equipoLocal, resultado);
    }
    
    [TestMethod]
    public void Partido_GolesIguales_EsEmpate()
    {
        // Arrange
        var partido = new Partido
        {
            Fecha = DateTime.Now,
            Estadio = new Estadio { Nombre = "Centenario", Ciudad = "Montevideo", Capacidad = 60000 },
            EquipoLocal = new Equipo { Nombre = "Uruguay", RankingFIFA = 1000, Confederacion = Confederacion.CONMEBOL },
            EquipoVisitante = new Equipo { Nombre = "Argentina", RankingFIFA = 800, Confederacion = Confederacion.CONMEBOL },
            Fase = FaseTorneo.FaseDeGrupos,
            GolesLocal = 1,
            GolesVisitante = 1
        };

        // Act
        var resultado = partido.EsEmpate();

        // Assert
        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void Partido_GolesVisitanteMayor_GanaVisitante()
    {
        // Arrange
        var equipoVisitante = new Equipo { Nombre = "Argentina", RankingFIFA = 800, Confederacion = Confederacion.CONMEBOL };
        var partido = new Partido
        {
            Fecha = DateTime.Now,
            Estadio = new Estadio { Nombre = "Centenario", Ciudad = "Montevideo", Capacidad = 60000 },
            EquipoLocal = new Equipo { Nombre = "Uruguay", RankingFIFA = 1000, Confederacion = Confederacion.CONMEBOL },
            EquipoVisitante = equipoVisitante,
            Fase = FaseTorneo.FaseDeGrupos,
            GolesLocal = 0,
            GolesVisitante = 2
        };

        // Act
        var resultado = partido.Ganador();

        // Assert
        Assert.AreEqual(equipoVisitante, resultado);
    }
     
    [TestMethod]
    public void Partido_SinGoles_GanadorEsNull()
    {
        // Arrange
        var partido = new Partido
        {
            Fecha = DateTime.Now,
            Estadio = new Estadio { Nombre = "Centenario", Ciudad = "Montevideo", Capacidad = 60000 },
            EquipoLocal = new Equipo { Nombre = "Uruguay", RankingFIFA = 1000, Confederacion = Confederacion.CONMEBOL },
            EquipoVisitante = new Equipo { Nombre = "Argentina", RankingFIFA = 800, Confederacion = Confederacion.CONMEBOL },
            Fase = FaseTorneo.FaseDeGrupos
        };

        // Act
        var resultado = partido.Ganador();

        // Assert
        Assert.IsNull(resultado);
    }
    
}
~~~

### UsuarioTests.cs

Ruta: WorldCupPlanner.DominioTests\UsuarioTests.cs

~~~csharp
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.DominioTests;

[TestClass]
public class UsuarioTests
{
    [TestMethod]
    public void CrearUsuario_ConDatosValidos_DebeCrearExitosamente()
    {
        // Arrange
        var nombre = "Juan";
        var apellido = "PÃ©rez";
        var email = "juan.perez@example.com";
        var fechaNacimiento = "05-15-1990";

        // Act
        var usuario = new Usuario(new DatosUsuario
        {
            Nombre = nombre,
            Apellido = apellido,
            Email = email,
            FechaNacimiento = fechaNacimiento
        });

        // Assert
        Assert.AreEqual(nombre, usuario.Nombre);
        Assert.AreEqual(apellido, usuario.Apellido);
        Assert.AreEqual(email, usuario.Email);
        Assert.AreEqual(new DateTime(1990, 5, 15), usuario.FechaNacimiento);
    }

    [TestMethod]
    public void CrearUsuario_ConNombreVacio_DebeLanzarExcepcion()
    {
        // Arrange
        var apellido = "PÃ©rez";
        var email = "juan.perez@example.com";
        var fechaNacimiento = "05-15-1990";

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            new Usuario(new DatosUsuario { Nombre = string.Empty, Apellido = apellido, Email = email, FechaNacimiento = fechaNacimiento })
        );
    }

    [TestMethod]
    public void CrearUsuario_ConApellidoVacio_DebeLanzarExcepcion()
    {
        // Arrange
        var nombre = "Juan";
        var email = "juan.perez@example.com";
        var fechaNacimiento = "05-15-1990";

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            new Usuario(new DatosUsuario { Nombre = nombre, Apellido = string.Empty, Email = email, FechaNacimiento = fechaNacimiento })
        );
    }

    [TestMethod]
    public void CrearUsuario_ConEmailInvalido_DebeLanzarExcepcion()
    {
        // Arrange
        var nombre = "Juan";
        var apellido = "PÃ©rez";
        var email = "email-invalido";
        var fechaNacimiento = "05-15-1990";

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            new Usuario(new DatosUsuario { Nombre = nombre, Apellido = apellido, Email = email, FechaNacimiento = fechaNacimiento })
        );
    }

    [TestMethod]
    public void CrearUsuario_ConEmailVacio_DebeLanzarExcepcion()
    {
        // Arrange
        var nombre = "Juan";
        var apellido = "PÃ©rez";
        var email = string.Empty;
        var fechaNacimiento = "05-15-1990";

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            new Usuario(new DatosUsuario { Nombre = nombre, Apellido = apellido, Email = email, FechaNacimiento = fechaNacimiento })
        );
    }

    [TestMethod]
    public void CrearUsuario_ConFechaNacimientoFutura_DebeLanzarExcepcion()
    {
        // Arrange
        var nombre = "Juan";
        var apellido = "PÃ©rez";
        var email = "juan.perez@example.com";
        var fechaNacimiento = DateTime.Now.AddDays(1).ToString("MM-dd-yyyy");

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            new Usuario(new DatosUsuario { Nombre = nombre, Apellido = apellido, Email = email, FechaNacimiento = fechaNacimiento })
        );
    }

    [TestMethod]
    public void CrearUsuario_ConFormatoFechaInvalido_DebeLanzarExcepcion()
    {
        // Arrange
        var nombre = "Juan";
        var apellido = "PÃ©rez";
        var email = "juan.perez@example.com";

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            new Usuario(new DatosUsuario { Nombre = nombre, Apellido = apellido, Email = email, FechaNacimiento = "1990-05-15" })
        );
        Assert.ThrowsException<ArgumentException>(() =>
            new Usuario(new DatosUsuario { Nombre = nombre, Apellido = apellido, Email = email, FechaNacimiento = "15/05/1990" })
        );
        Assert.ThrowsException<ArgumentException>(() =>
            new Usuario(new DatosUsuario { Nombre = nombre, Apellido = apellido, Email = email, FechaNacimiento = "05/15/1990" })
        );
    }



    [TestMethod]
    public void EstablecerContrasenia_ConContrasesiaValida_DebeEstablecerseExitosamente()
    {
        // Arrange
        var usuario = CrearUsuarioValido();
        var contrasenia = "MyPassword123!";

        // Act
        usuario.EstablecerContrasenia(contrasenia);

        // Assert
        Assert.IsTrue(usuario.VerificarContrasenia(contrasenia));
    }

    [TestMethod]
    public void EstablecerContrasenia_ConMenosDe8Caracteres_DebeLanzarExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido();
        var contrasenia = "Pass12!"; // 7 caracteres

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            usuario.EstablecerContrasenia(contrasenia)
        );
    }

    [TestMethod]
    public void EstablecerContrasenia_SinMayÃºsculas_DebeLanzarExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido();
        var contrasenia = "password123!"; // Sin mayÃºsculas

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            usuario.EstablecerContrasenia(contrasenia)
        );
    }

    [TestMethod]
    public void EstablecerContrasenia_SinMinÃºsculas_DebeLanzarExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido();
        var contrasenia = "PASSWORD123!"; // Sin minÃºsculas

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            usuario.EstablecerContrasenia(contrasenia)
        );
    }

    [TestMethod]
    public void EstablecerContrasenia_SinNÃºmeros_DebeLanzarExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido();
        var contrasenia = "PasswordAbc!"; // Sin nÃºmeros

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            usuario.EstablecerContrasenia(contrasenia)
        );
    }

    [TestMethod]
    public void EstablecerContrasenia_SinCaracteresEspeciales_DebeLanzarExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido();
        var contrasenia = "Password1234"; // Sin caracteres especiales

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            usuario.EstablecerContrasenia(contrasenia)
        );
    }

    [TestMethod]
    public void EstablecerContrasenia_ConContrasesiaVacia_DebeLanzarExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido();

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            usuario.EstablecerContrasenia(string.Empty)
        );
    }

    [TestMethod]
    public void EstablecerContrasenia_DebeAlmacenarCifrada_NoEnTextoPlano()
    {
        // Arrange
        var usuario = CrearUsuarioValido();
        var contrasenia = "MyPassword123!";

        // Act
        usuario.EstablecerContrasenia(contrasenia);

        // Assert
        Assert.IsFalse(usuario.ContraseniaCifrada == contrasenia,
            "La contrasenia no debe almacenarse en texto plano");
        Assert.IsNotNull(usuario.ContraseniaCifrada);
        Assert.IsTrue(usuario.ContraseniaCifrada.Length > 0);
    }

    [TestMethod]
    public void VerificarContrasenia_ConContrasesiaCorrecta_DebeRetornarVerdadero()
    {
        // Arrange
        var usuario = CrearUsuarioValido();
        var contrasenia = "MyPassword123!";
        usuario.EstablecerContrasenia(contrasenia);

        // Act
        var resultado = usuario.VerificarContrasenia(contrasenia);

        // Assert
        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void VerificarContrasenia_ConContrasesiaIncorrecta_DebeRetornarFalso()
    {
        // Arrange
        var usuario = CrearUsuarioValido();
        var contrasenia = "MyPassword123!";
        usuario.EstablecerContrasenia(contrasenia);

        // Act
        var resultado = usuario.VerificarContrasenia("WrongPassword123!");

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void VerificarContrasenia_ConContrasesiaCasoDiferente_DebeRetornarFalso()
    {
        // Arrange
        var usuario = CrearUsuarioValido();
        var contrasenia = "MyPassword123!";
        usuario.EstablecerContrasenia(contrasenia);

        // Act - Intenta con caso diferente
        var resultado = usuario.VerificarContrasenia("mypassword123!");

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void EstablecerContrasenia_ConCaracteresEspecialesValidos_DebeAceptarlos()
    {
        // Arrange
        var usuario = CrearUsuarioValido();
        var contrasenias = new[]
        {
            "Password123@",
            "Password123#",
            "Password123$",
            "Password123."
        };

        // Act & Assert
        foreach (var contrasenia in contrasenias)
        {
            usuario.EstablecerContrasenia(contrasenia);
            Assert.IsTrue(usuario.VerificarContrasenia(contrasenia),
                $"DeberÃ­a aceptar contrasenia con carÃ¡cter especial: {contrasenia}");
        }
    }



    [TestMethod]
    public void AgregarRol_ConRolAdministrador_DebeAgregarse()
    {
        // Arrange
        var usuario = CrearUsuarioValido();

        // Act
        usuario.AgregarRol(RolUsuario.AdministradorDelSistema);

        // Assert
        Assert.IsTrue(usuario.TieneRol(RolUsuario.AdministradorDelSistema));
    }

    [TestMethod]
    public void AgregarRol_ConRolEditor_DebeAgregarse()
    {
        // Arrange
        var usuario = CrearUsuarioValido();

        // Act
        usuario.AgregarRol(RolUsuario.Editor);

        // Assert
        Assert.IsTrue(usuario.TieneRol(RolUsuario.Editor));
    }

    [TestMethod]
    public void AgregarRol_ConRolesMultiples_DebePermitirRolesNoExcluyentes()
    {
        // Arrange
        var usuario = CrearUsuarioValido();

        // Act
        usuario.AgregarRol(RolUsuario.AdministradorDelSistema);
        usuario.AgregarRol(RolUsuario.Editor);

        // Assert
        Assert.IsTrue(usuario.TieneRol(RolUsuario.AdministradorDelSistema));
        Assert.IsTrue(usuario.TieneRol(RolUsuario.Editor));
    }

    [TestMethod]
    public void AgregarRol_MismoRolDosVeces_NoDebeAgregarseDuplicado()
    {
        // Arrange
        var usuario = CrearUsuarioValido();
        usuario.AgregarRol(RolUsuario.AdministradorDelSistema);

        // Act
        var rolesAntes = usuario.ObtenerRoles().Count();
        usuario.AgregarRol(RolUsuario.AdministradorDelSistema);
        var rolesDepues = usuario.ObtenerRoles().Count();

        // Assert
        Assert.AreEqual(rolesAntes, rolesDepues);
    }

    [TestMethod]
    public void RemoverRol_ConRolExistente_DebeRemoverse()
    {
        // Arrange
        var usuario = CrearUsuarioValido();
        usuario.AgregarRol(RolUsuario.AdministradorDelSistema);

        // Act
        usuario.RemoverRol(RolUsuario.AdministradorDelSistema);

        // Assert
        Assert.IsFalse(usuario.TieneRol(RolUsuario.AdministradorDelSistema));
    }

    [TestMethod]
    public void RemoverRol_NoAfectaOtrosRoles()
    {
        // Arrange
        var usuario = CrearUsuarioValido();
        usuario.AgregarRol(RolUsuario.AdministradorDelSistema);
        usuario.AgregarRol(RolUsuario.Editor);

        // Act
        usuario.RemoverRol(RolUsuario.AdministradorDelSistema);

        // Assert
        Assert.IsFalse(usuario.TieneRol(RolUsuario.AdministradorDelSistema));
        Assert.IsTrue(usuario.TieneRol(RolUsuario.Editor));
    }

    [TestMethod]
    public void ObtenerRoles_ConRolesAsignados_DebeRetornarTodos()
    {
        // Arrange
        var usuario = CrearUsuarioValido();
        usuario.AgregarRol(RolUsuario.AdministradorDelSistema);
        usuario.AgregarRol(RolUsuario.Editor);

        // Act
        var roles = usuario.ObtenerRoles();

        // Assert
        Assert.AreEqual(2, roles.Count());
        Assert.IsTrue(roles.Contains(RolUsuario.AdministradorDelSistema));
        Assert.IsTrue(roles.Contains(RolUsuario.Editor));
    }

    [TestMethod]
    public void TieneRol_UsuarioSinRoles_DebeRetornarFalso()
    {
        // Arrange
        var usuario = CrearUsuarioValido();

        // Act & Assert
        Assert.IsFalse(usuario.TieneRol(RolUsuario.AdministradorDelSistema));
        Assert.IsFalse(usuario.TieneRol(RolUsuario.Editor));
    }

    [TestMethod]
    public void ActualizarNombre_ConNombreValido_ActualizaElNombre()
    {
        // Arrange
        var usuario = CrearUsuarioValido();

        // Act
        usuario.ActualizarNombre("Carlos");

        // Assert
        Assert.AreEqual("Carlos", usuario.Nombre);
    }

    [TestMethod]
    public void ActualizarNombre_ConNombreVacio_LanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido();

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => usuario.ActualizarNombre(string.Empty));
    }

    [TestMethod]
    public void ActualizarApellido_ConApellidoValido_ActualizaElApellido()
    {
        // Arrange
        var usuario = CrearUsuarioValido();

        // Act
        usuario.ActualizarApellido("GarcÃ­a");

        // Assert
        Assert.AreEqual("GarcÃ­a", usuario.Apellido);
    }

    [TestMethod]
    public void ActualizarEmail_ConEmailValido_ActualizaElEmail()
    {
        // Arrange
        var usuario = CrearUsuarioValido();

        // Act
        usuario.ActualizarEmail("newemail@example.com");

        // Assert
        Assert.AreEqual("newemail@example.com", usuario.Email);
    }

    [TestMethod]
    public void ActualizarEmail_ConEmailInvalido_LanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido();

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => usuario.ActualizarEmail("email_invalido"));
    }

    [TestMethod]
    public void ActualizarFechaNacimiento_ConFechaValida_ActualizaLaFecha()
    {
        // Arrange
        var usuario = CrearUsuarioValido();

        // Act
        usuario.ActualizarFechaNacimiento("05-15-1995");

        // Assert
        Assert.AreEqual(new DateTime(1995, 5, 15), usuario.FechaNacimiento);
    }

    [TestMethod]
    public void ActualizarFechaNacimiento_ConFechaFutura_LanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido();
        var fechaFutura = DateTime.Now.AddYears(1).ToString("MM-dd-yyyy");

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => usuario.ActualizarFechaNacimiento(fechaFutura));
    }

    [TestMethod]
    public void ActualizarFechaNacimiento_ConFormatoInvalido_LanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido();

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => usuario.ActualizarFechaNacimiento("1995-05-15"));
        Assert.ThrowsException<ArgumentException>(() => usuario.ActualizarFechaNacimiento("15/05/1995"));
    }


    private static Usuario CrearUsuarioValido()
    {
        return new Usuario(new DatosUsuario
        {
            Nombre = "Juan",
            Apellido = "PÃ©rez",
            Email = "juan.perez@example.com",
            FechaNacimiento = "05-15-1990"
        });
    }
}
~~~

### WorldCupPlanner.DominioTests.csproj

Ruta: WorldCupPlanner.DominioTests\WorldCupPlanner.DominioTests.csproj

~~~xml
<Project Sdk="Microsoft.NET.Sdk">

    <PropertyGroup>
        <TargetFramework>net8.0</TargetFramework>
        <ImplicitUsings>enable</ImplicitUsings>
        <Nullable>enable</Nullable>
        <IsPackable>false</IsPackable>
        <IsTestProject>true</IsTestProject>
    </PropertyGroup>

    <ItemGroup>
        <PackageReference Include="coverlet.collector" Version="6.0.0" />
        <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
        <PackageReference Include="MSTest.TestAdapter" Version="3.1.1" />
        <PackageReference Include="MSTest.TestFramework" Version="3.1.1" />
    </ItemGroup>

    <ItemGroup>
        <Using Include="Microsoft.VisualStudio.TestTools.UnitTesting" />
    </ItemGroup>

    <ItemGroup>
        <ProjectReference Include="..\WorldCupPlanner.Dominio\WorldCupPlanner.Dominio.csproj" />
    </ItemGroup>

</Project>
~~~

End of WorldCupPlanner.DominioTests

## WorldCupPlanner.AccesoADatosTest

### RepositoriosEnMemoriaTests.cs

Ruta: WorldCupPlanner.AccesoADatosTest\RepositoriosEnMemoriaTests.cs

~~~csharp
using WorldCupPlanner.AccesoADatos.Repositorios;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.AccesoADatosTest;

[TestClass]
public class RepositoriosEnMemoriaTests
{
    [TestMethod]
    public void RepositorioEquipoEnMemoria_AgregarEquipo_GuardaEquipo()
    {
        var repositorio = new RepositorioEquipoEnMemoria();
        var equipo = CrearEquipoValido("Uruguay");

        repositorio.Agregar(equipo);

        Assert.AreEqual(1, repositorio.Buscar(_ => true).Count);
        Assert.AreEqual(equipo, repositorio.Buscar(_ => true).First());
    }

    [TestMethod]
    public void RepositorioEquipoEnMemoria_EliminarEquipo_EliminaEquipo()
    {
        var repositorio = new RepositorioEquipoEnMemoria();
        var equipo = CrearEquipoValido("Uruguay");
        repositorio.Agregar(equipo);

        repositorio.Eliminar(equipo);

        Assert.AreEqual(0, repositorio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void RepositorioEstadioEnMemoria_ActualizarEstadio_ModificaEstadio()
    {
        var repositorio = new RepositorioEstadioEnMemoria();
        var estadio = CrearEstadioValido("Estadio Centenario");
        repositorio.Agregar(estadio);
        estadio.Nombre = "Estadio Olimpico";

        repositorio.Actualizar(estadio);

        Assert.AreEqual("Estadio Olimpico", repositorio.Buscar(_ => true).First().Nombre);
    }

    [TestMethod]
    public void RepositorioGrupoEnMemoria_AgregarGrupo_GuardaGrupo()
    {
        var repositorio = new RepositorioGrupoEnMemoria();
        var grupo = new Grupo("A");

        repositorio.Agregar(grupo);

        Assert.AreEqual(1, repositorio.Buscar(_ => true).Count);
        Assert.AreEqual(grupo, repositorio.Buscar(_ => true).First());
    }

    [TestMethod]
    public void RepositorioPartidoEnMemoria_AgregarPartido_GuardaPartido()
    {
        var repositorio = new RepositorioPartidoEnMemoria();
        var partido = CrearPartidoValido();

        repositorio.Agregar(partido);

        Assert.AreEqual(1, repositorio.Buscar(_ => true).Count);
        Assert.AreEqual(partido, repositorio.Buscar(_ => true).First());
    }

    [TestMethod]
    public void RepositorioUsuarioEnMemoria_AgregarUsuario_GuardaUsuario()
    {
        var repositorio = new RepositorioUsuarioEnMemoria();
        var usuario = CrearUsuarioValido("juan@example.com");

        repositorio.Agregar(usuario);

        Assert.AreEqual(1, repositorio.Buscar(_ => true).Count);
        Assert.AreSame(usuario, repositorio.Buscar(_ => true).First());
    }

    [TestMethod]
    public void RepositorioUsuarioEnMemoria_AgregarVariosUsuarios_GuardaTodos()
    {
        var repositorio = new RepositorioUsuarioEnMemoria();
        var usuario1 = CrearUsuarioValido("juan@example.com");
        var usuario2 = CrearUsuarioValido("maria@example.com");
        var usuario3 = CrearUsuarioValido("pedro@example.com");

        repositorio.Agregar(usuario1);
        repositorio.Agregar(usuario2);
        repositorio.Agregar(usuario3);

        Assert.AreEqual(3, repositorio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void RepositorioUsuarioEnMemoria_ActualizarUsuarioExistente_ModificaUsuario()
    {
        var repositorio = new RepositorioUsuarioEnMemoria();
        var usuario = CrearUsuarioValido("juan@example.com");
        repositorio.Agregar(usuario);
        usuario.AgregarRol(RolUsuario.AdministradorDelSistema);

        repositorio.Actualizar(usuario);

        Assert.IsTrue(repositorio.Buscar(_ => true).First().TieneRol(RolUsuario.AdministradorDelSistema));
    }

    [TestMethod]
    public void RepositorioUsuarioEnMemoria_ActualizarUsuarioNoExistente_LanzaExcepcion()
    {
        var repositorio = new RepositorioUsuarioEnMemoria();
        var usuario = CrearUsuarioValido("juan@example.com");

        Assert.ThrowsException<InvalidOperationException>(() => repositorio.Actualizar(usuario));
    }

    [TestMethod]
    public void RepositorioUsuarioEnMemoria_EliminarUsuarioExistente_EliminaUsuario()
    {
        var repositorio = new RepositorioUsuarioEnMemoria();
        var usuario = CrearUsuarioValido("juan@example.com");
        repositorio.Agregar(usuario);

        repositorio.Eliminar(usuario);

        Assert.AreEqual(0, repositorio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void RepositorioUsuarioEnMemoria_EliminarUsuarioNoExistente_NoLanzaExcepcion()
    {
        var repositorio = new RepositorioUsuarioEnMemoria();
        var usuario = CrearUsuarioValido("juan@example.com");

        repositorio.Eliminar(usuario);

        Assert.AreEqual(0, repositorio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void RepositorioUsuarioEnMemoria_BuscarTodosSinUsuarios_DevuelveListaVacia()
    {
        var repositorio = new RepositorioUsuarioEnMemoria();

        var usuarios = repositorio.Buscar(_ => true);

        Assert.AreEqual(0, usuarios.Count);
    }

    [TestMethod]
    public void RepositorioUsuarioEnMemoria_BuscarTodos_DevuelveCopia()
    {
        var repositorio = new RepositorioUsuarioEnMemoria();
        var usuario = CrearUsuarioValido("juan@example.com");
        repositorio.Agregar(usuario);

        var usuarios1 = repositorio.Buscar(_ => true);
        var usuarios2 = repositorio.Buscar(_ => true);

        Assert.AreNotSame(usuarios1, usuarios2);
        Assert.AreEqual(usuarios1.Count, usuarios2.Count);
    }

    [TestMethod]
    public void RepositorioUsuarioEnMemoria_ObtenerPorEmailExistente_DevuelveUsuario()
    {
        var repositorio = new RepositorioUsuarioEnMemoria();
        var usuario = CrearUsuarioValido("juan@example.com");
        repositorio.Agregar(usuario);

        var usuarioEncontrado = repositorio.ObtenerPorEmail("juan@example.com");

        Assert.IsNotNull(usuarioEncontrado);
        Assert.AreSame(usuario, usuarioEncontrado);
    }

    [TestMethod]
    public void RepositorioUsuarioEnMemoria_ObtenerPorEmailNoExistente_DevuelveNull()
    {
        var repositorio = new RepositorioUsuarioEnMemoria();

        var usuarioEncontrado = repositorio.ObtenerPorEmail("noexiste@example.com");

        Assert.IsNull(usuarioEncontrado);
    }

    [TestMethod]
    public void RepositorioUsuarioEnMemoria_ObtenerPorEmailNoDistingueMayusculas_DevuelveUsuario()
    {
        var repositorio = new RepositorioUsuarioEnMemoria();
        var usuario = CrearUsuarioValido("Juan@Example.com");
        repositorio.Agregar(usuario);

        var usuarioEncontrado = repositorio.ObtenerPorEmail("juan@example.com");

        Assert.IsNotNull(usuarioEncontrado);
        Assert.AreSame(usuario, usuarioEncontrado);
    }

    private Equipo CrearEquipoValido(string nombre)
    {
        return new Equipo
        {
            Nombre = nombre,
            RankingFIFA = 1000,
            Confederacion = Confederacion.CONMEBOL
        };
    }

    private Estadio CrearEstadioValido(string nombre)
    {
        return new Estadio
        {
            Nombre = nombre,
            Ciudad = "Montevideo",
            Capacidad = 60000
        };
    }

    private Partido CrearPartidoValido()
    {
        return new Partido
        {
            Id = 1,
            Fecha = DateTime.Now,
            Estadio = CrearEstadioValido("Estadio Centenario"),
            EquipoLocal = CrearEquipoValido("Uruguay"),
            EquipoVisitante = CrearEquipoValido("Argentina"),
            Fase = FaseTorneo.FaseDeGrupos,
            Grupo = new Grupo("A")
        };
    }

    private static Usuario CrearUsuarioValido(string email)
    {
        return new Usuario(new DatosUsuario
        {
            Nombre = "Juan",
            Apellido = "PÃ©rez",
            Email = email,
            FechaNacimiento = "01-01-1990"
        });
    }
}
~~~

### WorldCupPlanner.AccesoADatosTest.csproj

Ruta: WorldCupPlanner.AccesoADatosTest\WorldCupPlanner.AccesoADatosTest.csproj

~~~xml
<Project Sdk="Microsoft.NET.Sdk">

    <PropertyGroup>
        <TargetFramework>net8.0</TargetFramework>
        <ImplicitUsings>enable</ImplicitUsings>
        <Nullable>enable</Nullable>
        <IsPackable>false</IsPackable>
        <IsTestProject>true</IsTestProject>
    </PropertyGroup>

    <ItemGroup>
        <PackageReference Include="coverlet.collector" Version="6.0.0" />
        <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
        <PackageReference Include="MSTest.TestAdapter" Version="3.1.1" />
        <PackageReference Include="MSTest.TestFramework" Version="3.1.1" />
    </ItemGroup>

    <ItemGroup>
        <Using Include="Microsoft.VisualStudio.TestTools.UnitTesting" />
    </ItemGroup>

    <ItemGroup>
        <ProjectReference Include="..\WorldCupPlanner.Dominio\WorldCupPlanner.Dominio.csproj" />
        <ProjectReference Include="..\WorldCupPlanner.AccesoADatos\WorldCupPlanner.AccesoADatos.csproj" />
    </ItemGroup>

</Project>
~~~

End of WorldCupPlanner.AccesoADatosTest

