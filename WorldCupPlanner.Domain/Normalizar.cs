using System.Text;

namespace WorldCupPlanner.Domain;

public class Normalizar
{
    public static string NormalizarTexto(string texto)
    {
        var normalizado = texto.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var c in normalizado)
            if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        var sinTildes = sb.ToString().Normalize(NormalizationForm.FormC).ToLower();
        var sinEspeciales = System.Text.RegularExpressions.Regex.Replace(sinTildes, @"[^a-z0-9 ]", " ");
        return System.Text.RegularExpressions.Regex.Replace(sinEspeciales, @" +", " ").Trim();
    }
}
