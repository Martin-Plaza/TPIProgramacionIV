using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiControl.Application.Authorization;
using ServiControl.Application.DTOs;
using ServiControl.Application.Interfaces;
using ServiControl.Domain.Enums;

namespace ServiControl.Presentation.Controllers;

// Modulo: Trabajos
// Capa: Presentation
// Responsabilidad: Mantiene endpoints REST sin reglas de negocio en el controller.
[ApiController]
[Authorize(Roles = Roles.Todos)]
[Route("api/trabajos")]
public class TrabajosController : ControllerBase
{
    private readonly ITrabajoService _trabajoService;

    public TrabajosController(ITrabajoService trabajoService)
    {
        _trabajoService = trabajoService;
    }

    [HttpPost]
    public async Task<ActionResult<TrabajoResponse>> Crear(
        CreateTrabajoRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var trabajo = await _trabajoService.CrearAsync(request, cancellationToken);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = trabajo.Id }, trabajo);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TrabajoResponse>>> ObtenerTodos(
        CancellationToken cancellationToken)
    {
        var trabajos = await _trabajoService.ObtenerTodosAsync(cancellationToken);
        return Ok(trabajos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TrabajoResponse>> ObtenerPorId(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            var trabajo = await _trabajoService.ObtenerPorIdAsync(id, cancellationToken);
            return Ok(trabajo);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("pendientes")]
    public async Task<ActionResult<IReadOnlyList<TrabajoResponse>>> ObtenerPendientes(
        CancellationToken cancellationToken)
    {
        var trabajos = await _trabajoService.ObtenerPendientesAsync(cancellationToken);
        return Ok(trabajos);
    }

    // Cambiar estados queda reservado a quienes gestionan el ciclo completo del trabajo.
    [Authorize(Roles = Roles.AdminTecnico)]
    [HttpPut("{id:int}/estado")]
    public async Task<ActionResult<TrabajoResponse>> CambiarEstado(
        int id,
        UpdateTrabajoStatusRequest request,
        CancellationToken cancellationToken)
    {
        return await CambiarEstadoInterno(id, request, cancellationToken);
    }

    [Authorize(Roles = Roles.AdminTecnico)]
    [HttpPut("{id:int}/finalizar")]
    public async Task<ActionResult<TrabajoResponse>> Finalizar(
        int id,
        CancellationToken cancellationToken)
    {
        return await CambiarEstadoInterno(
            id,
            new UpdateTrabajoStatusRequest(EstadoTrabajo.Finalizado),
            cancellationToken);
    }

    [Authorize(Roles = Roles.AdminTecnico)]
    [HttpPut("{id:int}/cancelar")]
    public async Task<ActionResult<TrabajoResponse>> Cancelar(
        int id,
        CancellationToken cancellationToken)
    {
        return await CambiarEstadoInterno(
            id,
            new UpdateTrabajoStatusRequest(EstadoTrabajo.Cancelado),
            cancellationToken);
    }

    private async Task<ActionResult<TrabajoResponse>> CambiarEstadoInterno(
        int id,
        UpdateTrabajoStatusRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var trabajo = await _trabajoService.CambiarEstadoAsync(id, request, cancellationToken);
            return Ok(trabajo);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }
}
