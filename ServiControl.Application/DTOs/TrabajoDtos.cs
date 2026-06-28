using ServiControl.Domain.Enums;

namespace ServiControl.Application.DTOs;

public record CreateTrabajoRequest(
    int ClienteId,
    CategoriaServicio CategoriaServicio,
    string Descripcion,
    DateOnly Fecha,
    string Direccion,
    string? Observaciones);

public record UpdateTrabajoStatusRequest(
    EstadoTrabajo Estado);

public record TrabajoResponse(
    int Id,
    int ClienteId,
    int UsuarioId,
    CategoriaServicio CategoriaServicio,
    string Descripcion,
    DateOnly Fecha,
    string Direccion,
    string? Observaciones,
    EstadoTrabajo Estado);
