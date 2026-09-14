using WsUtaDinardap.Api.Application.Handlers;
using WsUtaDinardap.Api.Application.Interfaces;
using Xunit;

namespace WsUtaDinardap.Api.Tests.PackageHandlers;

public class RegistroCivilPackageHandlerTests
{
    private static readonly RegistroCivilPackageHandler Handler = new();

    private static FichaGeneralDto FichaCon(params (string Codigo, string Valor)[] campos) => new(
        "113", null,
        new[]
        {
            new InstitucionDto(null, null,
                campos.Select(c => new RegistroDto(null, c.Codigo, c.Valor)).ToList(),
                Array.Empty<ItemDto>())
        });

    [Fact]
    public void Parse_TodosLosCampos_MapeaCorrectamente()
    {
        var ficha = FichaCon(
            ("1", "1234567890"),
            ("2", "Garcia Lopez Juan Carlos"),
            ("3", "M"),
            ("4", "Ciudadano"),
            ("5", "15/03/1990"),
            ("6", "Tungurahua/Ambato/Huachi"),
            ("7", "Ecuatoriana"),
            ("8", "Soltero"),
            ("10", ""),
            ("11", "Garcia Perez"),
            ("13", "Lopez Mora"));

        var resultado = Handler.Parse(ficha);

        Assert.Equal("1234567890", resultado.Codigo);
        Assert.Equal("Garcia", resultado.Apellido1);
        Assert.Equal("Lopez", resultado.Apellido2);
        Assert.Equal("Juan Carlos", resultado.Nombres);
        Assert.Equal(new DateTime(1990, 3, 15), resultado.FechaNacimiento);
        Assert.Equal("Tungurahua", resultado.LugarNacimientoProvincia);
        Assert.Equal("Ambato", resultado.LugarNacimientoCiudad);
        Assert.Equal("Huachi", resultado.LugarNacimientoParroquia);
        Assert.Equal("Ecuatoriana", resultado.Nacionalidad);
    }

    [Theory]
    [InlineData("de la Torre", "de la Torre", null, null)]
    [InlineData("Perez", "Perez", null, null)]
    public void SepararApellidos_PreservaPreposiciones_ComoElOriginal(string nombre, string apellido1Esperado, string? apellido2Esperado, string? nombresEsperados)
    {
        var ficha = FichaCon(("1", "x"), ("2", nombre));
        var resultado = Handler.Parse(ficha);

        Assert.Equal(apellido1Esperado, resultado.Apellido1);
        Assert.Equal(apellido2Esperado, resultado.Apellido2);
        Assert.Equal(nombresEsperados, resultado.Nombres);
    }

    [Fact]
    public void Parse_SinInstituciones_DevuelveTodoNull_NuncaLanza()
    {
        var ficha = new FichaGeneralDto("113", null, Array.Empty<InstitucionDto>());

        var resultado = Handler.Parse(ficha);

        Assert.Null(resultado.Codigo);
        Assert.Null(resultado.NombreCompleto);
        Assert.Null(resultado.FechaNacimiento);
    }

    [Fact]
    public void Parse_FechaNacimientoMalformada_NoLanza_QuedaNull()
    {
        var ficha = FichaCon(("1", "123"), ("5", "fecha-invalida"));

        var resultado = Handler.Parse(ficha);

        Assert.Equal("123", resultado.Codigo);
        Assert.Null(resultado.FechaNacimiento);
    }
}
