using Microsoft.AspNetCore.Components.Forms;

namespace WorldCupPlanner.UI.Helpers;

public static class ArchivoHelper
{
    private const long TamanioMaximoPorDefecto = 2 * 1024 * 1024;
    private static readonly HashSet<string> ContentTypesPermitidos =
    [
        "image/svg+xml",
        "image/jpeg",
        "image/png",
        "image/webp"
    ];

    public static async Task<string> ConvertirABase64DataUri(IBrowserFile archivo, long tamanioMaximo = TamanioMaximoPorDefecto)
    {
        if (!ContentTypesPermitidos.Contains(archivo.ContentType))
            throw new ArgumentException("Solo se permiten archivos JPG, PNG, SVG y WebP.");

        using var stream = archivo.OpenReadStream(maxAllowedSize: tamanioMaximo);
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        var bytes = ms.ToArray();
        return $"data:{archivo.ContentType};base64,{Convert.ToBase64String(bytes)}";
    }
}
