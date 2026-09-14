using WsUtaDinardap.Api.Application.Handlers;
using WsUtaDinardap.Api.Application.Interfaces;
using Xunit;

namespace WsUtaDinardap.Api.Tests.PackageHandlers;

public class TcePackageHandlerTests
{
    private static readonly TcePackageHandler Handler = new();

    private static FichaGeneralDto FichaCon(params (string Codigo, string Valor)[] campos) => new(
        "114", null,
        new[]
        {
            new InstitucionDto(null, null,
                campos.Select(c => new RegistroDto(null, c.Codigo, c.Valor)).ToList(),
                Array.Empty<ItemDto>())
        });

    [Fact]
    public void Parse_Sufrago_MapeaTrue()
    {
        var ficha = FichaCon(("44", "CERT-001"), ("45", "2023/02/05"), ("46", "SI"));

        var resultado = Handler.Parse(ficha);

        Assert.Equal("CERT-001", resultado.NumeroCertificado);
        Assert.Equal(new DateTime(2023, 2, 5), resultado.FechaSufragio);
        Assert.True(resultado.Sufrago);
    }

    [Fact]
    public void Parse_NoSufrago_MapeaFalse()
    {
        var ficha = FichaCon(("46", "NO"));
        var resultado = Handler.Parse(ficha);
        Assert.False(resultado.Sufrago);
    }

    [Fact]
    public void Parse_SinDatos_DevuelveNullHonesto_NoFalseImplicito()
    {
        // A diferencia del TCEDAL legacy (que por default(bool)=false confunde "sin dato" con
        // "no sufrago"), la API nueva distingue: campo ausente = null, no false.
        var ficha = new FichaGeneralDto("114", null, Array.Empty<InstitucionDto>());

        var resultado = Handler.Parse(ficha);

        Assert.Null(resultado.Sufrago);
        Assert.Null(resultado.NumeroCertificado);
        Assert.Null(resultado.FechaSufragio);
    }
}
