using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.DominioTests;

[TestClass]
public class PosicionEquipoDTOTests
{
    [TestMethod]
    public void FromEntity_CopiaTodasLasPropiedadesNecesarias()
    {
        var posicion = CrearPosicion();

        var dto = PosicionEquipoDTO.FromEntity(posicion);

        Assert.AreEqual(posicion.Equipo.Nombre, dto.Equipo?.Nombre);
        Assert.AreEqual(posicion.Grupo.Etiqueta, dto.GrupoEtiqueta);
        Assert.AreEqual(posicion.PartidosJugados, dto.PartidosJugados);
        Assert.AreEqual(posicion.PartidosGanados, dto.PartidosGanados);
        Assert.AreEqual(posicion.PartidosEmpatados, dto.PartidosEmpatados);
        Assert.AreEqual(posicion.PartidosPerdidos, dto.PartidosPerdidos);
        Assert.AreEqual(posicion.Puntos, dto.Puntos);
        Assert.AreEqual(posicion.GolesAFavor, dto.GolesAFavor);
        Assert.AreEqual(posicion.GolesEnContra, dto.GolesEnContra);
        Assert.AreEqual(posicion.DiferenciaGoles, dto.DiferenciaGoles);
    }

    [TestMethod]
    public void ToEntity_CopiaTodasLasPropiedadesNecesarias()
    {
        var dto = PosicionEquipoDTO.FromEntity(CrearPosicion());

        var posicion = dto.ToEntity();

        Assert.AreEqual(dto.Equipo?.Nombre, posicion.Equipo.Nombre);
        Assert.AreEqual(dto.GrupoEtiqueta, posicion.Grupo.Etiqueta);
        Assert.AreEqual(dto.PartidosJugados, posicion.PartidosJugados);
        Assert.AreEqual(dto.PartidosGanados, posicion.PartidosGanados);
        Assert.AreEqual(dto.PartidosEmpatados, posicion.PartidosEmpatados);
        Assert.AreEqual(dto.PartidosPerdidos, posicion.PartidosPerdidos);
        Assert.AreEqual(dto.Puntos, posicion.Puntos);
        Assert.AreEqual(dto.GolesAFavor, posicion.GolesAFavor);
        Assert.AreEqual(dto.GolesEnContra, posicion.GolesEnContra);
        Assert.AreEqual(dto.DiferenciaGoles, posicion.DiferenciaGoles);
    }

    [TestMethod]
    public void Constructor_PropiedadesPorDefecto_SonValoresSeguros()
    {
        var dto = new PosicionEquipoDTO();

        Assert.IsNull(dto.Equipo);
        Assert.IsNull(dto.GrupoEtiqueta);
        Assert.AreEqual(0, dto.PartidosJugados);
        Assert.AreEqual(0, dto.PartidosGanados);
        Assert.AreEqual(0, dto.PartidosEmpatados);
        Assert.AreEqual(0, dto.PartidosPerdidos);
        Assert.AreEqual(0, dto.Puntos);
        Assert.AreEqual(0, dto.GolesAFavor);
        Assert.AreEqual(0, dto.GolesEnContra);
        Assert.AreEqual(0, dto.DiferenciaGoles);
    }

    [TestMethod]
    public void FromEntity_ConEntidadNula_LanzaExcepcion()
    {
        Assert.ThrowsException<ArgumentNullException>(() => PosicionEquipoDTO.FromEntity(null!));
    }

    [TestMethod]
    public void ToEntity_SinEquipoNiGrupo_LosConservaNulos()
    {
        var dto = new PosicionEquipoDTO
        {
            Puntos = 1,
            GolesAFavor = 2,
            GolesEnContra = 1
        };

        var posicion = dto.ToEntity();

        Assert.IsNull(posicion.Equipo);
        Assert.IsNull(posicion.Grupo);
        Assert.AreEqual(1, posicion.Puntos);
    }

    private static PosicionEquipo CrearPosicion()
    {
        return new PosicionEquipo
        {
            Equipo = new Equipo
            {
                Nombre = "Uruguay",
                RankingFIFA = 1800,
                Confederacion = Confederacion.CONMEBOL
            },
            Grupo = new Grupo("A"),
            PartidosJugados = 3,
            PartidosGanados = 2,
            PartidosEmpatados = 1,
            PartidosPerdidos = 0,
            Puntos = 7,
            GolesAFavor = 6,
            GolesEnContra = 2
        };
    }
}
