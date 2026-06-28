using Microsoft.EntityFrameworkCore;
using ServiControl.Application.Interfaces;
using ServiControl.Domain.Entities;
using ServiControl.Infrastructure.Persistence.Context;

namespace ServiControl.Infrastructure.Persistence.Repositories;

public class CostoRepository : GenericRepository<Costo>, ICostoRepository
{
    public CostoRepository(ApplicationDbContext context)
        : base(context)
    {
    }
    //buscamos por ID el trabajo, pero puede que devuelva nulo el Costo
    public async Task<Costo?> GetByTrabajoIdAsync(int trabajoId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(costo => costo.TrabajoId == trabajoId, cancellationToken);
    }

    
    public async Task<IReadOnlyList<Costo>> GetByTrabajoIdsAsync(
        IEnumerable<int> trabajoIds,
        CancellationToken cancellationToken = default)
    {
        var ids = trabajoIds.ToList();

        //si ids no tiene nada hacemos un if para verificarlo, para no hacer consulta a la DB
        if (ids.Count == 0)
        {
            //retorna una lista vacia si es que no hay nada
            return [];
        }

        return await DbSet
            //asNoTracking quiere decir que EF solo leera los datos
            .AsNoTracking()
            .Where(costo => ids.Contains(costo.TrabajoId))
            .ToListAsync(cancellationToken);
    }
}
