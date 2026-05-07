namespace WorldCupPlanner.Dominio.Excepciones;

public class EntidadInexistenteExcepcion : Exception
{
    public EntidadInexistenteExcepcion(string mensaje) : base(mensaje)
    {
    }
}