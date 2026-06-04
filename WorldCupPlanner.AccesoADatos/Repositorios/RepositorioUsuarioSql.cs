using Microsoft.EntityFrameworkCore;
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioUsuarioSql : IRepositorioUsuario, IRepositorioConClaveEditable<Usuario>
{
    private readonly SqlContext _contexto;

    public RepositorioUsuarioSql(SqlContext contexto)
    {
        _contexto = contexto;
    }

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

        DesvincularInstanciaLocal(usuario.Id, usuario);
        _contexto.Usuarios.Update(usuario);
        _contexto.SaveChanges();
    }

    public void ActualizarClave(string claveOriginal, Usuario usuario)
    {
        var existe = _contexto.Usuarios.Any(u => u.Email == claveOriginal && u.Id == usuario.Id);
        if (!existe)
            throw new EntidadInexistenteExcepcion("El usuario no existe.");

        DesvincularInstanciaLocal(usuario.Id, usuario);
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
        if (usuario.Id > 0)
            return _contexto.Usuarios.FirstOrDefault(u => u.Id == usuario.Id);

        return _contexto.Usuarios.FirstOrDefault(u => u.Email == usuario.Email);
    }

    private void DesvincularInstanciaLocal(int id, Usuario entidad)
    {
        var local = _contexto.Usuarios.Local.FirstOrDefault(u => u.Id == id);
        if (local != null && !ReferenceEquals(local, entidad))
            _contexto.Entry(local).State = EntityState.Detached;
    }
}
