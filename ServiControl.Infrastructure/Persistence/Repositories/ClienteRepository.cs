using Microsoft.EntityFrameworkCore;
using ServiControl.Application.Interfaces;
using ServiControl.Domain.Entities;
using ServiControl.Infrastructure.Persistence.Context;

namespace ServiControl.Infrastructure.Persistence.Repositories;

public class ClienteRepository : GenericRepository<Cliente>, IClienteRepository
{
    public ClienteRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    //devuelve verdadero si encuentra un id igual al de la request
    public async Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AnyAsync(cliente => cliente.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Cliente>> GetByUsuarioAsync(
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(cliente => cliente.UsuarioId == usuarioId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Cliente?> GetByIdAndUsuarioAsync(
        int id,
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(
            cliente => cliente.Id == id && cliente.UsuarioId == usuarioId,
            cancellationToken);
    }
}
