using Microsoft.EntityFrameworkCore;
using ServiControl.Domain.Entities;
using ServiControl.Domain.Enums;

namespace ServiControl.Infrastructure.Persistence.Context;

// Modulo: Persistencia
// Capa: Infrastructure
// Responsabilidad: Mapea entidades de dominio a SQL Server con EF Core.
// Nota: Se usa Fluent API para mantener Domain libre de atributos de persistencia.
public class ApplicationDbContext : DbContext
{
    //options viene de program.cs builder.Services.AddDbContext 
    //EF crea un objeto dbcontextOptions, conection String, proveedor SQL Server, configuraciones
    //luego lo inyecta en base
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    //tablas de la DB
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Trabajo> Trabajos => Set<Trabajo>();
    public DbSet<Costo> Costos => Set<Costo>();
    public DbSet<Metrica> Metricas => Set<Metrica>();

    //esto se ejecuta una sola vez cuando construye el modelo
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //aca declaramos funciones para configurar cada tabla
        ConfigureUsuarios(modelBuilder);
        ConfigureClientes(modelBuilder);
        ConfigureTrabajos(modelBuilder);
        ConfigureCostos(modelBuilder);
        ConfigureMetricas(modelBuilder);
    }

    //luego configuramos cada tabla y le ponemos las restricciones que queremos y que son parte de la DB
    //esto es parte de CODE FIRST
    private static void ConfigureUsuarios(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuarios");

            entity.HasKey(usuario => usuario.Id);

            entity.Property(usuario => usuario.Nombre)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(usuario => usuario.Email)
                .HasMaxLength(150)
                .IsRequired();

            entity.HasIndex(usuario => usuario.Email)
                .IsUnique();

            entity.Property(usuario => usuario.PasswordHash)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(usuario => usuario.Rol)
                .HasConversion<int>()
                .IsRequired();

            entity.HasOne(usuario => usuario.UsuarioResponsable)
                .WithMany()
                .HasForeignKey(usuario => usuario.IdUsuarioResponsable)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureClientes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("Clientes");

            entity.HasKey(cliente => cliente.Id);

            entity.Property(cliente => cliente.UsuarioId)
                .IsRequired();

            entity.Property(cliente => cliente.Nombre)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(cliente => cliente.Telefono)
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(cliente => cliente.Email)
                .HasMaxLength(150);

            entity.Property(cliente => cliente.Observaciones)
                .HasMaxLength(500);

            entity.HasIndex(cliente => cliente.UsuarioId);

            entity.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(cliente => cliente.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureTrabajos(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Trabajo>(entity =>
        {
            entity.ToTable("Trabajos");

            entity.HasKey(trabajo => trabajo.Id);

            entity.Property(trabajo => trabajo.CategoriaServicio)
                .HasConversion<int>()
                .IsRequired();

            entity.Property(trabajo => trabajo.Descripcion)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(trabajo => trabajo.Fecha)
                .HasColumnType("date")
                .IsRequired();

            entity.Property(trabajo => trabajo.Direccion)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(trabajo => trabajo.Observaciones)
                .HasMaxLength(500);

            entity.Property(trabajo => trabajo.Estado)
                .HasConversion<int>()
                .IsRequired();

            entity.HasOne<Cliente>()
                .WithMany()
                .HasForeignKey(trabajo => trabajo.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(trabajo => trabajo.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureCostos(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Costo>(entity =>
        {
            entity.ToTable("Costos");

            entity.HasKey(costo => costo.Id);

            entity.Property(costo => costo.CostoEstimado)
                .HasPrecision(18, 2);

            entity.Property(costo => costo.CostoFinal)
                .HasPrecision(18, 2);

            entity.HasIndex(costo => costo.TrabajoId)
                .IsUnique();

            entity.HasOne<Trabajo>()
                .WithOne()
                .HasForeignKey<Costo>(costo => costo.TrabajoId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureMetricas(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Metrica>(entity =>
        {
            entity.ToTable("Metricas");

            entity.HasKey(metrica => metrica.Id);

            entity.Property(metrica => metrica.MontoTotalPeriodo)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(metrica => metrica.MontoPromedioTrabajo)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(metrica => metrica.CantidadTrabajos)
                .IsRequired();

            entity.Property(metrica => metrica.TrabajosPendientes)
                .IsRequired();

            entity.Property(metrica => metrica.PeriodoInicio)
                .IsRequired();

            entity.Property(metrica => metrica.PeriodoFin)
                .IsRequired();

            entity.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(metrica => metrica.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
