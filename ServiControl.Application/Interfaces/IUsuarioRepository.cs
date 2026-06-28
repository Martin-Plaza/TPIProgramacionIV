using ServiControl.Domain.Entities;

namespace ServiControl.Application.Interfaces;

public interface IUsuarioRepository : IGenericRepository<Usuario>
{
    Task<Usuario?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> HasRelatedRecordsAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> HasAssistantsAsync(int id, CancellationToken cancellationToken = default);
}
