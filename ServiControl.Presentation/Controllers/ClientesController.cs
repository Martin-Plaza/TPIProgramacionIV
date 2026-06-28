using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiControl.Application.Authorization;
using ServiControl.Application.DTOs;
using ServiControl.Application.Interfaces;

namespace ServiControl.Presentation.Controllers;

// Modulo: Clientes
// Capa: Presentation
// Responsabilidad: Recibe requests HTTP y delega el caso de uso al servicio de Application.
[ApiController]
[Authorize(Roles = Roles.Todos)]
[Route("api/clientes")]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;

    public ClientesController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    [HttpPost]
    public async Task<ActionResult<ClienteResponse>> Crear(
        CreateClienteRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            //en cliente se va a guardar el DTO creado en crearAsync y es lo que va a devolver el endPoint
            var cliente = await _clienteService.CrearAsync(request, cancellationToken);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = cliente.Id }, cliente);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    //IReadOnlyList es una interfaz que ya viene con .NET, para indicar que solo se puede leer lo que devuelva el endPoint
    public async Task<ActionResult<IReadOnlyList<ClienteResponse>>> ObtenerTodos(
        CancellationToken cancellationToken)
    {
        var clientes = await _clienteService.ObtenerTodosAsync(cancellationToken);
        return Ok(clientes);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClienteResponse>> ObtenerPorId(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            var cliente = await _clienteService.ObtenerPorIdAsync(id, cancellationToken);
            return Ok(cliente);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ClienteResponse>> Actualizar(
        int id,
        UpdateClienteRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var cliente = await _clienteService.ActualizarAsync(id, request, cancellationToken);
            return Ok(cliente);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
