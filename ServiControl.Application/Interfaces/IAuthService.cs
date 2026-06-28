using ServiControl.Application.DTOs;

namespace ServiControl.Application.Interfaces;

public interface IAuthService
{
    Task<UserResponseDto> RegisterAsync(
        RegisterUserRequestDto request,
        CancellationToken cancellationToken = default);

    Task<UserResponseDto> CreateAdminAsync(
        CreateAdminRequestDto request,
        CancellationToken cancellationToken = default);

    Task<LoginResponseDto> LoginAsync(
        LoginRequestDto request,
        CancellationToken cancellationToken = default);
}
