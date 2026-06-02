using WorldCupPlanner.AccesoADatos.Interfaces;
using Microsoft.EntityFrameworkCore;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioUsuarioSql : IRepositorioUsuario, IRepositorioConClaveEditable<Usuario>
{
    private readonly SqlContext _context;

    public RepositorioUsuarioSql(SqlContext context)
    {
        _context = context;
    }

    public void Agregar(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        GuardarCambios();
    }

    public void Actualizar(Usuario usuario)
    {
        if (!_context.Usuarios.AsNoTracking().Any(u => u.Email == usuario.Email))
        {
            throw new EntidadInexistenteExcepcion("El usuario no existe.");
        }

        DesadjuntarLocal(usuario.Email);
        _context.Usuarios.Update(usuario);
        GuardarCambios();
    }

    public void ActualizarClave(string emailOriginal, Usuario usuario)
    {
        if (emailOriginal.Equals(usuario.Email, StringComparison.OrdinalIgnoreCase))
        {
            Actualizar(usuario);
            return;
        }

        var existente = _context.Usuarios
            .AsNoTracking()
            .FirstOrDefault(u => u.Email == emailOriginal);

        if (existente == null)
        {
            throw new EntidadInexistenteExcepcion("El usuario no existe.");
        }

        DesadjuntarLocal(emailOriginal);
        DesadjuntarLocal(usuario.Email);

        using var transaction = _context.Database.BeginTransaction();
        try
        {
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            _context.Database.ExecuteSqlInterpolated(
                $"DELETE FROM Usuarios WHERE Email = {emailOriginal}");

            transaction.Commit();
        }
        finally
        {
            _context.ChangeTracker.Clear();
        }
    }

    public void Eliminar(Usuario usuario)
    {
        var existente = ObtenerPorEmail(usuario.Email);

        if (existente == null)
        {
            return;
        }

        _context.Usuarios.Remove(existente);
        GuardarCambios();
    }

    public List<Usuario> Buscar(Func<Usuario, bool> predicado)
    {
        return _context.Usuarios
            .AsNoTracking()
            .ToList()
            .Where(predicado)
            .ToList();
    }

    public Usuario? ObtenerPorEmail(string email)
    {
        return _context.Usuarios
            .AsNoTracking()
            .ToList()
            .FirstOrDefault(usuario => usuario.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }

    private void GuardarCambios()
    {
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
    }

    private void DesadjuntarLocal(string email)
    {
        var entidadLocal = _context.Usuarios.Local
            .FirstOrDefault(usuario => usuario.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

        if (entidadLocal != null)
        {
            _context.Entry(entidadLocal).State = EntityState.Detached;
        }
    }
}
