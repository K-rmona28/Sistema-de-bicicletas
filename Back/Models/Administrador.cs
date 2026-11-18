using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCycleOn.Models;

[Table("Administrador")]
public partial class Administrador
{
    [Key]
    [Column("idUsuario")]
    public int IdUsuario { get; set; }

    [StringLength(80)]
    public string Cargo { get; set; } = null!;

    [ForeignKey("IdUsuario")]
    [InverseProperty("Administrador")]
    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    [InverseProperty("IdUsuarioNavigation")]
    public virtual ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
}
