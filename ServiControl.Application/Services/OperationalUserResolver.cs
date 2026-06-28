using ServiControl.Application.Authorization;
using ServiControl.Application.Interfaces;
using ServiControl.Domain.Enums;

namespace ServiControl.Application.Services;

// Modulo: Autorizacion operativa
// Capa: Application
// Responsabilidad: Decide que UsuarioId se usa para trabajos, costos y metricas.
public class OperationalUserResolver : IOperationalUserResolver
{
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IUsuarioRepository _usuarioRepository;

    public OperationalUserResolver(
        ICurrentUserContext currentUserContext,
        IUsuarioRepository usuarioRepository)
    {
        _currentUserContext = currentUserContext;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<int> ObtenerUsuarioOperativoIdAsync(
        CancellationToken cancellationToken = default)
    {
        //validamos con usercontext cual es el usuario logeado y autenticado
        var usuario = await _usuarioRepository.GetByIdAsync(
            _currentUserContext.UsuarioId,
            cancellationToken)
            ?? throw new UnauthorizedAccessException("El usuario autenticado no existe.");

        //si el usaurio es asistente
        if (usuario.Rol == RolUsuario.Assistant)
        {
            //validamos que tenga usuario responsable asignado, sino sale del bloque con excepcion
            if (!usuario.IdUsuarioResponsable.HasValue)
            {
                throw new InvalidOperationException(
                    "El asistente no tiene un tecnico responsable asignado.");
            }

            //guardamos el responsable tecnico en una variable, si no existe arrojamos excepcion
            var responsable = await _usuarioRepository.GetByIdAsync(
                usuario.IdUsuarioResponsable.Value,
                cancellationToken)
                ?? throw new InvalidOperationException(
                    "El tecnico responsable asignado no existe.");

            //si no tiene rol tecnico arroja excepcion
            if (responsable.Rol != RolUsuario.Technician)
            {
                throw new InvalidOperationException(
                    "El usuario responsable asignado no tiene rol Tecnico.");
            }
            //devuelve el id del responsable en la variable usuario declarada arriba
            return responsable.Id;
        }
        //devuelve el id del responsable desde donde lo llamaron
        return usuario.Id;
    }
}
