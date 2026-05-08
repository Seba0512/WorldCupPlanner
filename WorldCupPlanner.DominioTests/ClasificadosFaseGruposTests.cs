using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.DominioTests;

[TestClass]
public class ClasificadosFaseGruposTests
{
    [TestMethod]
    public void ClasificadosFaseGrupos_Todos_IncluseLosTresListados()
    {
        // Arrange
        var clasificados = new ClasificadosFaseGrupos();
        var primero = CrearEquipo("Uruguay");
        var segundo = CrearEquipo("Argentina");
        var mejorTercero = CrearEquipo("Brasil");

        clasificados.Primeros.Add(primero);
        clasificados.Segundos.Add(segundo);
        clasificados.MejoresTerceros.Add(mejorTercero);

        // Act
        var todos = clasificados.Todos;

        // Assert
        Assert.AreEqual(3, todos.Count);
        Assert.IsTrue(todos.Contains(primero));
        Assert.IsTrue(todos.Contains(segundo));
        Assert.IsTrue(todos.Contains(mejorTercero));
    }

    [TestMethod]
    public void ClasificadosFaseGrupos_Todos_SinEquipos_RetornaListaVacia()
    {
        // Arrange
        var clasificados = new ClasificadosFaseGrupos();

        // Act
        var todos = clasificados.Todos;

        // Assert
        Assert.AreEqual(0, todos.Count);
    }

    [TestMethod]
    public void ClasificadosFaseGrupos_ObtenerGrupoDeOrigen_RetornaGrupoCorrectoCuandoExiste()
    {
        // Arrange
        var clasificados = new ClasificadosFaseGrupos();
        var equipo = CrearEquipo("Uruguay");
        var grupo = new Grupo("A");
        clasificados.GruposDeOrigen[equipo] = grupo;

        // Act
        var resultado = clasificados.ObtenerGrupoDeOrigen(equipo);

        // Assert
        Assert.AreEqual(grupo, resultado);
    }

    [TestMethod]
    public void ClasificadosFaseGrupos_ObtenerGrupoDeOrigen_RetornaNullCuandoNoExiste()
    {
        // Arrange
        var clasificados = new ClasificadosFaseGrupos();
        var equipoSinGrupo = CrearEquipo("Chile");

        // Act
        var resultado = clasificados.ObtenerGrupoDeOrigen(equipoSinGrupo);

        // Assert
        Assert.IsNull(resultado);
    }

    [TestMethod]
    public void ClasificadosFaseGrupos_PropiedadesIniciales_SonListasVacias()
    {
        // Act
        var clasificados = new ClasificadosFaseGrupos();

        // Assert
        Assert.AreEqual(0, clasificados.Primeros.Count);
        Assert.AreEqual(0, clasificados.Segundos.Count);
        Assert.AreEqual(0, clasificados.MejoresTerceros.Count);
        Assert.AreEqual(0, clasificados.GruposDeOrigen.Count);
    }

    private static Equipo CrearEquipo(string nombre)
    {
        return new Equipo { Nombre = nombre, RankingFIFA = 500, Confederacion = Confederacion.CONMEBOL };
    }
}
