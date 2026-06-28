using ServiControl.Application.Authorization;
using ServiControl.Application.DTOs;
using ServiControl.Application.Interfaces;
using ServiControl.Domain.Entities;
using ServiControl.Domain.Enums;

namespace ServiControl.Application.Services;

// Modulo: Metricas
// Capa: Application
// Responsabilidad: Calcula metricas de usuario a partir de trabajos finalizados y costos finales.
public class MetricaService : IMetricaService
{
    private readonly IMetricaRepository _metricaRepository;
    private readonly ITrabajoRepository _trabajoRepository;
    private readonly ICostoRepository _costoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IOperationalUserResolver _operationalUserResolver;

    public MetricaService(
        IMetricaRepository metricaRepository,
        ITrabajoRepository trabajoRepository,
        ICostoRepository costoRepository,
        IUsuarioRepository usuarioRepository,
        ICurrentUserContext currentUserContext,
        IOperationalUserResolver operationalUserResolver)
    {
        _metricaRepository = metricaRepository;
        _trabajoRepository = trabajoRepository;
        _costoRepository = costoRepository;
        _usuarioRepository = usuarioRepository;
        _currentUserContext = currentUserContext;
        _operationalUserResolver = operationalUserResolver;
    }


    public async Task<MetricaResponse> GenerarPropiasAsync(
        GenerateMetricaRequest request,
        CancellationToken cancellationToken = default)
    {
        var usuarioOperativoId = await _operationalUserResolver
            .ObtenerUsuarioOperativoIdAsync(cancellationToken);

        //aca llamo a la funcion de abajo generarAsync
        return await GenerarAsync(usuarioOperativoId, request, cancellationToken);
    }

    //es para generar metricas siendo admin
    public Task<MetricaResponse> GenerarParaUsuarioAsync(
        int usuarioId,
        GenerateMetricaRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!_currentUserContext.IsAdmin)
        {
            throw new UnauthorizedAccessException(
                "Solo un administrador puede generar metricas para otro usuario.");
        }

        return GenerarAsync(usuarioId, request, cancellationToken);
    }

    //funcion para generar metricas
    private async Task<MetricaResponse> GenerarAsync(
        int usuarioId,
        GenerateMetricaRequest request,
        CancellationToken cancellationToken)
    {
        if (request.PeriodoInicio > request.PeriodoFin)
        {
            throw new ArgumentException(
                "La fecha de inicio del periodo debe ser menor o igual a la fecha de fin.");
        }

        if (!await _usuarioRepository.ExistsByIdAsync(usuarioId, cancellationToken))
        {
            throw new ArgumentException("El usuario indicado no existe.", nameof(usuarioId));
        }

        var trabajos = await _trabajoRepository.GetByUsuarioAndFechaRangeAsync(
            usuarioId,
            request.PeriodoInicio,
            request.PeriodoFin,
            cancellationToken);

        var trabajosFinalizados = trabajos
            .Where(trabajo => trabajo.Estado == EstadoTrabajo.Finalizado)
            .ToList();

        var costos = await _costoRepository.GetByTrabajoIdsAsync(
            trabajosFinalizados.Select(trabajo => trabajo.Id),
            cancellationToken);

        var montoTotalPeriodo = costos
            .Where(costo => costo.CostoFinal.HasValue)
            .Sum(costo => costo.CostoFinal!.Value);

        var trabajosPendientes = trabajos.Count(trabajo => trabajo.Estado == EstadoTrabajo.Pendiente);

        var metrica = new Metrica(
            usuarioId,
            montoTotalPeriodo,
            trabajosFinalizados.Count,
            trabajosPendientes,
            request.PeriodoInicio.ToDateTime(TimeOnly.MinValue),
            request.PeriodoFin.ToDateTime(TimeOnly.MaxValue));

        var created = await _metricaRepository.AddAsync(metrica, cancellationToken);

        return MapToResponse(created);
    }

    private static MetricaResponse MapToResponse(Metrica metrica)
    {
        return new MetricaResponse(
            metrica.Id,
            metrica.UsuarioId,
            metrica.MontoTotalPeriodo,
            metrica.CantidadTrabajos,
            metrica.TrabajosPendientes,
            metrica.MontoPromedioTrabajo,
            metrica.PeriodoInicio,
            metrica.PeriodoFin);
    }
}
