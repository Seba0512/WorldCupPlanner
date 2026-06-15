using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.Servicios;

public record ComandoGuardarPartido(
    int PartidoId,
    DateTime NuevaFecha,
    string NombreEstadio,
    int? GolesLocal,
    int? GolesVisitante,
    Usuario? UsuarioActual,
    EstadoTorneo EstadoTorneo);
