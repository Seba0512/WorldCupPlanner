using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.DominioTests;

[TestClass]
public class ReglasConfederacionTests
{
    [TestMethod]
    public void CuposPorConfederacion_UEFA_TieneCupo16()
    {
        // Arrange & Act
        var cupo = ReglasConfederacion.CuposPorConfederacion[Confederacion.UEFA];

        // Assert
        Assert.AreEqual(16, cupo);
    }

    [TestMethod]
    public void CuposPorConfederacion_CONMEBOL_TieneCupo7()
    {
        // Act
        var cupo = ReglasConfederacion.CuposPorConfederacion[Confederacion.CONMEBOL];

        // Assert
        Assert.AreEqual(7, cupo);
    }

    [TestMethod]
    public void CuposPorConfederacion_CONCACAF_TieneCupo7()
    {
        // Act
        var cupo = ReglasConfederacion.CuposPorConfederacion[Confederacion.CONCACAF];

        // Assert
        Assert.AreEqual(7, cupo);
    }

    [TestMethod]
    public void CuposPorConfederacion_CAF_TieneCupo9()
    {
        // Act
        var cupo = ReglasConfederacion.CuposPorConfederacion[Confederacion.CAF];

        // Assert
        Assert.AreEqual(9, cupo);
    }

    [TestMethod]
    public void CuposPorConfederacion_AFC_TieneCupo8()
    {
        // Act
        var cupo = ReglasConfederacion.CuposPorConfederacion[Confederacion.AFC];

        // Assert
        Assert.AreEqual(8, cupo);
    }

    [TestMethod]
    public void CuposPorConfederacion_OFC_TieneCupo1()
    {
        // Act
        var cupo = ReglasConfederacion.CuposPorConfederacion[Confederacion.OFC];

        // Assert
        Assert.AreEqual(1, cupo);
    }

    [TestMethod]
    public void CuposPorConfederacion_SumaDeTodos_Es48()
    {
        // Act
        var total = ReglasConfederacion.CuposPorConfederacion.Values.Sum();

        // Assert
        Assert.AreEqual(48, total);
    }

    [TestMethod]
    public void CuposPorConfederacion_ContieneTodasLasConfederaciones()
    {
        // Act
        var confederaciones = ReglasConfederacion.CuposPorConfederacion.Keys;

        // Assert
        foreach (var confederacion in Enum.GetValues<Confederacion>())
        {
            Assert.IsTrue(confederaciones.Contains(confederacion),
                $"Falta la confederación {confederacion}");
        }
    }
}
