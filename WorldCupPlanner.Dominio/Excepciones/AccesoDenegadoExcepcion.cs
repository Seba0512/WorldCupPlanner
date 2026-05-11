namespace WorldCupPlanner.Dominio.Excepciones;

public class AccesoDenegadoExcepcion : Exception
{
    public AccesoDenegadoExcepcion(string mensaje) : base(mensaje) { }
}
