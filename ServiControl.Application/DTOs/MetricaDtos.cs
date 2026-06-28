namespace ServiControl.Application.DTOs;

public record GenerateMetricaRequest(
    DateOnly PeriodoInicio,
    DateOnly PeriodoFin);

public record MetricaResponse(
    int Id,
    int UsuarioId,
    decimal MontoTotalPeriodo,
    int CantidadTrabajos,
    int TrabajosPendientes,
    decimal MontoPromedioTrabajo,
    DateTime PeriodoInicio,
    DateTime PeriodoFin);
