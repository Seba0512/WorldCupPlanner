using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.Controladores;

public class ControladorEstadio
{
    private readonly ServicioEstadio _servicioEstadio;
    private readonly ServicioUsuario? _servicioUsuario;

    public ControladorEstadio(ServicioEstadio servicioEstadio, ServicioUsuario? servicioUsuario = null)
    {
        _servicioEstadio = servicioEstadio;
        _servicioUsuario = servicioUsuario;
    }

    public List<Estadio> Listar() =>
        _servicioEstadio.ObtenerEstadios();

    public List<EstadioDTO> ListarDTO() =>
        Listar().Select(EstadioDTO.FromEntity).ToList();

    public Estadio ObtenerPorNombre(string nombre) =>
        _servicioEstadio.ObtenerPorNombre(nombre);

    public EstadioDTO ObtenerPorNombreDTO(string nombre) =>
        EstadioDTO.FromEntity(ObtenerPorNombre(nombre));

    public Estadio ObtenerParaEdicion(string nombre) =>
        _servicioEstadio.ClonarParaEdicion(nombre);

    public EstadioDTO ObtenerParaEdicionDTO(string nombre) =>
        EstadioDTO.FromEntity(ObtenerParaEdicion(nombre));

    public void Crear(Estadio estadio, Usuario? usuarioActual = null) =>
        _servicioEstadio.AgregarEstadio(estadio, usuarioActual);

    public void Crear(EstadioDTO estadio, UsuarioDTO? usuarioActual = null) =>
        _servicioEstadio.AgregarEstadio(MapeadorDtoDominio.AEstadio(estadio), _servicioUsuario.ResolverUsuario(usuarioActual));

    public void Actualizar(string nombreOriginal, Estadio estadio, Usuario? usuarioActual = null) =>
        _servicioEstadio.EditarEstadio(nombreOriginal, estadio, usuarioActual);

    public void Actualizar(string nombreOriginal, EstadioDTO estadio, UsuarioDTO? usuarioActual = null) =>
        _servicioEstadio.EditarEstadio(nombreOriginal, MapeadorDtoDominio.AEstadio(estadio), _servicioUsuario.ResolverUsuario(usuarioActual));

    public void Eliminar(Estadio estadio, Usuario? usuarioActual = null) =>
        _servicioEstadio.EliminarEstadio(estadio, usuarioActual);

    public void Eliminar(string nombre, UsuarioDTO? usuarioActual = null) =>
        _servicioEstadio.EliminarEstadio(ObtenerPorNombre(nombre), _servicioUsuario.ResolverUsuario(usuarioActual));

    public void RegistrarFixtureGenerado(Fixture fixture) =>
        _servicioEstadio.RegistrarFixtureGenerado(fixture);

    public void LimpiarFixtureGenerado() =>
        _servicioEstadio.LimpiarFixtureGenerado();

}
