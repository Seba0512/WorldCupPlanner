using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.DominioTests;

[TestClass]
public class PartidoTests
{
    [TestMethod]
    public void Partido_EquipoLocalIgualAVisitante_EsInvalido()
    {
        // Arrange
        var partido = CrearPartido();
        partido.EquipoVisitante = partido.EquipoLocal;

        // Act
        var resultado = partido.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Partido_SinEstadio_EsInvalido()
    {
        // Arrange
        var partido = CrearPartido();
        partido.Estadio = null;

        // Act
        var resultado = partido.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Partido_SinEquipoLocal_EsInvalido()
    {
        // Arrange
        var partido = CrearPartido();
        partido.EquipoLocal = null;

        // Act
        var resultado = partido.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Partido_SinEquipoVisitante_EsInvalido()
    {
        // Arrange
        var partido = CrearPartido();
        partido.EquipoVisitante = null;

        // Act
        var resultado = partido.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Partido_ConGoles_EstaJugado()
    {
        // Arrange
        var partido = CrearPartido(golesLocal: 2, golesVisitante: 1);

        // Act
        var resultado = partido.EstaJugado();

        // Assert
        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void Partido_GolesLocalMayor_GanaLocal()
    {
        // Arrange
        var partido = CrearPartido(golesLocal: 2, golesVisitante: 1);

        // Act
        var resultado = partido.Ganador();

        // Assert
        Assert.AreEqual(partido.EquipoLocal, resultado);
    }

    [TestMethod]
    public void Partido_GolesIguales_EsEmpate()
    {
        // Arrange
        var partido = CrearPartido(golesLocal: 1, golesVisitante: 1);

        // Act
        var resultado = partido.EsEmpate();

        // Assert
        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void Partido_GolesVisitanteMayor_GanaVisitante()
    {
        // Arrange
        var partido = CrearPartido(golesLocal: 0, golesVisitante: 2);

        // Act
        var resultado = partido.Ganador();

        // Assert
        Assert.AreEqual(partido.EquipoVisitante, resultado);
    }

    [TestMethod]
    public void Partido_SinGoles_GanadorEsNull()
    {
        // Arrange
        var partido = CrearPartido();

        // Act
        var resultado = partido.Ganador();

        // Assert
        Assert.IsNull(resultado);
    }

    [TestMethod]
    public void Partido_ConDatosValidos_EsValido()
    {
        // Arrange
        var partido = CrearPartido();

        // Act
        var resultado = partido.EsValido();

        // Assert
        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void Partido_NoJugado_NoEsEmpate()
    {
        // Arrange
        var partido = CrearPartido();

        // Act
        var resultado = partido.EsEmpate();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Partido_SoloGolesLocal_NoEstaJugado()
    {
        // Arrange
        var partido = CrearPartido(golesLocal: 1);

        // Act
        var resultado = partido.EstaJugado();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Partido_Empate_GanadorEsNull()
    {
        // Arrange
        var partido = CrearPartido(golesLocal: 1, golesVisitante: 1);

        // Act
        var resultado = partido.Ganador();

        // Assert
        Assert.IsNull(resultado);
    }

    [TestMethod]
    public void Ganador_ConEmpateYGanadorPorPenalesLocal_RetornaEquipoLocal()
    {
        // Arrange
        var partido = CrearPartido(golesLocal: 1, golesVisitante: 1, fase: FaseTorneo.Octavos);
        partido.GanadorPorPenales = partido.EquipoLocal;

        // Act
        var resultado = partido.Ganador();

        // Assert
        Assert.AreEqual(partido.EquipoLocal, resultado);
    }

    [TestMethod]
    public void Ganador_ConEmpateYGanadorPorPenalesVisitante_RetornaEquipoVisitante()
    {
        // Arrange
        var partido = CrearPartido(golesLocal: 0, golesVisitante: 0, fase: FaseTorneo.Final);
        partido.GanadorPorPenales = partido.EquipoVisitante;

        // Act
        var resultado = partido.Ganador();

        // Assert
        Assert.AreEqual(partido.EquipoVisitante, resultado);
    }

    [TestMethod]
    public void FueDefinidoPorPenales_ConEmpateYGanadorAsignado_RetornaVerdadero()
    {
        // Arrange
        var partido = CrearPartido(golesLocal: 2, golesVisitante: 2, fase: FaseTorneo.Semifinal);
        partido.GanadorPorPenales = partido.EquipoLocal;

        // Act & Assert
        Assert.IsTrue(partido.FueDefinidoPorPenales());
    }

    [TestMethod]
    public void FueDefinidoPorPenales_ConVictoriaNormal_RetornaFalso()
    {
        // Arrange
        var partido = CrearPartido(golesLocal: 2, golesVisitante: 1, fase: FaseTorneo.Cuartos);

        // Act & Assert
        Assert.IsFalse(partido.FueDefinidoPorPenales());
    }

    [TestMethod]
    public void FueDefinidoPorPenales_ConEmpateYSinGanadorAsignado_RetornaFalso()
    {
        // Arrange
        var partido = CrearPartido(golesLocal: 1, golesVisitante: 1);

        // Act & Assert
        Assert.IsFalse(partido.FueDefinidoPorPenales());
    }

    [TestMethod]
    public void Partido_ConGrupoAsignado_RetornaGrupo()
    {
        // Arrange
        var grupo = new Grupo("A");
        var partido = CrearPartido();
        partido.Grupo = grupo;

        // Act & Assert
        Assert.AreEqual(grupo, partido.Grupo);
    }

    private static Partido CrearPartido(
        int? golesLocal = null,
        int? golesVisitante = null,
        FaseTorneo fase = FaseTorneo.FaseDeGrupos)
    {
        return new Partido
        {
            Fecha = DateTime.Now,
            Estadio = new Estadio { Nombre = "Centenario", Ciudad = "Montevideo", Capacidad = 60000 },
            EquipoLocal = new Equipo { Nombre = "Uruguay", RankingFIFA = 1000, Confederacion = Confederacion.CONMEBOL },
            EquipoVisitante = new Equipo { Nombre = "Argentina", RankingFIFA = 800, Confederacion = Confederacion.CONMEBOL },
            Fase = fase,
            GolesLocal = golesLocal,
            GolesVisitante = golesVisitante
        };
    }
}
