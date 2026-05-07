using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.UI.Data
{
    public class SesionServicio
    {
        public Usuario? UsuarioActual { get; private set; }

        public bool HaySesion => UsuarioActual != null;

        public void IniciarSesion(Usuario usuario)
        {
            UsuarioActual = usuario;
        }

        public void CerrarSesion()
        {
            UsuarioActual = null;
        }
    }
}