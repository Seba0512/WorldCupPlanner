namespace WorldCupPlanner.Dominio.Excepciones;

public class EntidadYaExistenteExcepcion : Exception
{  
    public EntidadYaExistenteExcepcion(string mensaje) : base(mensaje)
    {
    }
}