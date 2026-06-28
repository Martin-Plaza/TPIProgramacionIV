using System.ComponentModel.DataAnnotations;
using ServiControl.Domain.Enums;

namespace ServiControl.Application.DTOs;

public record RegisterUserRequestDto(
    [param: Required] string Nombre,
    [param: Required, EmailAddress] string Email,
    [param: Required] string Password,
    RolUsuario Rol,
    int? IdUsuarioResponsable = null);

public record CreateAdminRequestDto(
    [param: Required] string Nombre,
    [param: Required, EmailAddress] string Email,
    [param: Required] string Password);

public record LoginRequestDto(
    [param: Required, EmailAddress] string Email,
    [param: Required] string Password);

public record UserResponseDto(
    int Id,
    string Nombre,
    string Email,
    RolUsuario Rol,
    int? IdUsuarioResponsable);

public record LoginResponseDto(
    string Token,
    int IdUsuario,
    string Nombre,
    string Email,
    RolUsuario Rol);
