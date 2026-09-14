using WsUtaDinardap.Api.Application;
using WsUtaDinardap.Api.Application.Errors;
using WsUtaDinardap.Api.Application.Handlers;
using WsUtaDinardap.Api.Application.Interfaces;
using WsUtaDinardap.Api.Domain;
using Xunit;

namespace WsUtaDinardap.Api.Tests;

public class DinardapServiceTests
{
    private sealed class FakeGateway : IDinardapGateway
    {
        public DinardapPackage? UltimoPaqueteConsultado { get; private set; }
        public Func<FichaGeneralDto> Respuesta { get; set; } = () => new FichaGeneralDto(null, null, Array.Empty<InstitucionDto>());
        public DinardapException? ExcepcionAThrow { get; set; }

        public Task<FichaGeneralDto> ConsultarAsync(string numeroIdentificacion, DinardapPackage paquete, CancellationToken ct)
        {
            UltimoPaqueteConsultado = paquete;
            if (ExcepcionAThrow is not null) throw ExcepcionAThrow;
            return Task.FromResult(Respuesta());
        }
    }

    private static DinardapService CrearServicio(FakeGateway gateway) => new(
        gateway,
        new RegistroCivilPackageHandler(),
        new TcePackageHandler(),
        new TitulosPackageHandler());

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("12345678901234")]
    public async Task ConsultarRegistroCivilAsync_IdentificacionInvalida_LanzaInvalidIdentification(string identificacion)
    {
        var servicio = CrearServicio(new FakeGateway());

        var ex = await Assert.ThrowsAsync<DinardapException>(
            () => servicio.ConsultarRegistroCivilAsync(identificacion, default));

        Assert.Equal(DinardapErrorCode.InvalidIdentification, ex.Code);
    }

    [Fact]
    public async Task ConsultarRegistroCivilAsync_ConsultaElPaqueteCorrecto()
    {
        var gateway = new FakeGateway();
        var servicio = CrearServicio(gateway);

        await servicio.ConsultarRegistroCivilAsync("1234567890", default);

        Assert.Equal(DinardapPackage.RegistroCivil, gateway.UltimoPaqueteConsultado);
    }

    [Fact]
    public async Task ConsultarTceAsync_ConsultaElPaqueteCorrecto()
    {
        var gateway = new FakeGateway();
        var servicio = CrearServicio(gateway);

        await servicio.ConsultarTceAsync("1234567890", default);

        Assert.Equal(DinardapPackage.Tce, gateway.UltimoPaqueteConsultado);
    }

    [Fact]
    public async Task ConsultarTitulosAsync_ConsultaElPaqueteCorrecto()
    {
        var gateway = new FakeGateway();
        var servicio = CrearServicio(gateway);

        await servicio.ConsultarTitulosAsync("1234567890", default);

        Assert.Equal(DinardapPackage.Titulos, gateway.UltimoPaqueteConsultado);
    }

    [Fact]
    public async Task ConsultarRegistroCivilAsync_FallaDelGateway_PropagaDinardapException()
    {
        var gateway = new FakeGateway
        {
            ExcepcionAThrow = new DinardapException(DinardapErrorCode.DinardapTimeout, "timeout simulado")
        };
        var servicio = CrearServicio(gateway);

        var ex = await Assert.ThrowsAsync<DinardapException>(
            () => servicio.ConsultarRegistroCivilAsync("1234567890", default));

        Assert.Equal(DinardapErrorCode.DinardapTimeout, ex.Code);
    }
}
