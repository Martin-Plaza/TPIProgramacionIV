namespace ServiControl.Application.Interfaces;

public interface IOperationalUserResolver
{
    Task<int> ObtenerUsuarioOperativoIdAsync(CancellationToken cancellationToken = default);
}
