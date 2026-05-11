using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using WorldCupPlanner.UI.Data;
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.AccesoADatos.Repositorios;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Servicios;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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
builder.Services.AddSingleton<RepositorioUsuarioEnMemoria>();
builder.Services.AddSingleton<ServicioUsuario>();
builder.Services.AddScoped<SesionServicio>();

var app = builder.Build();

var servicioUsuario = app.Services.GetRequiredService<ServicioUsuario>();
var admin = new Usuario(new DatosUsuario { Nombre = "Admin", Apellido = "Admin", Email = "a@a.com", FechaNacimiento = "01-01-1990" });
admin.EstablecerContrasenia("Admin@2026");
servicioUsuario.AgregarUsuario(admin);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
