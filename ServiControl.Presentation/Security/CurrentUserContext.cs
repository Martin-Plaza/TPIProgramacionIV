using System.Security.Claims;
using ServiControl.Application.Authorization;

namespace ServiControl.Presentation.Security;

// Módulo: Seguridad
// Capa: Presentation
//
// Responsabilidad:
// Adaptar el usuario autenticado de ASP.NET (HttpContext.User)
// al contrato ICurrentUserContext definido en Application.
//
// Esta clase responde únicamente dos preguntas:
// 1. ¿Cuál es el Id del usuario autenticado?
// 2. ¿El usuario autenticado es administrador?
public class CurrentUserContext : ICurrentUserContext
{
    // Servicio de ASP.NET que permite acceder al HttpContext de la petición actual.
    // Gracias a él podemos obtener HttpContext.User desde cualquier clase.
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    // Propiedad calculada.
    // Cada vez que alguien solicita UsuarioId, se lee el claim "IdUsuario"
    // almacenado en HttpContext.User y se convierte a entero.
    public int UsuarioId
    {
        get
        {
            var value = User.FindFirstValue(AuthClaimTypes.UsuarioId);

            //doble validacion: si se pudo convertir a int y si es menor a 0
            if (!int.TryParse(value, out var usuarioId) || usuarioId <= 0)
            {
                throw new UnauthorizedAccessException("El token no contiene un UsuarioId valido.");
            }

            return usuarioId;
        }
    }
    // Indica si el usuario autenticado pertenece al rol Admin.
    public bool IsAdmin => User.IsInRole(Roles.Admin);

    // Devuelve el ClaimsPrincipal de la petición actual.
    // Este objeto NO se crea aquí.
    // Lo construye automáticamente el middleware JWT (UseAuthentication)
    // a partir del token enviado en el header Authorization.
    //esto solo lo lleva a donde lo piden, y si no tiene nada arroja la excepcion
    private ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User
        ?? throw new UnauthorizedAccessException("No existe un usuario autenticado.");
}
