using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCycleOn.Models;

[Table("Cliente")]
public partial class Cliente
{
    [Key]
    [Column("idUsuario")]
    public int IdUsuario { get; set; }

    [StringLength(120)]
    public string NombreContactoEmergencia { get; set; } = null!;

    [StringLength(20)]
    public string TelContactoEmergencia { get; set; } = null!;

    public int MaximoPrestamos { get; set; }

    [InverseProperty("IdUsuarioNavigation")]
    public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();

    [ForeignKey("IdUsuario")]
    [InverseProperty("Cliente")]
    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    [InverseProperty("IdUsuarioNavigation")]
    public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();

    [InverseProperty("IdUsuarioNavigation")]
    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
