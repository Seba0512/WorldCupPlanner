using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using WorldCupPlanner.AccesoADatos;
using WorldCupPlanner.UI.Data;
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.AccesoADatos.Repositorios;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Servicios;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddDbContext<SqlContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IRepositorioConId<Auditoria>, RepositorioAuditoriaSql>();
builder.Services.AddScoped<IRepositorio<Equipo>, RepositorioEquipoSql>();
builder.Services.AddScoped<IRepositorio<Estadio>, RepositorioEstadioSql>();
builder.Services.AddScoped<IRepositorio<Grupo>, RepositorioGrupoSql>();
builder.Services.AddScoped<IRepositorio<Partido>, RepositorioPartidoSql>();
builder.Services.AddScoped<ServicioAuditoria>();
builder.Services.AddScoped<ServicioEstadio>();
builder.Services.AddScoped<ServicioEquipo>();
builder.Services.AddSingleton<ServicioFixture>();
builder.Services.AddSingleton<ServicioClasificacion>();
builder.Services.AddSingleton<ServicioCruces>();
builder.Services.AddSingleton<ServicioSimulacion>();
builder.Services.AddSingleton(_ => new ServicioConfiguracionTorneo());
builder.Services.AddSingleton<EstadoTorneo>();
builder.Services.AddScoped<ServicioPersistenciaFixture>();
builder.Services.AddSingleton<ServicioPartido>(sp =>
    new ServicioPartido(sp.GetRequiredService<EstadoTorneo>()));
builder.Services.AddScoped<IRepositorioUsuario, RepositorioUsuarioSql>();
builder.Services.AddScoped<ServicioUsuario>();
builder.Services.AddScoped<ServicioImportacionLegible>();
builder.Services.AddScoped<InicializadorDatos>();

builder.Services.AddScoped<SesionServicio>();
builder.Services.AddScoped<ToastServicio>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<InicializadorDatos>().InicializarUsuariosSemilla();
}

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
