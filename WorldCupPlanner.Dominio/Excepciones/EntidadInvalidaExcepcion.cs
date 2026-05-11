namespace WorldCupPlanner.Dominio.Excepciones;

public class EntidadInvalidaExcepcion : Exception
{
    public EntidadInvalidaExcepcion(string mensaje) : base(mensaje)
    {
    }
}