using ServiControl.Domain.Enums;

namespace ServiControl.Domain.Entities;

public class Trabajo
{
    public int Id { get; private set; }
    public int ClienteId { get; private set; }
    public int UsuarioId { get; private set; }
    public CategoriaServicio CategoriaServicio { get; private set; }
    public string Descripcion { get; private set; }
    public DateOnly Fecha { get; private set; }
    public string Direccion { get; private set; }
    public string? Observaciones { get; private set; }
    public EstadoTrabajo Estado { get; private set; }

    public Trabajo(
        int clienteId,
        int usuarioId,
        CategoriaServicio categoriaServicio,
        string descripcion,
        DateOnly fecha,
        string direccion,
        string? observaciones = null)
    {
        if (clienteId <= 0)
        {
            throw new ArgumentException("El cliente asociado es obligatorio.", nameof(clienteId));
        }

        if (usuarioId <= 0)
        {
            throw new ArgumentException("El usuario asociado es obligatorio.", nameof(usuarioId));
        }

        if (string.IsNullOrWhiteSpace(descripcion))
        {
            throw new ArgumentException("La descripcion del trabajo es obligatoria.", nameof(descripcion));
        }

        if (string.IsNullOrWhiteSpace(direccion))
        {
            throw new ArgumentException("La direccion del trabajo es obligatoria.", nameof(direccion));
        }

        ClienteId = clienteId;
        UsuarioId = usuarioId;
        CategoriaServicio = categoriaServicio;
        Descripcion = descripcion;
        Fecha = fecha;
        Direccion = direccion;
        Observaciones = observaciones;
        Estado = EstadoTrabajo.Pendiente;
    }

    public void ActualizarDatos(
        CategoriaServicio categoriaServicio,
        string descripcion,
        DateOnly fecha,
        string direccion,
        string? observaciones)
    {
        ValidarModificable();

        if (string.IsNullOrWhiteSpace(descripcion))
        {
            throw new ArgumentException("La descripcion del trabajo es obligatoria.", nameof(descripcion));
        }

        if (string.IsNullOrWhiteSpace(direccion))
        {
            throw new ArgumentException("La direccion del trabajo es obligatoria.", nameof(direccion));
        }

        CategoriaServicio = categoriaServicio;
        Descripcion = descripcion;
        Fecha = fecha;
        Direccion = direccion;
        Observaciones = observaciones;
    }

    public void Iniciar()
    {
        ValidarModificable();
        Estado = EstadoTrabajo.EnProceso;
    }

    public void Finalizar()
    {
        if (Estado == EstadoTrabajo.Cancelado)
        {
            throw new InvalidOperationException("No se puede finalizar un trabajo cancelado.");
        }

        ValidarModificable();
        Estado = EstadoTrabajo.Finalizado;
    }

    public void Cancelar()
    {
        ValidarModificable();
        Estado = EstadoTrabajo.Cancelado;
    }

    private void ValidarModificable()
    {
        if (Estado == EstadoTrabajo.Finalizado)
        {
            throw new InvalidOperationException("No se puede modificar un trabajo finalizado.");
        }
    }
}
