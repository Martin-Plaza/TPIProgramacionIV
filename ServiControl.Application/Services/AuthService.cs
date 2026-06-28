using ServiControl.Application.DTOs;
using ServiControl.Application.Interfaces;
using ServiControl.Domain.Entities;
using ServiControl.Domain.Enums;

namespace ServiControl.Application.Services;

// Modulo: Autenticacion
// Capa: Application
// Responsabilidad: Orquesta registro y login sin acoplarse a infraestructura concreta.
// Nota: La generacion de JWT y el hashing se consumen mediante abstracciones.
public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    //solo admin tiene acceso a register
    public async Task<UserResponseDto> RegisterAsync(
        RegisterUserRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ArgumentException("La password es obligatoria.", nameof(request.Password));
        }

        var email = request.Email.Trim();

        if (await _usuarioRepository.ExistsByEmailAsync(email, cancellationToken))
        {
            throw new InvalidOperationException("El email ya esta registrado.");
        }

        //hashea password
        var passwordHash = _passwordHasher.Hash(request.Password);

        await ValidarResponsableAsync(request.Rol, request.IdUsuarioResponsable, cancellationToken);

        //crea instancia de usuario (capa dominio)
        var usuario = new Usuario(
            request.Nombre,
            email,
            passwordHash,
            request.Rol,
            request.IdUsuarioResponsable);
        //ingresa el usuario en el repositorio
        var created = await _usuarioRepository.AddAsync(usuario, cancellationToken);

        //le pasamos el DTO para mostrarlo
        return MapToResponse(created);
    }


    //esta funcion es para crear un admin desde la terminal.
    //dotnet run --project .\ServiControl.Presentation\ServiControl.Presentation.csproj -- create-admin
    public Task<UserResponseDto> CreateAdminAsync(
        CreateAdminRequestDto request,
        CancellationToken cancellationToken = default)
    {
        return RegisterAsync(
            new RegisterUserRequestDto(
                request.Nombre,
                request.Email,
                request.Password,
                RolUsuario.Admin,
                IdUsuarioResponsable: null),
            cancellationToken);
    }

    
    
    public async Task<LoginResponseDto> LoginAsync(
        LoginRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            throw new InvalidOperationException("Email o password invalidos.");
        }

        var usuario = await _usuarioRepository.GetByEmailAsync(request.Email.Trim(), cancellationToken);

        if (usuario is null || !_passwordHasher.Verify(request.Password, usuario.PasswordHash))
        {
            throw new InvalidOperationException("Email o password invalidos.");
        }

        var token = _jwtTokenGenerator.GenerateToken(usuario);

        return new LoginResponseDto(
            token,
            usuario.Id,
            usuario.Nombre,
            usuario.Email,
            usuario.Rol);
    }

    // funcion para tomar a la entidad usuario y convertirlo en DTO
    private static UserResponseDto MapToResponse(Usuario usuario)
    {
        return new UserResponseDto(
            usuario.Id,
            usuario.Nombre,
            usuario.Email,
            usuario.Rol,
            usuario.IdUsuarioResponsable);
    }

    //aca validamos el responsable para los asistentes
    private async Task ValidarResponsableAsync(
        RolUsuario rol,
        int? idUsuarioResponsable,
        CancellationToken cancellationToken)
    {
        //si no es asistente retornamos nada
        if (rol != RolUsuario.Assistant)
        {
            return;
        }

        //si pasa el filtro anterior (es decir, es asistente), pero no tiene responsable arroja excep
        if (!idUsuarioResponsable.HasValue || idUsuarioResponsable.Value <= 0)
        {
            throw new ArgumentException(
                "Un asistente debe tener un tecnico responsable.",
                nameof(idUsuarioResponsable));
        }

        //preguntamos al repo si existe, si existe lo guarda
        var responsable = await _usuarioRepository.GetByIdAsync(
            idUsuarioResponsable.Value,
            cancellationToken)
            ?? throw new ArgumentException(
                "El tecnico responsable indicado no existe.",
                nameof(idUsuarioResponsable));

        //si ese responsable no es tecnico arroja excep
        if (responsable.Rol != RolUsuario.Technician)
        {
            throw new ArgumentException(
                "El usuario responsable debe tener rol Tecnico.",
                nameof(idUsuarioResponsable));
        }
    }
}
