using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiControl.Application.Authorization;
using ServiControl.Application.DTOs;
using ServiControl.Application.Interfaces;

namespace ServiControl.Presentation.Controllers;

// Modulo: Integracion externa
// Capa: Presentation
// Responsabilidad: Expone el consumo protegido de feriados de Argentina
[ApiController]
[Authorize(Roles = Roles.Todos)]
[Route("api/feriados")]
public class FeriadosController : ControllerBase
{
    private readonly IFeriadoService _feriadoService;

    public FeriadosController(IFeriadoService feriadoService)
    {
        _feriadoService = feriadoService;
    }

    [HttpGet("{anio:int}")]
    public async Task<ActionResult<IReadOnlyList<FeriadoDto>>> GetFeriadosPorAnio(
        int anio,
        CancellationToken cancellationToken)
    {
        try
        {
            var feriados = await _feriadoService.GetFeriadosPorAnioAsync(anio, cancellationToken);
            return Ok(feriados);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
        }
    }
}
