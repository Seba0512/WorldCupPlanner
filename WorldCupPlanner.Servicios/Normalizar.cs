using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace WorldCupPlanner.Servicios;

public class Normalizar
{
    public static string NormalizarTexto(string texto)
    {
        var normalizado = texto.Normalize(NormalizationForm.FormD);
        var constructor = new StringBuilder();
        foreach (var caracter in normalizado)
            if (CharUnicodeInfo.GetUnicodeCategory(caracter) != UnicodeCategory.NonSpacingMark)
                constructor.Append(caracter);
        var sinTildes = constructor.ToString().Normalize(NormalizationForm.FormC).ToLower();
        var sinEspeciales = Regex.Replace(sinTildes, @"[^a-z0-9 ]", " ");
        return Regex.Replace(sinEspeciales, @" +", " ").Trim();
    }
}
