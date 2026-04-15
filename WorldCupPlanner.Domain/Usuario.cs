namespace WorldCupPlanner.Domain;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

public class Usuario
{
    private string? _contraseñaCifrada;
    private HashSet<Rol> _roles;

    public string Nombre { get; private set; }
    public string Apellido { get; private set; }
    public string Email { get; private set; }
    public DateTime FechaNacimiento { get; private set; }
    public string? ContraseñaCifrada => _contraseñaCifrada;

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
        _roles = new HashSet<Rol>();
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

    #region Gestión de Contraseña

    public void EstablecerContraseña(string contraseña)
    {
        ValidarContraseña(contraseña);
        _contraseñaCifrada = CifrarContraseña(contraseña);
    }

    private void ValidarContraseña(string contraseña)
    {
        if (string.IsNullOrWhiteSpace(contraseña))
        {
            throw new ArgumentException("La contraseña no puede estar vacía.", nameof(contraseña));
        }

        if (contraseña.Length < 8)
        {
            throw new ArgumentException("La contraseña debe tener al menos 8 caracteres.", nameof(contraseña));
        }

        if (!Regex.IsMatch(contraseña, @"[A-Z]"))
        {
            throw new ArgumentException("La contraseña debe incluir al menos una letra mayúscula.", nameof(contraseña));
        }

        if (!Regex.IsMatch(contraseña, @"[a-z]"))
        {
            throw new ArgumentException("La contraseña debe incluir al menos una letra minúscula.", nameof(contraseña));
        }

        if (!Regex.IsMatch(contraseña, @"[0-9]"))
        {
            throw new ArgumentException("La contraseña debe incluir al menos un número.", nameof(contraseña));
        }

        if (!Regex.IsMatch(contraseña, @"[@#$\.\!\&\*\-_\+=\?]"))
        {
            throw new ArgumentException("La contraseña debe incluir al menos un carácter especial (@, #, $, ., !, &, *, -, _, +, =, ?).", nameof(contraseña));
        }
    }

    private string CifrarContraseña(string contraseña)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(contraseña));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    public bool VerificarContraseña(string contraseña)
    {
        if (string.IsNullOrEmpty(_contraseñaCifrada))
        {
            return false;
        }

        var contraseñaCifradaIngresada = CifrarContraseña(contraseña);
        return _contraseñaCifrada == contraseñaCifradaIngresada;
    }

    #endregion

    #region Gestión de Roles

    public void AgregarRol(Rol rol)
    {
        _roles.Add(rol);
    }

    public void RemoverRol(Rol rol)
    {
        _roles.Remove(rol);
    }

    public bool TieneRol(Rol rol)
    {
        return _roles.Contains(rol);
    }

    public IEnumerable<Rol> ObtenerRoles()
    {
        return _roles.AsEnumerable();
    }

    #endregion
}