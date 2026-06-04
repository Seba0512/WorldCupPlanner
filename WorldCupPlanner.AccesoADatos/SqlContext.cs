using Microsoft.EntityFrameworkCore;
using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.AccesoADatos
{
    public class SqlContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Equipo> Equipos { get; set; }
        public DbSet<Estadio> Estadios { get; set; }
        public DbSet<Grupo> Grupos { get; set; }
        public DbSet<Partido> Partidos { get; set; }
        public DbSet<Auditoria> Auditorias { get; set; }

        public SqlContext(DbContextOptions<SqlContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigurarUsuario(modelBuilder);
            ConfigurarEquipo(modelBuilder);
            ConfigurarEstadio(modelBuilder);
            ConfigurarGrupo(modelBuilder);
            ConfigurarPartido(modelBuilder);
            ConfigurarAuditoria(modelBuilder);
            ConfigurarIncidencia(modelBuilder);
        }

        private static void ConfigurarUsuario(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Id).ValueGeneratedOnAdd();
                entity.Property(u => u.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Apellido).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(200);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.FechaNacimiento).IsRequired();
                entity.Property(u => u.DebeCambiarContrasenia).IsRequired();

                entity.Property<string>("_contraseniaCifrada")
                    .HasColumnName("ContraseniaCifrada")
                    .HasField("_contraseniaCifrada");

                entity.Property<string>("RolesSerializados")
                    .HasColumnName("Roles")
                    .HasMaxLength(200);
            });
        }

        private static void ConfigurarEquipo(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Equipo>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(60);
                entity.Property(e => e.RankingFIFA).IsRequired();
                entity.Property(e => e.Confederacion).IsRequired();
                entity.HasIndex(e => e.Nombre).IsUnique();
            });
        }

        private static void ConfigurarEstadio(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Estadio>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(80);
                entity.Property(e => e.Ciudad).IsRequired().HasMaxLength(60);
                entity.Property(e => e.Capacidad).IsRequired();
                entity.Property(e => e.Descripcion).HasMaxLength(400);
                entity.HasIndex(e => e.Nombre).IsUnique();
            });
        }

        private static void ConfigurarGrupo(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Grupo>(entity =>
            {
                entity.HasKey(g => g.Id);
                entity.Property(g => g.Id).ValueGeneratedOnAdd();
                entity.Property(g => g.Etiqueta).IsRequired().HasMaxLength(1);

                entity.HasMany(g => g.Equipos)
                    .WithMany()
                    .UsingEntity("GrupoEquipo");

                entity.HasMany(g => g.Partidos)
                    .WithOne(p => p.Grupo!)
                    .HasForeignKey("GrupoId")
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private static void ConfigurarPartido(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Partido>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id).ValueGeneratedOnAdd();
                entity.Property(p => p.Fecha).IsRequired();
                entity.Property(p => p.Fase).IsRequired();

                entity.HasOne(p => p.Estadio)
                    .WithMany()
                    .HasForeignKey("EstadioId")
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.EquipoLocal)
                    .WithMany()
                    .HasForeignKey("EquipoLocalId")
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.EquipoVisitante)
                    .WithMany()
                    .HasForeignKey("EquipoVisitanteId")
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.GanadorPorPenales)
                    .WithMany()
                    .HasForeignKey("GanadorPorPenalesId")
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);
                
                entity.HasMany(p => p.Incidencias)
                    .WithOne()
                    .HasForeignKey("PartidoId")
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private static void ConfigurarAuditoria(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Auditoria>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Id).ValueGeneratedOnAdd();
                entity.Property(a => a.Timestamp).IsRequired().HasMaxLength(50);
                entity.Property(a => a.Usuario).IsRequired().HasMaxLength(200);
                entity.Property(a => a.Accion).IsRequired().HasMaxLength(200);
                entity.Property(a => a.Detalle).IsRequired().HasMaxLength(1000);
            });
        }
        private static void ConfigurarIncidencia(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Incidencia>(entity =>
            {
                entity.HasKey(i => i.Id);
                entity.Property(i => i.Id).ValueGeneratedOnAdd();
                entity.Property(i => i.MinutoDelPartido).IsRequired();

                entity.HasOne(i => i.Equipo)
                    .WithMany()
                    .HasForeignKey("EquipoId")
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasDiscriminator<string>("Tipo")
                    .HasValue<TarjetaAmarilla>("TarjetaAmarilla")
                    .HasValue<TarjetaRoja>("TarjetaRoja");
            });
        }
    }
}

