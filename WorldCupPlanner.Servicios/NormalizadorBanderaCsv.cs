using System.Text;

namespace WorldCupPlanner.Servicios;

public static class NormalizadorBanderaCsv
{
    private const string PrefijoSvgDataUri = "data:image/svg+xml;base64,";
    private const string PrefijoHttps      = "https://";
    private const string PrefijoHttp       = "http://";
    private const string PrefijoDataUri    = "data:";
    private const int TamanoMaximoBanderaCaracteres = 2_800_000;

    public static string Normalizar(string valor, IDescargadorContenido? descargador)
    {
        if (string.IsNullOrEmpty(valor))
            return string.Empty;

        if (EsUrl(valor))
            return NormalizarDesdeUrl(valor, descargador);

        if (EsDataUri(valor))
            return NormalizarDesdeDataUri(valor);

        return NormalizarDesdeBase64Puro(valor);
    }

    private static bool EsUrl(string valor) =>
        valor.StartsWith(PrefijoHttps, StringComparison.OrdinalIgnoreCase) ||
        valor.StartsWith(PrefijoHttp,  StringComparison.OrdinalIgnoreCase);

    private static bool EsDataUri(string valor) =>
        valor.StartsWith(PrefijoDataUri, StringComparison.OrdinalIgnoreCase);

    private static string NormalizarDesdeUrl(string url, IDescargadorContenido? descargador)
    {
        if (descargador == null)
            throw new InvalidOperationException(
                "No se puede descargar la Bandera desde una URL: el descargador no está configurado.");

        var contenido = descargador.Descargar(url);
        ValidarContenidoSvg(contenido, $"la URL \"{url}\"");
        var bytes = Encoding.UTF8.GetBytes(contenido);
        var resultado = PrefijoSvgDataUri + Convert.ToBase64String(bytes);
        ValidarTamano(resultado);
        return resultado;
    }

    private static string NormalizarDesdeDataUri(string dataUri)
    {
        if (!dataUri.StartsWith(PrefijoSvgDataUri, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException(
                "El Data URI debe ser de tipo image/svg+xml con codificación Base64 (data:image/svg+xml;base64,...).");

        ValidarTamano(dataUri);
        return dataUri;
    }

    private static string NormalizarDesdeBase64Puro(string base64)
    {
        byte[] bytes;
        try
        {
            bytes = Convert.FromBase64String(base64);
        }
        catch (FormatException)
        {
            throw new ArgumentException(
                "El valor de Bandera no es una URL, Data URI ni Base64 válido.");
        }

        var contenido = Encoding.UTF8.GetString(bytes);
        ValidarContenidoSvg(contenido, "el Base64 proporcionado");
        var resultado = PrefijoSvgDataUri + base64;
        ValidarTamano(resultado);
        return resultado;
    }

    private static void ValidarContenidoSvg(string contenido, string fuente)
    {
        if (string.IsNullOrWhiteSpace(contenido))
            throw new ArgumentException($"El contenido obtenido de {fuente} está vacío.");

        if (!contenido.Contains("<svg", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException($"El contenido obtenido de {fuente} no es un SVG válido.");
    }

    private static void ValidarTamano(string valor)
    {
        if (valor.Length > TamanoMaximoBanderaCaracteres)
            throw new ArgumentException(
                "La Bandera supera el tamaño máximo permitido (2.800.000 caracteres).");
    }
}
