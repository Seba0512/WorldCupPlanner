using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.UI.Helpers;

public static class UsuarioFormHelper
{
    public static List<RolUsuario> ConstruirRoles(bool esAdmin, bool esEditor, bool esPeriodista)
    {
        var roles = new List<RolUsuario>();
        if (esAdmin)      roles.Add(RolUsuario.AdministradorDelSistema);
        if (esEditor)     roles.Add(RolUsuario.Editor);
        if (esPeriodista) roles.Add(RolUsuario.Periodista);
        return roles;
    }
}
