using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using WorldCupPlanner.UI.Data;
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.AccesoADatos.Repositorios;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Servicios;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddSingleton<IRepositorioConId<Auditoria>, RepositorioAuditoriaEnMemoria>();
builder.Services.AddSingleton<IRepositorio<Equipo>, RepositorioEquipoEnMemoria>();
builder.Services.AddSingleton<IRepositorio<Estadio>, RepositorioEstadioEnMemoria>();
builder.Services.AddSingleton<ServicioAuditoria>();
builder.Services.AddSingleton<ServicioEstadio>();
builder.Services.AddSingleton<ServicioEquipo>();
builder.Services.AddSingleton<ServicioFixture>();
builder.Services.AddSingleton<ServicioClasificacion>();
builder.Services.AddSingleton<ServicioCruces>();
builder.Services.AddSingleton<ServicioSimulacion>();
builder.Services.AddSingleton<ServicioConfiguracionTorneo>();
builder.Services.AddSingleton<EstadoTorneo>();
builder.Services.AddSingleton<ServicioPartido>(sp =>
    new ServicioPartido(sp.GetRequiredService<EstadoTorneo>()));
builder.Services.AddSingleton<ConfiguracionTorneo>();
builder.Services.AddSingleton<IRepositorioUsuario, RepositorioUsuarioEnMemoria>();
builder.Services.AddSingleton<ServicioUsuario>();
builder.Services.AddSingleton<InicializadorDatos>();

builder.Services.AddScoped<SesionServicio>();
builder.Services.AddScoped<ToastServicio>();

var app = builder.Build();

app.Services.GetRequiredService<InicializadorDatos>().InicializarUsuariosSemilla();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
