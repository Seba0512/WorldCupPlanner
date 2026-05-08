using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.DominioTests;

[TestClass]
public class AuditoriaTests
{
    [TestMethod]
    public void Auditoria_Constructor_EstableceTodasLasPropiedades()
    {
        // Arrange
        var id = 1;
        var usuario = "admin@test.com";
        var accion = "CrearEquipo";
        var detalle = "Se creó el equipo Uruguay";

        // Act
        var auditoria = new Auditoria(id, usuario, accion, detalle);

        // Assert
        Assert.AreEqual(id, auditoria.Id);
        Assert.AreEqual(usuario, auditoria.Usuario);
        Assert.AreEqual(accion, auditoria.Accion);
        Assert.AreEqual(detalle, auditoria.Detalle);
    }

    [TestMethod]
    public void Auditoria_Timestamp_EsFormatoISO8601()
    {
        // Arrange & Act
        var auditoria = new Auditoria(1, "usuario", "accion", "detalle");

        // Assert
        Assert.IsNotNull(auditoria.Timestamp);
        Assert.IsTrue(DateTime.TryParse(auditoria.Timestamp, out _),
            "El timestamp debe ser parseable como fecha ISO-8601");
    }

    [TestMethod]
    public void Auditoria_Timestamp_EsCercanoAlMomentoDeCreacion()
    {
        // Arrange
        var antes = DateTimeOffset.UtcNow;

        // Act
        var auditoria = new Auditoria(2, "usuario", "accion", "detalle");
        var despues = DateTimeOffset.UtcNow;

        // Assert
        var timestamp = DateTimeOffset.Parse(auditoria.Timestamp);
        Assert.IsTrue(timestamp >= antes && timestamp <= despues);
    }
}
