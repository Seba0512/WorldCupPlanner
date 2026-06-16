using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Dominio.Entidades
{
    public class Equipo
    {
        private const string PrefijoSvgBase64 = "data:image/svg+xml;base64,";
        private const int TamanoMaximoBanderaCaracteres = 2_800_000;

        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public int RankingFIFA { get; set; }
        public int RankingInicial { get; set; }
        public Confederacion? Confederacion { get; set; }
        public string Bandera { get; set; } = "";

        public bool EsValido()
        {
            return !string.IsNullOrWhiteSpace(Nombre) && Nombre.Length <= 60 && RankingFIFA > 299 &&
                   RankingFIFA < 2501 && Confederacion.HasValue && BanderaEsValida();
        }

        private bool BanderaEsValida() =>
            string.IsNullOrEmpty(Bandera) ||
            (Bandera.Length <= TamanoMaximoBanderaCaracteres &&
             Bandera.StartsWith(PrefijoSvgBase64));
    }
}
