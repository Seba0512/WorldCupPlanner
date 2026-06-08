using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using WorldCupPlanner.UI.Data;
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.AccesoADatos.Repositorios;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Servicios;
using Microsoft.EntityFrameworkCore;
using WorldCupPlanner.AccesoADatos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddScoped<IRepositorioConId<Auditoria>, RepositorioAuditoriaSql>();
builder.Services.AddScoped<IRepositorio<Equipo>, RepositorioEquipoSql>();
builder.Services.AddScoped<IRepositorio<Estadio>, RepositorioEstadioSql>();
builder.Services.AddScoped<IRepositorio<Grupo>, RepositorioGrupoSql>();
builder.Services.AddScoped<IRepositorio<Partido>, RepositorioPartidoSql>();
builder.Services.AddScoped<ServicioAuditoria>();
builder.Services.AddScoped<ServicioEstadio>();
builder.Services.AddScoped<ServicioEquipo>(sp =>
    new ServicioEquipo(
        sp.GetRequiredService<IRepositorio<Equipo>>(),
        sp.GetRequiredService<ServicioAuditoria>()));
builder.Services.AddSingleton<ServicioFixture>();
builder.Services.AddSingleton<ServicioClasificacion>();
builder.Services.AddSingleton<ServicioCruces>();
builder.Services.AddSingleton<ServicioSimulacion>();
builder.Services.AddScoped<IRepositorioConfiguracionTorneo, RepositorioConfiguracionTorneoSql>();
builder.Services.AddScoped<IRepositorioTorneoArchivado, RepositorioTorneoArchivadoSql>();
builder.Services.AddScoped<ServicioConfiguracionTorneo>();
builder.Services.AddSingleton<EstadoTorneo>();
builder.Services.AddScoped<ServicioPersistenciaFixture>();
builder.Services.AddScoped<ServicioPartido>(sp =>
    new ServicioPartido(sp.GetRequiredService<IRepositorio<Partido>>()));
builder.Services.AddScoped<IRepositorioUsuario, RepositorioUsuarioSql>();
builder.Services.AddScoped<ServicioUsuario>();
builder.Services.AddScoped<IRepositorioNotificacion, RepositorioNotificacionSql>();
builder.Services.AddScoped<ServicioNotificacion>();
builder.Services.AddScoped<IRepositorioIncidencia, RepositorioIncidenciaSql>();
builder.Services.AddScoped<ServicioIncidencia>();
builder.Services.AddScoped<ServicioRankingDinamico>(sp =>
    new ServicioRankingDinamico(
        sp.GetRequiredService<IRepositorio<Equipo>>(),
        sp.GetRequiredService<IRepositorio<Partido>>(),
        sp.GetRequiredService<ServicioAuditoria>()));
builder.Services.AddScoped<ServicioExportacion>();
builder.Services.AddScoped<ServicioImportacionLegible>();
builder.Services.AddScoped<InicializadorDatos>();

builder.Services.AddScoped<SesionServicio>();
builder.Services.AddScoped<ToastServicio>();
builder.Services.AddDbContext<SqlContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<SqlContext>().Database.Migrate();
    scope.ServiceProvider.GetRequiredService<InicializadorDatos>().InicializarUsuariosSemilla();

    var persistencia = scope.ServiceProvider.GetRequiredService<ServicioPersistenciaFixture>();
    var estadoTorneo = app.Services.GetRequiredService<EstadoTorneo>();
    persistencia.CargarEnEstadoTorneo(estadoTorneo);
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
