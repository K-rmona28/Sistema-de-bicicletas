using ApiCycleOn.Enums;
using ApiCycleOn.Patterns.State;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCycleOn.Models;

[Table("Sancion")]
[Index("IdUsuario", Name = "IX_Sancion_Usuario")]
public partial class Sancion
{
    [Key]
    [Column("idSancion")]
    public int IdSancion { get; set; }

    [Column("idUsuario")]
    public int IdUsuario { get; set; }

    [Column("idPrestamo")]
    public int? IdPrestamo { get; set; }

    [Column("idReporte")]
    public int? IdReporte { get; set; }

    [StringLength(200)]
    public string Motivo { get; set; } = null!;

    [Precision(0)]
    public DateTime FechaHora { get; set; }

    [StringLength(20)]
    public EstadoSancionEnum Estado { get; set; } = EstadoSancionEnum.ACTIVA_TEMPORAL;

    [StringLength(500)]
    public string? Descripcion { get; set; }

    [StringLength(50)]
    public string TipoDePenalizacion { get; set; } = null!;

    [ForeignKey("IdPrestamo")]
    [InverseProperty("Sancions")]
    public virtual Prestamo? IdPrestamoNavigation { get; set; }

    [ForeignKey("IdReporte")]
    [InverseProperty("Sancions")]
    public virtual Reporte? IdReporteNavigation { get; set; }

    [ForeignKey("IdUsuario")]
    [InverseProperty("Sancions")]
    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
