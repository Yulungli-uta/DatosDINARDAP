using WsUtaDinardap.Api.Application.Handlers;
using WsUtaDinardap.Api.Application.Interfaces;
using Xunit;

namespace WsUtaDinardap.Api.Tests.PackageHandlers;

public class TitulosPackageHandlerTests
{
    private static readonly TitulosPackageHandler Handler = new();

    private static ItemDto Item(string nombreNivel, params (string Codigo, string Valor)[] campos) => new(
        nombreNivel, campos.Select(c => new RegistroDto(null, c.Codigo, c.Valor)).ToList());

    private static FichaGeneralDto FichaCon(params ItemDto[] items) => new(
        "115", null,
        new[] { new InstitucionDto(null, null, Array.Empty<RegistroDto>(), items) });

    [Theory]
    [InlineData("Títulos de Tercer Nivel", 3)]
    [InlineData("Tercer Nivel o Pregrado", 3)]
    [InlineData("Títulos Cuarto Nivel", 4)]
    [InlineData("Cuarto Nivel o Posgrado", 4)]
    [InlineData("CUARTO_NIVEL", 4)]
    [InlineData("Bachillerato", 2)] // cualquier otro nombre cae en nivel 2, igual que el DAL original
    public void Parse_ClasificaNivel_IgualQueElDalOriginal(string nombreNivel, int nivelEsperado)
    {
        var ficha = FichaCon(Item(nombreNivel, ("54", "Ingeniero de Sistemas")));

        var resultado = Handler.Parse(ficha);

        Assert.Single(resultado);
        Assert.Equal(nivelEsperado, resultado[0].Nivel);
    }

    [Fact]
    public void Parse_TodosLosCampos_MapeaCorrectamente()
    {
        var ficha = FichaCon(Item("Tercer Nivel o Pregrado",
            ("51", "2015/07/10"),
            ("52", "2015/09/01"),
            ("53", "Universidad Tecnica de Ambato"),
            ("54", "Ingeniero en Sistemas"),
            ("55", "REG-123"),
            ("56", "Nacional"),
            ("57", "")));

        var titulo = Handler.Parse(ficha).Single();

        Assert.Equal(new DateTime(2015, 7, 10), titulo.FechaGrado);
        Assert.Equal(new DateTime(2015, 9, 1), titulo.FechaRegistro);
        Assert.Equal("Universidad Tecnica de Ambato", titulo.InstitucionEducacionSuperior);
        Assert.Equal("Ingeniero en Sistemas", titulo.NombreTitulo);
        Assert.Equal("REG-123", titulo.NumeroRegistro);
        Assert.Equal("Nacional", titulo.Tipo);
    }

    [Fact]
    public void Parse_FechaCortaOAusente_DevuelveNull_NoLanza()
    {
        var ficha = FichaCon(Item("Tercer Nivel o Pregrado", ("51", "2015"))); // < 8 caracteres

        var titulo = Handler.Parse(ficha).Single();

        Assert.Null(titulo.FechaGrado);
    }

    [Fact]
    public void Parse_SinTitulos_DevuelveListaVacia_NuncaNull()
    {
        var ficha = new FichaGeneralDto("115", null, Array.Empty<InstitucionDto>());

        var resultado = Handler.Parse(ficha);

        Assert.NotNull(resultado);
        Assert.Empty(resultado);
    }
}
