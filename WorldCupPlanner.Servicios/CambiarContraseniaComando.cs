namespace WorldCupPlanner.Servicios;

public class CambiarContraseniaComando
{
    public const string MensajeContrasenasNoCoinciden = "Las contraseñas no coinciden.";

    public string ContraseniaAnterior { get; set; } = string.Empty;
    public string NuevaContrasenia { get; set; } = string.Empty;
    public string ConfirmarContrasenia { get; set; } = string.Empty;
}
