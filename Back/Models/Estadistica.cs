using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCycleOn.Models;

[Table("Estadistica")]
public partial class Estadistica
{
    [Key]
    [Column("idEstadistica")]
    public int IdEstadistica { get; set; }

    public int Historial { get; set; }

    public int TiempoDeUso { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal DistanciaRecorrida { get; set; }

    public int Estaciones { get; set; }
}
