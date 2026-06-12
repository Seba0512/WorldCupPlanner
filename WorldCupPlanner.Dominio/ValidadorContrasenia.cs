using System.Text.RegularExpressions;

namespace WorldCupPlanner.Dominio;

public static class ValidadorContrasenia
{
    public const int LongitudMinima = 8;

    private const string PatronMayuscula = @"[A-Z]";
    private const string PatronMinuscula = @"[a-z]";
    private const string PatronNumero    = @"[0-9]";
    private const string PatronEspecial  = @"[@#$\.\!\&\*\-_\+=\?]";

    public static bool CumpleLongitud(string contrasenia) => contrasenia.Length >= LongitudMinima;
    public static bool CumpleMayuscula(string contrasenia) => Regex.IsMatch(contrasenia, PatronMayuscula);
    public static bool CumpleMinuscula(string contrasenia) => Regex.IsMatch(contrasenia, PatronMinuscula);
    public static bool CumpleNumero(string contrasenia)    => Regex.IsMatch(contrasenia, PatronNumero);
    public static bool CumpleEspecial(string contrasenia)  => Regex.IsMatch(contrasenia, PatronEspecial);

    public static bool EsValida(string contrasenia) =>
        CumpleLongitud(contrasenia)  &&
        CumpleMayuscula(contrasenia) &&
        CumpleMinuscula(contrasenia) &&
        CumpleNumero(contrasenia)    &&
        CumpleEspecial(contrasenia);
}
