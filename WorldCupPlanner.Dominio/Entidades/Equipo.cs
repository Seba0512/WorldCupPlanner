using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Dominio.Entidades
{
	public class Equipo
	{

		public string Nombre { get; set; } = ""; //arranca vacio por defecto
		public int RankingFIFA { get; set; } 
		public Confederacion? Confederacion { get; set; } //puede ser nula
	
	public bool EsValido()
	{
		return !string.IsNullOrWhiteSpace(Nombre) && Nombre.Length <= 60 && RankingFIFA > 299 && RankingFIFA <2501 && Confederacion.HasValue;

	}

        public Equipo()
		{
			
		}
	}
}

