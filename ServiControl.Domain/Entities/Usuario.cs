using ServiControl.Domain.Enums;

namespace ServiControl.Domain.Entities;

public class Usuario
{
    public int Id { get; private set; }
    public string Nombre { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public RolUsuario Rol { get; private set; }
    public int? IdUsuarioResponsable { get; private set; }
    public Usuario? UsuarioResponsable { get; private set; }

    private Usuario()
    {
        Nombre = string.Empty;
        Email = string.Empty;
        PasswordHash = string.Empty;
    }

    public Usuario(
        string nombre,
        string email,
        string passwordHash,
        RolUsuario rol,
        int? responsableId = null)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("El nombre del usuario es obligatorio.", nameof(nombre));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("El email del usuario es obligatorio.", nameof(email));
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("La password del usuario es obligatoria.", nameof(passwordHash));
        }

        Nombre = nombre;
        Email = email;
        PasswordHash = passwordHash;
        ConfigurarRolYResponsable(rol, responsableId);
    }

    public void ActualizarDatos(string nombre, string email)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("El nombre del usuario es obligatorio.", nameof(nombre));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("El email del usuario es obligatorio.", nameof(email));
        }

        Nombre = nombre;
        Email = email;
    }

    public void CambiarRol(RolUsuario rol, int? idUsuarioResponsable = null)
    {
        ConfigurarRolYResponsable(rol, idUsuarioResponsable);
    }

    private void ConfigurarRolYResponsable(RolUsuario rol, int? idUsuarioResponsable)
    {
        ValidarRol(rol);

        if (rol == RolUsuario.Assistant)
        {
            if (!idUsuarioResponsable.HasValue || idUsuarioResponsable.Value <= 0)
            {
                throw new ArgumentException(
                    "Un asistente debe tener un tecnico responsable.",
                    nameof(idUsuarioResponsable));
            }

            Rol = rol;
            IdUsuarioResponsable = idUsuarioResponsable;
            return;
        }

        Rol = rol;
        IdUsuarioResponsable = null;
    }

    private static void ValidarRol(RolUsuario rol)
    {
        if (!Enum.IsDefined(rol))
        {
            throw new ArgumentException("El rol indicado no es valido.", nameof(rol));
        }
    }
}
