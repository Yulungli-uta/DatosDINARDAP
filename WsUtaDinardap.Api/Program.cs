using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using WsUtaDinardap.Api.Application;
using WsUtaDinardap.Api.Application.Handlers;
using WsUtaDinardap.Api.Application.Interfaces;
using WsUtaDinardap.Api.Domain;
using WsUtaDinardap.Api.Endpoints;
using WsUtaDinardap.Api.Infrastructure.Dinardap;
using WsUtaDinardap.Api.Infrastructure.ErrorHandling;
using WsUtaDinardap.Api.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

// =========================================================
// Opciones (Dinardap: secretos solo por entorno/Secret Manager;
// AuthService: URL/issuer/audience de WsSegu)
// =========================================================
builder.Services.Configure<DinardapOptions>(builder.Configuration.GetSection(DinardapOptions.SectionName));
builder.Services.Configure<WsSeguOptions>(builder.Configuration.GetSection(WsSeguOptions.SectionName));

builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();

// =========================================================
// JWT (RS256) - validacion local via JWKS publicado por WsSegu.
// Mismo patron ya vigente en HrBackend (AuthService:ValidationMode=Local),
// aqui como el mecanismo estandar (unico) desde el dia 1 de este proyecto nuevo.
// =========================================================
var wsSeguSection = builder.Configuration.GetSection(WsSeguOptions.SectionName);
var wsSeguOptions = wsSeguSection.Get<WsSeguOptions>() ?? new WsSeguOptions();

builder.Services.AddSingleton<JwksKeyResolver>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = wsSeguOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = wsSeguOptions.Audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.FromMinutes(2)
        };
        // IssuerSigningKeyResolver necesita el JwksKeyResolver ya construido; se resuelve
        // desde el ServiceProvider raiz en el momento del primer request de auth.
        o.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var resolver = context.HttpContext.RequestServices.GetRequiredService<JwksKeyResolver>();
                context.Options.TokenValidationParameters.IssuerSigningKeyResolver =
                    (token, securityToken, kid, parameters) => resolver.ResolveSigningKeys(token, securityToken, kid, parameters);
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// =========================================================
// Permisos (reutiliza el catalogo/endpoint ya existente de WsSegu)
// =========================================================
builder.Services.AddScoped<IUserActionPermissionService, UserActionPermissionService>();

// =========================================================
// DINARDAP: gateway unico + handlers por paquete + orquestador
// =========================================================
builder.Services.AddScoped<IDinardapGateway, DinardapSoapGateway>();
builder.Services.AddScoped<IDinardapPackageHandler<RegistroCivilData>, RegistroCivilPackageHandler>();
builder.Services.AddScoped<IDinardapPackageHandler<TceData>, TcePackageHandler>();
builder.Services.AddScoped<IDinardapPackageHandler<IReadOnlyList<TituloData>>, TitulosPackageHandler>();
builder.Services.AddScoped<IDinardapService, DinardapService>();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseDinardapExceptionHandling();

app.UseAuthentication();
app.UseAuthorization();

app.MapDinardapEndpoints();

app.MapGet("/health", () => Results.Ok(new { status = "ok" })).AllowAnonymous();

app.Run();
