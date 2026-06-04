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
                $"El usuario no tiene el rol requerido: {rolRequerido}.");
    }

    public void VerificarEsAdministrador(Usuario usuario)
        => VerificarPermiso(usuario, RolUsuario.AdministradorDelSistema);

    public void VerificarEsPeriodista(Usuario usuario)
        => VerificarPermiso(usuario, RolUsuario.Periodista);

    public void VerificarEsEditorOAdministrador(Usuario usuario)
    {
        if (!usuario.TieneRol(RolUsuario.Editor) && !usuario.TieneRol(RolUsuario.AdministradorDelSistema))
            throw new AccesoDenegadoExcepcion(
                "El usuario debe tener al menos el rol Editor o Administrador del Sistema.");
    }
}
