using ServiControl.Domain.Entities;

namespace ServiControl.Application.Interfaces;

public interface IClienteRepository : IGenericRepository<Cliente>
{
    Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Cliente>> GetByUsuarioAsync(
        int usuarioId,
        CancellationToken cancellationToken = default);
    Task<Cliente?> GetByIdAndUsuarioAsync(
        int id,
        int usuarioId,
        CancellationToken cancellationToken = default);
}
