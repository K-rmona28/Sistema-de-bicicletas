using ApiCycleOn.Enums;
using ApiCycleOn.Models;
using ApiCycleOn.Patterns.State;
using Cycleon.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace ApiCycleOn.Data;

public partial class CycleOnContext : DbContext
{
    public CycleOnContext()
    {
    }

    public CycleOnContext(DbContextOptions<CycleOnContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Administrador> Administradors { get; set; }

    public virtual DbSet<Bicicletum> Bicicleta { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Estacion> Estacions { get; set; }

    public virtual DbSet<Estadistica> Estadisticas { get; set; }

    public virtual DbSet<Factura> Facturas { get; set; }

    public virtual DbSet<Mantenimiento> Mantenimientos { get; set; }

    public virtual DbSet<Movimiento> Movimientos { get; set; }

    public virtual DbSet<NotificacionEmail> NotificacionEmails { get; set; }

    public virtual DbSet<NotificacionWhatsApp> NotificacionWhatsApps { get; set; }

    public virtual DbSet<Pago> Pagos { get; set; }

    public virtual DbSet<Prestamo> Prestamos { get; set; }

    public virtual DbSet<Reporte> Reportes { get; set; }

    public virtual DbSet<Reserva> Reservas { get; set; }

    public virtual DbSet<Sancion> Sancions { get; set; }

    public virtual DbSet<Seguimiento> Seguimientos { get; set; }

    public virtual DbSet<Tarifa> Tarifas { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrador>(entity =>
        {
            entity.Property(e => e.IdUsuario).ValueGeneratedNever();

            entity.HasOne(d => d.IdUsuarioNavigation).WithOne(p => p.Administrador).HasConstraintName("FK_Administrador_Usuario");
        });

        modelBuilder.Entity<Bicicletum>(entity =>
        {
            entity.Property(e => e.Estado)
                .HasConversion<string>()
                .HasDefaultValue(EstadoBicicletaEnum.DISPONIBLE, "DF_Bicicleta_Estado");

            entity.HasOne(d => d.IdEstacionNavigation).WithMany(p => p.Bicicleta).HasConstraintName("FK_Bicicleta_Estacion");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.Property(e => e.IdUsuario).ValueGeneratedNever();
            entity.Property(e => e.MaximoPrestamos).HasDefaultValue(1, "DF_Cliente_MaxPrest");

            entity.HasOne(d => d.IdUsuarioNavigation).WithOne(p => p.Cliente).HasConstraintName("FK_Cliente_Usuario");
        });

        modelBuilder.Entity<Estacion>(entity =>
        {
            entity.Property(e => e.Estado)
                .HasConversion<string>()
                .HasDefaultValue(EstadoEstacionEnum.OPERATIVA, "DF_Estacion_Estado");
        });

        modelBuilder.Entity<Factura>(entity =>
        {
            entity.Property(e => e.FechaEmision).HasDefaultValueSql("(sysdatetime())", "DF_Factura_Fecha");

            entity.HasOne(d => d.IdPagoNavigation).WithOne(p => p.Factura)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Factura_Pago");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Facturas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Factura_Cliente");
        });

        modelBuilder.Entity<Mantenimiento>(entity =>
        {
            entity.Property(e => e.Estado)
                .HasConversion<string>()
                .HasDefaultValue(EstadoMantenimientoEnum.PROGRAMADO, "DF_Mantenimiento_Estado");

            entity.HasOne(d => d.IdBicicletaNavigation).WithMany(p => p.Mantenimientos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Mantenimiento_Bicicleta");
        });

        modelBuilder.Entity<Movimiento>(entity =>
        {
            entity.Property(e => e.FechaHora).HasDefaultValueSql("(sysdatetime())", "DF_Movimiento_Fecha");

            entity.HasOne(d => d.IdEstacionDestinoNavigation).WithMany(p => p.MovimientoIdEstacionDestinoNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Movimiento_Destino");

            entity.HasOne(d => d.IdEstacionOrigenNavigation).WithMany(p => p.MovimientoIdEstacionOrigenNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Movimiento_Origen");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Movimientos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Movimiento_Admin");

            entity.HasMany(d => d.IdBicicleta).WithMany(p => p.IdMovimientos)
                .UsingEntity<Dictionary<string, object>>(
                    "DetalleMovimiento",
                    r => r.HasOne<Bicicletum>().WithMany()
                        .HasForeignKey("IdBicicleta")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_DetMov_Bicicleta"),
                    l => l.HasOne<Movimiento>().WithMany()
                        .HasForeignKey("IdMovimiento")
                        .HasConstraintName("FK_DetMov_Movimiento"),
                    j =>
                    {
                        j.HasKey("IdMovimiento", "IdBicicleta");
                        j.ToTable("DetalleMovimiento");
                        j.IndexerProperty<int>("IdMovimiento").HasColumnName("idMovimiento");
                        j.IndexerProperty<int>("IdBicicleta").HasColumnName("idBicicleta");
                    });
        });

        modelBuilder.Entity<NotificacionEmail>(entity =>
        {
            entity.Property(e => e.Estado).HasDefaultValue("PENDIENTE", "DF_NotEmail_Estado");
            entity.Property(e => e.FechaHora).HasDefaultValueSql("(sysdatetime())", "DF_NotEmail_Fecha");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.NotificacionEmails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NotEmail_Usuario");
        });

        modelBuilder.Entity<NotificacionWhatsApp>(entity =>
        {
            entity.Property(e => e.Estado).HasDefaultValue("PENDIENTE", "DF_NotWa_Estado");
            entity.Property(e => e.FechaHora).HasDefaultValueSql("(sysdatetime())", "DF_NotWa_Fecha");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.NotificacionWhatsApps)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NotWa_Usuario");
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.Property(e => e.Estado)
                .HasConversion<string>()
                .HasDefaultValue(EstadoPagoEnum.PENDIENTE, "DF_Pago_Estado");
            entity.Property(e => e.FechaHoraPago).HasDefaultValueSql("(sysdatetime())", "DF_Pago_Fecha");

            entity.HasOne(d => d.IdPrestamoNavigation).WithOne(p => p.Pago)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pago_Prestamo");
        });

        modelBuilder.Entity<Prestamo>(entity =>
        {
            entity.HasIndex(e => e.IdBicicleta, "UX_Prestamo_BicicletaActiva")
                .IsUnique()
                .HasFilter("([Estado] IN (N'ACEPTADO', N'EN_PROCESO'))");

            entity.HasIndex(e => e.IdUsuario, "UX_Prestamo_ClienteActivo")
                .IsUnique()
                .HasFilter("([Estado] IN (N'ACEPTADO', N'EN_PROCESO'))");

            entity.HasIndex(e => e.IdReserva, "UX_Prestamo_Reserva")
                .IsUnique()
                .HasFilter("([idReserva] IS NOT NULL)");

            entity.Property(e => e.Estado)
                .HasConversion<string>()
                .HasDefaultValue(EstadoPrestamoEnum.ACEPTADO, "DF_Prestamo_Estado");
            entity.Property(e => e.FechaHoraInicio).HasDefaultValueSql("(sysdatetime())", "DF_Prestamo_Inicio");
            entity.Property(e => e.TiempoUso).HasComputedColumnSql("(datediff(minute,[FechaHoraInicio],[FechaHoraFin]))", true);

            entity.HasOne(d => d.EstacionDestinoNavigation).WithMany(p => p.PrestamoEstacionDestinoNavigations).HasConstraintName("FK_Prestamo_EstDestino");

            entity.HasOne(d => d.EstacionOrigenNavigation).WithMany(p => p.PrestamoEstacionOrigenNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prestamo_EstOrigen");

            entity.HasOne(d => d.IdBicicletaNavigation).WithMany(p => p.Prestamos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prestamo_Bicicleta");

            entity.HasOne(d => d.IdReservaNavigation).WithOne(p => p.Prestamo).HasConstraintName("FK_Prestamo_Reserva");

            entity.HasOne(d => d.IdTarifaNavigation).WithMany(p => p.Prestamos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prestamo_Tarifa");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Prestamos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prestamo_Cliente");
        });

        modelBuilder.Entity<Reporte>(entity =>
        {
            entity.Property(e => e.Estado)
                .HasConversion<string>()
                .HasDefaultValue(EstadoReporteEnum.REGISTRADO, "DF_Reporte_Estado");
            entity.Property(e => e.FechaHora).HasDefaultValueSql("(sysdatetime())", "DF_Reporte_Fecha");

            entity.HasOne(d => d.IdBicicletaNavigation).WithMany(p => p.Reportes).HasConstraintName("FK_Reporte_Bicicleta");

            entity.HasOne(d => d.IdEstacionNavigation).WithMany(p => p.Reportes).HasConstraintName("FK_Reporte_Estacion");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Reportes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reporte_Usuario");
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.Property(e => e.Estado)
                .HasConversion<string>()
                .HasDefaultValue(EstadoReservaEnum.VIGENTE, "DF_Reserva_Estado");
            entity.Property(e => e.FechaHoraReserva).HasDefaultValueSql("(sysdatetime())", "DF_Reserva_Fecha");

            entity.HasOne(d => d.IdBicicletaNavigation).WithMany(p => p.Reservas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reserva_Bicicleta");

            entity.HasOne(d => d.IdEstacionNavigation).WithMany(p => p.Reservas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reserva_Estacion");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Reservas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reserva_Cliente");
        });

        modelBuilder.Entity<Sancion>(entity =>
        {
            entity.Property(e => e.Estado)
                .HasConversion<string>()
                .HasDefaultValue(EstadoSancionEnum.ACTIVA_TEMPORAL, "DF_Sancion_Estado");
            entity.Property(e => e.FechaHora).HasDefaultValueSql("(sysdatetime())", "DF_Sancion_Fecha");

            entity.HasOne(d => d.IdPrestamoNavigation).WithMany(p => p.Sancions).HasConstraintName("FK_Sancion_Prestamo");

            entity.HasOne(d => d.IdReporteNavigation).WithMany(p => p.Sancions).HasConstraintName("FK_Sancion_Reporte");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Sancions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Sancion_Usuario");
        });

        modelBuilder.Entity<Seguimiento>(entity =>
        {
            entity.Property(e => e.FechaHora).HasDefaultValueSql("(sysdatetime())", "DF_Seguimiento_Fecha");

            entity.HasOne(d => d.IdPrestamoNavigation).WithMany(p => p.Seguimientos).HasConstraintName("FK_Seguimiento_Prestamo");
        });

        modelBuilder.Entity<Tarifa>(entity =>
        {
            entity.Property(e => e.Estado).HasDefaultValue("ACTIVA", "DF_Tarifa_Estado");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.Property(e => e.Edad).HasComputedColumnSql("(datediff(year,[FechaDeNacimiento],getdate())-case when dateadd(year,datediff(year,[FechaDeNacimiento],getdate()),[FechaDeNacimiento])>getdate() then (1) else (0) end)", false);
            entity.Property(e => e.Estado).HasDefaultValue("ACTIVO", "DF_Usuario_Estado");
            entity.Property(e => e.FechaDeRegistro).HasDefaultValueSql("(sysdatetime())", "DF_Usuario_FechaReg");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}