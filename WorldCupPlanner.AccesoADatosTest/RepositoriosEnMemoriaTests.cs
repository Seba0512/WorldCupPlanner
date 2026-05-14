using WorldCupPlanner.AccesoADatos.Repositorios;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;

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

        Assert.ThrowsException<EntidadInexistenteExcepcion>(() => repositorio.Actualizar(usuario));
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

    // RepositorioEquipoEnMemoria

    [TestMethod]
    public void RepositorioEquipoEnMemoria_ActualizarEquipoExistente_ModificaEquipo()
    {
        var repositorio = new RepositorioEquipoEnMemoria();
        var equipo = CrearEquipoValido("Uruguay");
        repositorio.Agregar(equipo);
        equipo.Nombre = "Uruguay Modificado";

        repositorio.Actualizar(equipo);

        Assert.AreEqual("Uruguay Modificado", repositorio.Buscar(_ => true).First().Nombre);
    }

    [TestMethod]
    public void RepositorioEquipoEnMemoria_ActualizarEquipoNoExistente_LanzaExcepcion()
    {
        var repositorio = new RepositorioEquipoEnMemoria();
        var equipo = CrearEquipoValido("Uruguay");

        Assert.ThrowsException<EntidadInexistenteExcepcion>(() => repositorio.Actualizar(equipo));
    }

    [TestMethod]
    public void RepositorioEquipoEnMemoria_BuscarConPredicado_DevuelveSoloCoincidencias()
    {
        var repositorio = new RepositorioEquipoEnMemoria();
        var equipo1 = CrearEquipoValido("Uruguay");
        var equipo2 = CrearEquipoValido("Argentina");
        repositorio.Agregar(equipo1);
        repositorio.Agregar(equipo2);

        var resultado = repositorio.Buscar(e => e.Nombre == "Uruguay");

        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual("Uruguay", resultado.First().Nombre);
    }

    [TestMethod]
    public void RepositorioEquipoEnMemoria_BuscarSinCoincidencias_DevuelveListaVacia()
    {
        var repositorio = new RepositorioEquipoEnMemoria();
        repositorio.Agregar(CrearEquipoValido("Uruguay"));

        var resultado = repositorio.Buscar(e => e.Nombre == "Brasil");

        Assert.AreEqual(0, resultado.Count);
    }

    // RepositorioEstadioEnMemoria

    [TestMethod]
    public void RepositorioEstadioEnMemoria_AgregarEstadio_GuardaEstadio()
    {
        var repositorio = new RepositorioEstadioEnMemoria();
        var estadio = CrearEstadioValido("Estadio Centenario");

        repositorio.Agregar(estadio);

        Assert.AreEqual(1, repositorio.Buscar(_ => true).Count);
        Assert.AreEqual(estadio, repositorio.Buscar(_ => true).First());
    }

    [TestMethod]
    public void RepositorioEstadioEnMemoria_ActualizarEstadioNoExistente_LanzaExcepcion()
    {
        var repositorio = new RepositorioEstadioEnMemoria();
        var estadio = CrearEstadioValido("Estadio Centenario");

        Assert.ThrowsException<EntidadInexistenteExcepcion>(() => repositorio.Actualizar(estadio));
    }

    [TestMethod]
    public void RepositorioEstadioEnMemoria_EliminarEstadioExistente_EliminaEstadio()
    {
        var repositorio = new RepositorioEstadioEnMemoria();
        var estadio = CrearEstadioValido("Estadio Centenario");
        repositorio.Agregar(estadio);

        repositorio.Eliminar(estadio);

        Assert.AreEqual(0, repositorio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void RepositorioEstadioEnMemoria_EliminarEstadioNoExistente_NoLanzaExcepcion()
    {
        var repositorio = new RepositorioEstadioEnMemoria();
        var estadio = CrearEstadioValido("Estadio Centenario");

        repositorio.Eliminar(estadio);

        Assert.AreEqual(0, repositorio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void RepositorioEstadioEnMemoria_BuscarConPredicado_DevuelveSoloCoincidencias()
    {
        var repositorio = new RepositorioEstadioEnMemoria();
        var estadio1 = CrearEstadioValido("Estadio Centenario");
        var estadio2 = CrearEstadioValido("Estadio Olimpico");
        repositorio.Agregar(estadio1);
        repositorio.Agregar(estadio2);

        var resultado = repositorio.Buscar(e => e.Nombre == "Estadio Centenario");

        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual("Estadio Centenario", resultado.First().Nombre);
    }

    [TestMethod]
    public void RepositorioEstadioEnMemoria_BuscarSinCoincidencias_DevuelveListaVacia()
    {
        var repositorio = new RepositorioEstadioEnMemoria();
        repositorio.Agregar(CrearEstadioValido("Estadio Centenario"));

        var resultado = repositorio.Buscar(e => e.Nombre == "Estadio Inexistente");

        Assert.AreEqual(0, resultado.Count);
    }

    // RepositorioGrupoEnMemoria

    [TestMethod]
    public void RepositorioGrupoEnMemoria_ActualizarGrupoExistente_ModificaGrupo()
    {
        var repositorio = new RepositorioGrupoEnMemoria();
        var grupo = new Grupo("A");
        repositorio.Agregar(grupo);

        repositorio.Actualizar(grupo);

        Assert.AreEqual(1, repositorio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void RepositorioGrupoEnMemoria_ActualizarGrupoNoExistente_LanzaExcepcion()
    {
        var repositorio = new RepositorioGrupoEnMemoria();
        var grupo = new Grupo("A");

        Assert.ThrowsException<EntidadInexistenteExcepcion>(() => repositorio.Actualizar(grupo));
    }

    [TestMethod]
    public void RepositorioGrupoEnMemoria_EliminarGrupoExistente_EliminaGrupo()
    {
        var repositorio = new RepositorioGrupoEnMemoria();
        var grupo = new Grupo("A");
        repositorio.Agregar(grupo);

        repositorio.Eliminar(grupo);

        Assert.AreEqual(0, repositorio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void RepositorioGrupoEnMemoria_EliminarGrupoNoExistente_NoLanzaExcepcion()
    {
        var repositorio = new RepositorioGrupoEnMemoria();
        var grupo = new Grupo("A");

        repositorio.Eliminar(grupo);

        Assert.AreEqual(0, repositorio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void RepositorioGrupoEnMemoria_BuscarConPredicado_DevuelveSoloCoincidencias()
    {
        var repositorio = new RepositorioGrupoEnMemoria();
        var grupoA = new Grupo("A");
        var grupoB = new Grupo("B");
        repositorio.Agregar(grupoA);
        repositorio.Agregar(grupoB);

        var resultado = repositorio.Buscar(g => g.Etiqueta == "A");

        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual("A", resultado.First().Etiqueta);
    }

    [TestMethod]
    public void RepositorioGrupoEnMemoria_BuscarSinCoincidencias_DevuelveListaVacia()
    {
        var repositorio = new RepositorioGrupoEnMemoria();
        repositorio.Agregar(new Grupo("A"));

        var resultado = repositorio.Buscar(g => g.Etiqueta == "Z");

        Assert.AreEqual(0, resultado.Count);
    }

    // RepositorioPartidoEnMemoria

    [TestMethod]
    public void RepositorioPartidoEnMemoria_ActualizarPartidoExistente_ModificaPartido()
    {
        var repositorio = new RepositorioPartidoEnMemoria();
        var partido = CrearPartidoValido();
        repositorio.Agregar(partido);

        repositorio.Actualizar(partido);

        Assert.AreEqual(1, repositorio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void RepositorioPartidoEnMemoria_ActualizarPartidoNoExistente_LanzaExcepcion()
    {
        var repositorio = new RepositorioPartidoEnMemoria();
        var partido = CrearPartidoValido();

        Assert.ThrowsException<EntidadInexistenteExcepcion>(() => repositorio.Actualizar(partido));
    }

    [TestMethod]
    public void RepositorioPartidoEnMemoria_EliminarPartidoExistente_EliminaPartido()
    {
        var repositorio = new RepositorioPartidoEnMemoria();
        var partido = CrearPartidoValido();
        repositorio.Agregar(partido);

        repositorio.Eliminar(partido);

        Assert.AreEqual(0, repositorio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void RepositorioPartidoEnMemoria_EliminarPartidoNoExistente_NoLanzaExcepcion()
    {
        var repositorio = new RepositorioPartidoEnMemoria();
        var partido = CrearPartidoValido();

        repositorio.Eliminar(partido);

        Assert.AreEqual(0, repositorio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void RepositorioPartidoEnMemoria_BuscarConPredicado_DevuelveSoloCoincidencias()
    {
        var repositorio = new RepositorioPartidoEnMemoria();
        var partido1 = CrearPartidoValido();
        partido1.Id = 1;
        var partido2 = CrearPartidoValido();
        partido2.Id = 2;
        repositorio.Agregar(partido1);
        repositorio.Agregar(partido2);

        var resultado = repositorio.Buscar(p => p.Id == 1);

        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual(1, resultado.First().Id);
    }

    [TestMethod]
    public void RepositorioPartidoEnMemoria_BuscarSinCoincidencias_DevuelveListaVacia()
    {
        var repositorio = new RepositorioPartidoEnMemoria();
        repositorio.Agregar(CrearPartidoValido());

        var resultado = repositorio.Buscar(p => p.Id == 99);

        Assert.AreEqual(0, resultado.Count);
    }

    // RepositorioAuditoriaEnMemoria

    [TestMethod]
    public void RepositorioAuditoriaEnMemoria_SiguienteId_IncrementaContador()
    {
        var repositorio = new RepositorioAuditoriaEnMemoria();

        var id1 = repositorio.SiguienteId();
        var id2 = repositorio.SiguienteId();
        var id3 = repositorio.SiguienteId();

        Assert.AreEqual(1, id1);
        Assert.AreEqual(2, id2);
        Assert.AreEqual(3, id3);
    }

    [TestMethod]
    public void RepositorioAuditoriaEnMemoria_AgregarLog_GuardaLog()
    {
        var repositorio = new RepositorioAuditoriaEnMemoria();
        var log = CrearAuditoriaValida(1);

        repositorio.Agregar(log);

        Assert.AreEqual(1, repositorio.Buscar(_ => true).Count);
        Assert.AreEqual(log, repositorio.Buscar(_ => true).First());
    }

    [TestMethod]
    public void RepositorioAuditoriaEnMemoria_ActualizarLogExistente_ModificaLog()
    {
        var repositorio = new RepositorioAuditoriaEnMemoria();
        var id = repositorio.SiguienteId();
        var log = CrearAuditoriaValida(id);
        repositorio.Agregar(log);
        var logActualizado = new Auditoria(id, "admin", "UPDATE", "detalle actualizado");

        repositorio.Actualizar(logActualizado);

        Assert.AreEqual("detalle actualizado", repositorio.Buscar(_ => true).First().Detalle);
    }

    [TestMethod]
    public void RepositorioAuditoriaEnMemoria_ActualizarLogNoExistente_LanzaExcepcion()
    {
        var repositorio = new RepositorioAuditoriaEnMemoria();
        var log = CrearAuditoriaValida(99);

        Assert.ThrowsException<EntidadInexistenteExcepcion>(() => repositorio.Actualizar(log));
    }

    [TestMethod]
    public void RepositorioAuditoriaEnMemoria_EliminarLogExistente_EliminaLog()
    {
        var repositorio = new RepositorioAuditoriaEnMemoria();
        var log = CrearAuditoriaValida(1);
        repositorio.Agregar(log);

        repositorio.Eliminar(log);

        Assert.AreEqual(0, repositorio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void RepositorioAuditoriaEnMemoria_EliminarLogNoExistente_NoLanzaExcepcion()
    {
        var repositorio = new RepositorioAuditoriaEnMemoria();
        var log = CrearAuditoriaValida(1);

        repositorio.Eliminar(log);

        Assert.AreEqual(0, repositorio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void RepositorioAuditoriaEnMemoria_BuscarConPredicado_DevuelveSoloCoincidencias()
    {
        var repositorio = new RepositorioAuditoriaEnMemoria();
        var log1 = CrearAuditoriaValida(1);
        var log2 = new Auditoria(2, "admin", "DELETE", "detalle2");
        repositorio.Agregar(log1);
        repositorio.Agregar(log2);

        var resultado = repositorio.Buscar(l => l.Accion == "LOGIN");

        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual("LOGIN", resultado.First().Accion);
    }

    [TestMethod]
    public void RepositorioAuditoriaEnMemoria_BuscarSinCoincidencias_DevuelveListaVacia()
    {
        var repositorio = new RepositorioAuditoriaEnMemoria();
        repositorio.Agregar(CrearAuditoriaValida(1));

        var resultado = repositorio.Buscar(l => l.Accion == "INEXISTENTE");

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
