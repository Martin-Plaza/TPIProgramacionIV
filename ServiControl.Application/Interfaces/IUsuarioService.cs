using ServiControl.Application.DTOs;

namespace ServiControl.Application.Interfaces;

public interface IUsuarioService
{
    Task<IReadOnlyList<UserResponseDto>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    Task<UserResponseDto> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<UserResponseDto> ActualizarAsync(
        int id,
        UpdateUsuarioRequestDto request,
        CancellationToken cancellationToken = default);
    Task<UserResponseDto> CambiarRolAsync(
        int id,
        UpdateUsuarioRolRequestDto request,
        CancellationToken cancellationToken = default);
    Task EliminarAsync(int id, CancellationToken cancellationToken = default);
}
