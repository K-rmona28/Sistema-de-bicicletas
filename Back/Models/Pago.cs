using ApiCycleOn.Enums;
using ApiCycleOn.Patterns.State;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCycleOn.Models;

[Table("Pago")]
[Index("IdPrestamo", Name = "UQ_Pago_Prestamo", IsUnique = true)]
public partial class Pago
{
    [Key]
    [Column("idPago")]
    public int IdPago { get; set; }

    [Column("idPrestamo")]
    public int IdPrestamo { get; set; }

    [StringLength(30)]
    public string MetodoPago { get; set; } = null!;

    [Column(TypeName = "decimal(12, 2)")]
    public decimal Monto { get; set; }

    [Precision(0)]
    public DateTime FechaHoraPago { get; set; }

    [StringLength(20)]
    public EstadoPagoEnum Estado { get; set; } = EstadoPagoEnum.PENDIENTE;

    [InverseProperty("IdPagoNavigation")]
    public virtual Factura? Factura { get; set; }

    [ForeignKey("IdPrestamo")]
    [InverseProperty("Pago")]
    public virtual Prestamo IdPrestamoNavigation { get; set; } = null!;
}
