using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.UI.Data
{
    public class SesionServicio
    {
        public UsuarioDTO? UsuarioActual { get; private set; }

        public bool HaySesion => UsuarioActual != null;

        public bool EsAdministrador => UsuarioActual?.Roles.Contains(RolUsuario.AdministradorDelSistema) ?? false;

        public bool EsEditor => UsuarioActual?.Roles.Contains(RolUsuario.Editor) ?? false;

        public bool EsPeriodista => UsuarioActual?.Roles.Contains(RolUsuario.Periodista) ?? false;

        public bool PuedeEditarPartidos => EsAdministrador || EsEditor;

        public bool PuedeExportar => EsAdministrador || EsPeriodista;

        public bool PuedeGestionarEquipos => EsAdministrador;

        public bool PuedeImportarEquipos => EsAdministrador;

        public event Action? OnChange;

        public void IniciarSesion(UsuarioDTO usuario)
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
