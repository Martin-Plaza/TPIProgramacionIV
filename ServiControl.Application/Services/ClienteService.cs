using ServiControl.Application.Authorization;
using ServiControl.Application.DTOs;
using ServiControl.Application.Interfaces;
using ServiControl.Domain.Entities;

namespace ServiControl.Application.Services;

// Modulo: Clientes
// Capa: Application
// Responsabilidad: Coordina casos de uso de clientes y devuelve DTOs a la API.
public class ClienteService : IClienteService
{
    private readonly IClienteRepository _clienteRepository;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IOperationalUserResolver _operationalUserResolver;

    public ClienteService(
        IClienteRepository clienteRepository,
        ICurrentUserContext currentUserContext,
        IOperationalUserResolver operationalUserResolver)
    {
        _clienteRepository = clienteRepository;
        _currentUserContext = currentUserContext;
        _operationalUserResolver = operationalUserResolver;
    }

    public async Task<ClienteResponse> CrearAsync(
        CreateClienteRequest request,
        CancellationToken cancellationToken = default)
    {
        var usuarioOperativoId = await _operationalUserResolver
            .ObtenerUsuarioOperativoIdAsync(cancellationToken);

        //crea un cliente de dominio (en el return devolvemos el DTO creado al final de este modulo MapToResponse)
        var cliente = new Cliente(
            usuarioOperativoId,
            request.Nombre,
            request.Telefono,
            request.Email,
            request.Observaciones);

        var created = await _clienteRepository.AddAsync(cliente, cancellationToken);

        //funcion para devolver DTO creada abajo de este archivo
        return MapToResponse(created);
    }

    public async Task<IReadOnlyList<ClienteResponse>> ObtenerTodosAsync(
        CancellationToken cancellationToken = default)
    {
        var usuarioOperativoId = await _operationalUserResolver
            .ObtenerUsuarioOperativoIdAsync(cancellationToken);

        var clientes = _currentUserContext.IsAdmin
            ? await _clienteRepository.GetAllAsync(cancellationToken)
            : await _clienteRepository.GetByUsuarioAsync(
                usuarioOperativoId,
                cancellationToken);

        return clientes.Select(MapToResponse).ToList();
    }

    public async Task<ClienteResponse> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        //si devuelve algo el repositorio, es decir, se obtuvo algun ID se guarda, sino arroja la excepcion (por eso el "??")
        var cliente = await ObtenerClienteAccesibleAsync(id, cancellationToken);

        return MapToResponse(cliente);
    }

    public async Task<ClienteResponse> ActualizarAsync(
        int id,
        UpdateClienteRequest request,
        CancellationToken cancellationToken = default)
    {
        var cliente = await ObtenerClienteAccesibleAsync(id, cancellationToken);

        cliente.ActualizarDatos(
            request.Nombre,
            request.Telefono,
            request.Email,
            request.Observaciones);

        await _clienteRepository.UpdateAsync(cliente, cancellationToken);

        return MapToResponse(cliente);
    }

    //creador del DTO para devolver respuesta al endPoint
    private static ClienteResponse MapToResponse(Cliente cliente)
    {
        return new ClienteResponse(
            cliente.Id,
            cliente.UsuarioId,
            cliente.Nombre,
            cliente.Telefono,
            cliente.Email,
            cliente.Observaciones);
    }

    private async Task<Cliente> ObtenerClienteAccesibleAsync(
        int clienteId,
        CancellationToken cancellationToken)
    {
        var usuarioOperativoId = await _operationalUserResolver
            .ObtenerUsuarioOperativoIdAsync(cancellationToken);

        var cliente = _currentUserContext.IsAdmin
            ? await _clienteRepository.GetByIdAsync(clienteId, cancellationToken)
            : await _clienteRepository.GetByIdAndUsuarioAsync(
                clienteId,
                usuarioOperativoId,
                cancellationToken);

        return cliente
            ?? throw new ArgumentException("El cliente indicado no existe.", nameof(clienteId));
    }
}
