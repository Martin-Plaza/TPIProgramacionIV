using ServiControl.Application.DTOs;

namespace ServiControl.Application.Interfaces;

public interface IMetricaService
{
    Task<MetricaResponse> GenerarPropiasAsync(
        GenerateMetricaRequest request,
        CancellationToken cancellationToken = default);

    Task<MetricaResponse> GenerarParaUsuarioAsync(
        int usuarioId,
        GenerateMetricaRequest request,
        CancellationToken cancellationToken = default);
}
