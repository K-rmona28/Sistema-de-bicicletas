using ApiCycleOn.Enums;
using ApiCycleOn.Patterns.State;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCycleOn.Models;

[Table("Reserva")]
[Index("IdBicicleta", Name = "IX_Reserva_Bicicleta")]
[Index("IdEstacion", Name = "IX_Reserva_Estacion")]
[Index("IdUsuario", Name = "IX_Reserva_Usuario")]
public partial class Reserva
{
    [Key]
    [Column("idReserva")]
    public int IdReserva { get; set; }

    [Column("idUsuario")]
    public int IdUsuario { get; set; }

    [Column("idEstacion")]
    public int IdEstacion { get; set; }

    [Column("idBicicleta")]
    public int IdBicicleta { get; set; }

    [Precision(0)]
    public DateTime FechaHoraReserva { get; set; }

    [StringLength(20)]
    public EstadoReservaEnum Estado { get; set; } = EstadoReservaEnum.VIGENTE;

    [ForeignKey("IdBicicleta")]
    [InverseProperty("Reservas")]
    public virtual Bicicletum IdBicicletaNavigation { get; set; } = null!;

    [ForeignKey("IdEstacion")]
    [InverseProperty("Reservas")]
    public virtual Estacion IdEstacionNavigation { get; set; } = null!;

    [ForeignKey("IdUsuario")]
    [InverseProperty("Reservas")]
    public virtual Cliente IdUsuarioNavigation { get; set; } = null!;

    [InverseProperty("IdReservaNavigation")]
    public virtual Prestamo? Prestamo { get; set; }
}
