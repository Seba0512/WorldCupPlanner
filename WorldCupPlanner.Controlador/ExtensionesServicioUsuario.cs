using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.Controladores;

internal static class ExtensionesServicioUsuario
{
    public static Usuario? ResolverUsuario(this ServicioUsuario? servicioUsuario, UsuarioDTO? dto)
    {
        if (dto == null) return null;

        if (servicioUsuario == null)
            throw new InvalidOperationException("El servicio de usuario no fue configurado.");

        return servicioUsuario.ObtenerPorEmail(dto.Email)
               ?? throw new EntidadInexistenteExcepcion($"No existe un usuario con el email '{dto.Email}'.");
    }
}
