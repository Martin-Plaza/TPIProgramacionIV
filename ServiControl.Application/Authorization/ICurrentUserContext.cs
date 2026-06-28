namespace ServiControl.Application.Authorization;

// Expone identidad y privilegio global sin acoplar Application a HttpContext.
//firma para modificar httpcontext.User
public interface ICurrentUserContext
{
    int UsuarioId { get; }
    bool IsAdmin { get; }
}
