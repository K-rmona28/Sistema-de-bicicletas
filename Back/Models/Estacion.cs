using System;
using System.Collections.Generic;
using ApiCycleOn.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCycleOn.Models;

[Table("Estacion")]
public partial class Estacion
{
    [Key]
    [Column("idEstacion")]
    public int IdEstacion { get; set; }

    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [StringLength(200)]
    public string Ubicacion { get; set; } = null!;

    public int Capacidad { get; set; }

    [StringLength(20)]
    public EstadoEstacionEnum Estado { get; set; } = EstadoEstacionEnum.OPERATIVA;

    [InverseProperty("IdEstacionNavigation")]
    public virtual ICollection<Bicicletum> Bicicleta { get; set; } = new List<Bicicletum>();

    [InverseProperty("IdEstacionDestinoNavigation")]
    public virtual ICollection<Movimiento> MovimientoIdEstacionDestinoNavigations { get; set; } = new List<Movimiento>();

    [InverseProperty("IdEstacionOrigenNavigation")]
    public virtual ICollection<Movimiento> MovimientoIdEstacionOrigenNavigations { get; set; } = new List<Movimiento>();

    [InverseProperty("EstacionDestinoNavigation")]
    public virtual ICollection<Prestamo> PrestamoEstacionDestinoNavigations { get; set; } = new List<Prestamo>();

    [InverseProperty("EstacionOrigenNavigation")]
    public virtual ICollection<Prestamo> PrestamoEstacionOrigenNavigations { get; set; } = new List<Prestamo>();

    [InverseProperty("IdEstacionNavigation")]
    public virtual ICollection<Reporte> Reportes { get; set; } = new List<Reporte>();

    [InverseProperty("IdEstacionNavigation")]
    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
