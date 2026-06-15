using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.Servicios;

public class ServicioAutorizacion
{
    public void VerificarPermiso(Usuario usuario, RolUsuario rolRequerido)
    {
        if (!usuario.TieneRol(rolRequerido))
            throw new AccesoDenegadoExcepcion(
                "No tenés los permisos necesarios para realizar esta acción.");
    }

    public void VerificarEsAdministrador(Usuario usuario)
        => VerificarPermiso(usuario, RolUsuario.AdministradorDelSistema);

    public void VerificarEsPeriodista(Usuario usuario)
        => VerificarPermiso(usuario, RolUsuario.Periodista);

    public void VerificarPuedeConsultarFixture(Usuario usuario)
        => VerificarTieneAlgunRol(usuario,
            RolUsuario.Periodista,
            RolUsuario.Editor,
            RolUsuario.AdministradorDelSistema);

    public void VerificarEsEditorOAdministrador(Usuario usuario)
        => VerificarTieneAlgunRol(usuario,
            RolUsuario.Editor,
            RolUsuario.AdministradorDelSistema);

    private void VerificarTieneAlgunRol(Usuario usuario, params RolUsuario[] roles)
    {
        if (!roles.Any(usuario.TieneRol))
            throw new AccesoDenegadoExcepcion(
                "No tenés los permisos necesarios para realizar esta acción.");
    }
}
