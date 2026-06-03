namespace WorldCupPlanner.Dominio.Entidades
{
    public class Estadio
    {
        public int Id { get; set; } 
        public string Nombre { get; set; } = "";
        public string Ciudad { get; set; } = "";
        public int Capacidad { get; set; }
        public string? Descripcion { get; set; }
        


        public bool EsValido()
        {
            return !string.IsNullOrWhiteSpace(Nombre) && Nombre.Length <= 80 && !string.IsNullOrWhiteSpace(Ciudad) && Ciudad.Length <= 60 && Capacidad >= 20000 && (Descripcion == null || Descripcion.Length <= 400);
        }
    }
}
