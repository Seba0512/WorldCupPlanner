WorldCupPlanner.AccesoADatos:

Interfaces\IRepositorioEquipo.cs:
```csharp
using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.AccesoADatos.Interfaces;

public interface IRepositorioEquipo
{
    void Agregar(Equipo equipo);
    void Actualizar(Equipo equipo);
    void Eliminar(Equipo equipo);
    List<Equipo> ObtenerTodos();
}
```

Repositorios\RepositorioEquipoEnMemoria.cs:
```csharp
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioEquipoEnMemoria : IRepositorioEquipo
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

    public List<Equipo> ObtenerTodos()
    {
        return _equipos;
    }
}
```

Repositorios\RepositorioEstadioEnMemoria.cs:
```csharp
using  WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioEstadioEnMemoria
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

    public List<Estadio> ObtenerTodos()
    {
        return _estadios;
    }
}
```

Repositorios\RepositorioGrupoEnMemoria.cs:
```csharp
using  WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioGrupoEnMemoria
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

    public List<Grupo> ObtenerTodos()
    {
        return _grupos;
    }
}
```

Repositorios\RepositorioPartidoEnMemoria.cs:
```csharp
using  WorldCupPlanner.Dominio.Entidades;


namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioPartidoEnMemoria
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

    public List<Partido> ObtenerTodos()
    {
        return _partidos;
    }
}
```

End of WorldCupPlanner.AccesoADatos

WorldCupPlanner.Dominio:

Entidades\Auditoria.cs:
```csharp
namespace WorldCupPlanner.Dominio.Entidades;

public class Auditoria
{
}
```

Entidades\ClasificadosFaseGrupos.cs:
```csharp
using System.Linq;

namespace WorldCupPlanner.Dominio.Entidades;

public class ClasificadosFaseGrupos
{
    public List<Equipo> Primeros { get; set; } = new List<Equipo>();
    public List<Equipo> Segundos { get; set; } = new List<Equipo>();
    public List<Equipo> MejoresTerceros { get; set; } = new List<Equipo>();

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
}
```

Entidades\ConfiguracionTorneo.cs:
```csharp
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

```

Entidades\Equipo.cs:
```csharp
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
		return !string.IsNullOrWhiteSpace(Nombre) && Nombre.Length <= 60 && RankingFIFA > 299 && RankingFIFA <2501 && Confederacion.HasValue;

	}

        public Equipo()
		{
			
		}
	}
}

```

Entidades\Estadio.cs:
```csharp
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
```

Entidades\Fixture.cs:
```csharp
using System.Linq;

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
```

Entidades\Grupo.cs:
```csharp
using System.Linq;
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
```

Entidades\Partido.cs:
```csharp
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
```

Entidades\PosicionEquipo.cs:
```csharp
namespace WorldCupPlanner.Dominio.Entidades;

public class PosicionEquipo
{
    public Equipo Equipo { get; set; } = null!;
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
```

Entidades\Usuario.cs:
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Dominio.Entidades;

public class Usuario
{
    private string? _contraseniaCifrada;
    private HashSet<RolUsuario> _roles;

    public string Nombre { get; private set; }
    public string Apellido { get; private set; }
    public string Email { get; private set; }
    public DateTime FechaNacimiento { get; private set; }
    public string? ContraseniaCifrada => _contraseniaCifrada;

    public Usuario(string nombre, string apellido, string email, DateTime fechaNacimiento)
    {
        ValidarNombre(nombre);
        ValidarApellido(apellido);
        ValidarEmail(email);
        ValidarFechaNacimiento(fechaNacimiento);

        Nombre = nombre;
        Apellido = apellido;
        Email = email;
        FechaNacimiento = fechaNacimiento;
        _roles = new HashSet<RolUsuario>();
    }

    #region Validaciones

    private void ValidarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("El nombre no puede estar vacío.", nameof(nombre));
        }
    }

    private void ValidarApellido(string apellido)
    {
        if (string.IsNullOrWhiteSpace(apellido))
        {
            throw new ArgumentException("El apellido no puede estar vacío.", nameof(apellido));
        }
    }

    private void ValidarEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("El email no puede estar vacío.", nameof(email));
        }

        // Validar formato de email básico
        var patronEmail = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";
        if (!Regex.IsMatch(email, patronEmail))
        {
            throw new ArgumentException("El email no es válido.", nameof(email));
        }
    }

    private void ValidarFechaNacimiento(DateTime fechaNacimiento)
    {
        if (fechaNacimiento > DateTime.Now)
        {
            throw new ArgumentException("La fecha de nacimiento no puede ser futura.", nameof(fechaNacimiento));
        }
    }

    #endregion

    #region Gestión de Contrasenia

    public void EstablecerContrasenia(string contrasenia)
    {
        ValidarContrasenia(contrasenia);
        _contraseniaCifrada = CifrarContrasenia(contrasenia);
    }

    private void ValidarContrasenia(string contrasenia)
    {
        if (string.IsNullOrWhiteSpace(contrasenia))
        {
            throw new ArgumentException("La contrasenia no puede estar vacía.", nameof(contrasenia));
        }

        if (contrasenia.Length < 8)
        {
            throw new ArgumentException("La contrasenia debe tener al menos 8 caracteres.", nameof(contrasenia));
        }

        if (!Regex.IsMatch(contrasenia, @"[A-Z]"))
        {
            throw new ArgumentException("La contrasenia debe incluir al menos una letra mayúscula.", nameof(contrasenia));
        }

        if (!Regex.IsMatch(contrasenia, @"[a-z]"))
        {
            throw new ArgumentException("La contrasenia debe incluir al menos una letra minúscula.", nameof(contrasenia));
        }

        if (!Regex.IsMatch(contrasenia, @"[0-9]"))
        {
            throw new ArgumentException("La contrasenia debe incluir al menos un número.", nameof(contrasenia));
        }

        if (!Regex.IsMatch(contrasenia, @"[@#$\.\!\&\*\-_\+=\?]"))
        {
            throw new ArgumentException("La contrasenia debe incluir al menos un carácter especial (@, #, $, ., !, &, *, -, _, +, =, ?).", nameof(contrasenia));
        }
    }

    private string CifrarContrasenia(string contrasenia)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(contrasenia));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    public bool VerificarContrasenia(string contrasenia)
    {
        if (string.IsNullOrEmpty(_contraseniaCifrada))
        {
            return false;
        }

        var contraseniaCifradaIngresada = CifrarContrasenia(contrasenia);
        return _contraseniaCifrada == contraseniaCifradaIngresada;
    }

    #endregion

    #region Gestión de Roles

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

    #endregion
}
```

Enumeraciones\Confederacion.cs:
```csharp
namespace WorldCupPlanner.Dominio.Enumeraciones;

public enum Confederacion
{
    UEFA, CONMEBOL, CONCACAF, CAF, AFC, OFC
}
```

Enumeraciones\FaseTorneo.cs:
```csharp
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
```

Enumeraciones\RolUsuario.cs:
```csharp
namespace WorldCupPlanner.Dominio.Enumeraciones;

public enum RolUsuario
{
    AdministradorDelSistema,
    Editor
}

```

End of WorldCupPlanner.Dominio

WorldCupPlanner.DominioTest:

EquipoTests.cs:
```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorldCupPlanner.Dominio;
using  WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

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
    public void Equipo_SinConferacion_NoEsValido()
    {
        // Arrange
        var equipo = new Equipo();
        equipo.Nombre = "Barcelona";
        equipo.RankingFIFA = 500;
        equipo.Confederacion = Confederacion.UEFA;

        // Act
        var resultado = equipo.EsValido();

        // Assert
        Assert.IsTrue(resultado);
    }
}
```

EstadioTests.cs:
```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
```

FixtureTests.cs:
```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorldCupPlanner.Dominio;
using  WorldCupPlanner.Dominio.Entidades;
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
```

GrupoTests.cs:
```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using  WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.DominioTests;

[TestClass]
public class GrupoTests
{
    #region Constructor - Validación de Etiqueta

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

    #endregion

    #region Constructor - Validación de Equipos

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

    #endregion

    #region Validación - Equipos Válidos y Confederaciones

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

    #endregion

    #region Métodos Helper

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

    #endregion
}
```

PartidoTests.cs:
```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
```

End of WorldCupPlanner.DominioTest

WorldCupPlanner.Servicios:

Normalizar.cs:
```csharp
using System;
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
```

ServicioClasificacion.cs:
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using  WorldCupPlanner.Dominio.Entidades;

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

    private Dictionary<Equipo, PosicionEquipo> CrearPosicionesIniciales(Grupo grupo)
    {
        var posiciones = new Dictionary<Equipo, PosicionEquipo>();

        foreach (var equipo in grupo.Equipos)
        {
            posiciones[equipo] = new PosicionEquipo
            {
                Equipo = equipo
            };
        }

        return posiciones;
    }

    private void ActualizarPosicionesConPartido(Dictionary<Equipo, PosicionEquipo> posiciones, Partido partido)
    {
        var posicionLocal = posiciones[partido.EquipoLocal];
        var posicionVisitante = posiciones[partido.EquipoVisitante];

        int golesLocal = partido.GolesLocal!.Value;
        int golesVisitante = partido.GolesVisitante!.Value;

        posicionLocal.GolesAFavor += golesLocal;
        posicionLocal.GolesEnContra += golesVisitante;

        posicionVisitante.GolesAFavor += golesVisitante;
        posicionVisitante.GolesEnContra += golesLocal;

        ActualizarPuntos(posicionLocal, posicionVisitante, golesLocal, golesVisitante);
    }

    private void ActualizarPuntos(
        PosicionEquipo posicionLocal,
        PosicionEquipo posicionVisitante,
        int golesLocal,
        int golesVisitante)
    {
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

    private List<PosicionEquipo> AplicarDesempateDeterministico(
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
                posiciones = MezclarConFisherYates(posiciones, random);
            }

            tablaFinal.AddRange(posiciones);
        }

        return tablaFinal;
    }

    private List<PosicionEquipo> MezclarConFisherYates(List<PosicionEquipo> posiciones, Random random)
    {
        for (int i = posiciones.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (posiciones[i], posiciones[j]) = (posiciones[j], posiciones[i]);
        }

        return posiciones;
    }
    
    public ClasificadosFaseGrupos ObtenerClasificadosAFaseEliminatoria(
        Fixture fixture,
        ConfiguracionTorneo configuracionTorneo)
    {
        var primeros = new List<PosicionEquipo>();
        var segundos = new List<PosicionEquipo>();
        var terceros = new List<PosicionEquipo>();

        foreach (var grupo in fixture.Grupos)
        {
            var tabla = CalcularTabla(grupo, configuracionTorneo);

            primeros.Add(tabla[0]);
            segundos.Add(tabla[1]);
            terceros.Add(tabla[2]);
        }

        return new ClasificadosFaseGrupos
        {
            Primeros = primeros.Select(p => p.Equipo).ToList(),
            Segundos = segundos.Select(p => p.Equipo).ToList(),
            MejoresTerceros = OrdenarPosicionesConDesempateDeterministico(
                    terceros,
                    configuracionTorneo.SemillaCrucesFase
                )
                .Take(8)
                .Select(p => p.Equipo)
                .ToList()
        };
    }
    
    private List<PosicionEquipo> OrdenarPosicionesConDesempateDeterministico(
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
```

ServicioCruces.cs:
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
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
            estadios,
            configuracionTorneo
        );

        var segundosRestantes = MezclarEquiposConFisherYates(
            clasificados.Segundos.Take(8).ToList(),
            random);

        AgregarCrucesSegundosRestantesMinimos(
            partidos,
            segundosRestantes,
            estadios,
            configuracionTorneo
        );

        return partidos;
    }

    private void AgregarCrucesPrimerosContraTerceros(
        List<Partido> partidos,
        List<Equipo> primeros,
        List<Equipo> terceros,
        List<Estadio> estadios,
        ConfiguracionTorneo configuracionTorneo)
    {
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
        List<Estadio> estadios,
        ConfiguracionTorneo configuracionTorneo)
    {
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
        List<Estadio> estadios,
        ConfiguracionTorneo configuracionTorneo)
    {
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
}
```

ServicioEquipo.cs:
```csharp
using WorldCupPlanner.AccesoADatos.Repositorios;
using  WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.Servicios
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
```

ServicioEstadio.cs:
```csharp
using System;
using System.Linq;
using WorldCupPlanner.AccesoADatos.Repositorios;
using  WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.Servicios;

public class ServicioEstadio
{
    private readonly RepositorioEstadioEnMemoria _repositorioEstadio;

    public ServicioEstadio()
    {
        _repositorioEstadio = new RepositorioEstadioEnMemoria();
    }

    public ServicioEstadio(RepositorioEstadioEnMemoria repositorioEstadio)
    {
        _repositorioEstadio = repositorioEstadio;
    }

    public void AgregarEstadio(Estadio estadio)
    {
        if (!estadio.EsValido())
            throw new Exception("El estadio no es valido.");
        if (_repositorioEstadio.ObtenerTodos().Any(e => e.Nombre == estadio.Nombre))
            throw new Exception("Ya existe un estadio con ese nombre.");
        _repositorioEstadio.Agregar(estadio);
    }

    public void EditarEstadio(Estadio estadio)
    {
        var existente = _repositorioEstadio.ObtenerTodos().FirstOrDefault(e => e == estadio);
        if (existente == null)
            throw new Exception("El estadio no existe.");
        _repositorioEstadio.Actualizar(estadio);
    }

    public void EliminarEstadio(Estadio estadio)
    {
        _repositorioEstadio.Eliminar(estadio);
    }

    public List<Estadio> ObtenerEstadios()
    {
        return _repositorioEstadio.ObtenerTodos();
    }
}
```

ServicioFixture.cs:
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
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
```

End of WorldCupPlanner.Servicios

WorldCupPlanner.ServicioTests:

ConfiguracionTorneoTests.cs:
```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
```

NormalizarTest.cs:
```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
}
```

RepositoriosEnMemoriaTests.cs:
```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using WorldCupPlanner.AccesoADatos.Repositorios;
using  WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public class RepositoriosEnMemoriaTests
{
    [TestMethod]
    public void RepositorioEquipoEnMemoria_AgregarEquipo_GuardaEquipo()
    {
        var repositorio = new RepositorioEquipoEnMemoria();
        var equipo = CrearEquipoValido("Uruguay");

        repositorio.Agregar(equipo);

        Assert.AreEqual(1, repositorio.ObtenerTodos().Count);
        Assert.AreEqual(equipo, repositorio.ObtenerTodos().First());
    }

    [TestMethod]
    public void RepositorioEquipoEnMemoria_EliminarEquipo_EliminaEquipo()
    {
        var repositorio = new RepositorioEquipoEnMemoria();
        var equipo = CrearEquipoValido("Uruguay");
        repositorio.Agregar(equipo);

        repositorio.Eliminar(equipo);

        Assert.AreEqual(0, repositorio.ObtenerTodos().Count);
    }

    [TestMethod]
    public void RepositorioEstadioEnMemoria_ActualizarEstadio_ModificaEstadio()
    {
        var repositorio = new RepositorioEstadioEnMemoria();
        var estadio = CrearEstadioValido("Estadio Centenario");
        repositorio.Agregar(estadio);
        estadio.Nombre = "Estadio Olimpico";

        repositorio.Actualizar(estadio);

        Assert.AreEqual("Estadio Olimpico", repositorio.ObtenerTodos().First().Nombre);
    }

    [TestMethod]
    public void RepositorioGrupoEnMemoria_AgregarGrupo_GuardaGrupo()
    {
        var repositorio = new RepositorioGrupoEnMemoria();
        var grupo = new Grupo("A");

        repositorio.Agregar(grupo);

        Assert.AreEqual(1, repositorio.ObtenerTodos().Count);
        Assert.AreEqual(grupo, repositorio.ObtenerTodos().First());
    }

    [TestMethod]
    public void RepositorioPartidoEnMemoria_AgregarPartido_GuardaPartido()
    {
        var repositorio = new RepositorioPartidoEnMemoria();
        var partido = CrearPartidoValido();

        repositorio.Agregar(partido);

        Assert.AreEqual(1, repositorio.ObtenerTodos().Count);
        Assert.AreEqual(partido, repositorio.ObtenerTodos().First());
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
}
```

ServicioClasificacionTests.cs:
```csharp
using  WorldCupPlanner.Dominio.Entidades;
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


}
```

ServicioCrucesTests.cs:
```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
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

    private List<string> ObtenerCruces(List<Partido> partidos)
    {
        return partidos
            .Select(p => $"{p.EquipoLocal.Nombre}-{p.EquipoVisitante.Nombre}")
            .ToList();
    }
}
```

ServicioEquipoTests.cs:
```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using  WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
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
        Assert.ThrowsException<Exception>(() => _servicio.AgregarEquipo(equipo2));
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
        Assert.ThrowsException<Exception>(() => _servicio.AgregarEquipo(equipo));
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
        Assert.ThrowsException<Exception>(() => _servicio.EditarEquipo(equipo));
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
        Assert.ThrowsException<Exception>(() => _servicio.EditarEquipo(equipo2));
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
```

ServicioEstadioTests.cs:
```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using  WorldCupPlanner.Dominio.Entidades;
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
        Assert.ThrowsException<Exception>(() => _servicio.AgregarEstadio(estadio2));
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
        Assert.ThrowsException<Exception>(() => _servicio.AgregarEstadio(estadio));
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
        estadio.Nombre = "Estadio Olimpico";
        _servicio.EditarEstadio(estadio);

        // Assert
        Assert.AreEqual("Estadio Olimpico", _servicio.ObtenerEstadios().First().Nombre);
    }
}
```

ServicioFixtureTests.cs:
```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using  WorldCupPlanner.Dominio.Entidades;
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
```

UsuarioTests.cs:
```csharp
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.ServicioTests;


[TestClass]
public class UsuarioTests
{
    #region Información del Usuario - Tests Básicos
    
    [TestMethod]
    public void CrearUsuario_ConDatosValidos_DebeCrearExitosamente()
    {
        // Arrange
        var nombre = "Juan";
        var apellido = "Pérez";
        var email = "juan.perez@example.com";
        var fechaNacimiento = new DateTime(1990, 5, 15);

        // Act
        var usuario = new Usuario(nombre, apellido, email, fechaNacimiento);

        // Assert
        Assert.AreEqual(nombre, usuario.Nombre);
        Assert.AreEqual(apellido, usuario.Apellido);
        Assert.AreEqual(email, usuario.Email);
        Assert.AreEqual(fechaNacimiento, usuario.FechaNacimiento);
    }

    [TestMethod]
    public void CrearUsuario_ConNombreVacio_DebeLanzarExcepcion()
    {
        // Arrange
        var apellido = "Pérez";
        var email = "juan.perez@example.com";
        var fechaNacimiento = new DateTime(1990, 5, 15);

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            new Usuario(string.Empty, apellido, email, fechaNacimiento)
        );
    }

    [TestMethod]
    public void CrearUsuario_ConApellidoVacio_DebeLanzarExcepcion()
    {
        // Arrange
        var nombre = "Juan";
        var email = "juan.perez@example.com";
        var fechaNacimiento = new DateTime(1990, 5, 15);

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            new Usuario(nombre, string.Empty, email, fechaNacimiento)
        );
    }

    [TestMethod]
    public void CrearUsuario_ConEmailInvalido_DebeLanzarExcepcion()
    {
        // Arrange
        var nombre = "Juan";
        var apellido = "Pérez";
        var email = "email-invalido";
        var fechaNacimiento = new DateTime(1990, 5, 15);

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            new Usuario(nombre, apellido, email, fechaNacimiento)
        );
    }

    [TestMethod]
    public void CrearUsuario_ConEmailVacio_DebeLanzarExcepcion()
    {
        // Arrange
        var nombre = "Juan";
        var apellido = "Pérez";
        var email = string.Empty;
        var fechaNacimiento = new DateTime(1990, 5, 15);

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            new Usuario(nombre, apellido, email, fechaNacimiento)
        );
    }

    [TestMethod]
    public void CrearUsuario_ConFechaNacimientoFutura_DebeLanzarExcepcion()
    {
        // Arrange
        var nombre = "Juan";
        var apellido = "Pérez";
        var email = "juan.perez@example.com";
        var fechaNacimiento = DateTime.Now.AddDays(1);

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            new Usuario(nombre, apellido, email, fechaNacimiento)
        );
    }

    #endregion

    #region Contrasenia - Tests de Validación

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
    public void EstablecerContrasenia_SinMayúsculas_DebeLanzarExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido();
        var contrasenia = "password123!"; // Sin mayúsculas

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            usuario.EstablecerContrasenia(contrasenia)
        );
    }

    [TestMethod]
    public void EstablecerContrasenia_SinMinúsculas_DebeLanzarExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido();
        var contrasenia = "PASSWORD123!"; // Sin minúsculas

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            usuario.EstablecerContrasenia(contrasenia)
        );
    }

    [TestMethod]
    public void EstablecerContrasenia_SinNúmeros_DebeLanzarExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido();
        var contrasenia = "PasswordAbc!"; // Sin números

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
                $"Debería aceptar contrasenia con carácter especial: {contrasenia}");
        }
    }

    #endregion

    #region Roles - Tests de Gestión de Roles

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

    #endregion

    #region Métodos Helper

    private Usuario CrearUsuarioValido()
    {
        return new Usuario(
            "Juan",
            "Pérez",
            "juan.perez@example.com",
            new DateTime(1990, 5, 15)
        );
    }

    #endregion
}
```

End of WorldCupPlanner.ServicioTests

WorldCupPlanner.UI:

_Imports.razor:
```razor
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
```

App.razor:
```razor
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
```

Data\WeatherForecast.cs:
```csharp
namespace WorldCupPlanner.UI.Data;

public class WeatherForecast
{
    public DateOnly Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary { get; set; }
}
```

Data\WeatherForecastService.cs:
```csharp
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
```

Pages\_Host.cshtml:
```cshtml
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
```

Pages\Counter.razor:
```razor
@page "/counter"

<PageTitle>Counter</PageTitle>

<h1>Counter</h1>

<p role="status">Current count: @currentCount</p>

<button class="btn btn-primary" @onclick="IncrementCount">Click me</button>

@code {
    private int currentCount = 0;

    private void IncrementCount()
    {
        currentCount++;
    }
}
```

Pages\Error.cshtml:
```cshtml
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
```

Pages\Error.cshtml.cs:
```csharp
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
```

Pages\FetchData.razor:
```razor
@page "/fetchdata"
@using WorldCupPlanner.UI.Data
@inject WeatherForecastService ForecastService

<PageTitle>Weather forecast</PageTitle>

<h1>Weather forecast</h1>

<p>This component demonstrates fetching data from a service.</p>

@if (forecasts == null)
{
    <p><em>Loading...</em></p>
}
else
{
    <table class="table">
        <thead>
            <tr>
                <th>Date</th>
                <th>Temp. (C)</th>
                <th>Temp. (F)</th>
                <th>Summary</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var forecast in forecasts)
            {
                <tr>
                    <td>@forecast.Date.ToShortDateString()</td>
                    <td>@forecast.TemperatureC</td>
                    <td>@forecast.TemperatureF</td>
                    <td>@forecast.Summary</td>
                </tr>
            }
        </tbody>
    </table>
}

@code {
    private WeatherForecast[]? forecasts;

    protected override async Task OnInitializedAsync()
    {
        forecasts = await ForecastService.GetForecastAsync(DateOnly.FromDateTime(DateTime.Now));
    }
}
```

Pages\Index.razor:
```razor
@page "/"

<PageTitle>Index</PageTitle>

<h1>Hello, world!</h1>

Welcome to your new app.

<SurveyPrompt Title="How is Blazor working for you?" />
```

Program.cs:
```csharp
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using WorldCupPlanner.UI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

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
```

Shared\MainLayout.razor:
```razor
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
```

Shared\NavMenu.razor:
```razor
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
            <NavLink class="nav-link" href="counter">
                <span class="oi oi-plus" aria-hidden="true"></span> Counter
            </NavLink>
        </div>
        <div class="nav-item px-3">
            <NavLink class="nav-link" href="fetchdata">
                <span class="oi oi-list-rich" aria-hidden="true"></span> Fetch data
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
```

Shared\SurveyPrompt.razor:
```razor
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
```

End of WorldCupPlanner.UI

