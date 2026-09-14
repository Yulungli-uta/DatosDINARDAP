using DatosDINARDAP.Client.Models;

namespace DatosDINARDAP.Client;

public interface IDinardapClient
{
    Task<RegistroCivilDto> ObtenerRegistroCivilAsync(string identificacion, CancellationToken ct = default);
    Task<TceDto> ObtenerTceAsync(string identificacion, CancellationToken ct = default);
    Task<IReadOnlyList<TituloDto>> ObtenerTitulosAsync(string identificacion, CancellationToken ct = default);
}
