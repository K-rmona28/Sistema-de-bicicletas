using ApiCycleOn.Enums;
using ApiCycleOn.Patterns.State;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCycleOn.Models;

[Index("IdEstacion", Name = "IX_Bicicleta_Estacion")]
[Index("Serial", Name = "UQ_Bicicleta_Serial", IsUnique = true)]
public partial class Bicicletum
{
    [Key]
    [Column("idBicicleta")]
    public int IdBicicleta { get; set; }

    [Column("idEstacion")]
    public int? IdEstacion { get; set; }

    [StringLength(50)]
    public string Tipo { get; set; } = null!;

    [StringLength(20)]
    public string Tamano { get; set; } = null!;

    [StringLength(250)]
    public string? Especificacion { get; set; }

    [StringLength(20)]
    public EstadoBicicletaEnum Estado { get; set; } = EstadoBicicletaEnum.DISPONIBLE;

    [StringLength(60)]
    public string Marca { get; set; } = null!;

    [StringLength(60)]
    public string Modelo { get; set; } = null!;

    [StringLength(60)]
    public string Serial { get; set; } = null!;

    [Column(TypeName = "decimal(12, 2)")]
    public decimal Valor { get; set; }
    [StringLength(300)]
    public string? ImagenUrl { get; set; }

    [ForeignKey("IdEstacion")]
    [InverseProperty("Bicicleta")]
    public virtual Estacion? IdEstacionNavigation { get; set; }

    [InverseProperty("IdBicicletaNavigation")]
    public virtual ICollection<Mantenimiento> Mantenimientos { get; set; } = new List<Mantenimiento>();

    public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();

    [InverseProperty("IdBicicletaNavigation")]
    public virtual ICollection<Reporte> Reportes { get; set; } = new List<Reporte>();

    [InverseProperty("IdBicicletaNavigation")]
    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();

    [ForeignKey("IdBicicleta")]
    [InverseProperty("IdBicicleta")]
    public virtual ICollection<Movimiento> IdMovimientos { get; set; } = new List<Movimiento>();
}
