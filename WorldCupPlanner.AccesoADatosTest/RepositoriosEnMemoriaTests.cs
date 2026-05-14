using WorldCupPlanner.AccesoADatos.Repositorios;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.AccesoADatosTest;

[TestClass]
public class RepositoriosEnMemoriaTests
{
    [TestMethod]
    public void Equipo_Agregar_GuardaEquipo()
    {
        // Arrange
        var repositorio = new RepositorioEquipoEnMemoria();
        var equipo = CrearEquipoValido("Uruguay");

        // Act
        repositorio.Agregar(equipo);

        // Assert
        Assert.AreEqual(1, repositorio.Buscar(_ => true).Count);
        Assert.AreEqual(equipo, repositorio.Buscar(_ => true).First());
    }

    [TestMethod]
    public void Equipo_Eliminar_EliminaEquipo()
    {
        // Arrange
        var repositorio = new RepositorioEquipoEnMemoria();
        var equipo = CrearEquipoValido("Uruguay");
        repositorio.Agregar(equipo);

        // Act
        repositorio.Eliminar(equipo);

        // Assert
        Assert.AreEqual(0, repositorio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void Equipo_ActualizarExistente_ModificaEquipo()
    {
        // Arrange
        var repositorio = new RepositorioEquipoEnMemoria();
        var equipo = CrearEquipoValido("Uruguay");
        repositorio.Agregar(equipo);
        equipo.Nombre = "Uruguay Modificado";

        // Act
        repositorio.Actualizar(equipo);

        // Assert
        Assert.AreEqual("Uruguay Modificado", repositorio.Buscar(_ => true).First().Nombre);
    }

    [TestMethod]
    public void Equipo_ActualizarNoExistente_LanzaExcepcion()
    {
        // Arrange
        var repositorio = new RepositorioEquipoEnMemoria();
        var equipo = CrearEquipoValido("Uruguay");

        // Act & Assert
        Assert.ThrowsException<EntidadInexistenteExcepcion>(() => repositorio.Actualizar(equipo));
    }

    [TestMethod]
    public void Equipo_BuscarConPredicado_DevuelveSoloCoincidencias()
    {
        // Arrange
        var repositorio = new RepositorioEquipoEnMemoria();
        var equipo1 = CrearEquipoValido("Uruguay");
        var equipo2 = CrearEquipoValido("Argentina");
        repositorio.Agregar(equipo1);
        repositorio.Agregar(equipo2);

        // Act
        var resultado = repositorio.Buscar(e => e.Nombre == "Uruguay");

        // Assert
        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual("Uruguay", resultado.First().Nombre);
    }

    [TestMethod]
    public void Equipo_BuscarSinCoincidencias_DevuelveListaVacia()
    {
        // Arrange
        var repositorio = new RepositorioEquipoEnMemoria();
        repositorio.Agregar(CrearEquipoValido("Uruguay"));

        // Act
        var resultado = repositorio.Buscar(e => e.Nombre == "Brasil");

        // Assert
        Assert.AreEqual(0, resultado.Count);
    }

    // RepositorioEstadioEnMemoria

    [TestMethod]
    public void Estadio_Agregar_GuardaEstadio()
    {
        // Arrange
        var repositorio = new RepositorioEstadioEnMemoria();
        var estadio = CrearEstadioValido("Estadio Centenario");

        // Act
        repositorio.Agregar(estadio);

        // Assert
        Assert.AreEqual(1, repositorio.Buscar(_ => true).Count);
        Assert.AreEqual(estadio, repositorio.Buscar(_ => true).First());
    }

    [TestMethod]
    public void Estadio_Actualizar_ModificaEstadio()
    {
        // Arrange
        var repositorio = new RepositorioEstadioEnMemoria();
        var estadio = CrearEstadioValido("Estadio Centenario");
        repositorio.Agregar(estadio);
        estadio.Nombre = "Estadio Olimpico";

        // Act
        repositorio.Actualizar(estadio);

        // Assert
        Assert.AreEqual("Estadio Olimpico", repositorio.Buscar(_ => true).First().Nombre);
    }

    [TestMethod]
    public void Estadio_ActualizarNoExistente_LanzaExcepcion()
    {
        // Arrange
        var repositorio = new RepositorioEstadioEnMemoria();
        var estadio = CrearEstadioValido("Estadio Centenario");

        // Act & Assert
        Assert.ThrowsException<EntidadInexistenteExcepcion>(() => repositorio.Actualizar(estadio));
    }

    [TestMethod]
    public void Estadio_EliminarExistente_EliminaEstadio()
    {
        // Arrange
        var repositorio = new RepositorioEstadioEnMemoria();
        var estadio = CrearEstadioValido("Estadio Centenario");
        repositorio.Agregar(estadio);

        // Act
        repositorio.Eliminar(estadio);

        // Assert
        Assert.AreEqual(0, repositorio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void Estadio_EliminarNoExistente_NoLanzaExcepcion()
    {
        // Arrange
        var repositorio = new RepositorioEstadioEnMemoria();
        var estadio = CrearEstadioValido("Estadio Centenario");

        // Act
        repositorio.Eliminar(estadio);

        // Assert
        Assert.AreEqual(0, repositorio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void Estadio_BuscarConPredicado_DevuelveSoloCoincidencias()
    {
        // Arrange
        var repositorio = new RepositorioEstadioEnMemoria();
        var estadio1 = CrearEstadioValido("Estadio Centenario");
        var estadio2 = CrearEstadioValido("Estadio Olimpico");
        repositorio.Agregar(estadio1);
        repositorio.Agregar(estadio2);

        // Act
        var resultado = repositorio.Buscar(e => e.Nombre == "Estadio Centenario");

        // Assert
        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual("Estadio Centenario", resultado.First().Nombre);
    }

    [TestMethod]
    public void Estadio_BuscarSinCoincidencias_DevuelveListaVacia()
    {
        // Arrange
        var repositorio = new RepositorioEstadioEnMemoria();
        repositorio.Agregar(CrearEstadioValido("Estadio Centenario"));

        // Act
        var resultado = repositorio.Buscar(e => e.Nombre == "Estadio Inexistente");

        // Assert
        Assert.AreEqual(0, resultado.Count);
    }

    // RepositorioGrupoEnMemoria

    [TestMethod]
    public void Grupo_Agregar_GuardaGrupo()
    {
        // Arrange
        var repositorio = new RepositorioGrupoEnMemoria();
        var grupo = new Grupo("A");

        // Act
        repositorio.Agregar(grupo);

        // Assert
        Assert.AreEqual(1, repositorio.Buscar(_ => true).Count);
        Assert.AreEqual(grupo, repositorio.Buscar(_ => true).First());
    }

    [TestMethod]
    public void Grupo_ActualizarExistente_ModificaGrupo()
    {
        // Arrange
        var repositorio = new RepositorioGrupoEnMemoria();
        var grupo = new Grupo("A");
        repositorio.Agregar(grupo);

        // Act
        repositorio.Actualizar(grupo);

        // Assert
        Assert.AreEqual(1, repositorio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void Grupo_ActualizarNoExistente_LanzaExcepcion()
    {
        // Arrange
        var repositorio = new RepositorioGrupoEnMemoria();
        var grupo = new Grupo("A");

        // Act & Assert
        Assert.ThrowsException<EntidadInexistenteExcepcion>(() => repositorio.Actualizar(grupo));
    }

    [TestMethod]
    public void Grupo_EliminarExistente_EliminaGrupo()
    {
        // Arrange
        var repositorio = new RepositorioGrupoEnMemoria();
        var grupo = new Grupo("A");
        repositorio.Agregar(grupo);

        // Act
        repositorio.Eliminar(grupo);

        // Assert
        Assert.AreEqual(0, repositorio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void Grupo_EliminarNoExistente_NoLanzaExcepcion()
    {
        // Arrange
        var repositorio = new RepositorioGrupoEnMemoria();
        var grupo = new Grupo("A");

        // Act
        repositorio.Eliminar(grupo);

        // Assert
        Assert.AreEqual(0, repositorio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void Grupo_BuscarConPredicado_DevuelveSoloCoincidencias()
    {
        // Arrange
        var repositorio = new RepositorioGrupoEnMemoria();
        var grupoA = new Grupo("A");
        var grupoB = new Grupo("B");
        repositorio.Agregar(grupoA);
        repositorio.Agregar(grupoB);

        // Act
        var resultado = repositorio.Buscar(g => g.Etiqueta == "A");

        // Assert
        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual("A", resultado.First().Etiqueta);
    }

    [TestMethod]
    public void Grupo_BuscarSinCoincidencias_DevuelveListaVacia()
    {
        // Arrange
        var repositorio = new RepositorioGrupoEnMemoria();
        repositorio.Agregar(new Grupo("A"));

        // Act
        var resultado = repositorio.Buscar(g => g.Etiqueta == "Z");

        // Assert
        Assert.AreEqual(0, resultado.Count);
    }

    // RepositorioPartidoEnMemoria

    [TestMethod]
    public void Partido_Agregar_GuardaPartido()
    {
        // Arrange
        var repositorio = new RepositorioPartidoEnMemoria();
        var partido = CrearPartidoValido();

        // Act
        repositorio.Agregar(partido);

        // Assert
        Assert.AreEqual(1, repositorio.Buscar(_ => true).Count);
        Assert.AreEqual(partido, repositorio.Buscar(_ => true).First());
    }

    [TestMethod]
    public void Partido_ActualizarExistente_ModificaPartido()
    {
        // Arrange
        var repositorio = new RepositorioPartidoEnMemoria();
        var partido = CrearPartidoValido();
        repositorio.Agregar(partido);

        // Act
        repositorio.Actualizar(partido);

        // Assert
        Assert.AreEqual(1, repositorio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void Partido_ActualizarNoExistente_LanzaExcepcion()
    {
        // Arrange
        var repositorio = new RepositorioPartidoEnMemoria();
        var partido = CrearPartidoValido();

        // Act & Assert
        Assert.ThrowsException<EntidadInexistenteExcepcion>(() => repositorio.Actualizar(partido));
    }

    [TestMethod]
    public void Partido_EliminarExistente_EliminaPartido()
    {
        // Arrange
        var repositorio = new RepositorioPartidoEnMemoria();
        var partido = CrearPartidoValido();
        repositorio.Agregar(partido);

        // Act
        repositorio.Eliminar(partido);

        // Assert
        Assert.AreEqual(0, repositorio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void Partido_EliminarNoExistente_NoLanzaExcepcion()
    {
        // Arrange
        var repositorio = new RepositorioPartidoEnMemoria();
        var partido = CrearPartidoValido();

        // Act
        repositorio.Eliminar(partido);

        // Assert
        Assert.AreEqual(0, repositorio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void Partido_BuscarConPredicado_DevuelveSoloCoincidencias()
    {
        // Arrange
        var repositorio = new RepositorioPartidoEnMemoria();
        var partido1 = CrearPartidoValido();
        partido1.Id = 1;
        var partido2 = CrearPartidoValido();
        partido2.Id = 2;
        repositorio.Agregar(partido1);
        repositorio.Agregar(partido2);

        // Act
        var resultado = repositorio.Buscar(p => p.Id == 1);

        // Assert
        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual(1, resultado.First().Id);
    }

    [TestMethod]
    public void Partido_BuscarSinCoincidencias_DevuelveListaVacia()
    {
        // Arrange
        var repositorio = new RepositorioPartidoEnMemoria();
        repositorio.Agregar(CrearPartidoValido());

        // Act
        var resultado = repositorio.Buscar(p => p.Id == 99);

        // Assert
        Assert.AreEqual(0, resultado.Count);
    }

    // RepositorioUsuarioEnMemoria

    [TestMethod]
    public void Usuario_Agregar_GuardaUsuario()
    {
        // Arrange
        var repositorio = new RepositorioUsuarioEnMemoria();
        var usuario = CrearUsuarioValido("juan@example.com");

        // Act
        repositorio.Agregar(usuario);

        // Assert
        Assert.AreEqual(1, repositorio.Buscar(_ => true).Count);
        Assert.AreSame(usuario, repositorio.Buscar(_ => true).First());
    }

    [TestMethod]
    public void Usuario_AgregarVarios_GuardaTodos()
    {
        // Arrange
        var repositorio = new RepositorioUsuarioEnMemoria();
        var usuario1 = CrearUsuarioValido("juan@example.com");
        var usuario2 = CrearUsuarioValido("maria@example.com");
        var usuario3 = CrearUsuarioValido("pedro@example.com");

        // Act
        repositorio.Agregar(usuario1);
        repositorio.Agregar(usuario2);
        repositorio.Agregar(usuario3);

        // Assert
        Assert.AreEqual(3, repositorio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void Usuario_ActualizarExistente_ModificaUsuario()
    {
        // Arrange
        var repositorio = new RepositorioUsuarioEnMemoria();
        var usuario = CrearUsuarioValido("juan@example.com");
        repositorio.Agregar(usuario);
        usuario.AgregarRol(RolUsuario.AdministradorDelSistema);

        // Act
        repositorio.Actualizar(usuario);

        // Assert
        Assert.IsTrue(repositorio.Buscar(_ => true).First().TieneRol(RolUsuario.AdministradorDelSistema));
    }

    [TestMethod]
    public void Usuario_ActualizarNoExistente_LanzaExcepcion()
    {
        // Arrange
        var repositorio = new RepositorioUsuarioEnMemoria();
        var usuario = CrearUsuarioValido("juan@example.com");

        // Act & Assert
        Assert.ThrowsException<EntidadInexistenteExcepcion>(() => repositorio.Actualizar(usuario));
    }

    [TestMethod]
    public void Usuario_EliminarExistente_EliminaUsuario()
    {
        // Arrange
        var repositorio = new RepositorioUsuarioEnMemoria();
        var usuario = CrearUsuarioValido("juan@example.com");
        repositorio.Agregar(usuario);

        // Act
        repositorio.Eliminar(usuario);

        // Assert
        Assert.AreEqual(0, repositorio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void Usuario_EliminarNoExistente_NoLanzaExcepcion()
    {
        // Arrange
        var repositorio = new RepositorioUsuarioEnMemoria();
        var usuario = CrearUsuarioValido("juan@example.com");

        // Act
        repositorio.Eliminar(usuario);

        // Assert
        Assert.AreEqual(0, repositorio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void Usuario_BuscarSinEntidades_DevuelveListaVacia()
    {
        // Arrange
        var repositorio = new RepositorioUsuarioEnMemoria();

        // Act
        var usuarios = repositorio.Buscar(_ => true);

        // Assert
        Assert.AreEqual(0, usuarios.Count);
    }

    [TestMethod]
    public void Usuario_BuscarTodos_DevuelveCopia()
    {
        // Arrange
        var repositorio = new RepositorioUsuarioEnMemoria();
        var usuario = CrearUsuarioValido("juan@example.com");
        repositorio.Agregar(usuario);

        // Act
        var usuarios1 = repositorio.Buscar(_ => true);
        var usuarios2 = repositorio.Buscar(_ => true);

        // Assert
        Assert.AreNotSame(usuarios1, usuarios2);
        Assert.AreEqual(usuarios1.Count, usuarios2.Count);
    }

    [TestMethod]
    public void Usuario_ObtenerPorEmailExistente_DevuelveUsuario()
    {
        // Arrange
        var repositorio = new RepositorioUsuarioEnMemoria();
        var usuario = CrearUsuarioValido("juan@example.com");
        repositorio.Agregar(usuario);

        // Act
        var usuarioEncontrado = repositorio.ObtenerPorEmail("juan@example.com");

        // Assert
        Assert.IsNotNull(usuarioEncontrado);
        Assert.AreSame(usuario, usuarioEncontrado);
    }

    [TestMethod]
    public void Usuario_ObtenerPorEmailNoExistente_DevuelveNull()
    {
        // Arrange
        var repositorio = new RepositorioUsuarioEnMemoria();

        // Act
        var usuarioEncontrado = repositorio.ObtenerPorEmail("noexiste@example.com");

        // Assert
        Assert.IsNull(usuarioEncontrado);
    }

    [TestMethod]
    public void Usuario_ObtenerPorEmailCaseInsensitive_DevuelveUsuario()
    {
        // Arrange
        var repositorio = new RepositorioUsuarioEnMemoria();
        var usuario = CrearUsuarioValido("Juan@Example.com");
        repositorio.Agregar(usuario);

        // Act
        var usuarioEncontrado = repositorio.ObtenerPorEmail("juan@example.com");

        // Assert
        Assert.IsNotNull(usuarioEncontrado);
        Assert.AreSame(usuario, usuarioEncontrado);
    }

    // RepositorioAuditoriaEnMemoria

    [TestMethod]
    public void Auditoria_SiguienteId_IncrementaContador()
    {
        // Arrange
        var repositorio = new RepositorioAuditoriaEnMemoria();

        // Act
        var id1 = repositorio.SiguienteId();
        var id2 = repositorio.SiguienteId();
        var id3 = repositorio.SiguienteId();

        // Assert
        Assert.AreEqual(1, id1);
        Assert.AreEqual(2, id2);
        Assert.AreEqual(3, id3);
    }

    [TestMethod]
    public void Auditoria_Agregar_GuardaLog()
    {
        // Arrange
        var repositorio = new RepositorioAuditoriaEnMemoria();
        var log = CrearAuditoriaValida(1);

        // Act
        repositorio.Agregar(log);

        // Assert
        Assert.AreEqual(1, repositorio.Buscar(_ => true).Count);
        Assert.AreEqual(log, repositorio.Buscar(_ => true).First());
    }

    [TestMethod]
    public void Auditoria_ActualizarExistente_ModificaLog()
    {
        // Arrange
        var repositorio = new RepositorioAuditoriaEnMemoria();
        var id = repositorio.SiguienteId();
        var log = CrearAuditoriaValida(id);
        repositorio.Agregar(log);
        var logActualizado = new Auditoria(id, "admin", "UPDATE", "detalle actualizado");

        // Act
        repositorio.Actualizar(logActualizado);

        // Assert
        Assert.AreEqual("detalle actualizado", repositorio.Buscar(_ => true).First().Detalle);
    }

    [TestMethod]
    public void Auditoria_ActualizarNoExistente_LanzaExcepcion()
    {
        // Arrange
        var repositorio = new RepositorioAuditoriaEnMemoria();
        var log = CrearAuditoriaValida(99);

        // Act & Assert
        Assert.ThrowsException<EntidadInexistenteExcepcion>(() => repositorio.Actualizar(log));
    }

    [TestMethod]
    public void Auditoria_EliminarExistente_EliminaLog()
    {
        // Arrange
        var repositorio = new RepositorioAuditoriaEnMemoria();
        var log = CrearAuditoriaValida(1);
        repositorio.Agregar(log);

        // Act
        repositorio.Eliminar(log);

        // Assert
        Assert.AreEqual(0, repositorio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void Auditoria_EliminarNoExistente_NoLanzaExcepcion()
    {
        // Arrange
        var repositorio = new RepositorioAuditoriaEnMemoria();
        var log = CrearAuditoriaValida(1);

        // Act
        repositorio.Eliminar(log);

        // Assert
        Assert.AreEqual(0, repositorio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void Auditoria_BuscarConPredicado_DevuelveSoloCoincidencias()
    {
        // Arrange
        var repositorio = new RepositorioAuditoriaEnMemoria();
        var log1 = CrearAuditoriaValida(1);
        var log2 = new Auditoria(2, "admin", "DELETE", "detalle2");
        repositorio.Agregar(log1);
        repositorio.Agregar(log2);

        // Act
        var resultado = repositorio.Buscar(l => l.Accion == "LOGIN");

        // Assert
        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual("LOGIN", resultado.First().Accion);
    }

    [TestMethod]
    public void Auditoria_BuscarSinCoincidencias_DevuelveListaVacia()
    {
        // Arrange
        var repositorio = new RepositorioAuditoriaEnMemoria();
        repositorio.Agregar(CrearAuditoriaValida(1));

        // Act
        var resultado = repositorio.Buscar(l => l.Accion == "INEXISTENTE");

        // Assert
        Assert.AreEqual(0, resultado.Count);
    }

    private Auditoria CrearAuditoriaValida(int id)
    {
        return new Auditoria(id, "usuario@example.com", "LOGIN", "Inicio de sesión exitoso");
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
            Apellido = "Pérez",
            Email = email,
            FechaNacimiento = "01-01-1990"
        });
    }
}
