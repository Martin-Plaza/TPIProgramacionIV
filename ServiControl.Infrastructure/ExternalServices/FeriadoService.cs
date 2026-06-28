using System.Net.Http.Json;
using ServiControl.Application.DTOs;
using ServiControl.Application.Interfaces;

namespace ServiControl.Infrastructure.ExternalServices;

// Modulo: Integracion externa
// Capa: Infrastructure
// Responsabilidad: Consume una API publica de feriados mediante HttpClientFactory.
public class FeriadoService : IFeriadoService
{
    private readonly HttpClient _httpClient;

    public FeriadoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<FeriadoDto>> GetFeriadosPorAnioAsync(
        int anio,
        CancellationToken cancellationToken = default)
    {
        if (anio is < 2000 or > 2100)
        {
            throw new ArgumentException("El anio debe estar entre 2000 y 2100.", nameof(anio));
        }

        try
        {
            //esta es la inyeccion de httpClientFactory, en program.cs
            var feriados = await _httpClient.GetFromJsonAsync<List<FeriadoApiResponse>>(
                $"feriados/{anio}",
                cancellationToken);
            //si devolvio algo lo mapea en el DTO
            return feriados?
                //FeriadoDTO es lo que vamos a devolver en el endpoint
                .Select(feriado => new FeriadoDto(
                    feriado.Fecha,
                    feriado.Tipo,
                    feriado.Nombre))
                .ToList() ?? [];
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException("No se pudieron obtener los feriados desde el servicio externo.", ex);
        }
    }
    //feriadoApiResponse se usa en la var feriados
    private sealed record FeriadoApiResponse(
        string Fecha,
        string Tipo,
        string Nombre);
}
