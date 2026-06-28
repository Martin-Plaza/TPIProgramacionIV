using ServiControl.Domain.Entities;

namespace ServiControl.Application.Interfaces;

public interface ITrabajoRepository : IGenericRepository<Trabajo>
{
    Task<IReadOnlyList<Trabajo>> GetPendientesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Trabajo>> GetByUsuarioAsync(
        int usuarioId,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Trabajo>> GetPendientesByUsuarioAsync(
        int usuarioId,
        CancellationToken cancellationToken = default);
    Task<Trabajo?> GetByIdAndUsuarioAsync(
        int id,
        int usuarioId,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Trabajo>> GetByUsuarioAndFechaRangeAsync(
        int usuarioId,
        DateOnly periodoInicio,
        DateOnly periodoFin,
        CancellationToken cancellationToken = default);
}
