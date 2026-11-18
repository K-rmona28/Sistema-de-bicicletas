using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCycleOn.Models;

[Table("Seguimiento")]
[Index("IdPrestamo", "FechaHora", Name = "IX_Seguimiento_Prestamo")]
public partial class Seguimiento
{
    [Key]
    [Column("idSeguimiento")]
    public int IdSeguimiento { get; set; }

    [Column("idPrestamo")]
    public int IdPrestamo { get; set; }

    [Column(TypeName = "decimal(9, 6)")]
    public decimal Latitud { get; set; }

    [Column(TypeName = "decimal(9, 6)")]
    public decimal Longitud { get; set; }

    [Precision(0)]
    public DateTime FechaHora { get; set; }

    [ForeignKey("IdPrestamo")]
    [InverseProperty("Seguimientos")]
    public virtual Prestamo IdPrestamoNavigation { get; set; } = null!;
}
