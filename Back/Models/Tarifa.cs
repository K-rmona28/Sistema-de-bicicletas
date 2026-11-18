using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCycleOn.Models;

[Table("Tarifa")]
public partial class Tarifa
{
    [Key]
    [Column("idTarifa")]
    public int IdTarifa { get; set; }

    [Column(TypeName = "decimal(12, 2)")]
    public decimal ValorHora { get; set; }

    [StringLength(20)]
    public string Estado { get; set; } = null!;

    [Precision(0)]
    public DateTime VigenciaInicio { get; set; }

    [Precision(0)]
    public DateTime? VigenciaFin { get; set; }

    [InverseProperty("IdTarifaNavigation")]
    public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
}
