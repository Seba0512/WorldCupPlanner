using Microsoft.EntityFrameworkCore;
using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.AccesoADatos
{
    public class SqlContext : DbContext
    {
        public DbSet<Equipo> Equipos => Set<Equipo>();

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
        }
    }
}
