using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.UI.Helpers;

public static class MensajeErrorHelper
{
    private const string MensajeGenerico =
        "Ocurrió un error inesperado. Intentá de nuevo o contactá al soporte si el problema persiste.";

    public static string ParaUsuario(Exception ex) => ex switch
    {
        EntidadInvalidaExcepcion or
        EntidadYaExistenteExcepcion or
        EntidadInexistenteExcepcion or
        AccesoDenegadoExcepcion or
        InvalidOperationException => ex.Message,
        ArgumentException argEx => argEx.Message.Split(" (Parameter")[0],
        _ => MensajeGenerico
    };
}
