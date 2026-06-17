using WorldCupPlanner.AccesoADatos.Repositorios;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.AccesoADatosTest;

[TestClass]
public class RepositorioGrupoSqlTests
{
    private static Equipo CrearEquipo(string nombre, Confederacion confederacion = Confederacion.UEFA)
    {
        return new Equipo { Nombre = nombre, Confederacion = confederacion, RankingFIFA = 1000 };
    }

    [TestMethod]
    public void AgregarGrupo_SinEquipos_DebePersistirGrupo()
    {
        // Arrange
        using var contexto = SqlContextFactory.Crear(nameof(AgregarGrupo_SinEquipos_DebePersistirGrupo));
        var repositorio = new RepositorioGrupoSql(contexto);
        var grupo = new Grupo("A");

        // Act
        repositorio.Agregar(grupo);

        // Assert
        using var contextoVerificacion = SqlContextFactory.Crear(nameof(AgregarGrupo_SinEquipos_DebePersistirGrupo));
        var repositorioVerificacion = new RepositorioGrupoSql(contextoVerificacion);
        var grupos = repositorioVerificacion.Buscar(_ => true);
        Assert.AreEqual(1, grupos.Count);
        Assert.AreEqual("A", grupos[0].Etiqueta);
    }

    [TestMethod]
    public void AgregarGrupo_ConEquipos_DebePersistirEquiposDelGrupo()
    {
        // Arrange
        using var contexto = SqlContextFactory.Crear(nameof(AgregarGrupo_ConEquipos_DebePersistirEquiposDelGrupo));
        var equipo1 = CrearEquipo("Uruguay");
        var equipo2 = CrearEquipo("Argentina");
        contexto.Equipos.Add(equipo1);
        contexto.Equipos.Add(equipo2);
        contexto.SaveChanges();

        var grupo = new Grupo("A", new List<Equipo> { equipo1, equipo2 });
        var repositorio = new RepositorioGrupoSql(contexto);

        // Act
        repositorio.Agregar(grupo);

        // Assert
        using var contextoVerificacion = SqlContextFactory.Crear(nameof(AgregarGrupo_ConEquipos_DebePersistirEquiposDelGrupo));
        var repositorioVerificacion = new RepositorioGrupoSql(contextoVerificacion);
        var grupos = repositorioVerificacion.Buscar(g => g.Etiqueta == "A");
        Assert.AreEqual(1, grupos.Count);
        Assert.AreEqual(2, grupos[0].Equipos.Count);
    }

    [TestMethod]
    public void BuscarGrupo_PorEtiqueta_DebeRetornarGrupoCorrecto()
    {
        // Arrange
        using var contexto = SqlContextFactory.Crear(nameof(BuscarGrupo_PorEtiqueta_DebeRetornarGrupoCorrecto));
        var repositorio = new RepositorioGrupoSql(contexto);
        repositorio.Agregar(new Grupo("A"));
        repositorio.Agregar(new Grupo("B"));

        // Act
        var resultado = repositorio.Buscar(g => g.Etiqueta == "B");

        // Assert
        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual("B", resultado[0].Etiqueta);
    }

    [TestMethod]
    public void BuscarGrupos_SinCoincidencias_DebeRetornarListaVacia()
    {
        // Arrange
        using var contexto = SqlContextFactory.Crear(nameof(BuscarGrupos_SinCoincidencias_DebeRetornarListaVacia));
        var repositorio = new RepositorioGrupoSql(contexto);
        repositorio.Agregar(new Grupo("A"));

        // Act
        var resultado = repositorio.Buscar(g => g.Etiqueta == "Z");

        // Assert
        Assert.AreEqual(0, resultado.Count);
    }

    [TestMethod]
    public void ActualizarGrupo_NoExistente_LanzaExcepcion()
    {
        // Arrange
        using var contexto = SqlContextFactory.Crear(nameof(ActualizarGrupo_NoExistente_LanzaExcepcion));
        var repositorio = new RepositorioGrupoSql(contexto);
        var grupo = new Grupo("A");

        // Act & Assert
        Assert.ThrowsException<EntidadInexistenteExcepcion>(() => repositorio.Actualizar(grupo));
    }

    [TestMethod]
    public void EliminarGrupo_Existente_EliminaGrupo()
    {
        // Arrange
        using var contexto = SqlContextFactory.Crear(nameof(EliminarGrupo_Existente_EliminaGrupo));
        var repositorio = new RepositorioGrupoSql(contexto);
        var grupo = new Grupo("A");
        repositorio.Agregar(grupo);

        // Act
        repositorio.Eliminar(grupo);

        // Assert
        var grupos = repositorio.Buscar(_ => true);
        Assert.AreEqual(0, grupos.Count);
    }

    [TestMethod]
    public void EliminarGrupo_NoExistente_LanzaExcepcion()
    {
        // Arrange
        using var contexto = SqlContextFactory.Crear(nameof(EliminarGrupo_NoExistente_LanzaExcepcion));
        var repositorio = new RepositorioGrupoSql(contexto);
        var grupo = new Grupo("A");

        // Act & Assert
        Assert.ThrowsException<EntidadInexistenteExcepcion>(() => repositorio.Eliminar(grupo));
    }

    [TestMethod]
    public void AgregarVariosGrupos_DebePersistirTodos()
    {
        // Arrange
        using var contexto = SqlContextFactory.Crear(nameof(AgregarVariosGrupos_DebePersistirTodos));
        var repositorio = new RepositorioGrupoSql(contexto);

        // Act
        foreach (var etiqueta in new[] { "A", "B", "C", "D" })
            repositorio.Agregar(new Grupo(etiqueta));

        // Assert
        using var contextoVerificacion = SqlContextFactory.Crear(nameof(AgregarVariosGrupos_DebePersistirTodos));
        var repositorioVerificacion = new RepositorioGrupoSql(contextoVerificacion);
        var grupos = repositorioVerificacion.Buscar(_ => true);
        Assert.AreEqual(4, grupos.Count);
    }

    [TestMethod]
    public void AgregarGrupo_DespuesDeEliminarConMismosEquipos_NoDebeDuplicarTracking()
    {
        // Arrange
        using var contexto = SqlContextFactory.Crear(nameof(AgregarGrupo_DespuesDeEliminarConMismosEquipos_NoDebeDuplicarTracking));
        var equipos = new List<Equipo>
        {
            CrearEquipo("Uruguay", Confederacion.CONMEBOL),
            CrearEquipo("Argentina", Confederacion.CONMEBOL),
            CrearEquipo("Francia"),
            CrearEquipo("Espana")
        };
        contexto.Equipos.AddRange(equipos);
        contexto.SaveChanges();

        var repositorio = new RepositorioGrupoSql(contexto);
        var grupoAnterior = new Grupo("A", equipos);
        repositorio.Agregar(grupoAnterior);
        repositorio.Eliminar(grupoAnterior);

        var equiposDelNuevoTorneo = equipos
            .Select(e => new Equipo
            {
                Id = e.Id,
                Nombre = e.Nombre,
                RankingFIFA = e.RankingFIFA,
                Confederacion = e.Confederacion,
                Bandera = e.Bandera
            })
            .ToList();
        var grupoNuevo = new Grupo("B", equiposDelNuevoTorneo);

        // Act
        repositorio.Agregar(grupoNuevo);

        // Assert
        var grupos = repositorio.Buscar(g => g.Etiqueta == "B");
        Assert.AreEqual(1, grupos.Count);
        Assert.AreEqual(4, grupos[0].Equipos.Count);
    }
}
