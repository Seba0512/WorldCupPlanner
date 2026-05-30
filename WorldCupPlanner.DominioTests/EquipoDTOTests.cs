using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.DominioTests
{
    [TestClass]
    public class EquipoDTOTests
    {
        [TestMethod]
        public void ToEntity_CopiaTodasLasPropiedades()
        {
            var dto = new EquipoDTO
            {
                Nombre = "Uruguay",
                RankingFIFA = 1800,
                Confederacion = Confederacion.CONMEBOL
            };

            var equipo = dto.ToEntity();

            Assert.AreEqual(dto.Nombre, equipo.Nombre);
            Assert.AreEqual(dto.RankingFIFA, equipo.RankingFIFA);
            Assert.AreEqual(dto.Confederacion, equipo.Confederacion);
        }

        [TestMethod]
        public void FromEntity_CopiaTodasLasPropiedades()
        {
            var equipo = new Equipo
            {
                Nombre = "Japon",
                RankingFIFA = 1500,
                Confederacion = Confederacion.AFC
            };

            var dto = EquipoDTO.FromEntity(equipo);

            Assert.AreEqual(equipo.Nombre, dto.Nombre);
            Assert.AreEqual(equipo.RankingFIFA, dto.RankingFIFA);
            Assert.AreEqual(equipo.Confederacion, dto.Confederacion);
        }

        [TestMethod]
        public void Constructor_PropiedadesPorDefecto_SonValoresSeguros()
        {
            var dto = new EquipoDTO();

            Assert.AreEqual(string.Empty, dto.Nombre);
            Assert.AreEqual(0, dto.RankingFIFA);
            Assert.IsNull(dto.Confederacion);
        }

        [TestMethod]
        public void FromEntity_ConEntidadNula_LanzaExcepcion()
        {
            Assert.ThrowsException<ArgumentNullException>(() => EquipoDTO.FromEntity(null!));
        }
    }
}
