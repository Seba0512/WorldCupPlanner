using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.DominioTests;

[TestClass]
public class ClasificadosFaseGruposDTOTests
{
    [TestMethod]
    public void FromEntity_CopiaListasYGruposDeOrigen()
    {
        var primero = CrearEquipo("Uruguay", Confederacion.CONMEBOL);
        var segundo = CrearEquipo("Francia", Confederacion.UEFA);
        var tercero = CrearEquipo("Japon", Confederacion.AFC);
        var grupoA = new Grupo("A", new List<Equipo> { primero, segundo, tercero });
        var clasificados = new ClasificadosFaseGrupos
        {
            Primeros = new List<Equipo> { primero },
            Segundos = new List<Equipo> { segundo },
            MejoresTerceros = new List<Equipo> { tercero },
            GruposDeOrigen = new Dictionary<Equipo, Grupo>
            {
                { primero, grupoA },
                { segundo, grupoA },
                { tercero, grupoA }
            }
        };

        var dto = ClasificadosFaseGruposDTO.FromEntity(clasificados);

        Assert.AreEqual(1, dto.Primeros.Count);
        Assert.AreEqual("Uruguay", dto.Primeros[0].Nombre);
        Assert.AreEqual(1, dto.Segundos.Count);
        Assert.AreEqual("Francia", dto.Segundos[0].Nombre);
        Assert.AreEqual(1, dto.MejoresTerceros.Count);
        Assert.AreEqual("Japon", dto.MejoresTerceros[0].Nombre);
        Assert.AreEqual(3, dto.GruposDeOrigen.Count);
        Assert.AreEqual("A", dto.GruposDeOrigen[0].Grupo.Etiqueta);
    }

    [TestMethod]
    public void ToEntity_CopiaListasYGruposDeOrigen()
    {
        var dto = new ClasificadosFaseGruposDTO
        {
            Primeros = new List<EquipoClasificadoDTO> { CrearEquipoDto("Uruguay", Confederacion.CONMEBOL) },
            Segundos = new List<EquipoClasificadoDTO> { CrearEquipoDto("Francia", Confederacion.UEFA) },
            MejoresTerceros = new List<EquipoClasificadoDTO> { CrearEquipoDto("Japon", Confederacion.AFC) },
            GruposDeOrigen = new List<GrupoOrigenClasificadoDTO>
            {
                new()
                {
                    Equipo = CrearEquipoDto("Uruguay", Confederacion.CONMEBOL),
                    Grupo = CrearGrupoDto("A")
                }
            }
        };

        var entidad = dto.ToEntity();

        Assert.AreEqual(1, entidad.Primeros.Count);
        Assert.AreEqual("Uruguay", entidad.Primeros[0].Nombre);
        Assert.AreEqual(1, entidad.Segundos.Count);
        Assert.AreEqual("Francia", entidad.Segundos[0].Nombre);
        Assert.AreEqual(1, entidad.MejoresTerceros.Count);
        Assert.AreEqual("Japon", entidad.MejoresTerceros[0].Nombre);
        Assert.AreEqual("A", entidad.ObtenerGrupoDeOrigen(entidad.Primeros[0])?.Etiqueta);
    }

    [TestMethod]
    public void Todos_DevuelveTodosLosEquiposDelDto()
    {
        var dto = new ClasificadosFaseGruposDTO
        {
            Primeros = new List<EquipoClasificadoDTO> { CrearEquipoDto("Uruguay", Confederacion.CONMEBOL) },
            Segundos = new List<EquipoClasificadoDTO> { CrearEquipoDto("Francia", Confederacion.UEFA) },
            MejoresTerceros = new List<EquipoClasificadoDTO> { CrearEquipoDto("Japon", Confederacion.AFC) }
        };

        var todos = dto.Todos;

        Assert.AreEqual(3, todos.Count);
        CollectionAssert.AreEqual(
            new[] { "Uruguay", "Francia", "Japon" },
            todos.Select(e => e.Nombre).ToArray());
    }

    [TestMethod]
    public void FromEntity_ConEntidadNula_LanzaExcepcion()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            ClasificadosFaseGruposDTO.FromEntity(null!));
    }

    private static Equipo CrearEquipo(string nombre, Confederacion confederacion)
    {
        return new Equipo { Nombre = nombre, RankingFIFA = 1000, Confederacion = confederacion };
    }

    private static EquipoClasificadoDTO CrearEquipoDto(string nombre, Confederacion confederacion)
    {
        return new EquipoClasificadoDTO
        {
            Nombre = nombre,
            RankingFIFA = 1000,
            Confederacion = confederacion
        };
    }

    private static GrupoClasificadoDTO CrearGrupoDto(string etiqueta)
    {
        return new GrupoClasificadoDTO { Etiqueta = etiqueta };
    }
}
