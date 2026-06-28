using Microsoft.EntityFrameworkCore;
using ServiControl.Application.Interfaces;
using ServiControl.Domain.Entities;
using ServiControl.Domain.Enums;
using ServiControl.Infrastructure.Persistence.Context;

namespace ServiControl.Infrastructure.Persistence.Repositories;

public class TrabajoRepository : GenericRepository<Trabajo>, ITrabajoRepository
{
    public TrabajoRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyList<Trabajo>> GetPendientesAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            //AsNoTracking es decirle a EF que no solo vas a leer los datos, no modificarlos
            .AsNoTracking()
            .Where(trabajo => trabajo.Estado == EstadoTrabajo.Pendiente)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Trabajo>> GetByUsuarioAsync(
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(trabajo => trabajo.UsuarioId == usuarioId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Trabajo>> GetPendientesByUsuarioAsync(
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(trabajo =>
                trabajo.UsuarioId == usuarioId &&
                trabajo.Estado == EstadoTrabajo.Pendiente)
            .ToListAsync(cancellationToken);
    }

    public async Task<Trabajo?> GetByIdAndUsuarioAsync(
        int id,
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(
            trabajo => trabajo.Id == id && trabajo.UsuarioId == usuarioId,
            cancellationToken);
    }

    public async Task<IReadOnlyList<Trabajo>> GetByUsuarioAndFechaRangeAsync(
        int usuarioId,
        DateOnly periodoInicio,
        DateOnly periodoFin,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(trabajo =>
                trabajo.UsuarioId == usuarioId &&
                trabajo.Fecha >= periodoInicio &&
                trabajo.Fecha <= periodoFin)
            .ToListAsync(cancellationToken);
    }
}
