using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.UI.Helpers;

public class DescargadorHttpSvg : IDescargadorContenido
{
    private readonly HttpClient _httpClient;

    public DescargadorHttpSvg(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public string Descargar(string url)
    {
        try
        {
            return _httpClient.GetStringAsync(url).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"No se pudo descargar la Bandera desde \"{url}\": {ex.Message}", ex);
        }
    }
}
