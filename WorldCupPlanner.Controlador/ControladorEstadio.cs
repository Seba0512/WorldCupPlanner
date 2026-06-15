using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.Controladores;

public class ControladorEstadio
{
    private readonly ServicioEstadio _servicioEstadio;

    public ControladorEstadio(ServicioEstadio servicioEstadio)
    {
        _servicioEstadio = servicioEstadio;
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

    public void Crear(EstadioDTO estadio, Usuario? usuarioActual = null) =>
        _servicioEstadio.AgregarEstadio(estadio.ToEntity(), usuarioActual);

    public void Actualizar(string nombreOriginal, Estadio estadio, Usuario? usuarioActual = null) =>
        _servicioEstadio.EditarEstadio(nombreOriginal, estadio, usuarioActual);

    public void Actualizar(string nombreOriginal, EstadioDTO estadio, Usuario? usuarioActual = null) =>
        _servicioEstadio.EditarEstadio(nombreOriginal, estadio.ToEntity(), usuarioActual);

    public void Eliminar(Estadio estadio, Usuario? usuarioActual = null) =>
        _servicioEstadio.EliminarEstadio(estadio, usuarioActual);

    public void Eliminar(string nombre, Usuario? usuarioActual = null) =>
        _servicioEstadio.EliminarEstadio(ObtenerPorNombre(nombre), usuarioActual);

    public void RegistrarFixtureGenerado(Fixture fixture) =>
        _servicioEstadio.RegistrarFixtureGenerado(fixture);

    public void LimpiarFixtureGenerado() =>
        _servicioEstadio.LimpiarFixtureGenerado();
}
