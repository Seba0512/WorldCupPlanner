using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.DominioTests;

[TestClass]
public class AuditoriaDTOTests
{
    [TestMethod]
    public void ToEntity_CopiaTodasLasPropiedades()
    {
        var dto = new AuditoriaDTO
        {
            Id = 12,
            Timestamp = "2026-05-29T20:45:00.0000000Z",
            Usuario = "admin@example.com",
            Accion = "Exportacion",
            Detalle = "Se exporto el log de auditoria"
        };

        var entidad = dto.ToEntity();

        Assert.AreEqual(dto.Id, entidad.Id);
        Assert.AreEqual(dto.Timestamp, entidad.Timestamp);
        Assert.AreEqual(dto.Usuario, entidad.Usuario);
        Assert.AreEqual(dto.Accion, entidad.Accion);
        Assert.AreEqual(dto.Detalle, entidad.Detalle);
    }

    [TestMethod]
    public void ToEntity_SinTimestamp_GeneraTimestampIso()
    {
        var dto = new AuditoriaDTO
        {
            Id = 3,
            Usuario = "sistema",
            Accion = "Alta",
            Detalle = "Alta de equipo"
        };

        var entidad = dto.ToEntity();

        Assert.AreEqual(dto.Id, entidad.Id);
        Assert.AreEqual(dto.Usuario, entidad.Usuario);
        Assert.AreEqual(dto.Accion, entidad.Accion);
        Assert.AreEqual(dto.Detalle, entidad.Detalle);
        Assert.IsTrue(DateTimeOffset.TryParse(entidad.Timestamp, out _));
    }

    [TestMethod]
    public void FromEntity_CopiaTodasLasPropiedades()
    {
        var entidad = new Auditoria(
            7,
            "2026-05-29T21:10:00.0000000Z",
            "periodista@example.com",
            "LecturaNotificacion",
            "El periodista marco una notificacion como leida");

        var dto = AuditoriaDTO.FromEntity(entidad);

        Assert.AreEqual(entidad.Id, dto.Id);
        Assert.AreEqual(entidad.Timestamp, dto.Timestamp);
        Assert.AreEqual(entidad.Usuario, dto.Usuario);
        Assert.AreEqual(entidad.Accion, dto.Accion);
        Assert.AreEqual(entidad.Detalle, dto.Detalle);
    }

    [TestMethod]
    public void FromEntity_ConEntidadNula_LanzaExcepcion()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            AuditoriaDTO.FromEntity(null!));
    }
}
