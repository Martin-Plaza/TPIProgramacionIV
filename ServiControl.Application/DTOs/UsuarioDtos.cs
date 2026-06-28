using System.ComponentModel.DataAnnotations;
using ServiControl.Domain.Enums;

namespace ServiControl.Application.DTOs;

public record UpdateUsuarioRequestDto(
    [param: Required] string Nombre,
    [param: Required, EmailAddress] string Email);

public record UpdateUsuarioRolRequestDto(
    RolUsuario Rol,
    int? IdUsuarioResponsable = null);
