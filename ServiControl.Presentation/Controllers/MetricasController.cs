using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiControl.Application.Authorization;
using ServiControl.Application.DTOs;
using ServiControl.Application.Interfaces;

namespace ServiControl.Presentation.Controllers;

// Modulo: Metricas
// Capa: Presentation
// Responsabilidad: Expone generacion de metricas por rango de fechas.
[ApiController]
[Authorize(Roles = Roles.Todos)]
[Route("api/metricas")]
public class MetricasController : ControllerBase
{
    private readonly IMetricaService _metricaService;

    public MetricasController(IMetricaService metricaService)
    {
        _metricaService = metricaService;
    }

    [HttpGet("mis-metricas")]
    public async Task<ActionResult<MetricaResponse>> GenerarPropias(
        [FromQuery] DateOnly periodoInicio,
        [FromQuery] DateOnly periodoFin,
        CancellationToken cancellationToken)
    {
        try
        {
            var metrica = await _metricaService.GenerarPropiasAsync(
                new GenerateMetricaRequest(periodoInicio, periodoFin),
                cancellationToken);

            return Ok(metrica);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    //generar metricas para algun usuario desde admin
    [Authorize(Roles = Roles.Admin)]
    [HttpGet("usuario/{usuarioId:int}")]
    public async Task<ActionResult<MetricaResponse>> GenerarParaUsuario(
        int usuarioId,
        [FromQuery] DateOnly periodoInicio,
        [FromQuery] DateOnly periodoFin,
        CancellationToken cancellationToken)
    {
        try
        {
            var metrica = await _metricaService.GenerarParaUsuarioAsync(
                usuarioId,
                new GenerateMetricaRequest(periodoInicio, periodoFin),
                cancellationToken);

            return Ok(metrica);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
