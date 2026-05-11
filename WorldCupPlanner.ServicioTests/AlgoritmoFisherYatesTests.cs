using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public class AlgoritmoFisherYatesTests
{
    [TestMethod]
    public void Mezclar_ConMismaSemilla_ProduceMismoOrden()
    {
        // Arrange
        var lista1 = new List<int> { 1, 2, 3, 4, 5 };
        var lista2 = new List<int> { 1, 2, 3, 4, 5 };

        // Act
        AlgoritmoFisherYates.Mezclar(lista1, new Random(42));
        AlgoritmoFisherYates.Mezclar(lista2, new Random(42));

        // Assert
        CollectionAssert.AreEqual(lista1, lista2);
    }

    [TestMethod]
    public void Mezclar_ConDistintaSemilla_ProduceOrdenDistinto()
    {
        // Arrange
        var lista1 = new List<int> { 1, 2, 3, 4, 5 };
        var lista2 = new List<int> { 1, 2, 3, 4, 5 };

        // Act
        AlgoritmoFisherYates.Mezclar(lista1, new Random(1));
        AlgoritmoFisherYates.Mezclar(lista2, new Random(999));

        // Assert
        CollectionAssert.AreNotEqual(lista1, lista2);
    }

    [TestMethod]
    public void Mezclar_ConservaLosElementos()
    {
        // Arrange
        var original = new List<string> { "A", "B", "C", "D" };
        var lista = new List<string>(original);

        // Act
        AlgoritmoFisherYates.Mezclar(lista, new Random(7));

        // Assert
        CollectionAssert.AreEquivalent(original, lista);
    }

    [TestMethod]
    public void Mezclar_ListaDeUnElemento_NoLanzaExcepcion()
    {
        // Arrange
        var lista = new List<int> { 42 };

        // Act
        AlgoritmoFisherYates.Mezclar(lista, new Random(0));

        // Assert
        Assert.AreEqual(42, lista[0]);
    }

    [TestMethod]
    public void Mezclar_ListaVacia_NoLanzaExcepcion()
    {
        // Arrange
        var lista = new List<int>();

        // Act & Assert
        AlgoritmoFisherYates.Mezclar(lista, new Random(0));
    }

    [TestMethod]
    public void Mezclar_ModificaListaOriginal()
    {
        // Arrange
        var lista = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var ordenOriginal = new List<int>(lista);

        // Act
        AlgoritmoFisherYates.Mezclar(lista, new Random(42));

        // Assert
        CollectionAssert.AreNotEqual(ordenOriginal, lista);
    }

    [TestMethod]
    public void Mezclar_EsReproducible_ConMismaSemillaYTipo()
    {
        // Arrange
        var listaStrings1 = new List<string> { "Uruguay", "Brasil", "Argentina", "Francia", "España" };
        var listaStrings2 = new List<string>(listaStrings1);

        // Act
        AlgoritmoFisherYates.Mezclar(listaStrings1, new Random(100));
        AlgoritmoFisherYates.Mezclar(listaStrings2, new Random(100));

        // Assert
        CollectionAssert.AreEqual(listaStrings1, listaStrings2);
    }
}
