using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Dominio.Entidades;

public class Usuario
{
    private const string PatronEmail = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";
    private const string PatronFormatoFecha = @"^\d{2}-\d{2}-\d{4}$";
    private const string PatronMayuscula = @"[A-Z]";
    private const string PatronMinuscula = @"[a-z]";
    private const string PatronNumero = @"[0-9]";
    private const string PatronEspecial = @"[@#$\.\!\&\*\-_\+=\?]";

    private string? _contraseniaCifrada;
    private HashSet<RolUsuario> _roles;

    public string Nombre { get; private set; }
    public string Apellido { get; private set; }
    public string Email { get; private set; }
    public DateTime FechaNacimiento { get; private set; }
    public string? ContraseniaCifrada => _contraseniaCifrada;

    public Usuario(DatosUsuario datos)
    {
        ValidarNombre(datos.Nombre);
        ValidarApellido(datos.Apellido);
        ValidarEmail(datos.Email);

        var fecha = ParsearFechaNacimiento(datos.FechaNacimiento);
        ValidarFechaNacimiento(fecha);

        Nombre = datos.Nombre;
        Apellido = datos.Apellido;
        Email = datos.Email;
        FechaNacimiento = fecha;
        _roles = new HashSet<RolUsuario>();
    }

    private static void ValidarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre no puede estar vacío.", nameof(nombre));
    }

    private static void ValidarApellido(string apellido)
    {
        if (string.IsNullOrWhiteSpace(apellido))
            throw new ArgumentException("El apellido no puede estar vacío.", nameof(apellido));
    }

    private static void ValidarEmail(string email)
    {
        ValidarEmailNoVacio(email);
        ValidarFormatoEmail(email);
    }

    private static void ValidarEmailNoVacio(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("El email no puede estar vacío.", nameof(email));
    }

    private static void ValidarFormatoEmail(string email)
    {
        if (!Regex.IsMatch(email, PatronEmail))
            throw new ArgumentException("El email no es válido.", nameof(email));
    }

    private static void ValidarFechaNacimientoNoVacia(string fechaNacimiento)
    {
        if (string.IsNullOrWhiteSpace(fechaNacimiento))
            throw new ArgumentException("La fecha de nacimiento no puede estar vacía.", nameof(fechaNacimiento));
    }

    private static void ValidarFormatoFechaNacimiento(string fechaNacimiento)
    {
        if (!Regex.IsMatch(fechaNacimiento, PatronFormatoFecha))
            throw new ArgumentException("La fecha de nacimiento debe tener el formato MM-DD-AAAA.", nameof(fechaNacimiento));
    }

    private static DateTime ParsearFechaNacimiento(string fechaNacimiento)
    {
        const string formato = "MM-dd-yyyy";

        ValidarFechaNacimientoNoVacia(fechaNacimiento);
        ValidarFormatoFechaNacimiento(fechaNacimiento);

        if (!DateTime.TryParseExact(fechaNacimiento, formato,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out var fecha))
            throw new ArgumentException("La fecha de nacimiento no es una fecha válida.", nameof(fechaNacimiento));

        return fecha;
    }

    private static void ValidarFechaNacimiento(DateTime fechaNacimiento)
    {
        if (fechaNacimiento > DateTime.Now)
            throw new ArgumentException("La fecha de nacimiento no puede ser futura.", nameof(fechaNacimiento));
    }

    public void EstablecerContrasenia(string contrasenia)
    {
        ValidarContrasenia(contrasenia);
        _contraseniaCifrada = CifrarContrasenia(contrasenia);
    }

    private static void ValidarContrasenia(string contrasenia)
    {
        ValidarContraseniaNoVacia(contrasenia);
        ValidarLongitudMinima(contrasenia);
        ValidarTieneMayuscula(contrasenia);
        ValidarTieneMinuscula(contrasenia);
        ValidarTieneNumero(contrasenia);
        ValidarTieneCaracterEspecial(contrasenia);
    }

    private static void ValidarContraseniaNoVacia(string contrasenia)
    {
        if (string.IsNullOrWhiteSpace(contrasenia))
            throw new ArgumentException("La contrasenia no puede estar vacía.", nameof(contrasenia));
    }

    private static void ValidarLongitudMinima(string contrasenia)
    {
        if (contrasenia.Length < 8)
            throw new ArgumentException("La contrasenia debe tener al menos 8 caracteres.", nameof(contrasenia));
    }

    private static void ValidarTieneMayuscula(string contrasenia)
    {
        if (!Regex.IsMatch(contrasenia, PatronMayuscula))
            throw new ArgumentException("La contrasenia debe incluir al menos una letra mayúscula.", nameof(contrasenia));
    }

    private static void ValidarTieneMinuscula(string contrasenia)
    {
        if (!Regex.IsMatch(contrasenia, PatronMinuscula))
            throw new ArgumentException("La contrasenia debe incluir al menos una letra minúscula.", nameof(contrasenia));
    }

    private static void ValidarTieneNumero(string contrasenia)
    {
        if (!Regex.IsMatch(contrasenia, PatronNumero))
            throw new ArgumentException("La contrasenia debe incluir al menos un número.", nameof(contrasenia));
    }

    private static void ValidarTieneCaracterEspecial(string contrasenia)
    {
        if (!Regex.IsMatch(contrasenia, PatronEspecial))
            throw new ArgumentException("La contrasenia debe incluir al menos un carácter especial (@, #, $, ., !, &, *, -, _, +, =, ?).", nameof(contrasenia));
    }

    private static string CifrarContrasenia(string contrasenia)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(contrasenia));
        return Convert.ToBase64String(hashedBytes);
    }

    public bool VerificarContrasenia(string contrasenia)
    {
        if (string.IsNullOrEmpty(_contraseniaCifrada))
            return false;

        var contraseniaCifradaIngresada = CifrarContrasenia(contrasenia);
        return _contraseniaCifrada == contraseniaCifradaIngresada;
    }

    public void AgregarRol(RolUsuario rolUsuario)
    {
        _roles.Add(rolUsuario);
    }

    public void RemoverRol(RolUsuario rolUsuario)
    {
        if (_roles.Count == 1 && _roles.Contains(rolUsuario))
            throw new InvalidOperationException("El usuario debe tener al menos un rol.");
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

    public void ActualizarNombre(string nuevoNombre)
    {
        ValidarNombre(nuevoNombre);
        Nombre = nuevoNombre;
    }

    public void ActualizarApellido(string nuevoApellido)
    {
        ValidarApellido(nuevoApellido);
        Apellido = nuevoApellido;
    }

    public void ActualizarEmail(string nuevoEmail)
    {
        ValidarEmail(nuevoEmail);
        Email = nuevoEmail;
    }

    public void ActualizarFechaNacimiento(string nuevaFecha)
    {
        var fecha = ParsearFechaNacimiento(nuevaFecha);
        ValidarFechaNacimiento(fecha);
        FechaNacimiento = fecha;
    }
}
