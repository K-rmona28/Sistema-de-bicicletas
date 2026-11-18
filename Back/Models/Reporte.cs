using System;
using System.Collections.Generic;
using ApiCycleOn.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCycleOn.Models;

[Table("Reporte")]
[Index("IdBicicleta", Name = "IX_Reporte_Bicicleta")]
[Index("IdUsuario", Name = "IX_Reporte_Usuario")]
public partial class Reporte
{
    [Key]
    [Column("idReporte")]
    public int IdReporte { get; set; }

    [StringLength(50)]
    public string Tipo { get; set; } = null!;

    [StringLength(500)]
    public string Descripcion { get; set; } = null!;

    [Precision(0)]
    public DateTime FechaHora { get; set; }

    [Column("idEstacion")]
    public int? IdEstacion { get; set; }

    [Column("idUsuario")]
    public int IdUsuario { get; set; }

    [Column("idBicicleta")]
    public int? IdBicicleta { get; set; }

    [StringLength(20)]
    public EstadoReporteEnum Estado { get; set; } = EstadoReporteEnum.REGISTRADO;

    [ForeignKey("IdBicicleta")]
    [InverseProperty("Reportes")]
    public virtual Bicicletum? IdBicicletaNavigation { get; set; }

    [ForeignKey("IdEstacion")]
    [InverseProperty("Reportes")]
    public virtual Estacion? IdEstacionNavigation { get; set; }

    [ForeignKey("IdUsuario")]
    [InverseProperty("Reportes")]
    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    [InverseProperty("IdReporteNavigation")]
    public virtual ICollection<Sancion> Sancions { get; set; } = new List<Sancion>();
}
