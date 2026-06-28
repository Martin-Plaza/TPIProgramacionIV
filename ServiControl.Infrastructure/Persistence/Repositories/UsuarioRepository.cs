using Microsoft.EntityFrameworkCore;
using ServiControl.Application.Interfaces;
using ServiControl.Domain.Entities;
using ServiControl.Infrastructure.Persistence.Context;

namespace ServiControl.Infrastructure.Persistence.Repositories;

public class UsuarioRepository : GenericRepository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    //trae un usuario por el mail 
    public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(usuario => usuario.Email == email, cancellationToken);
    }

    //compara si existe ese usuario por id
    public async Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AnyAsync(usuario => usuario.Id == id, cancellationToken);
    }

    //compara si existe ese usuario por mail
    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AnyAsync(usuario => usuario.Email == email, cancellationToken);
    }
    
    //funcion para verificar si el id del usuario tiene relacionado con trabajos y metricas (no impide que se borre)
    public async Task<bool> HasRelatedRecordsAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        //pregunta si al menos tiene 1 trabajo, si es true retorna eso
        var tieneTrabajos = await Context.Set<Trabajo>()
            .AnyAsync(trabajo => trabajo.UsuarioId == id, cancellationToken);

        if (tieneTrabajos)
        {
            return true;
        }

        //hace lo mismo que con trabajos
        var tieneMetricas = await Context.Set<Metrica>()
            .AnyAsync(metrica => metrica.UsuarioId == id, cancellationToken);

        if (tieneMetricas)
        {
            return true;
        }

        return await HasAssistantsAsync(id, cancellationToken);
    }

    //trae asistentes de un técnico
    public async Task<bool> HasAssistantsAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AnyAsync(usuario => usuario.IdUsuarioResponsable == id, cancellationToken);
    }
}
