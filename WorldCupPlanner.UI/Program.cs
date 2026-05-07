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
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddSingleton<IAuditoria, RepositorioAuditoriaEnMemoria>();
builder.Services.AddSingleton<ServicioAuditoria>();
builder.Services.AddSingleton<RepositorioEstadioEnMemoria>();
builder.Services.AddSingleton<ServicioEstadio>();
builder.Services.AddSingleton<ServicioEquipo>();
builder.Services.AddSingleton<RepositorioUsuarioEnMemoria>();
builder.Services.AddSingleton<ServicioUsuario>();
builder.Services.AddScoped<SesionServicio>();

var app = builder.Build();

var servicioUsuario = app.Services.GetRequiredService<ServicioUsuario>();
var admin = new Usuario("Admin", "Admin", "a@a.com", new DateTime(1990, 1, 1));
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
