using WorldCupPlanner.Domain;

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
}