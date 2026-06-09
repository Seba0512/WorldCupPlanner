using Microsoft.AspNetCore.Components.Forms;

namespace WorldCupPlanner.UI.Helpers;

public static class ArchivoHelper
{
    private const long TamanioMaximoPorDefecto = 2 * 1024 * 1024;

    public static async Task<string> ConvertirABase64DataUri(IBrowserFile archivo, long tamanioMaximo = TamanioMaximoPorDefecto)
    {
        using var stream = archivo.OpenReadStream(maxAllowedSize: tamanioMaximo);
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        var bytes = ms.ToArray();
        return $"data:{archivo.ContentType};base64,{Convert.ToBase64String(bytes)}";
    }
}
