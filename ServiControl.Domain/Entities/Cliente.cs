namespace ServiControl.Domain.Entities;

public class Cliente
{
    public int Id { get; private set; }
    public int UsuarioId { get; private set; }
    public string Nombre { get; private set; }
    public string Telefono { get; private set; }
    public string? Email { get; private set; }
    public string? Observaciones { get; private set; }

    public Cliente(
        int usuarioId,
        string nombre,
        string telefono,
        string? email = null,
        string? observaciones = null)
    {
        if (usuarioId <= 0)
        {
            throw new ArgumentException("El usuario asociado es obligatorio.", nameof(usuarioId));
        }

        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("El nombre del cliente es obligatorio.", nameof(nombre));
        }

        if (string.IsNullOrWhiteSpace(telefono))
        {
            throw new ArgumentException("El telefono del cliente es obligatorio.", nameof(telefono));
        }

        UsuarioId = usuarioId;
        Nombre = nombre;
        Telefono = telefono;
        Email = email;
        Observaciones = observaciones;
    }

    public void ActualizarDatos(string nombre, string telefono, string? email, string? observaciones)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("El nombre del cliente es obligatorio.", nameof(nombre));
        }

        if (string.IsNullOrWhiteSpace(telefono))
        {
            throw new ArgumentException("El telefono del cliente es obligatorio.", nameof(telefono));
        }

        Nombre = nombre;
        Telefono = telefono;
        Email = email;
        Observaciones = observaciones;
    }
}
