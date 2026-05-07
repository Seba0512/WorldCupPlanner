namespace WorldCupPlanner.Dominio.Entidades;

public record DatosUsuario
{
    public string Nombre { get; init; } = string.Empty;
    public string Apellido { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FechaNacimiento { get; init; } = string.Empty;
}
