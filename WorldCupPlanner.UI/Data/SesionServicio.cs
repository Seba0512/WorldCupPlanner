using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.UI.Data
{
    public class SesionServicio
    {
        public Usuario? UsuarioActual { get; private set; }

        public bool HaySesion => UsuarioActual != null;

        public bool EsAdministrador => UsuarioActual?.TieneRol(RolUsuario.AdministradorDelSistema) ?? false;

        public bool EsEditor => UsuarioActual?.TieneRol(RolUsuario.Editor) ?? false;

        public bool EsPeriodista => UsuarioActual?.TieneRol(RolUsuario.Periodista) ?? false;

        public bool PuedeEditarPartidos => EsAdministrador || EsEditor;

        public bool PuedeExportar => EsAdministrador || EsPeriodista;

        public bool PuedeGestionarEquipos => EsAdministrador;

        public bool PuedeImportarEquipos => EsAdministrador;

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
