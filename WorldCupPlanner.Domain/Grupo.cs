namespace WorldCupPlanner.Domain;

public class Grupo
{
    public string Etiqueta { get; set; } = "";
    public List<Equipo> Equipos { get; set; } = new List<Equipo>();
    // public List<Partido> Partidos { get; set; } = new List<Partido>(); // Será agregado cuando Partido esté disponible

    public bool EsValido()
    {
        if (string.IsNullOrEmpty(Etiqueta))
        {
            return false;
        }

        if (Etiqueta.Length != 1)
        {
            return false;
        }

        if (Etiqueta[0] < 'A' || Etiqueta[0] > 'L')
        {
            return false;
        }


        if (Equipos.Count > 4)
        {
            return false;
        }

        // Validar que todos los equipos sean válidos
        foreach (var equipo in Equipos)
        {
            if (!equipo.EsValido())
            {
                return false;
            }
        }

        // Validación de restricciones por confederaciones
        if (!ValidarConfederaciones())
        {
            return false;
        }

        return true;
    }

    private bool ValidarConfederaciones()
    {
        // Contar equipos por confederación
        var conteoConfederaciones = new Dictionary<Confederacion, int>();

        foreach (var equipo in Equipos)
        {
            if (equipo.Confederacion.HasValue)
            {
                var confederacion = equipo.Confederacion.Value;
                if (!conteoConfederaciones.ContainsKey(confederacion))
                {
                    conteoConfederaciones[confederacion] = 0;
                }
                conteoConfederaciones[confederacion]++;
            }
        }

        // UEFA puede tener máximo 2 equipos en un grupo
        if (conteoConfederaciones.ContainsKey(Confederacion.UEFA) && conteoConfederaciones[Confederacion.UEFA] > 2)
        {
            return false;
        }

        // Las demás confederaciones pueden tener máximo 1 equipo en un grupo
        foreach (var confederacion in new[] { Confederacion.CONMEBOL, Confederacion.CONCACAF, Confederacion.CAF, Confederacion.AFC, Confederacion.OFC })
        {
            if (conteoConfederaciones.ContainsKey(confederacion) && conteoConfederaciones[confederacion] > 1)
            {
                return false;
            }
        }

        return true;
    }
}