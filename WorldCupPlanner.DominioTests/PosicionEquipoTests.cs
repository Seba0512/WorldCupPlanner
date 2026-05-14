using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.DominioTests;

[TestClass]
public class PosicionEquipoTests
{
    [TestMethod]
    public void PosicionEquipo_DiferenciaGoles_EsPositivaCuandoGolesAFavorMayores()
    {
        // Arrange
        var posicion = new PosicionEquipo
        {
            Equipo = CrearEquipo("Uruguay"),
            Grupo = new Grupo("A"),
            Puntos = 6,
            GolesAFavor = 5,
            GolesEnContra = 2
        };

        // Act
        var diferencia = posicion.DiferenciaGoles;

        // Assert
        Assert.AreEqual(3, diferencia);
    }

    [TestMethod]
    public void PosicionEquipo_DiferenciaGoles_EsNegativaCuandoGolesEnContraMayores()
    {
        // Arrange
        var posicion = new PosicionEquipo
        {
            Equipo = CrearEquipo("Uruguay"),
            Grupo = new Grupo("A"),
            Puntos = 0,
            GolesAFavor = 1,
            GolesEnContra = 4
        };

        // Act
        var diferencia = posicion.DiferenciaGoles;

        // Assert
        Assert.AreEqual(-3, diferencia);
    }

    [TestMethod]
    public void PosicionEquipo_DiferenciaGoles_EsCeroCuandoGolesIguales()
    {
        // Arrange
        var posicion = new PosicionEquipo
        {
            Equipo = CrearEquipo("Uruguay"),
            Grupo = new Grupo("A"),
            Puntos = 3,
            GolesAFavor = 2,
            GolesEnContra = 2
        };

        // Act
        var diferencia = posicion.DiferenciaGoles;

        // Assert
        Assert.AreEqual(0, diferencia);
    }

    [TestMethod]
    public void PosicionEquipo_Propiedades_SeEstablecenCorrectamente()
    {
        // Arrange
        var equipo = CrearEquipo("Argentina");
        var grupo = new Grupo("B");

        // Act
        var posicion = new PosicionEquipo
        {
            Equipo = equipo,
            Grupo = grupo,
            Puntos = 9,
            GolesAFavor = 7,
            GolesEnContra = 1
        };

        // Assert
        Assert.AreEqual(equipo, posicion.Equipo);
        Assert.AreEqual(grupo, posicion.Grupo);
        Assert.AreEqual(9, posicion.Puntos);
        Assert.AreEqual(7, posicion.GolesAFavor);
        Assert.AreEqual(1, posicion.GolesEnContra);
    }

    [TestMethod]
    public void PosicionEquipo_ContadoresDePartidos_SeEstablecenCorrectamente()
    {
        // Arrange & Act
        var posicion = new PosicionEquipo
        {
            Equipo = CrearEquipo("Brasil"),
            Grupo = new Grupo("C"),
            PartidosJugados = 3,
            PartidosGanados = 2,
            PartidosEmpatados = 1,
            PartidosPerdidos = 0,
            Puntos = 7
        };

        // Assert
        Assert.AreEqual(3, posicion.PartidosJugados);
        Assert.AreEqual(2, posicion.PartidosGanados);
        Assert.AreEqual(1, posicion.PartidosEmpatados);
        Assert.AreEqual(0, posicion.PartidosPerdidos);
        Assert.AreEqual(7, posicion.Puntos);
    }

    private static Equipo CrearEquipo(string nombre)
    {
        return new Equipo { Nombre = nombre, RankingFIFA = 500, Confederacion = Confederacion.CONMEBOL };
    }
}
