namespace WorldCupPlanner.UI.Helpers;

public static class AuditoriaDisplayHelper
{
    public static string FormatearTimestamp(string iso) =>
        DateTime.TryParse(iso, out var dt)
            ? dt.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss")
            : iso;

    public static string ObtenerClaseBadge(string accion) => accion.ToLower() switch
    {
        var a when a.Contains("elimin") || a.Contains("borr") => "badge-red",
        var a when a.Contains("cre")    || a.Contains("agre") => "badge-green",
        var a when a.Contains("edit")   || a.Contains("actu") => "badge-gold",
        var a when a.Contains("login")  || a.Contains("sesion") => "badge-blue",
        _ => "badge-muted"
    };
}
