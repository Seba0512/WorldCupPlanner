namespace WorldCupPlanner.UI.Helpers;

public static class TablaGruposHelper
{
    public static string ObtenerEstiloFilaPosicion(int posicion) => posicion switch
    {
        1 => "background:rgba(34,197,94,0.14); border-left:3px solid rgba(34,197,94,0.8);",
        2 => "background:rgba(34,197,94,0.08); border-left:3px solid rgba(34,197,94,0.55);",
        3 => "background:rgba(148,163,184,0.06);",
        _ => string.Empty
    };
}
