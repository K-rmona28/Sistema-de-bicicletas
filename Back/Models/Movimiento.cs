using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCycleOn.Models;

[Table("Movimiento")]
public partial class Movimiento
{
    [Key]
    [Column("idMovimiento")]
    public int IdMovimiento { get; set; }

    [Column("idUsuario")]
    public int IdUsuario { get; set; }

    [Column("idEstacionOrigen")]
    public int IdEstacionOrigen { get; set; }

    [Column("idEstacionDestino")]
    public int IdEstacionDestino { get; set; }

    [Precision(0)]
    public DateTime FechaHora { get; set; }

    [StringLength(250)]
    public string Motivo { get; set; } = null!;

    [ForeignKey("IdEstacionDestino")]
    [InverseProperty("MovimientoIdEstacionDestinoNavigations")]
    public virtual Estacion IdEstacionDestinoNavigation { get; set; } = null!;

    [ForeignKey("IdEstacionOrigen")]
    [InverseProperty("MovimientoIdEstacionOrigenNavigations")]
    public virtual Estacion IdEstacionOrigenNavigation { get; set; } = null!;

    [ForeignKey("IdUsuario")]
    [InverseProperty("Movimientos")]
    public virtual Administrador IdUsuarioNavigation { get; set; } = null!;

    [ForeignKey("IdMovimiento")]
    [InverseProperty("IdMovimientos")]
    public virtual ICollection<Bicicletum> IdBicicleta { get; set; } = new List<Bicicletum>();
}
