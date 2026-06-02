using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioUsuarioSql : IRepositorioUsuario
{
    private readonly SqlContext _context;

    public RepositorioUsuarioSql(SqlContext context)
    {
        _context = context;
    }

    public void Agregar(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        _context.SaveChanges();
    }

    public void Actualizar(Usuario usuario)
    {
        var existente = ObtenerPorEmail(usuario.Email);

        if (existente == null)
        {
            throw new EntidadInexistenteExcepcion("El usuario no existe.");
        }

        _context.SaveChanges();
    }

    public void Eliminar(Usuario usuario)
    {
        var existente = ObtenerPorEmail(usuario.Email);

        if (existente == null)
        {
            return;
        }

        _context.Usuarios.Remove(existente);
        _context.SaveChanges();
    }

    public List<Usuario> Buscar(Func<Usuario, bool> predicado)
    {
        return _context.Usuarios
            .ToList()
            .Where(predicado)
            .ToList();
    }

    public Usuario? ObtenerPorEmail(string email)
    {
        return _context.Usuarios
            .ToList()
            .FirstOrDefault(usuario => usuario.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }
}
