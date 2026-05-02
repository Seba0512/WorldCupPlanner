using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Dominio.Entidades;

public class Usuario
{
    private string? _contraseniaCifrada;
    private HashSet<RolUsuario> _roles;

    public string Nombre { get; private set; }
    public string Apellido { get; private set; }
    public string Email { get; private set; }
    public DateTime FechaNacimiento { get; private set; }
    public string? ContraseniaCifrada => _contraseniaCifrada;

    public Usuario(string nombre, string apellido, string email, DateTime fechaNacimiento)
    {
        ValidarNombre(nombre);
        ValidarApellido(apellido);
        ValidarEmail(email);
        ValidarFechaNacimiento(fechaNacimiento);

        Nombre = nombre;
        Apellido = apellido;
        Email = email;
        FechaNacimiento = fechaNacimiento;
        _roles = new HashSet<RolUsuario>();
    }

    #region Validaciones

    private void ValidarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("El nombre no puede estar vacío.", nameof(nombre));
        }
    }

    private void ValidarApellido(string apellido)
    {
        if (string.IsNullOrWhiteSpace(apellido))
        {
            throw new ArgumentException("El apellido no puede estar vacío.", nameof(apellido));
        }
    }

    private void ValidarEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("El email no puede estar vacío.", nameof(email));
        }

        // Validar formato de email básico
        var patronEmail = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";
        if (!Regex.IsMatch(email, patronEmail))
        {
            throw new ArgumentException("El email no es válido.", nameof(email));
        }
    }

    private void ValidarFechaNacimiento(DateTime fechaNacimiento)
    {
        if (fechaNacimiento > DateTime.Now)
        {
            throw new ArgumentException("La fecha de nacimiento no puede ser futura.", nameof(fechaNacimiento));
        }
    }

    #endregion

    #region Gestión de Contrasenia

    public void EstablecerContrasenia(string contrasenia)
    {
        ValidarContrasenia(contrasenia);
        _contraseniaCifrada = CifrarContrasenia(contrasenia);
    }

    private void ValidarContrasenia(string contrasenia)
    {
        if (string.IsNullOrWhiteSpace(contrasenia))
        {
            throw new ArgumentException("La contrasenia no puede estar vacía.", nameof(contrasenia));
        }

        if (contrasenia.Length < 8)
        {
            throw new ArgumentException("La contrasenia debe tener al menos 8 caracteres.", nameof(contrasenia));
        }

        if (!Regex.IsMatch(contrasenia, @"[A-Z]"))
        {
            throw new ArgumentException("La contrasenia debe incluir al menos una letra mayúscula.", nameof(contrasenia));
        }

        if (!Regex.IsMatch(contrasenia, @"[a-z]"))
        {
            throw new ArgumentException("La contrasenia debe incluir al menos una letra minúscula.", nameof(contrasenia));
        }

        if (!Regex.IsMatch(contrasenia, @"[0-9]"))
        {
            throw new ArgumentException("La contrasenia debe incluir al menos un número.", nameof(contrasenia));
        }

        if (!Regex.IsMatch(contrasenia, @"[@#$\.\!\&\*\-_\+=\?]"))
        {
            throw new ArgumentException("La contrasenia debe incluir al menos un carácter especial (@, #, $, ., !, &, *, -, _, +, =, ?).", nameof(contrasenia));
        }
    }

    private string CifrarContrasenia(string contrasenia)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(contrasenia));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    public bool VerificarContrasenia(string contrasenia)
    {
        if (string.IsNullOrEmpty(_contraseniaCifrada))
        {
            return false;
        }

        var contraseniaCifradaIngresada = CifrarContrasenia(contrasenia);
        return _contraseniaCifrada == contraseniaCifradaIngresada;
    }

    #endregion

    #region Gestión de Roles

    public void AgregarRol(RolUsuario rolUsuario)
    {
        _roles.Add(rolUsuario);
    }

    public void RemoverRol(RolUsuario rolUsuario)
    {
        _roles.Remove(rolUsuario);
    }

    public bool TieneRol(RolUsuario rolUsuario)
    {
        return _roles.Contains(rolUsuario);
    }

    public IEnumerable<RolUsuario> ObtenerRoles()
    {
        return _roles.AsEnumerable();
    }

    #endregion
}