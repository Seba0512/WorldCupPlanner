using Microsoft.EntityFrameworkCore;
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioUsuarioSql : RepositorioBase<Usuario>, IRepositorioUsuario, IRepositorioConClaveEditable<Usuario>
{
    public RepositorioUsuarioSql(SqlContext contexto) : base(contexto) { }

    public void Agregar(Usuario usuario)
    {
        _contexto.Usuarios.Add(usuario);
        _contexto.SaveChanges();
    }

    public void Actualizar(Usuario usuario)
    {
        var existe = _contexto.Usuarios.Any(u => u.Id == usuario.Id);
        if (!existe)
            throw new EntidadInexistenteExcepcion("El usuario no existe.");

        DesvincularInstanciaLocal(_contexto.Usuarios, usuario.Id, usuario);
        _contexto.Usuarios.Update(usuario);
        _contexto.SaveChanges();
    }

    public void ActualizarClave(string claveOriginal, Usuario usuario)
    {
        var existe = _contexto.Usuarios.Any(u => u.Email == claveOriginal && u.Id == usuario.Id);
        if (!existe)
            throw new EntidadInexistenteExcepcion("El usuario no existe.");

        DesvincularInstanciaLocal(_contexto.Usuarios, usuario.Id, usuario);
        _contexto.Usuarios.Update(usuario);
        _contexto.SaveChanges();
    }

    public void Eliminar(Usuario usuario)
    {
        var existente = ObtenerUsuarioPersistido(usuario);
        if (existente == null)
            throw new EntidadInexistenteExcepcion("El usuario no existe.");

        _contexto.Usuarios.Remove(existente);
        _contexto.SaveChanges();
    }

    public List<Usuario> Buscar(Func<Usuario, bool> predicado)
    {
        return _contexto.Usuarios.AsNoTracking().Where(predicado).ToList();
    }

    public Usuario? ObtenerPorEmail(string email)
    {
        return _contexto.Usuarios.AsNoTracking()
            .FirstOrDefault(u => u.Email == email);
    }

    private Usuario? ObtenerUsuarioPersistido(Usuario usuario)
    {
        if (usuario.Id > IdNoAsignado)
            return _contexto.Usuarios.FirstOrDefault(u => u.Id == usuario.Id);

        return _contexto.Usuarios.FirstOrDefault(u => u.Email == usuario.Email);
    }
}
