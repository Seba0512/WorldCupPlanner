# Codigo completo por proyecto

Generado desde los proyectos incluidos actualmente en WorldCupPlanner.sln.

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
        var resultados = new List<Auditoria>();

        foreach (var log in _logs)
        {
            if (predicado(log))
            {
                resultados.Add(log);
            }
        }

        return resultados;
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
        var resultados = new List<Equipo>();

        foreach (var equipo in _equipos)
        {
            if (predicado(equipo))
            {
                resultados.Add(equipo);
            }
        }

        return resultados;
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
        var resultados = new List<Estadio>();

        foreach (var estadio in _estadios)
        {
            if (predicado(estadio))
            {
                resultados.Add(estadio);
            }
        }

        return resultados;
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
        var resultados = new List<Grupo>();

        foreach (var grupo in _grupos)
        {
            if (predicado(grupo))
            {
                resultados.Add(grupo);
            }
        }

        return resultados;
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
        var resultados = new List<Partido>();

        foreach (var partido in _partidos)
        {
            if (predicado(partido))
            {
                resultados.Add(partido);
            }
        }

        return resultados;
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
        var resultados = new List<Usuario>();

        foreach (var usuario in _usuarios)
        {
            if (predicado(usuario))
            {
                resultados.Add(usuario);
            }
        }

        return resultados;
    }

    public Usuario? ObtenerPorEmail(string email)
    {
        return _usuarios.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }
}

~~~

End of WorldCupPlanner.AccesoADatos

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

End of WorldCupPlanner.AccesoADatosTest

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
            var todos = new List<Equipo>();
            todos.AddRange(Primeros);
            todos.AddRange(Segundos);
            todos.AddRange(MejoresTerceros);

            return todos;
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
            var partidos = new List<Partido>();

            foreach (var grupo in Grupos)
            {
                partidos.AddRange(grupo.Partidos);
            }

            return partidos;
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
    TercerPuesto,
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

End of WorldCupPlanner.Dominio

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

End of WorldCupPlanner.DominioTests

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
using WorldCupPlanner.Dominio.Excepciones;

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
            new List<PosicionEquipo>(posiciones.Values),
            configuracionTorneo.SemillaCrucesFase
        );
    }

    public ClasificadosFaseGrupos ObtenerClasificadosAFaseEliminatoria(
        Fixture fixture,
        ConfiguracionTorneo configuracionTorneo)
    {
        ValidarFaseDeGruposCompleta(fixture);

        var tablas = new List<List<PosicionEquipo>>(fixture.Grupos
            .Select(g => CalcularTabla(g, configuracionTorneo)));

        var primerosOrdenados = OrdenarPosicionesConDesempateDeterministico(
            new List<PosicionEquipo>(tablas.Select(t => t[0])),
            configuracionTorneo.SemillaCrucesFase
        );

        var segundosOrdenados = OrdenarPosicionesConDesempateDeterministico(
            new List<PosicionEquipo>(tablas.Select(t => t[1])),
            configuracionTorneo.SemillaCrucesFase
        );

        var mejoresTerceros = new List<PosicionEquipo>(OrdenarPosicionesConDesempateDeterministico(
                new List<PosicionEquipo>(tablas.Select(t => t[2])),
                configuracionTorneo.SemillaCrucesFase
            )
            .Take(8));

        var clasificados = new ClasificadosFaseGrupos
        {
            Primeros = new List<Equipo>(primerosOrdenados.Select(p => p.Equipo)),
            Segundos = new List<Equipo>(segundosOrdenados.Select(p => p.Equipo)),
            MejoresTerceros = new List<Equipo>(mejoresTerceros.Select(p => p.Equipo))
        };

        foreach (var posicion in primerosOrdenados.Concat(segundosOrdenados).Concat(mejoresTerceros))
        {
            clasificados.GruposDeOrigen[posicion.Equipo] = posicion.Grupo;
        }

        return clasificados;
    }

    private static void ValidarFaseDeGruposCompleta(Fixture fixture)
    {
        if (fixture.Grupos.Any(g => g.Partidos.Any(p => !p.EstaJugado())))
        {
            throw new EntidadInvalidaExcepcion(
                "No se pueden obtener clasificados si hay partidos de fase de grupos sin resultado.");
        }
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
            var posiciones = new List<PosicionEquipo>(grupoEmpatado);

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
        var posicionesOrdenadas = new List<PosicionEquipo>(posiciones
            .OrderByDescending(p => p.Puntos)
            .ThenByDescending(p => p.DiferenciaGoles)
            .ThenByDescending(p => p.GolesAFavor));

        return AplicarDesempateDeterministico(posicionesOrdenadas, semillaCrucesFase);
    }
}
~~~

### ServicioCruces.cs

Ruta: WorldCupPlanner.Servicios\ServicioCruces.cs

~~~csharp
using  WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.Servicios;

public class ServicioCruces
{
    private const int PrimerIdPrimeraRondaEliminatoria = 73;

    public List<Partido> GenerarPrimeraRondaEliminatoria(
        ClasificadosFaseGrupos clasificados,
        List<Estadio> estadios,
        ConfiguracionTorneo configuracionTorneo)
    {
        var partidos = new List<Partido>();
        var random = new Random(configuracionTorneo.SemillaCrucesFase);

        var primerosParaTerceros = MezclarEquiposConFisherYates(
            new List<Equipo>(clasificados.Primeros.Take(8)),
            random);

        var terceros = MezclarEquiposConFisherYates(
            new List<Equipo>(clasificados.MejoresTerceros),
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
            new List<Equipo>(clasificados.Primeros.Skip(8).Take(4)),
            random);

        var segundosDeMenorRendimiento = MezclarEquiposConFisherYates(
            new List<Equipo>(clasificados.Segundos.Skip(8).Take(4)),
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
            new List<Equipo>(clasificados.Segundos.Take(8)),
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
                PrimerIdPrimeraRondaEliminatoria + partidos.Count,
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
                PrimerIdPrimeraRondaEliminatoria + partidos.Count,
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
                PrimerIdPrimeraRondaEliminatoria + partidos.Count,
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
        ValidarPartidosDeFaseAnterior(
            partidosPrimeraRonda,
            16,
            FaseTorneo.PrimeraRondaEliminatoria,
            "octavos",
            "primera ronda",
            "primera ronda eliminatoria");

        var ganadores = ObtenerGanadores(partidosPrimeraRonda);
        var partidos = new List<Partido>();
        var proximoId = partidosPrimeraRonda.Max(p => p.Id) + 1;
        var fecha = configuracionTorneo.FechaInicioTorneo.AddDays(configuracionTorneo.SeparacionEntreFechas);

        for (int i = 0; i < 8; i++)
        {
            partidos.Add(CrearPartidoEliminatorio(
                proximoId++,
                ganadores[i],
                ganadores[i + 8],
                estadios[partidos.Count % estadios.Count],
                fecha,
                FaseTorneo.Octavos));
        }

        return partidos;
    }

    public List<Partido> GenerarCuartos(
        List<Partido> octavos,
        List<Estadio> estadios,
        ConfiguracionTorneo configuracionTorneo)
    {
        ValidarPartidosDeFaseAnterior(
            octavos,
            8,
            FaseTorneo.Octavos,
            "cuartos",
            "octavos");

        var ganadores = ObtenerGanadores(octavos);
        var partidos = new List<Partido>();
        var proximoId = octavos.Max(p => p.Id) + 1;
        var fecha = configuracionTorneo.FechaInicioTorneo.AddDays(configuracionTorneo.SeparacionEntreFechas * 2);

        for (int i = 0; i < 4; i++)
        {
            partidos.Add(CrearPartidoEliminatorio(
                proximoId++,
                ganadores[i * 2],
                ganadores[i * 2 + 1],
                estadios[partidos.Count % estadios.Count],
                fecha,
                FaseTorneo.Cuartos));
        }

        return partidos;
    }

    public List<Partido> GenerarSemifinales(
        List<Partido> cuartos,
        List<Estadio> estadios,
        ConfiguracionTorneo configuracionTorneo)
    {
        ValidarPartidosDeFaseAnterior(
            cuartos,
            4,
            FaseTorneo.Cuartos,
            "semifinales",
            "cuartos");

        var ganadores = ObtenerGanadores(cuartos);
        var partidos = new List<Partido>();
        var proximoId = cuartos.Max(p => p.Id) + 1;
        var fecha = configuracionTorneo.FechaInicioTorneo.AddDays(configuracionTorneo.SeparacionEntreFechas * 3);

        for (int i = 0; i < 2; i++)
        {
            partidos.Add(CrearPartidoEliminatorio(
                proximoId++,
                ganadores[i * 2],
                ganadores[i * 2 + 1],
                estadios[partidos.Count % estadios.Count],
                fecha,
                FaseTorneo.Semifinal));
        }

        return partidos;
    }

    private void ValidarPartidosDeFaseAnterior(
        List<Partido> partidos,
        int cantidadEsperada,
        FaseTorneo faseEsperada,
        string faseAGenerar,
        string faseAnterior,
        string? faseAnteriorEsperada = null)
    {
        ValidarCantidadPartidos(partidos, cantidadEsperada, faseAGenerar, faseAnterior);
        ValidarFasePartidos(partidos, faseEsperada, faseAGenerar, faseAnteriorEsperada ?? faseAnterior);
        ValidarPartidosJugados(partidos, faseAGenerar, faseAnterior);
        ValidarPartidosConGanador(partidos, faseAGenerar, faseAnterior);
    }

    private void ValidarCantidadPartidos(
        List<Partido> partidos,
        int cantidadEsperada,
        string faseAGenerar,
        string faseAnterior)
    {
        if (partidos.Count != cantidadEsperada)
        {
            throw new EntidadInvalidaExcepcion(
                $"Para generar {faseAGenerar} deben existir exactamente {cantidadEsperada} partidos de {faseAnterior}.");
        }
    }

    private void ValidarFasePartidos(
        List<Partido> partidos,
        FaseTorneo faseEsperada,
        string faseAGenerar,
        string faseAnterior)
    {
        if (partidos.Any(p => p.Fase != faseEsperada))
        {
            throw new EntidadInvalidaExcepcion(
                $"Para generar {faseAGenerar} todos los partidos deben ser de {faseAnterior}.");
        }
    }

    private void ValidarPartidosJugados(
        List<Partido> partidos,
        string faseAGenerar,
        string faseAnterior)
    {
        if (partidos.Any(p => !p.EstaJugado()))
        {
            throw new EntidadInvalidaExcepcion(
                $"No se pueden generar {faseAGenerar} si hay partidos de {faseAnterior} sin jugar.");
        }
    }

    private void ValidarPartidosConGanador(
        List<Partido> partidos,
        string faseAGenerar,
        string faseAnterior)
    {
        if (partidos.Any(p => p.Ganador() == null))
        {
            throw new EntidadInvalidaExcepcion(
                $"No se pueden generar {faseAGenerar} si hay partidos de {faseAnterior} empatados.");
        }
    }

    private List<Equipo> ObtenerGanadores(List<Partido> partidos)
    {
        return new List<Equipo>(partidos
            .Select(p => p.Ganador()!));
    }

    private Partido CrearPartidoEliminatorio(
        int id,
        Equipo equipoLocal,
        Equipo equipoVisitante,
        Estadio estadio,
        DateTime fecha,
        FaseTorneo fase)
    {
        return new Partido
        {
            Id = id,
            EquipoLocal = equipoLocal,
            EquipoVisitante = equipoVisitante,
            Estadio = estadio,
            Fecha = fecha,
            Fase = fase
        };
    }

    public Partido GenerarTercerPuesto(
        List<Partido> semifinales,
        List<Estadio> estadios,
        ConfiguracionTorneo configuracionTorneo)
    {
        ValidarPartidosDeFaseAnterior(
            semifinales,
            2,
            FaseTorneo.Semifinal,
            "tercer puesto",
            "semifinales");

        var perdedores = ObtenerPerdedores(semifinales);

        return CrearPartidoEliminatorio(
            semifinales.Max(p => p.Id) + 1,
            perdedores[0],
            perdedores[1],
            estadios[0],
            configuracionTorneo.FechaInicioTorneo.AddDays(configuracionTorneo.SeparacionEntreFechas * 4),
            FaseTorneo.TercerPuesto);
    }
    
    private List<Equipo> ObtenerPerdedores(List<Partido> partidos)
    {
        return new List<Equipo>(partidos
            .Select(ObtenerPerdedor));
    }

    private Equipo ObtenerPerdedor(Partido partido)
    {
        var ganador = partido.Ganador();

        if (ganador == null)
        {
            throw new EntidadInvalidaExcepcion("No se puede obtener perdedor de un partido sin ganador.");
        }

        if (ReferenceEquals(ganador, partido.EquipoLocal))
        {
            return partido.EquipoVisitante!;
        }

        return partido.EquipoLocal!;
    }
    
    public Partido GenerarFinal(
        List<Partido> semifinales,
        List<Estadio> estadios,
        ConfiguracionTorneo configuracionTorneo)
    {
        ValidarPartidosDeFaseAnterior(
            semifinales,
            2,
            FaseTorneo.Semifinal,
            "la final",
            "semifinales");

        var ganadores = ObtenerGanadores(semifinales);

        return CrearPartidoEliminatorio(
            semifinales.Max(p => p.Id) + 2,
            ganadores[0],
            ganadores[1],
            estadios[0],
            configuracionTorneo.FechaInicioTorneo.AddDays(configuracionTorneo.SeparacionEntreFechas * 5),
            FaseTorneo.Final);
    }
}
~~~

### ServicioEquipo.cs

Ruta: WorldCupPlanner.Servicios\ServicioEquipo.cs

~~~csharp
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

        public void AgregarEquipo(Equipo equipo)
        {
            ValidarEquipo(equipo);
            ValidarNombreUnico(equipo.Nombre);

            var cupo = CuposPorConfederacion[equipo.Confederacion!.Value];
            var inscritos = _repositorio.Buscar(e => e.Confederacion == equipo.Confederacion).Count;
            if (inscritos >= cupo)
                throw new InvalidOperationException(
                    $"La confederaciÃ³n {equipo.Confederacion!.Value} ya alcanzÃ³ su cupo mÃ¡ximo de {cupo} equipos.");

            _repositorio.Agregar(equipo);
        }

        public void EditarEquipo(Equipo equipo, string nombreOriginal)
        {
            ValidarEquipo(equipo);
            ValidarNombreUnico(equipo.Nombre, excluir: nombreOriginal);

            var existente = _repositorio.Buscar(e => e.Nombre == nombreOriginal).FirstOrDefault();
            if (existente == null)
                throw new EntidadInvalidaExcepcion("El equipo no existe.");

            existente.Nombre = equipo.Nombre;
            existente.Confederacion = equipo.Confederacion;
            existente.RankingFIFA = equipo.RankingFIFA;
            _repositorio.Actualizar(existente);
        }

        public void EliminarEquipo(Equipo equipo) => Eliminar(equipo);

        public List<Equipo> ObtenerEquipos() => _repositorio.Buscar(_ => true);

        public void Agregar(Equipo entidad) => AgregarEquipo(entidad);

        public void Actualizar(Equipo entidad)
        {
            if (entidad == null)
                throw new EntidadInvalidaExcepcion("El equipo no puede ser nulo.");
            var existente = _repositorio.Buscar(e => ReferenceEquals(e, entidad)).FirstOrDefault();
            if (existente == null)
                throw new EntidadInexistenteExcepcion("El equipo no existe en el sistema.");
            _repositorio.Actualizar(existente);
        }

        public void Eliminar(Equipo entidad) => _repositorio.Eliminar(entidad);

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
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.Servicios;

public class ServicioFixture
{
    private const int CantidadExactaEquipos = 48;
    private const int CantidadMinimaEstadios = 4;
    private const int CantidadGrupos = 12;
    private const int CantidadEquiposPorGrupo = 4;
    private const int MaximoEquiposUefaPorGrupo = 2;
    private const int MaximoEquiposOtrasConfederacionesPorGrupo = 1;
    private const int CupoUefa = 16;
    private const int CupoConmebol = 7;
    private const int CupoConcacaf = 7;
    private const int CupoCaf = 9;
    private const int CupoAfc = 8;
    private const int CupoOfc = 1;
    private const int RankingMinimoEquipoFicticio = 300;
    private const int RankingMaximoEquipoFicticioExclusivo = 501;

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

    private static readonly Confederacion[] OrdenConfederacionesParaCompletar =
    {
        Confederacion.UEFA,
        Confederacion.CONMEBOL,
        Confederacion.CONCACAF,
        Confederacion.CAF,
        Confederacion.AFC,
        Confederacion.OFC
    };

    public Fixture GenerarFixture(List<Equipo> equipos, List<Estadio> estadios, ConfiguracionTorneo configuracionTorneo)
    {
        var equiposCompletos = CompletarEquipos(equipos, configuracionTorneo);

        ValidarCantidadEquipos(equiposCompletos);
        ValidarCantidadEstadios(estadios);

        return CrearFixtureConDistribucionRoundRobin(equiposCompletos, estadios, configuracionTorneo);
    }

    public List<Equipo> CompletarEquipos(
        List<Equipo> equipos,
        ConfiguracionTorneo configuracionTorneo)
    {
        ValidarEquiposParaCompletar(equipos);

        var equiposCompletos = CopiarEquipos(equipos);
        var random = new Random(configuracionTorneo.SemillaCompletar);

        while (FaltanEquiposParaCompletar(equiposCompletos))
        {
            AgregarEquipoFicticio(equiposCompletos, random);
        }

        return equiposCompletos;
    }

    private void ValidarEquiposParaCompletar(List<Equipo> equipos)
    {
        if (equipos.Count > CantidadExactaEquipos)
        {
            throw new EntidadInvalidaExcepcion("No se puede completar una lista con más de 48 equipos.");
        }

        ValidarTodosLosEquiposSonValidos(equipos);
    }

    private bool FaltanEquiposParaCompletar(List<Equipo> equipos)
    {
        return equipos.Count < CantidadExactaEquipos;
    }

    private void AgregarEquipoFicticio(List<Equipo> equipos, Random random)
    {
        var confederacion = ObtenerConfederacionConCupoDisponible(equipos);
        equipos.Add(CrearEquipoFicticio(equipos, confederacion, random));
    }

    private Equipo CrearEquipoFicticio(
        List<Equipo> equipos,
        Confederacion confederacion,
        Random random)
    {
        return new Equipo
        {
            Nombre = GenerarNombreEquipoFicticio(equipos, confederacion),
            RankingFIFA = GenerarRankingEquipoFicticio(random),
            Confederacion = confederacion
        };
    }

    private int GenerarRankingEquipoFicticio(Random random)
    {
        return random.Next(RankingMinimoEquipoFicticio, RankingMaximoEquipoFicticioExclusivo);
    }

    private string GenerarNombreEquipoFicticio(List<Equipo> equipos, Confederacion confederacion)
    {
        var numero = 1;
        var nombre = CrearNombreEquipoFicticio(confederacion, numero);

        while (ExisteEquipoConNombre(equipos, nombre))
        {
            numero++;
            nombre = CrearNombreEquipoFicticio(confederacion, numero);
        }

        return nombre;
    }

    private string CrearNombreEquipoFicticio(Confederacion confederacion, int numero)
    {
        return $"{confederacion}_{numero:00}";
    }

    private bool ExisteEquipoConNombre(List<Equipo> equipos, string nombre)
    {
        return equipos.Any(e => e.Nombre == nombre);
    }

    private List<Equipo> CopiarEquipos(List<Equipo> equipos)
    {
        var equiposCopiados = new List<Equipo>();

        foreach (var equipo in equipos)
        {
            equiposCopiados.Add(equipo);
        }

        return equiposCopiados;
    }

    private Confederacion ObtenerConfederacionConCupoDisponible(List<Equipo> equipos)
    {
        foreach (var confederacion in OrdenConfederacionesParaCompletar)
        {
            if (CantidadEquiposPorConfederacion(equipos, confederacion) < ObtenerCupoConfederacion(confederacion))
            {
                return confederacion;
            }
        }

        throw new EntidadInvalidaExcepcion("No hay cupos disponibles para completar equipos ficticios.");
    }

    private int CantidadEquiposPorConfederacion(List<Equipo> equipos, Confederacion confederacion)
    {
        return equipos.Count(e => e.Confederacion == confederacion);
    }

    private int ObtenerCupoConfederacion(Confederacion confederacion)
    {
        return confederacion switch
        {
            Confederacion.UEFA => CupoUefa,
            Confederacion.CONMEBOL => CupoConmebol,
            Confederacion.CONCACAF => CupoConcacaf,
            Confederacion.CAF => CupoCaf,
            Confederacion.AFC => CupoAfc,
            Confederacion.OFC => CupoOfc,
            _ => throw new EntidadInvalidaExcepcion("Confederación inválida.")
        };
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
            throw new EntidadInvalidaExcepcion("No se puede generar el fixture con menos de 48 equipos.");
        }

        if (equipos.Count > CantidadExactaEquipos)
        {
            throw new EntidadInvalidaExcepcion("No se puede generar el fixture con más de 48 equipos.");
        }
    }

    private void ValidarTodosLosEquiposSonValidos(List<Equipo> equipos)
    {
        if (equipos.Any(e => !e.EsValido()))
        {
            throw new EntidadInvalidaExcepcion("No se puede generar el fixture con equipos inválidos.");
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
            throw new EntidadInvalidaExcepcion("No se puede generar el fixture con menos de 4 estadios.");
        }
    }

    private void ValidarTodosLosEstadiosSonValidos(List<Estadio> estadios)
    {
        if (estadios.Any(e => !e.EsValido()))
        {
            throw new EntidadInvalidaExcepcion("No se puede generar el fixture con estadios inválidos.");
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
            var equiposMezclados = MezclarEquiposConFisherYates(new List<Equipo>(grupoDeRanking), random);
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

        throw new EntidadInvalidaExcepcion(
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
        return new List<Estadio>(estadios
            .OrderBy(e => Normalizar.NormalizarTexto(e.Nombre))
        );
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

### ServicioSimulacion.cs

Ruta: WorldCupPlanner.Servicios\ServicioSimulacion.cs

~~~csharp
using WorldCupPlanner.Dominio.Entidades;

using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Servicios;

public class ServicioSimulacion
{
    private const int GolesMinimos = 0;
    private const int GolesMaximosExclusivo = 6;

    public void SimularResultados(
        List<Partido> partidos,
        ConfiguracionTorneo configuracionTorneo)
    {
        var random = new Random(configuracionTorneo.SemillaSimulation);

        foreach (var partido in partidos)
        {
            if (partido.EstaJugado())
            {
                continue;
            }

            partido.GolesLocal = random.Next(GolesMinimos, GolesMaximosExclusivo);
            partido.GolesVisitante = random.Next(GolesMinimos, GolesMaximosExclusivo);

            if (EsFaseEliminatoria(partido) && partido.EsEmpate())
            {
                DesempatarPartido(partido);
            }
        }
    }

    private bool EsFaseEliminatoria(Partido partido)
    {
        return partido.Fase != FaseTorneo.FaseDeGrupos;
    }

    private void DesempatarPartido(Partido partido)
    {
        if (partido.GolesLocal < GolesMaximosExclusivo - 1)
        {
            partido.GolesLocal++;
        }
        else
        {
            partido.GolesVisitante--;
        }
    }
}
~~~

### ServicioUsuario.cs

Ruta: WorldCupPlanner.Servicios\ServicioUsuario.cs

~~~csharp
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.AccesoADatos.Repositorios;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.Servicios;

public class ServicioUsuario : IRepositorio<Usuario>
{
    private readonly RepositorioUsuarioEnMemoria _repositorio;
    private readonly ServicioAuditoria _auditoria;

    public ServicioUsuario(RepositorioUsuarioEnMemoria repositorio, ServicioAuditoria auditoria)
    {
        _repositorio = repositorio;
        _auditoria = auditoria;
    }

    public void AgregarUsuario(Usuario usuario, string? ejecutadoPor = null)
    {
        ValidarEmailUnico(usuario.Email);
        _repositorio.Agregar(usuario);
        _auditoria.Registrar(ejecutadoPor ?? "sistema", "ALTA_USUARIO", $"Usuario creado: {usuario.Email}");
    }

    public void ActualizarUsuario(Usuario usuario, string? nombre = null, string? apellido = null,
        string? email = null, string? fechaNacimiento = null, string? ejecutadoPor = null)
    {
        var usuarioActual = _repositorio.Buscar(u => ReferenceEquals(u, usuario))
            .FirstOrDefault();

        if (usuarioActual == null)
        {
            throw new EntidadInvalidaExcepcion("El usuario no existe en el sistema.");
        }

        var campos = new List<string>();

        if (email != null && !email.Equals(usuarioActual.Email, StringComparison.OrdinalIgnoreCase))
        {
            ValidarEmailUnico(email);
            usuarioActual.ActualizarEmail(email);
            campos.Add($"email: {email}");
        }

        if (nombre != null)
        {
            usuarioActual.ActualizarNombre(nombre);
            campos.Add($"nombre: {nombre}");
        }

        if (apellido != null)
        {
            usuarioActual.ActualizarApellido(apellido);
            campos.Add($"apellido: {apellido}");
        }

        if (fechaNacimiento != null)
        {
            usuarioActual.ActualizarFechaNacimiento(fechaNacimiento);
            campos.Add($"fechaNacimiento: {fechaNacimiento}");
        }

        _repositorio.Actualizar(usuarioActual);

        var detalle = campos.Count > 0
            ? $"Campos modificados en {usuarioActual.Email}: {string.Join(", ", campos)}"
            : $"Sin cambios efectivos en {usuarioActual.Email}";
        _auditoria.Registrar(ejecutadoPor ?? usuarioActual.Email, "EDICION_USUARIO", detalle);
    }

    public void EliminarUsuario(Usuario usuario, string? ejecutadoPor = null)
    {
        _repositorio.Eliminar(usuario);
        _auditoria.Registrar(ejecutadoPor ?? "sistema", "BAJA_USUARIO", $"Usuario eliminado: {usuario.Email}");
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
        _auditoria.Registrar(administrador.Email, "REINICIO_CONTRASENIA", $"Contraseña reiniciada para: {usuario.Email}");
    }

    public void CambiarContrasenia(Usuario usuario, string contraseniaAnterior, string nuevaContrasenia)
    {
        if (!usuario.VerificarContrasenia(contraseniaAnterior))
        {
            throw new InvalidOperationException("La contraseña actual es incorrecta.");
        }

        usuario.EstablecerContrasenia(nuevaContrasenia);
        _auditoria.Registrar(usuario.Email, "CAMBIO_CONTRASENIA", $"Contraseña cambiada para: {usuario.Email}");
    }

    public Usuario Login(string email, string contrasenia)
    {
        var usuario = _repositorio.ObtenerPorEmail(email);

        if (usuario == null || !usuario.VerificarContrasenia(contrasenia))
        {
            _auditoria.Registrar(email, "LOGIN_FALLIDO", $"Intento fallido para: {email}");
            throw new InvalidOperationException("Email o contraseña incorrectos.");
        }

        _auditoria.Registrar(usuario.Email, "LOGIN_EXITOSO", $"Login exitoso: {email}");
        return usuario;
    }

    public void Agregar(Usuario entidad) => AgregarUsuario(entidad);

    public void Actualizar(Usuario entidad)
    {
        if (entidad == null)
            throw new EntidadInvalidaExcepcion("El usuario no puede ser nulo.");
        var existente = _repositorio.Buscar(u => ReferenceEquals(u, entidad)).FirstOrDefault();
        if (existente == null)
            throw new EntidadInexistenteExcepcion("El usuario no existe en el sistema.");
        _repositorio.Actualizar(existente);
    }

    public void Eliminar(Usuario entidad) => EliminarUsuario(entidad);

    public List<Usuario> Buscar(Func<Usuario, bool> predicado) => _repositorio.Buscar(predicado);
}

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
using WorldCupPlanner.Dominio.Excepciones;
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

    [TestMethod]
    public void ObtenerClasificadosAFaseEliminatoria_ConPartidoDeGrupoSinResultado_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioClasificacion = new ServicioClasificacion();
        var fixture = CrearFixtureConDoceGruposResueltos();
        var configuracionTorneo = new ConfiguracionTorneo { SemillaCrucesFase = 1 };

        fixture.Grupos[0].Partidos[0].GolesLocal = null;
        fixture.Grupos[0].Partidos[0].GolesVisitante = null;

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioClasificacion.ObtenerClasificadosAFaseEliminatoria(
                fixture,
                configuracionTorneo
            )
        );
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
using WorldCupPlanner.Dominio.Excepciones;
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

    [TestMethod]
    public void GenerarPrimeraRondaEliminatoria_DebeAsignarIdsIncrementalesLuegoDeFaseDeGrupos()
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
        var idsEsperados = Enumerable.Range(73, 16).ToList();
        var idsObtenidos = partidos.Select(p => p.Id).ToList();

        CollectionAssert.AreEqual(idsEsperados, idsObtenidos);
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
                Id = 72 + i,
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
                Id = 72 + i,
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

    private List<Partido> CrearPartidosOctavosJugados(int cantidad)
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
                Id = 88 + i,
                EquipoLocal = new Equipo
                {
                    Nombre = $"LocalOctavos{i}",
                    RankingFIFA = 1000 + i,
                    Confederacion = Confederacion.UEFA
                },
                EquipoVisitante = new Equipo
                {
                    Nombre = $"VisitanteOctavos{i}",
                    RankingFIFA = 900 + i,
                    Confederacion = Confederacion.CONMEBOL
                },
                Estadio = estadio,
                Fecha = DateTime.Now,
                Fase = FaseTorneo.Octavos,
                GolesLocal = 2,
                GolesVisitante = 0
            });
        }

        return partidos;
    }

    private List<Partido> CrearPartidosCuartosJugados(int cantidad)
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
                Id = 96 + i,
                EquipoLocal = new Equipo
                {
                    Nombre = $"LocalCuartos{i}",
                    RankingFIFA = 1000 + i,
                    Confederacion = Confederacion.UEFA
                },
                EquipoVisitante = new Equipo
                {
                    Nombre = $"VisitanteCuartos{i}",
                    RankingFIFA = 900 + i,
                    Confederacion = Confederacion.CONMEBOL
                },
                Estadio = estadio,
                Fecha = DateTime.Now,
                Fase = FaseTorneo.Cuartos,
                GolesLocal = 2,
                GolesVisitante = 0
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
    public void GenerarOctavos_DebeCruzarGanadoresAContraGanadoresB()
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
        for (int i = 0; i < 8; i++)
        {
            Assert.AreEqual(partidosPrimeraRonda[i].Ganador(), octavos[i].EquipoLocal);
            Assert.AreEqual(partidosPrimeraRonda[i + 8].Ganador(), octavos[i].EquipoVisitante);
        }
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
        var idsEsperados = Enumerable.Range(89, 8).ToList();
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
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioCruces.GenerarOctavos(
                partidosPrimeraRonda,
                estadios,
                configuracionTorneo
            )
        );
    }

    [TestMethod]
    public void GenerarOctavos_ConCantidadDistintaA16Partidos_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var partidosPrimeraRonda = CrearPartidosPrimeraRondaJugados(15);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioCruces.GenerarOctavos(
                partidosPrimeraRonda,
                estadios,
                configuracionTorneo
            )
        );
    }

    [TestMethod]
    public void GenerarOctavos_ConPartidoQueNoEsDePrimeraRonda_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var partidosPrimeraRonda = CrearPartidosPrimeraRondaJugados(16);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        partidosPrimeraRonda[0].Fase = FaseTorneo.FaseDeGrupos;

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioCruces.GenerarOctavos(
                partidosPrimeraRonda,
                estadios,
                configuracionTorneo
            )
        );
    }

    [TestMethod]
    public void GenerarOctavos_ConPartidoPrimeraRondaEmpatado_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var partidosPrimeraRonda = CrearPartidosPrimeraRondaJugados(16);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        partidosPrimeraRonda[0].GolesLocal = 1;
        partidosPrimeraRonda[0].GolesVisitante = 1;

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioCruces.GenerarOctavos(
                partidosPrimeraRonda,
                estadios,
                configuracionTorneo
            )
        );
    }

    [TestMethod]
    public void GenerarCuartos_Con8OctavosJugados_DebeGenerar4Partidos()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var octavos = CrearPartidosOctavosJugados(8);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var cuartos = servicioCruces.GenerarCuartos(
            octavos,
            estadios,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(4, cuartos.Count);
        Assert.IsTrue(cuartos.All(p => p.Fase == FaseTorneo.Cuartos));
    }

    [TestMethod]
    public void GenerarCuartos_ConCantidadDistintaA8Octavos_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var octavos = CrearPartidosOctavosJugados(7);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioCruces.GenerarCuartos(
                octavos,
                estadios,
                configuracionTorneo
            )
        );
    }

    [TestMethod]
    public void GenerarCuartos_ConPartidoQueNoEsOctavos_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var octavos = CrearPartidosOctavosJugados(8);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        octavos[0].Fase = FaseTorneo.PrimeraRondaEliminatoria;

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioCruces.GenerarCuartos(
                octavos,
                estadios,
                configuracionTorneo
            )
        );
    }

    [TestMethod]
    public void GenerarCuartos_ConOctavoNoJugado_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var octavos = CrearPartidosOctavosJugados(8);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        octavos[0].GolesLocal = null;
        octavos[0].GolesVisitante = null;

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioCruces.GenerarCuartos(
                octavos,
                estadios,
                configuracionTorneo
            )
        );
    }

    [TestMethod]
    public void GenerarCuartos_ConOctavoEmpatado_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var octavos = CrearPartidosOctavosJugados(8);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        octavos[0].GolesLocal = 1;
        octavos[0].GolesVisitante = 1;

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioCruces.GenerarCuartos(
                octavos,
                estadios,
                configuracionTorneo
            )
        );
    }

    [TestMethod]
    public void GenerarCuartos_DebeUsarGanadoresDeOctavos()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var octavos = CrearPartidosOctavosJugados(8);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var cuartos = servicioCruces.GenerarCuartos(
            octavos,
            estadios,
            configuracionTorneo
        );

        // Assert
        var ganadoresOctavos = octavos
            .Select(p => p.Ganador())
            .ToList();

        var equiposEnCuartos = cuartos
            .SelectMany(p => new[] { p.EquipoLocal, p.EquipoVisitante })
            .ToList();

        CollectionAssert.AreEquivalent(ganadoresOctavos, equiposEnCuartos);
    }

    [TestMethod]
    public void GenerarCuartos_DebeCruzarGanadoresDeOctavosDeADos()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var octavos = CrearPartidosOctavosJugados(8);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var cuartos = servicioCruces.GenerarCuartos(
            octavos,
            estadios,
            configuracionTorneo
        );

        // Assert
        for (int i = 0; i < 4; i++)
        {
            Assert.AreEqual(octavos[i * 2].Ganador(), cuartos[i].EquipoLocal);
            Assert.AreEqual(octavos[i * 2 + 1].Ganador(), cuartos[i].EquipoVisitante);
        }
    }

    [TestMethod]
    public void GenerarCuartos_DebeAsignarIdsIncrementalesLuegoDeOctavos()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var octavos = CrearPartidosOctavosJugados(8);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var cuartos = servicioCruces.GenerarCuartos(
            octavos,
            estadios,
            configuracionTorneo
        );

        // Assert
        var idsEsperados = Enumerable.Range(97, 4).ToList();
        var idsObtenidos = cuartos.Select(p => p.Id).ToList();

        CollectionAssert.AreEqual(idsEsperados, idsObtenidos);
    }

    [TestMethod]
    public void GenerarCuartos_DebeAsignarEstadiosPorRotacion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var octavos = CrearPartidosOctavosJugados(8);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var cuartos = servicioCruces.GenerarCuartos(
            octavos,
            estadios,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(estadios[0], cuartos[0].Estadio);
        Assert.AreEqual(estadios[1], cuartos[1].Estadio);
        Assert.AreEqual(estadios[2], cuartos[2].Estadio);
        Assert.AreEqual(estadios[3], cuartos[3].Estadio);
    }

    [TestMethod]
    public void GenerarCuartos_DebeAsignarFechaSegunConfiguracion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var octavos = CrearPartidosOctavosJugados(8);
        var estadios = CrearEstadiosValidos(4);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            FechaInicioTorneo = new DateTime(2026, 6, 1),
            SeparacionEntreFechas = 3
        };

        // Act
        var cuartos = servicioCruces.GenerarCuartos(
            octavos,
            estadios,
            configuracionTorneo
        );

        // Assert
        var fechaEsperada = new DateTime(2026, 6, 7);

        Assert.IsTrue(cuartos.All(p => p.Fecha == fechaEsperada));
    }

    [TestMethod]
    public void GenerarSemifinales_Con4CuartosJugados_DebeGenerar2Partidos()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var cuartos = CrearPartidosCuartosJugados(4);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var semifinales = servicioCruces.GenerarSemifinales(
            cuartos,
            estadios,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(2, semifinales.Count);
        Assert.IsTrue(semifinales.All(p => p.Fase == FaseTorneo.Semifinal));
    }

    [TestMethod]
    public void GenerarSemifinales_ConCantidadDistintaA4Cuartos_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var cuartos = CrearPartidosCuartosJugados(3);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioCruces.GenerarSemifinales(
                cuartos,
                estadios,
                configuracionTorneo
            )
        );
    }

    [TestMethod]
    public void GenerarSemifinales_ConPartidoQueNoEsCuartos_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var cuartos = CrearPartidosCuartosJugados(4);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        cuartos[0].Fase = FaseTorneo.Octavos;

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioCruces.GenerarSemifinales(
                cuartos,
                estadios,
                configuracionTorneo
            )
        );
    }

    [TestMethod]
    public void GenerarSemifinales_ConCuartoNoJugado_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var cuartos = CrearPartidosCuartosJugados(4);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        cuartos[0].GolesLocal = null;
        cuartos[0].GolesVisitante = null;

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioCruces.GenerarSemifinales(
                cuartos,
                estadios,
                configuracionTorneo
            )
        );
    }

    [TestMethod]
    public void GenerarSemifinales_ConCuartoEmpatado_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var cuartos = CrearPartidosCuartosJugados(4);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        cuartos[0].GolesLocal = 1;
        cuartos[0].GolesVisitante = 1;

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioCruces.GenerarSemifinales(
                cuartos,
                estadios,
                configuracionTorneo
            )
        );
    }

    [TestMethod]
    public void GenerarSemifinales_DebeUsarGanadoresDeCuartos()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var cuartos = CrearPartidosCuartosJugados(4);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var semifinales = servicioCruces.GenerarSemifinales(
            cuartos,
            estadios,
            configuracionTorneo
        );

        // Assert
        var ganadoresCuartos = cuartos
            .Select(p => p.Ganador())
            .ToList();

        var equiposEnSemifinales = semifinales
            .SelectMany(p => new[] { p.EquipoLocal, p.EquipoVisitante })
            .ToList();

        CollectionAssert.AreEquivalent(ganadoresCuartos, equiposEnSemifinales);
    }

    [TestMethod]
    public void GenerarSemifinales_DebeCruzarGanadoresDeCuartosDeADos()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var cuartos = CrearPartidosCuartosJugados(4);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var semifinales = servicioCruces.GenerarSemifinales(
            cuartos,
            estadios,
            configuracionTorneo
        );

        // Assert
        for (int i = 0; i < 2; i++)
        {
            Assert.AreEqual(cuartos[i * 2].Ganador(), semifinales[i].EquipoLocal);
            Assert.AreEqual(cuartos[i * 2 + 1].Ganador(), semifinales[i].EquipoVisitante);
        }
    }
    
    [TestMethod]
    public void GenerarSemifinales_DebeAsignarIdsIncrementalesLuegoDeCuartos()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var cuartos = CrearPartidosCuartosJugados(4);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var semifinales = servicioCruces.GenerarSemifinales(
            cuartos,
            estadios,
            configuracionTorneo
        );

        // Assert
        var idsEsperados = Enumerable.Range(101, 2).ToList();
        var idsObtenidos = semifinales.Select(p => p.Id).ToList();

        CollectionAssert.AreEqual(idsEsperados, idsObtenidos);
    }
    
    [TestMethod]
    public void GenerarSemifinales_DebeAsignarEstadiosPorRotacion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var cuartos = CrearPartidosCuartosJugados(4);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var semifinales = servicioCruces.GenerarSemifinales(
            cuartos,
            estadios,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(estadios[0], semifinales[0].Estadio);
        Assert.AreEqual(estadios[1], semifinales[1].Estadio);
    }
    
    [TestMethod]
    public void GenerarSemifinales_DebeAsignarFechaSegunConfiguracion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var cuartos = CrearPartidosCuartosJugados(4);
        var estadios = CrearEstadiosValidos(4);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            FechaInicioTorneo = new DateTime(2026, 6, 1),
            SeparacionEntreFechas = 3
        };

        // Act
        var semifinales = servicioCruces.GenerarSemifinales(
            cuartos,
            estadios,
            configuracionTorneo
        );

        // Assert
        var fechaEsperada = new DateTime(2026, 6, 10);

        Assert.IsTrue(semifinales.All(p => p.Fecha == fechaEsperada));
    }
    
    private List<Partido> CrearPartidosSemifinalesJugados(int cantidad)
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
                Id = 100 + i,
                EquipoLocal = new Equipo
                {
                    Nombre = $"LocalSemifinal{i}",
                    RankingFIFA = 1000 + i,
                    Confederacion = Confederacion.UEFA
                },
                EquipoVisitante = new Equipo
                {
                    Nombre = $"VisitanteSemifinal{i}",
                    RankingFIFA = 900 + i,
                    Confederacion = Confederacion.CONMEBOL
                },
                Estadio = estadio,
                Fecha = DateTime.Now,
                Fase = FaseTorneo.Semifinal,
                GolesLocal = 2,
                GolesVisitante = 0
            });
        }

        return partidos;
    }
    
    [TestMethod]
    public void GenerarTercerPuesto_Con2SemifinalesJugadas_DebeGenerar1Partido()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var semifinales = CrearPartidosSemifinalesJugados(2);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var tercerPuesto = servicioCruces.GenerarTercerPuesto(
            semifinales,
            estadios,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(FaseTorneo.TercerPuesto, tercerPuesto.Fase);
        Assert.IsNotNull(tercerPuesto.EquipoLocal);
        Assert.IsNotNull(tercerPuesto.EquipoVisitante);
    }
    
    [TestMethod]
    public void GenerarTercerPuesto_ConCantidadDistintaA2Semifinales_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var semifinales = CrearPartidosSemifinalesJugados(1);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioCruces.GenerarTercerPuesto(
                semifinales,
                estadios,
                configuracionTorneo
            )
        );
    }
    
    [TestMethod]
    public void GenerarTercerPuesto_ConPartidoQueNoEsSemifinal_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var semifinales = CrearPartidosSemifinalesJugados(2);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        semifinales[0].Fase = FaseTorneo.Cuartos;

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioCruces.GenerarTercerPuesto(
                semifinales,
                estadios,
                configuracionTorneo
            )
        );
    }
    
    [TestMethod]
    public void GenerarTercerPuesto_ConSemifinalNoJugada_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var semifinales = CrearPartidosSemifinalesJugados(2);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        semifinales[0].GolesLocal = null;
        semifinales[0].GolesVisitante = null;

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioCruces.GenerarTercerPuesto(
                semifinales,
                estadios,
                configuracionTorneo
            )
        );
    }
    
    [TestMethod]
    public void GenerarTercerPuesto_ConSemifinalEmpatada_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var semifinales = CrearPartidosSemifinalesJugados(2);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        semifinales[0].GolesLocal = 1;
        semifinales[0].GolesVisitante = 1;

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioCruces.GenerarTercerPuesto(
                semifinales,
                estadios,
                configuracionTorneo
            )
        );
    }
    
    [TestMethod]
    public void GenerarTercerPuesto_DebeUsarPerdedoresDeSemifinales()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var semifinales = CrearPartidosSemifinalesJugados(2);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var tercerPuesto = servicioCruces.GenerarTercerPuesto(
            semifinales,
            estadios,
            configuracionTorneo
        );

        // Assert
        var perdedorSemifinal1 = semifinales[0].EquipoVisitante;
        var perdedorSemifinal2 = semifinales[1].EquipoVisitante;

        Assert.AreEqual(perdedorSemifinal1, tercerPuesto.EquipoLocal);
        Assert.AreEqual(perdedorSemifinal2, tercerPuesto.EquipoVisitante);
    }
    
    [TestMethod]
    public void GenerarTercerPuesto_DebeAsignarIdLuegoDeSemifinales()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var semifinales = CrearPartidosSemifinalesJugados(2);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var tercerPuesto = servicioCruces.GenerarTercerPuesto(
            semifinales,
            estadios,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(103, tercerPuesto.Id);
    }
    
    [TestMethod]
    public void GenerarTercerPuesto_DebeAsignarFechaSegunConfiguracion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var semifinales = CrearPartidosSemifinalesJugados(2);
        var estadios = CrearEstadiosValidos(4);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            FechaInicioTorneo = new DateTime(2026, 6, 1),
            SeparacionEntreFechas = 3
        };

        // Act
        var tercerPuesto = servicioCruces.GenerarTercerPuesto(
            semifinales,
            estadios,
            configuracionTorneo
        );

        // Assert
        var fechaEsperada = new DateTime(2026, 6, 13);

        Assert.AreEqual(fechaEsperada, tercerPuesto.Fecha);
    }
    
    [TestMethod]
    public void GenerarTercerPuesto_DebeAsignarPrimerEstadio()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var semifinales = CrearPartidosSemifinalesJugados(2);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var tercerPuesto = servicioCruces.GenerarTercerPuesto(
            semifinales,
            estadios,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(estadios[0], tercerPuesto.Estadio);
    }
    
    [TestMethod]
    public void GenerarFinal_Con2SemifinalesJugadas_DebeGenerar1Partido()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var semifinales = CrearPartidosSemifinalesJugados(2);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var final = servicioCruces.GenerarFinal(
            semifinales,
            estadios,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(FaseTorneo.Final, final.Fase);
        Assert.IsNotNull(final.EquipoLocal);
        Assert.IsNotNull(final.EquipoVisitante);
    }
    
    [TestMethod]
    public void GenerarFinal_ConCantidadDistintaA2Semifinales_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var semifinales = CrearPartidosSemifinalesJugados(1);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioCruces.GenerarFinal(
                semifinales,
                estadios,
                configuracionTorneo
            )
        );
    }
    
    [TestMethod]
    public void GenerarFinal_ConPartidoQueNoEsSemifinal_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var semifinales = CrearPartidosSemifinalesJugados(2);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        semifinales[0].Fase = FaseTorneo.Cuartos;

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioCruces.GenerarFinal(
                semifinales,
                estadios,
                configuracionTorneo
            )
        );
    }
    
    [TestMethod]
    public void GenerarFinal_ConSemifinalNoJugada_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var semifinales = CrearPartidosSemifinalesJugados(2);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        semifinales[0].GolesLocal = null;
        semifinales[0].GolesVisitante = null;

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioCruces.GenerarFinal(
                semifinales,
                estadios,
                configuracionTorneo
            )
        );
    }
    
    [TestMethod]
    public void GenerarFinal_ConSemifinalEmpatada_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var semifinales = CrearPartidosSemifinalesJugados(2);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        semifinales[0].GolesLocal = 1;
        semifinales[0].GolesVisitante = 1;

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioCruces.GenerarFinal(
                semifinales,
                estadios,
                configuracionTorneo
            )
        );
    }
    
    [TestMethod]
    public void GenerarFinal_DebeUsarGanadoresDeSemifinales()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var semifinales = CrearPartidosSemifinalesJugados(2);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var final = servicioCruces.GenerarFinal(
            semifinales,
            estadios,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(semifinales[0].Ganador(), final.EquipoLocal);
        Assert.AreEqual(semifinales[1].Ganador(), final.EquipoVisitante);
    }
    
    [TestMethod]
    public void GenerarFinal_DebeAsignarIdLuegoDeTercerPuesto()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var semifinales = CrearPartidosSemifinalesJugados(2);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var final = servicioCruces.GenerarFinal(
            semifinales,
            estadios,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(104, final.Id);
    }
    
    [TestMethod]
    public void GenerarFinal_DebeAsignarFechaSegunConfiguracion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var semifinales = CrearPartidosSemifinalesJugados(2);
        var estadios = CrearEstadiosValidos(4);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            FechaInicioTorneo = new DateTime(2026, 6, 1),
            SeparacionEntreFechas = 3
        };

        // Act
        var final = servicioCruces.GenerarFinal(
            semifinales,
            estadios,
            configuracionTorneo
        );

        // Assert
        var fechaEsperada = new DateTime(2026, 6, 16);

        Assert.AreEqual(fechaEsperada, final.Fecha);
    }
    
    [TestMethod]
    public void GenerarFinal_DebeAsignarPrimerEstadio()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var semifinales = CrearPartidosSemifinalesJugados(2);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var final = servicioCruces.GenerarFinal(
            semifinales,
            estadios,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(estadios[0], final.Estadio);
    }
    
    private void MarcarPartidosComoJugados(List<Partido> partidos)
    {
        foreach (var partido in partidos)
        {
            partido.GolesLocal = 2;
            partido.GolesVisitante = 0;
        }
    }
    
    [TestMethod]
    public void GenerarFaseEliminatoria_CompletaDesdePrimeraRonda_DebeGenerarHastaFinal()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo
        {
            FechaInicioTorneo = new DateTime(2026, 6, 1),
            SeparacionEntreFechas = 3
        };

        var primeraRonda = CrearPartidosPrimeraRondaJugados(16);

        // Act
        var octavos = servicioCruces.GenerarOctavos(
            primeraRonda,
            estadios,
            configuracionTorneo
        );

        MarcarPartidosComoJugados(octavos);

        var cuartos = servicioCruces.GenerarCuartos(
            octavos,
            estadios,
            configuracionTorneo
        );

        MarcarPartidosComoJugados(cuartos);

        var semifinales = servicioCruces.GenerarSemifinales(
            cuartos,
            estadios,
            configuracionTorneo
        );

        MarcarPartidosComoJugados(semifinales);

        var tercerPuesto = servicioCruces.GenerarTercerPuesto(
            semifinales,
            estadios,
            configuracionTorneo
        );

        var final = servicioCruces.GenerarFinal(
            semifinales,
            estadios,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(8, octavos.Count);
        Assert.AreEqual(4, cuartos.Count);
        Assert.AreEqual(2, semifinales.Count);

        Assert.AreEqual(FaseTorneo.TercerPuesto, tercerPuesto.Fase);
        Assert.AreEqual(FaseTorneo.Final, final.Fase);

        Assert.AreEqual(103, tercerPuesto.Id);
        Assert.AreEqual(104, final.Id);
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

    [TestMethod]
    public void AgregarEquipo_CupoLlenoOFC_LanzaExcepcion()
    {
        // Arrange - OFC tiene cupo 1
        _servicio.AgregarEquipo(CrearEquipo("Nueva Zelanda", Confederacion.OFC));

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            _servicio.AgregarEquipo(CrearEquipo("Australia", Confederacion.OFC)));
    }

    [TestMethod]
    public void AgregarEquipo_UltimoEquipoDentroDelCupo_SeAgregaCorrectamente()
    {
        // Arrange - llenar CONMEBOL hasta el lÃ­mite (7)
        for (var i = 1; i <= 7; i++)
            _servicio.AgregarEquipo(CrearEquipo($"Equipo CONMEBOL {i}", Confederacion.CONMEBOL));

        // Assert
        Assert.AreEqual(7, _servicio.ObtenerEquipos().Count);
    }

    [TestMethod]
    public void AgregarEquipo_CupoLlenoCONMEBOL_LanzaExcepcion()
    {
        // Arrange - llenar CONMEBOL (cupo 7) y luego intentar agregar uno mÃ¡s
        for (var i = 1; i <= 7; i++)
            _servicio.AgregarEquipo(CrearEquipo($"Equipo CONMEBOL {i}", Confederacion.CONMEBOL));

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            _servicio.AgregarEquipo(CrearEquipo("Equipo CONMEBOL 8", Confederacion.CONMEBOL)));
    }

    [TestMethod]
    public void AgregarEquipo_CupoLlenoEnUnaConfederacion_NoAfectaOtraConfederacion()
    {
        // Arrange - llenar OFC
        _servicio.AgregarEquipo(CrearEquipo("Nueva Zelanda", Confederacion.OFC));

        // Act - agregar en UEFA no debe verse afectado
        _servicio.AgregarEquipo(CrearEquipo("EspaÃ±a", Confederacion.UEFA));

        // Assert
        Assert.AreEqual(2, _servicio.ObtenerEquipos().Count);
    }

    [TestMethod]
    public void AgregarEquipo_CupoLlenoUEFA_LanzaExcepcion()
    {
        // Arrange - llenar UEFA (cupo 16)
        for (var i = 1; i <= 16; i++)
            _servicio.AgregarEquipo(CrearEquipo($"Equipo UEFA {i}", Confederacion.UEFA));

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            _servicio.AgregarEquipo(CrearEquipo("Equipo UEFA 17", Confederacion.UEFA)));
    }

    // IRepositorio<Equipo>

    [TestMethod]
    public void Agregar_EquipoValido_SeAgregaAlRepositorio()
    {
        // Arrange
        var equipo = CrearEquipo("EspaÃ±a", Confederacion.UEFA);

        // Act
        _servicio.Agregar(equipo);

        // Assert
        Assert.AreEqual(1, _servicio.ObtenerEquipos().Count);
    }

    [TestMethod]
    public void Agregar_EquipoInvalido_LanzaExcepcion()
    {
        // Arrange
        var equipo = new Equipo { Nombre = "", RankingFIFA = 1000, Confederacion = Confederacion.UEFA };

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() => _servicio.Agregar(equipo));
    }

    [TestMethod]
    public void Actualizar_EquipoExistente_PersisteCambios()
    {
        // Arrange
        var equipo = CrearEquipo("EspaÃ±a", Confederacion.UEFA);
        _servicio.AgregarEquipo(equipo);
        equipo.RankingFIFA = 500;

        // Act
        _servicio.Actualizar(equipo);

        // Assert
        Assert.AreEqual(500, _servicio.Buscar(e => e.Nombre == "EspaÃ±a").First().RankingFIFA);
    }

    [TestMethod]
    public void Actualizar_EquipoNoExistente_LanzaExcepcion()
    {
        // Arrange
        var equipo = CrearEquipo("EspaÃ±a", Confederacion.UEFA);

        // Act & Assert
        Assert.ThrowsException<EntidadInexistenteExcepcion>(() => _servicio.Actualizar(equipo));
    }

    [TestMethod]
    public void Actualizar_EquipoNulo_LanzaExcepcion()
    {
        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() => _servicio.Actualizar(null!));
    }

    [TestMethod]
    public void Eliminar_EquipoExistente_SeEliminaDelRepositorio()
    {
        // Arrange
        var equipo = CrearEquipo("EspaÃ±a", Confederacion.UEFA);
        _servicio.AgregarEquipo(equipo);

        // Act
        _servicio.Eliminar(equipo);

        // Assert
        Assert.AreEqual(0, _servicio.ObtenerEquipos().Count);
    }

    [TestMethod]
    public void Buscar_ConPredicadoCoincidente_DevuelveEquiposCorrespondientes()
    {
        // Arrange
        _servicio.AgregarEquipo(CrearEquipo("EspaÃ±a", Confederacion.UEFA));
        _servicio.AgregarEquipo(CrearEquipo("Francia", Confederacion.UEFA));
        _servicio.AgregarEquipo(CrearEquipo("Uruguay", Confederacion.CONMEBOL));

        // Act
        var resultado = _servicio.Buscar(e => e.Confederacion == Confederacion.UEFA);

        // Assert
        Assert.AreEqual(2, resultado.Count);
    }

    [TestMethod]
    public void Buscar_SinCoincidencias_DevuelveListaVacia()
    {
        // Arrange
        _servicio.AgregarEquipo(CrearEquipo("Uruguay", Confederacion.CONMEBOL));

        // Act
        var resultado = _servicio.Buscar(e => e.Confederacion == Confederacion.OFC);

        // Assert
        Assert.AreEqual(0, resultado.Count);
    }

    private static Equipo CrearEquipo(string nombre, Confederacion confederacion) => new()
    {
        Nombre = nombre,
        RankingFIFA = 1000,
        Confederacion = confederacion
    };
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
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public class ServicioFixtureTests
{
    [TestMethod]
    public void GenerarFixture_ConMenosDe48Equipos_DebeCompletarEquiposYGenerarFixture()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();

        var equipos = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(46)
            .ToList();

        var estadios = CrearEstadiosValidos(4);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1,
            SemillaFixture = 1
        };

        // Act
        var fixture = servicioFixture.GenerarFixture(
            equipos,
            estadios,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(12, fixture.Grupos.Count);
        Assert.IsTrue(fixture.Grupos.All(g => g.Equipos.Count == 4));

        var todosLosEquipos = fixture.Grupos
            .SelectMany(g => g.Equipos)
            .ToList();

        Assert.AreEqual(48, todosLosEquipos.Count);
        Assert.IsTrue(todosLosEquipos.Any(e => !equipos.Contains(e)));
    }

    [TestMethod]
    public void CompletarEquipos_ConMenosDe48Equipos_DebeAgregarEquiposFicticiosHasta48()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();

        var equipos = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(46)
            .ToList();

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1
        };

        // Act
        var equiposCompletos = servicioFixture.CompletarEquipos(
            equipos,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(48, equiposCompletos.Count);
    }

    [TestMethod]
    public void CompletarEquipos_ConMasDe48Equipos_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();

        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();

        equipos.Add(new Equipo
        {
            Nombre = "EquipoExtra",
            RankingFIFA = 1000,
            Confederacion = Confederacion.CONMEBOL
        });

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1
        };

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioFixture.CompletarEquipos(
                equipos,
                configuracionTorneo
            )
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
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
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
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo)
        );
    }

    [TestMethod]
    public void GenerarFixture_ConEquipoInvalido_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidos(47);
        // Agregar un equipo invalido (sin confederacion)
        equipos.Add(new Equipo
        {
            Nombre = "EquipoInvalido",
            RankingFIFA = 500,
            Confederacion = null
        });
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
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
        // Agregar un estadio invalido (capacidad menor a 20000)
        estadios.Add(new Estadio
        {
            Nombre = "EstadioInvalido",
            Ciudad = "Ciudad",
            Capacidad = 10000
        });
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
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

        // Assert - Todos los originales estan en el fixture
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
                    $"El estadio {partido.Estadio.Nombre} asignado debe ser valido");
            }
        }
    }
    
    [TestMethod]
    public void CompletarEquipos_DebeCrearEquiposFicticiosValidos()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();

        var equipos = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(46)
            .ToList();

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1
        };

        // Act
        var equiposCompletos = servicioFixture.CompletarEquipos(
            equipos,
            configuracionTorneo
        );

        // Assert
        var equiposFicticios = equiposCompletos
            .Where(e => !equipos.Contains(e))
            .ToList();

        Assert.AreEqual(2, equiposFicticios.Count);
        Assert.IsTrue(equiposFicticios.All(e => e.EsValido()));
    }
    
    [TestMethod]
    public void CompletarEquipos_ConDistintasSemillas_DebePermitirEquiposFicticiosDistintos()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();

        var equiposBase1 = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(46)
            .ToList();

        var equiposBase2 = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(46)
            .ToList();

        var configuracionTorneo1 = new ConfiguracionTorneo
        {
            SemillaCompletar = 1
        };

        var configuracionTorneo2 = new ConfiguracionTorneo
        {
            SemillaCompletar = 2
        };

        // Act
        var equiposCompletos1 = servicioFixture.CompletarEquipos(
            equiposBase1,
            configuracionTorneo1
        );

        var equiposCompletos2 = servicioFixture.CompletarEquipos(
            equiposBase2,
            configuracionTorneo2
        );

        // Assert
        var rankingsFicticios1 = equiposCompletos1
            .Where(e => !equiposBase1.Contains(e))
            .Select(e => e.RankingFIFA)
            .ToList();

        var rankingsFicticios2 = equiposCompletos2
            .Where(e => !equiposBase2.Contains(e))
            .Select(e => e.RankingFIFA)
            .ToList();

        CollectionAssert.AreNotEqual(rankingsFicticios1, rankingsFicticios2);
    }
    
    [TestMethod]
    public void CompletarEquipos_ConMismaSemilla_DebeGenerarMismosEquiposFicticios()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();

        var equiposBase1 = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(46)
            .ToList();

        var equiposBase2 = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(46)
            .ToList();

        var configuracionTorneo1 = new ConfiguracionTorneo
        {
            SemillaCompletar = 7
        };

        var configuracionTorneo2 = new ConfiguracionTorneo
        {
            SemillaCompletar = 7
        };

        // Act
        var equiposCompletos1 = servicioFixture.CompletarEquipos(
            equiposBase1,
            configuracionTorneo1
        );

        var equiposCompletos2 = servicioFixture.CompletarEquipos(
            equiposBase2,
            configuracionTorneo2
        );

        // Assert
        var rankingsFicticios1 = equiposCompletos1
            .Where(e => !equiposBase1.Contains(e))
            .Select(e => e.RankingFIFA)
            .ToList();

        var rankingsFicticios2 = equiposCompletos2
            .Where(e => !equiposBase2.Contains(e))
            .Select(e => e.RankingFIFA)
            .ToList();

        CollectionAssert.AreEqual(rankingsFicticios1, rankingsFicticios2);
    }
    
    [TestMethod]
    public void CompletarEquipos_NoDebeModificarListaOriginal()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();

        var equipos = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(24)
            .ToList();

        var cantidadOriginal = equipos.Count;

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1
        };

        // Act
        var equiposCompletos = servicioFixture.CompletarEquipos(
            equipos,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(24, cantidadOriginal);
        Assert.AreEqual(24, equipos.Count);
        Assert.AreEqual(48, equiposCompletos.Count);
    }
    
    [TestMethod]
    public void CompletarEquipos_Con48Equipos_NoDebeAgregarEquiposFicticios()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();

        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1
        };

        // Act
        var equiposCompletos = servicioFixture.CompletarEquipos(
            equipos,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(48, equiposCompletos.Count);
        Assert.IsFalse(equiposCompletos.Any(e => !equipos.Contains(e)));
    }
    
    [TestMethod]
    public void CompletarEquipos_ConEquipoOriginalInvalido_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();

        var equipos = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(47)
            .ToList();

        equipos[0].Nombre = "";

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1
        };

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioFixture.CompletarEquipos(
                equipos,
                configuracionTorneo
            )
        );
    }
    
    [TestMethod]
    public void GenerarFixture_ConMenosDe48Equipos_DebeIncluirEquiposFicticiosEnGrupos()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();

        var equipos = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(46)
            .ToList();

        var estadios = CrearEstadiosValidos(4);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1,
            SemillaFixture = 1
        };

        // Act
        var fixture = servicioFixture.GenerarFixture(
            equipos,
            estadios,
            configuracionTorneo
        );

        // Assert
        var equiposDelFixture = fixture.Grupos
            .SelectMany(g => g.Equipos)
            .ToList();

        var equiposFicticios = equiposDelFixture
            .Where(e => !equipos.Contains(e))
            .ToList();

        Assert.AreEqual(2, equiposFicticios.Count);
        Assert.IsTrue(equiposFicticios.All(e => e.EsValido()));
    }
    
    [TestMethod]
    public void GenerarFixture_ConMenosDe48Equipos_DebeGenerarGruposValidos()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();

        var equipos = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(46)
            .ToList();

        var estadios = CrearEstadiosValidos(4);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1,
            SemillaFixture = 1
        };

        // Act
        var fixture = servicioFixture.GenerarFixture(
            equipos,
            estadios,
            configuracionTorneo
        );

        // Assert
        Assert.IsTrue(fixture.Grupos.All(g => g.EsValido()));
    }

    [TestMethod]
    public void CompletarEquipos_ConMuchosEquiposFaltantes_DebeRespetarCuposPorConfederacion()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();

        var equipos = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(24)
            .ToList();

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1
        };

        // Act
        var equiposCompletos = servicioFixture.CompletarEquipos(
            equipos,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(48, equiposCompletos.Count);

        Assert.IsTrue(equiposCompletos.Count(e => e.Confederacion == Confederacion.UEFA) <= 16);
        Assert.IsTrue(equiposCompletos.Count(e => e.Confederacion == Confederacion.CONMEBOL) <= 7);
        Assert.IsTrue(equiposCompletos.Count(e => e.Confederacion == Confederacion.CONCACAF) <= 7);
        Assert.IsTrue(equiposCompletos.Count(e => e.Confederacion == Confederacion.CAF) <= 9);
        Assert.IsTrue(equiposCompletos.Count(e => e.Confederacion == Confederacion.AFC) <= 8);
        Assert.IsTrue(equiposCompletos.Count(e => e.Confederacion == Confederacion.OFC) <= 1);
    }
    
    [TestMethod]
    public void GenerarFixture_ConMuchosEquiposFaltantes_DebeCompletarYGenerarGruposValidos()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();

        var equipos = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(24)
            .ToList();

        var estadios = CrearEstadiosValidos(4);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1,
            SemillaFixture = 1
        };

        // Act
        var fixture = servicioFixture.GenerarFixture(
            equipos,
            estadios,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(12, fixture.Grupos.Count);
        Assert.IsTrue(fixture.Grupos.All(g => g.Equipos.Count == 4));
        Assert.IsTrue(fixture.Grupos.All(g => g.EsValido()));

        var equiposDelFixture = fixture.Grupos
            .SelectMany(g => g.Equipos)
            .ToList();

        Assert.AreEqual(48, equiposDelFixture.Count);
        Assert.AreEqual(24, equiposDelFixture.Count(e => !equipos.Contains(e)));
    }

    [TestMethod]
    public void CompletarEquipos_NoDebeGenerarNombresRepetidosParaEquiposFicticios()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();

        var equipos = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(46)
            .ToList();

        equipos[0].Nombre = "AFC_01";

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1
        };

        // Act
        var equiposCompletos = servicioFixture.CompletarEquipos(
            equipos,
            configuracionTorneo
        );

        // Assert
        var nombres = equiposCompletos
            .Select(e => e.Nombre)
            .ToList();

        Assert.AreEqual(48, nombres.Count);
        Assert.AreEqual(48, nombres.Distinct().Count());
    }

    [TestMethod]
    public void CompletarEquipos_DebeGenerarNombresDeterministicosPorConfederacion()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();

        var equipos = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(24)
            .ToList();

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1
        };

        // Act
        var equiposCompletos = servicioFixture.CompletarEquipos(
            equipos,
            configuracionTorneo
        );

        // Assert
        var equiposAgregados = equiposCompletos
            .Where(e => !equipos.Contains(e))
            .ToList();

        Assert.AreEqual(24, equiposAgregados.Count);

        CollectionAssert.AreEqual(
            new List<string>
            {
                "CONCACAF_01", "CONCACAF_02", "CONCACAF_03", "CONCACAF_04",
                "CONCACAF_05", "CONCACAF_06", "CONCACAF_07"
            },
            ObtenerNombresPorConfederacion(equiposAgregados, Confederacion.CONCACAF)
        );

        CollectionAssert.AreEqual(
            new List<string>
            {
                "CAF_01", "CAF_02", "CAF_03", "CAF_04", "CAF_05",
                "CAF_06", "CAF_07", "CAF_08", "CAF_09"
            },
            ObtenerNombresPorConfederacion(equiposAgregados, Confederacion.CAF)
        );

        CollectionAssert.AreEqual(
            new List<string>
            {
                "AFC_01", "AFC_02", "AFC_03", "AFC_04",
                "AFC_05", "AFC_06", "AFC_07", "AFC_08"
            },
            ObtenerNombresPorConfederacion(equiposAgregados, Confederacion.AFC)
        );
    }
    
    [TestMethod]
    public void GenerarFixture_ConMenosDe48Equipos_NoDebeDuplicarNombresEnGrupos()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();

        var equipos = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(46)
            .ToList();

        equipos[0].Nombre = "AFC_01";

        var estadios = CrearEstadiosValidos(4);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1,
            SemillaFixture = 1
        };

        // Act
        var fixture = servicioFixture.GenerarFixture(
            equipos,
            estadios,
            configuracionTorneo
        );

        // Assert
        var nombresEquipos = fixture.Grupos
            .SelectMany(g => g.Equipos)
            .Select(e => e.Nombre)
            .ToList();

        Assert.AreEqual(48, nombresEquipos.Count);
        Assert.AreEqual(48, nombresEquipos.Distinct().Count());
    }

    private List<string> ObtenerNombresPorConfederacion(
        List<Equipo> equipos,
        Confederacion confederacion)
    {
        return equipos
            .Where(e => e.Confederacion == confederacion)
            .Select(e => e.Nombre)
            .OrderBy(nombre => nombre)
            .ToList();
    }
    
    [TestMethod]
    public void GenerarFixture_ConMenosDe48Equipos_DebeIncluirFicticiosConNombresDeterministicosPorConfederacion()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();

        var equipos = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(24)
            .ToList();

        var estadios = CrearEstadiosValidos(4);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1,
            SemillaFixture = 1
        };

        // Act
        var fixture = servicioFixture.GenerarFixture(
            equipos,
            estadios,
            configuracionTorneo
        );

        // Assert
        var equiposDelFixture = fixture.Grupos
            .SelectMany(g => g.Equipos)
            .ToList();

        var equiposAgregados = equiposDelFixture
            .Where(e => !equipos.Contains(e))
            .ToList();

        Assert.AreEqual(24, equiposAgregados.Count);

        Assert.IsTrue(equiposAgregados.Any(e => e.Nombre == "CONCACAF_01"));
        Assert.IsTrue(equiposAgregados.Any(e => e.Nombre == "CAF_01"));
        Assert.IsTrue(equiposAgregados.Any(e => e.Nombre == "AFC_01"));

        Assert.AreEqual(
            equiposDelFixture.Count,
            equiposDelFixture.Select(e => e.Nombre).Distinct().Count()
        );
    }
    
    [TestMethod]
    public void CompletarEquipos_NoDebeDuplicarNombreCuandoYaExisteNombreDeterministico()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();

        var equipos = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(24)
            .ToList();

        equipos[0].Nombre = "CONCACAF_01";

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1
        };

        // Act
        var equiposCompletos = servicioFixture.CompletarEquipos(
            equipos,
            configuracionTorneo
        );

        // Assert
        var nombres = equiposCompletos
            .Select(e => e.Nombre)
            .ToList();

        Assert.AreEqual(48, nombres.Count);
        Assert.AreEqual(48, nombres.Distinct().Count());

        Assert.IsTrue(nombres.Contains("CONCACAF_01"));
        Assert.IsTrue(nombres.Contains("CONCACAF_02"));
    }
}


~~~

### ServicioSimulacionTests.cs

Ruta: WorldCupPlanner.ServicioTests\ServicioSimulacionTests.cs

~~~csharp
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public class ServicioSimulacionTests
{
    [TestMethod]
    public void SimularResultados_ConPartidosSinResultado_DebeAsignarGoles()
    {
        // Arrange
        var servicioSimulacion = new ServicioSimulacion();
        var partidos = CrearPartidosSinResultado(5);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaSimulation = 1
        };

        // Act
        servicioSimulacion.SimularResultados(partidos, configuracionTorneo);

        // Assert
        Assert.IsTrue(partidos.All(p => p.GolesLocal.HasValue));
        Assert.IsTrue(partidos.All(p => p.GolesVisitante.HasValue));
    }

    private List<Partido> CrearPartidosSinResultado(int cantidad)
    {
        var partidos = new List<Partido>();

        for (int i = 1; i <= cantidad; i++)
        {
            partidos.Add(new Partido
            {
                Id = i,
                EquipoLocal = CrearEquipo($"Local{i}"),
                EquipoVisitante = CrearEquipo($"Visitante{i}"),
                Estadio = CrearEstadio(),
                Fecha = DateTime.Now,
                Fase = FaseTorneo.FaseDeGrupos
            });
        }

        return partidos;
    }

    private List<Partido> CrearPartidosEliminatoriosSinResultado(int cantidad)
    {
        var partidos = new List<Partido>();

        for (int i = 1; i <= cantidad; i++)
        {
            partidos.Add(new Partido
            {
                Id = i,
                EquipoLocal = CrearEquipo($"LocalEliminatorio{i}"),
                EquipoVisitante = CrearEquipo($"VisitanteEliminatorio{i}"),
                Estadio = CrearEstadio(),
                Fecha = DateTime.Now,
                Fase = FaseTorneo.PrimeraRondaEliminatoria
            });
        }

        return partidos;
    }

    private Equipo CrearEquipo(string nombre)
    {
        return new Equipo
        {
            Nombre = nombre,
            RankingFIFA = 1000,
            Confederacion = Confederacion.UEFA
        };
    }

    private Estadio CrearEstadio()
    {
        return new Estadio
        {
            Nombre = "Estadio",
            Ciudad = "Ciudad",
            Capacidad = 30000
        };
    }
    
    private List<string> ObtenerResultados(List<Partido> partidos)
    {
        return partidos
            .Select(p => $"{p.GolesLocal}-{p.GolesVisitante}")
            .ToList();
    }
    
    [TestMethod]
    public void SimularResultados_ConMismaSemilla_DebeGenerarMismosResultados()
    {
        // Arrange
        var servicioSimulacion = new ServicioSimulacion();

        var partidos1 = CrearPartidosSinResultado(10);
        var partidos2 = CrearPartidosSinResultado(10);

        var configuracionTorneo1 = new ConfiguracionTorneo
        {
            SemillaSimulation = 7
        };

        var configuracionTorneo2 = new ConfiguracionTorneo
        {
            SemillaSimulation = 7
        };

        // Act
        servicioSimulacion.SimularResultados(partidos1, configuracionTorneo1);
        servicioSimulacion.SimularResultados(partidos2, configuracionTorneo2);

        // Assert
        var resultados1 = ObtenerResultados(partidos1);
        var resultados2 = ObtenerResultados(partidos2);

        CollectionAssert.AreEqual(resultados1, resultados2);
    }
    
    [TestMethod]
    public void SimularResultados_ConDistintasSemillas_DebePermitirResultadosDistintos()
    {
        // Arrange
        var servicioSimulacion = new ServicioSimulacion();

        var partidosReferencia = CrearPartidosSinResultado(10);

        var configuracionReferencia = new ConfiguracionTorneo
        {
            SemillaSimulation = 1
        };

        servicioSimulacion.SimularResultados(
            partidosReferencia,
            configuracionReferencia
        );

        var resultadosReferencia = ObtenerResultados(partidosReferencia);

        var existeResultadoDistinto = false;

        // Act
        for (int semilla = 2; semilla <= 20; semilla++)
        {
            var partidos = CrearPartidosSinResultado(10);

            var configuracionTorneo = new ConfiguracionTorneo
            {
                SemillaSimulation = semilla
            };

            servicioSimulacion.SimularResultados(
                partidos,
                configuracionTorneo
            );

            var resultados = ObtenerResultados(partidos);

            if (!resultados.SequenceEqual(resultadosReferencia))
            {
                existeResultadoDistinto = true;
                break;
            }
        }

        // Assert
        Assert.IsTrue(existeResultadoDistinto);
    }

    [TestMethod]
    public void SimularResultados_ConPartidoYaJugado_NoDebeSobrescribirResultado()
    {
        // Arrange
        var servicioSimulacion = new ServicioSimulacion();
        var partidos = CrearPartidosSinResultado(3);

        partidos[0].GolesLocal = 4;
        partidos[0].GolesVisitante = 2;

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaSimulation = 1
        };

        // Act
        servicioSimulacion.SimularResultados(partidos, configuracionTorneo);

        // Assert
        Assert.AreEqual(4, partidos[0].GolesLocal);
        Assert.AreEqual(2, partidos[0].GolesVisitante);

        Assert.IsTrue(partidos[1].GolesLocal.HasValue);
        Assert.IsTrue(partidos[1].GolesVisitante.HasValue);
        Assert.IsTrue(partidos[2].GolesLocal.HasValue);
        Assert.IsTrue(partidos[2].GolesVisitante.HasValue);
    }

    [TestMethod]
    public void SimularResultados_ConPartidosEliminatorios_NoDebeGenerarEmpates()
    {
        // Arrange
        var servicioSimulacion = new ServicioSimulacion();
        var partidos = CrearPartidosEliminatoriosSinResultado(100);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaSimulation = 1
        };

        // Act
        servicioSimulacion.SimularResultados(partidos, configuracionTorneo);

        // Assert
        Assert.IsTrue(partidos.All(p => p.EstaJugado()));
        Assert.IsTrue(partidos.All(p => !p.EsEmpate()));
        Assert.IsTrue(partidos.All(p => p.Ganador() != null));
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
        var auditoria = new ServicioAuditoria(new RepositorioAuditoriaEnMemoria());
        _servicio = new ServicioUsuario(_repositorio, auditoria);
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

    // IRepositorio<Usuario>

    [TestMethod]
    public void Agregar_UsuarioValido_SeAgregaAlRepositorio()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");

        // Act
        _servicio.Agregar(usuario);

        // Assert
        Assert.AreEqual(1, _servicio.ObtenerTodosLosUsuarios().Count);
    }

    [TestMethod]
    public void Agregar_EmailDuplicado_LanzaExcepcion()
    {
        // Arrange
        var usuario1 = CrearUsuarioValido("juan@example.com");
        var usuario2 = CrearUsuarioValido("juan@example.com");
        _servicio.Agregar(usuario1);

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => _servicio.Agregar(usuario2));
    }

    [TestMethod]
    public void Actualizar_UsuarioExistente_PersisteCambios()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario);
        usuario.AgregarRol(RolUsuario.AdministradorDelSistema);

        // Act
        _servicio.Actualizar(usuario);

        // Assert
        Assert.IsTrue(_servicio.Buscar(u => u.Email == "juan@example.com").First().TieneRol(RolUsuario.AdministradorDelSistema));
    }

    [TestMethod]
    public void Actualizar_UsuarioNoExistente_LanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");

        // Act & Assert
        Assert.ThrowsException<EntidadInexistenteExcepcion>(() => _servicio.Actualizar(usuario));
    }

    [TestMethod]
    public void Actualizar_UsuarioNulo_LanzaExcepcion()
    {
        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() => _servicio.Actualizar(null!));
    }

    [TestMethod]
    public void Eliminar_UsuarioExistente_SeEliminaDelRepositorio()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario);

        // Act
        _servicio.Eliminar(usuario);

        // Assert
        Assert.AreEqual(0, _servicio.ObtenerTodosLosUsuarios().Count);
    }

    [TestMethod]
    public void Buscar_ConPredicadoCoincidente_DevuelveUsuariosCorrespondientes()
    {
        // Arrange
        var admin = CrearUsuarioValido("admin@example.com");
        admin.AgregarRol(RolUsuario.AdministradorDelSistema);
        _servicio.AgregarUsuario(admin);
        _servicio.AgregarUsuario(CrearUsuarioValido("editor@example.com"));

        // Act
        var resultado = _servicio.Buscar(u => u.TieneRol(RolUsuario.AdministradorDelSistema));

        // Assert
        Assert.AreEqual(1, resultado.Count);
        Assert.AreSame(admin, resultado[0]);
    }

    [TestMethod]
    public void Buscar_SinCoincidencias_DevuelveListaVacia()
    {
        // Arrange
        _servicio.AgregarUsuario(CrearUsuarioValido("juan@example.com"));

        // Act
        var resultado = _servicio.Buscar(u => u.TieneRol(RolUsuario.AdministradorDelSistema));

        // Assert
        Assert.AreEqual(0, resultado.Count);
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

End of WorldCupPlanner.ServicioTests

## WorldCupPlanner.UI

### SesionServicio.cs

Ruta: WorldCupPlanner.UI\Data\SesionServicio.cs

~~~csharp
using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.UI.Data
{
    public class SesionServicio
    {
        public Usuario? UsuarioActual { get; private set; }

        public bool HaySesion => UsuarioActual != null;

        public void IniciarSesion(Usuario usuario)
        {
            UsuarioActual = usuario;
        }

        public void CerrarSesion()
        {
            UsuarioActual = null;
        }
    }
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
builder.Services.AddSingleton<RepositorioUsuarioEnMemoria>();
builder.Services.AddSingleton<ServicioUsuario>();
builder.Services.AddScoped<SesionServicio>();

var app = builder.Build();

var servicioUsuario = app.Services.GetRequiredService<ServicioUsuario>();
var admin = new Usuario(new DatosUsuario { Nombre = "Admin", Apellido = "Admin", Email = "a@a.com", FechaNacimiento = "01-01-1990" });
admin.EstablecerContrasenia("Admin@2026");
servicioUsuario.AgregarUsuario(admin);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
~~~

End of WorldCupPlanner.UI
