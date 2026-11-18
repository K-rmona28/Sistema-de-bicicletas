using ApiCycleOn.Enums;
using ApiCycleOn.Patterns.State;
using Cycleon.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCycleOn.Models;

[Table("Mantenimiento")]
[Index("IdBicicleta", Name = "IX_Mantenimiento_Bicicleta")]
public partial class Mantenimiento
{
    [Key]
    [Column("idMantenimiento")]
    public int IdMantenimiento { get; set; }

    [Column("idBicicleta")]
    public int IdBicicleta { get; set; }

    [StringLength(50)]
    public string Tipo { get; set; } = null!;

    [Precision(0)]
    public DateTime FechaProgramada { get; set; }

    [Precision(0)]
    public DateTime? FechaHoraInicio { get; set; }

    [Precision(0)]
    public DateTime? FechaFin { get; set; }

    [StringLength(500)]
    public string? Descripcion { get; set; }

    [StringLength(20)]
    public EstadoMantenimientoEnum Estado { get; set; } = EstadoMantenimientoEnum.PROGRAMADO;

    [ForeignKey("IdBicicleta")]
    [InverseProperty("Mantenimientos")]
    public virtual Bicicletum IdBicicletaNavigation { get; set; } = null!;
}
