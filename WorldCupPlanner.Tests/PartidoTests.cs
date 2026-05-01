using WorldCupPlanner.Dominio;

[TestClass]
public class PartidoTests
{
    [TestMethod]
    public void Partido_EquipoLocalIgualAVisitante_EsInvalido()
    {
        // Arrange
        var equipo = new Equipo
        {
            Nombre = "Uruguay",
            RankingFIFA = 1000,
            Confederacion = Confederacion.CONMEBOL
        };
        var partido = new Partido
        {
            Fecha = DateTime.Now,
            Estadio = new Estadio { Nombre = "Centenario", Ciudad = "Montevideo", Capacidad = 60000 },
            EquipoLocal = equipo,
            EquipoVisitante = equipo,
            Fase = FaseTorneo.FaseDeGrupos
        };

        // Act
        var resultado = partido.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }
    
    [TestMethod]
    public void Partido_SinEstadio_EsInvalido()
    {
        // Arrange
        var partido = new Partido
        {
            Fecha = DateTime.Now,
            EquipoLocal = new Equipo { Nombre = "Uruguay", RankingFIFA = 1000, Confederacion = Confederacion.CONMEBOL },
            EquipoVisitante = new Equipo { Nombre = "Argentina", RankingFIFA = 800, Confederacion = Confederacion.CONMEBOL },
            Fase = FaseTorneo.FaseDeGrupos
        };

        // Act
        var resultado = partido.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }
    
    [TestMethod]
    public void Partido_SinEquipoLocal_EsInvalido()
    {
        // Arrange
        var partido = new Partido
        {
            Fecha = DateTime.Now,
            Estadio = new Estadio { Nombre = "Centenario", Ciudad = "Montevideo", Capacidad = 60000 },
            EquipoVisitante = new Equipo { Nombre = "Argentina", RankingFIFA = 800, Confederacion = Confederacion.CONMEBOL },
            Fase = FaseTorneo.FaseDeGrupos
        };

        // Act
        var resultado = partido.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Partido_SinEquipoVisitante_EsInvalido()
    {
        // Arrange
        var partido = new Partido
        {
            Fecha = DateTime.Now,
            Estadio = new Estadio { Nombre = "Centenario", Ciudad = "Montevideo", Capacidad = 60000 },
            EquipoLocal = new Equipo { Nombre = "Uruguay", RankingFIFA = 1000, Confederacion = Confederacion.CONMEBOL },
            Fase = FaseTorneo.FaseDeGrupos
        };

        // Act
        var resultado = partido.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }
    [TestMethod]
    public void Partido_ConGoles_EstaJugado()
    {
        // Arrange
        var partido = new Partido
        {
            Fecha = DateTime.Now,
            Estadio = new Estadio { Nombre = "Centenario", Ciudad = "Montevideo", Capacidad = 60000 },
            EquipoLocal = new Equipo { Nombre = "Uruguay", RankingFIFA = 1000, Confederacion = Confederacion.CONMEBOL },
            EquipoVisitante = new Equipo { Nombre = "Argentina", RankingFIFA = 800, Confederacion = Confederacion.CONMEBOL },
            Fase = FaseTorneo.FaseDeGrupos,
            GolesLocal = 2,
            GolesVisitante = 1
        };

        // Act
        var resultado = partido.EstaJugado();

        // Assert
        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void Partido_GolesLocalMayor_GanaLocal()
    {
        // Arrange
        var equipoLocal = new Equipo { Nombre = "Uruguay", RankingFIFA = 1000, Confederacion = Confederacion.CONMEBOL };
        var partido = new Partido
        {
            Fecha = DateTime.Now,
            Estadio = new Estadio { Nombre = "Centenario", Ciudad = "Montevideo", Capacidad = 60000 },
            EquipoLocal = equipoLocal,
            EquipoVisitante = new Equipo { Nombre = "Argentina", RankingFIFA = 800, Confederacion = Confederacion.CONMEBOL },
            Fase = FaseTorneo.FaseDeGrupos,
            GolesLocal = 2,
            GolesVisitante = 1
        };

        // Act
        var resultado = partido.Ganador();

        // Assert
        Assert.AreEqual(equipoLocal, resultado);
    }
    
    [TestMethod]
    public void Partido_GolesIguales_EsEmpate()
    {
        // Arrange
        var partido = new Partido
        {
            Fecha = DateTime.Now,
            Estadio = new Estadio { Nombre = "Centenario", Ciudad = "Montevideo", Capacidad = 60000 },
            EquipoLocal = new Equipo { Nombre = "Uruguay", RankingFIFA = 1000, Confederacion = Confederacion.CONMEBOL },
            EquipoVisitante = new Equipo { Nombre = "Argentina", RankingFIFA = 800, Confederacion = Confederacion.CONMEBOL },
            Fase = FaseTorneo.FaseDeGrupos,
            GolesLocal = 1,
            GolesVisitante = 1
        };

        // Act
        var resultado = partido.EsEmpate();

        // Assert
        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void Partido_GolesVisitanteMayor_GanaVisitante()
    {
        // Arrange
        var equipoVisitante = new Equipo { Nombre = "Argentina", RankingFIFA = 800, Confederacion = Confederacion.CONMEBOL };
        var partido = new Partido
        {
            Fecha = DateTime.Now,
            Estadio = new Estadio { Nombre = "Centenario", Ciudad = "Montevideo", Capacidad = 60000 },
            EquipoLocal = new Equipo { Nombre = "Uruguay", RankingFIFA = 1000, Confederacion = Confederacion.CONMEBOL },
            EquipoVisitante = equipoVisitante,
            Fase = FaseTorneo.FaseDeGrupos,
            GolesLocal = 0,
            GolesVisitante = 2
        };

        // Act
        var resultado = partido.Ganador();

        // Assert
        Assert.AreEqual(equipoVisitante, resultado);
    }
     
    [TestMethod]
    public void Partido_SinGoles_GanadorEsNull()
    {
        // Arrange
        var partido = new Partido
        {
            Fecha = DateTime.Now,
            Estadio = new Estadio { Nombre = "Centenario", Ciudad = "Montevideo", Capacidad = 60000 },
            EquipoLocal = new Equipo { Nombre = "Uruguay", RankingFIFA = 1000, Confederacion = Confederacion.CONMEBOL },
            EquipoVisitante = new Equipo { Nombre = "Argentina", RankingFIFA = 800, Confederacion = Confederacion.CONMEBOL },
            Fase = FaseTorneo.FaseDeGrupos
        };

        // Act
        var resultado = partido.Ganador();

        // Assert
        Assert.IsNull(resultado);
    }
    
}