using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace WorldCupPlanner.Servicios;

public class Normalizar
{
    public static string NormalizarTexto(string texto)
    {
        var normalizado = texto.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var c in normalizado)
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        var sinTildes = sb.ToString().Normalize(NormalizationForm.FormC).ToLower();
        var sinEspeciales = Regex.Replace(sinTildes, @"[^a-z0-9 ]", " ");
        return Regex.Replace(sinEspeciales, @" +", " ").Trim();
    }
}
