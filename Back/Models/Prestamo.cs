using System;
using System.Collections.Generic;
using ApiCycleOn.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCycleOn.Models;

[Table("Prestamo")]
[Index("IdTarifa", Name = "IX_Prestamo_Tarifa")]
public partial class Prestamo
{
    [Key]
    [Column("idPrestamo")]
    public int IdPrestamo { get; set; }

    [Column("idUsuario")]
    public int IdUsuario { get; set; }

    [Column("idReserva")]
    public int? IdReserva { get; set; }

    [Column("idTarifa")]
    public int IdTarifa { get; set; }

    [Column("idBicicleta")]
    public int IdBicicleta { get; set; }

    [Precision(0)]
    public DateTime FechaHoraInicio { get; set; }

    [Precision(0)]
    public DateTime? FechaHoraFin { get; set; }

    [Column(TypeName = "decimal(12, 2)")]
    public decimal? CostoTotal { get; set; }

    [StringLength(20)]
    public EstadoPrestamoEnum Estado { get; set; } = EstadoPrestamoEnum.ACEPTADO;

    public int? TiempoUso { get; set; }

    public int EstacionOrigen { get; set; }

    public int? EstacionDestino { get; set; }

    [ForeignKey("EstacionDestino")]
    [InverseProperty("PrestamoEstacionDestinoNavigations")]
    public virtual Estacion? EstacionDestinoNavigation { get; set; }

    [ForeignKey("EstacionOrigen")]
    [InverseProperty("PrestamoEstacionOrigenNavigations")]
    public virtual Estacion EstacionOrigenNavigation { get; set; } = null!;

    [ForeignKey("IdBicicleta")]
    public virtual Bicicletum IdBicicletaNavigation { get; set; } = null!;

    [ForeignKey("IdReserva")]
    [InverseProperty("Prestamo")]
    public virtual Reserva? IdReservaNavigation { get; set; }

    [ForeignKey("IdTarifa")]
    [InverseProperty("Prestamos")]
    public virtual Tarifa IdTarifaNavigation { get; set; } = null!;

    [ForeignKey("IdUsuario")]
    public virtual Cliente IdUsuarioNavigation { get; set; } = null!;

    [InverseProperty("IdPrestamoNavigation")]
    public virtual Pago? Pago { get; set; }

    [InverseProperty("IdPrestamoNavigation")]
    public virtual ICollection<Sancion> Sancions { get; set; } = new List<Sancion>();

    [InverseProperty("IdPrestamoNavigation")]
    public virtual ICollection<Seguimiento> Seguimientos { get; set; } = new List<Seguimiento>();
}
