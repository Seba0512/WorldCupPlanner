using System;
namespace WorldCupPlanner.Domain
{
	public class Equipo
	{

        public string Nombre { get; set; } = ""; //arranca vacio por defecto
        public bool EsValido()
        {
	        return !string.IsNullOrWhiteSpace(Nombre) && Nombre.Length <= 60;
        }

        public Equipo()
		{
		}
	}
}

