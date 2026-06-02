using Microsoft.EntityFrameworkCore;
using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.AccesoADatos
{
    public class SqlContext : DbContext
    {
        public DbSet<Equipo> Equipos => Set<Equipo>();
        public DbSet<Estadio> Estadios => Set<Estadio>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();

        public SqlContext(DbContextOptions<SqlContext> options)
            : base(options)
        {
            this.Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Equipo>(builder =>
            {
                builder.ToTable("Equipos");

                builder.HasKey(equipo => equipo.Nombre);

                builder.Property(equipo => equipo.Nombre)
                    .IsRequired()
                    .HasMaxLength(60);

                builder.Property(equipo => equipo.RankingFIFA)
                    .IsRequired();

                builder.Property(equipo => equipo.Confederacion)
                    .HasConversion<string>()
                    .IsRequired();
            });

            modelBuilder.Entity<Estadio>(builder =>
            {
                builder.ToTable("Estadios");

                builder.HasKey(estadio => estadio.Nombre);

                builder.Property(estadio => estadio.Nombre)
                    .IsRequired()
                    .HasMaxLength(80);

                builder.Property(estadio => estadio.Ciudad)
                    .IsRequired()
                    .HasMaxLength(60);

                builder.Property(estadio => estadio.Direccion);

                builder.Property(estadio => estadio.Capacidad)
                    .IsRequired();

                builder.Property(estadio => estadio.Descripcion)
                    .HasMaxLength(400);
            });

            modelBuilder.Entity<Usuario>(builder =>
            {
                builder.ToTable("Usuarios");

                builder.HasKey(usuario => usuario.Email);

                builder.Property(usuario => usuario.Email)
                    .IsRequired()
                    .HasMaxLength(320);

                builder.Property(usuario => usuario.Nombre)
                    .IsRequired();

                builder.Property(usuario => usuario.Apellido)
                    .IsRequired();

                builder.Property(usuario => usuario.FechaNacimiento)
                    .IsRequired();

                builder.Property(usuario => usuario.DebeCambiarContrasenia)
                    .IsRequired();

                builder.Property<string?>("_contraseniaCifrada")
                    .HasColumnName("ContraseniaCifrada")
                    .HasMaxLength(100);

                builder.Property<string>("RolesSerializados")
                    .HasColumnName("Roles")
                    .IsRequired()
                    .HasMaxLength(200);
            });
        }
    }
}
