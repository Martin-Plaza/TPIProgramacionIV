using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiControl.Application.Authorization;
using ServiControl.Application.DTOs;
using ServiControl.Application.Interfaces;

namespace ServiControl.Presentation.Controllers;

// Modulo: Costos
// Capa: Presentation
// Responsabilidad: Expone operaciones de costos y delega validaciones a Application/Domain.
[ApiController]
[Authorize(Roles = Roles.Todos)]
[Route("api/costos")]
public class CostosController : ControllerBase
{
    private readonly ICostoService _costoService;

    public CostosController(ICostoService costoService)
    {
        _costoService = costoService;
    }

    [HttpPost]
    public async Task<ActionResult<CostoResponse>> RegistrarCostoEstimado(
        RegisterCostoEstimadoRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var costo = await _costoService.RegistrarCostoEstimadoAsync(request, cancellationToken);
            return Ok(costo);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}/estimado")]
    public async Task<ActionResult<CostoResponse>> RegistrarCostoEstimado(
        int id,
        [FromBody] decimal costoEstimado,
        CancellationToken cancellationToken)
    {
        try
        {
            var costo = await _costoService.RegistrarCostoEstimadoAsync(
                new RegisterCostoEstimadoRequest(id, costoEstimado),
                cancellationToken);

            return Ok(costo);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // El costo final representa informacion economica cerrada: Asistente no tiene acceso.
    [Authorize(Roles = Roles.AdminTecnico)]
    [HttpPut("{id:int}/final")]
    public async Task<ActionResult<CostoResponse>> RegistrarCostoFinal(
        int id,
        //ingresamos solo por body el costo final
        [FromBody] decimal costoFinal,
        CancellationToken cancellationToken)
    {
        try
        {
            //creara el DTO y se lo pasa al servicio de costo
            var costo = await _costoService.RegistrarCostoFinalAsync(
                new RegisterCostoFinalRequest(id, costoFinal),
                cancellationToken);

            return Ok(costo);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
