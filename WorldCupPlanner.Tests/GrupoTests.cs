using WorldCupPlanner.Domain;

namespace WorldCupPlanner.Tests;

[TestClass]
public class GrupoTests
{
    [TestMethod]
    public void Grupo_ConEtiquetaVacia_NoEsValido()
    {
        // Arrange
        var grupo = new Grupo();
        grupo.Etiqueta = "";

        // Act
        var resultado = grupo.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Grupo_ConEtiquetaDeMasDeUnCaracter_NoEsValido()
    {
        // Arrange
        var grupo = new Grupo();
        grupo.Etiqueta = "AB";

        // Act
        var resultado = grupo.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Grupo_ConEtiquetaFueraDeRango_NoEsValido()
    {
        // Arrange
        var grupo = new Grupo();
        grupo.Etiqueta = "M";

        // Act
        var resultado = grupo.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Grupo_ConMasDeCuatroEquipos_NoEsValido()
    {
        // Arrange
        var grupo = new Grupo();
        grupo.Etiqueta = "A";
        grupo.Equipos = new List<Equipo>
        {
            new Equipo(),
            new Equipo(),
            new Equipo(),
            new Equipo(),
            new Equipo()
        };

        // Act
        var resultado = grupo.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Grupo_ConEtiquetaValida_YHastaCuatroEquipos_EsValido()
    {
        // Arrange
        var grupo = new Grupo();
        grupo.Etiqueta = "B";
        grupo.Equipos = new List<Equipo>
        {
            new Equipo { Nombre = "Equipo1", RankingFIFA = 500, Confederacion = Confederacion.UEFA },
            new Equipo { Nombre = "Equipo2", RankingFIFA = 600, Confederacion = Confederacion.UEFA },
            new Equipo { Nombre = "Equipo3", RankingFIFA = 700, Confederacion = Confederacion.CONMEBOL },
            new Equipo { Nombre = "Equipo4", RankingFIFA = 800, Confederacion = Confederacion.AFC }
        };

        // Act
        var resultado = grupo.EsValido();

        // Assert
        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void Grupo_ConTresEquiposUEFA_NoEsValido()
    {
        // Arrange
        var grupo = new Grupo();
        grupo.Etiqueta = "C";
        grupo.Equipos = new List<Equipo>
        {
            new Equipo { Nombre = "Equipo1", RankingFIFA = 500, Confederacion = Confederacion.UEFA },
            new Equipo { Nombre = "Equipo2", RankingFIFA = 600, Confederacion = Confederacion.UEFA },
            new Equipo { Nombre = "Equipo3", RankingFIFA = 700, Confederacion = Confederacion.UEFA },
            new Equipo { Nombre = "Equipo4", RankingFIFA = 800, Confederacion = Confederacion.CONMEBOL }
        };

        // Act
        var resultado = grupo.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Grupo_ConDosEquiposConmebol_NoEsValido()
    {
        // Arrange
        var grupo = new Grupo();
        grupo.Etiqueta = "D";
        grupo.Equipos = new List<Equipo>
        {
            new Equipo { Nombre = "Equipo1", RankingFIFA = 500, Confederacion = Confederacion.UEFA },
            new Equipo { Nombre = "Equipo2", RankingFIFA = 600, Confederacion = Confederacion.CONMEBOL },
            new Equipo { Nombre = "Equipo3", RankingFIFA = 700, Confederacion = Confederacion.CONMEBOL },
            new Equipo { Nombre = "Equipo4", RankingFIFA = 800, Confederacion = Confederacion.AFC }
        };

        // Act
        var resultado = grupo.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Grupo_ConDosEquiposConcacaf_NoEsValido()
    {
        // Arrange
        var grupo = new Grupo();
        grupo.Etiqueta = "E";
        grupo.Equipos = new List<Equipo>
        {
            new Equipo { Nombre = "Equipo1", RankingFIFA = 500, Confederacion = Confederacion.UEFA },
            new Equipo { Nombre = "Equipo2", RankingFIFA = 600, Confederacion = Confederacion.CONCACAF },
            new Equipo { Nombre = "Equipo3", RankingFIFA = 700, Confederacion = Confederacion.CONCACAF },
            new Equipo { Nombre = "Equipo4", RankingFIFA = 800, Confederacion = Confederacion.AFC }
        };

        // Act
        var resultado = grupo.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Grupo_ConDosEquiposCAF_NoEsValido()
    {
        // Arrange
        var grupo = new Grupo();
        grupo.Etiqueta = "F";
        grupo.Equipos = new List<Equipo>
        {
            new Equipo { Nombre = "Equipo1", RankingFIFA = 500, Confederacion = Confederacion.UEFA },
            new Equipo { Nombre = "Equipo2", RankingFIFA = 600, Confederacion = Confederacion.CAF },
            new Equipo { Nombre = "Equipo3", RankingFIFA = 700, Confederacion = Confederacion.CAF },
            new Equipo { Nombre = "Equipo4", RankingFIFA = 800, Confederacion = Confederacion.AFC }
        };

        // Act
        var resultado = grupo.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Grupo_ConDosEquiposAFC_NoEsValido()
    {
        // Arrange
        var grupo = new Grupo();
        grupo.Etiqueta = "G";
        grupo.Equipos = new List<Equipo>
        {
            new Equipo { Nombre = "Equipo1", RankingFIFA = 500, Confederacion = Confederacion.UEFA },
            new Equipo { Nombre = "Equipo2", RankingFIFA = 600, Confederacion = Confederacion.AFC },
            new Equipo { Nombre = "Equipo3", RankingFIFA = 700, Confederacion = Confederacion.AFC },
            new Equipo { Nombre = "Equipo4", RankingFIFA = 800, Confederacion = Confederacion.CONMEBOL }
        };

        // Act
        var resultado = grupo.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Grupo_ConDosEquiposOFC_NoEsValido()
    {
        // Arrange
        var grupo = new Grupo();
        grupo.Etiqueta = "H";
        grupo.Equipos = new List<Equipo>
        {
            new Equipo { Nombre = "Equipo1", RankingFIFA = 500, Confederacion = Confederacion.UEFA },
            new Equipo { Nombre = "Equipo2", RankingFIFA = 600, Confederacion = Confederacion.OFC },
            new Equipo { Nombre = "Equipo3", RankingFIFA = 700, Confederacion = Confederacion.OFC },
            new Equipo { Nombre = "Equipo4", RankingFIFA = 800, Confederacion = Confederacion.CONMEBOL }
        };

        // Act
        var resultado = grupo.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Grupo_ConDosUEFAYUnoDeOtraConfederacion_EsValido()
    {
        // Arrange
        var grupo = new Grupo();
        grupo.Etiqueta = "I";
        grupo.Equipos = new List<Equipo>
        {
            new Equipo { Nombre = "Equipo1", RankingFIFA = 500, Confederacion = Confederacion.UEFA },
            new Equipo { Nombre = "Equipo2", RankingFIFA = 600, Confederacion = Confederacion.UEFA },
            new Equipo { Nombre = "Equipo3", RankingFIFA = 700, Confederacion = Confederacion.CONMEBOL },
            new Equipo { Nombre = "Equipo4", RankingFIFA = 800, Confederacion = Confederacion.AFC }
        };

        // Act
        var resultado = grupo.EsValido();

        // Assert
        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void Grupo_ConEquipoInvalido_NoEsValido()
    {
        // Arrange
        var grupo = new Grupo();
        grupo.Etiqueta = "A";
        grupo.Equipos = new List<Equipo>
        {
            new Equipo { Nombre = "", RankingFIFA = 500, Confederacion = Confederacion.UEFA }
        };

        // Act
        var resultado = grupo.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }
}