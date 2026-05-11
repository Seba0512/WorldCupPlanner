namespace WorldCupPlanner.Servicios;

public static class AlgoritmoFisherYates
{
    public static void Mezclar<T>(List<T> lista, Random random)
    {
        for (int i = lista.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (lista[i], lista[j]) = (lista[j], lista[i]);
        }
    }
}
