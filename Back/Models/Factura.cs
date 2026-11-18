using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCycleOn.Models;

[Table("Factura")]
[Index("CodigoFactura", Name = "UQ_Factura_Codigo", IsUnique = true)]
[Index("IdPago", Name = "UQ_Factura_Pago", IsUnique = true)]
public partial class Factura
{
    [Key]
    [Column("idFactura")]
    public int IdFactura { get; set; }

    [Column("idUsuario")]
    public int IdUsuario { get; set; }

    [Column("idPago")]
    public int IdPago { get; set; }

    [Precision(0)]
    public DateTime FechaEmision { get; set; }

    [Column(TypeName = "decimal(12, 2)")]
    public decimal MontoTotal { get; set; }

    [StringLength(40)]
    public string CodigoFactura { get; set; } = null!;

    [ForeignKey("IdPago")]
    [InverseProperty("Factura")]
    public virtual Pago IdPagoNavigation { get; set; } = null!;

    [ForeignKey("IdUsuario")]
    [InverseProperty("Facturas")]
    public virtual Cliente IdUsuarioNavigation { get; set; } = null!;
}
