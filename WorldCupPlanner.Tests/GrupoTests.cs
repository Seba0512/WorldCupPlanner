using WorldCupPlanner.Domain;

namespace WorldCupPlanner.Tests;

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
    public void Grupo_ConEtiquetaValida_YHastaCuatroEquipos_EsValido()
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