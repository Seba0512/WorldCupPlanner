using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.UI.Data
{
    public class SesionServicio
    {
        public Usuario? UsuarioActual { get; private set; }

        public bool HaySesion => UsuarioActual != null;

        public bool EsAdministrador => UsuarioActual?.TieneRol(RolUsuario.AdministradorDelSistema) ?? false;

        public event Action? OnChange;

        public void IniciarSesion(Usuario usuario)
        {
            UsuarioActual = usuario;
            OnChange?.Invoke();
        }

        public void CerrarSesion()
        {
            UsuarioActual = null;
            OnChange?.Invoke();
        }
    }
}