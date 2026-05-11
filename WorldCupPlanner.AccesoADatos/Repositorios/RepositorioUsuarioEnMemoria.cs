using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioUsuarioEnMemoria : IRepositorioUsuario
{
    private readonly List<Usuario> _usuarios = new();

    public void Agregar(Usuario usuario)
    {
        _usuarios.Add(usuario);
    }

    public void Actualizar(Usuario usuario)
    {
        var indice = _usuarios.FindIndex(u => ReferenceEquals(u, usuario));

        if (indice == -1)
        {
            throw new InvalidOperationException("El usuario no existe.");
        }

        _usuarios[indice] = usuario;
    }

    public void Eliminar(Usuario usuario)
    {
        _usuarios.Remove(usuario);
    }

    public List<Usuario> Buscar(Func<Usuario, bool> predicado)
    {
        var resultados = new List<Usuario>();

        foreach (var usuario in _usuarios)
        {
            if (predicado(usuario))
            {
                resultados.Add(usuario);
            }
        }

        return resultados;
    }

    public Usuario? ObtenerPorEmail(string email)
    {
        return _usuarios.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }
}

