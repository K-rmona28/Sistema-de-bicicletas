using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCycleOn.Models;

[Table("Usuario")]
[Index("Email", Name = "UQ_Usuario_Email", IsUnique = true)]
public partial class Usuario
{
    [Key]
    [Column("idUsuario")]
    public int IdUsuario { get; set; }

    [StringLength(80)]
    public string Nombre { get; set; } = null!;

    [StringLength(80)]
    public string Apellido { get; set; } = null!;

    [StringLength(150)]
    public string Email { get; set; } = null!;

    [StringLength(255)]
    public string Pass { get; set; } = null!;

    [StringLength(20)]
    public string Estado { get; set; } = null!;

    [StringLength(20)]
    public string? Telefono { get; set; }

    public DateOnly FechaDeNacimiento { get; set; }

    public int? Edad { get; set; }

    [Precision(0)]
    public DateTime FechaDeRegistro { get; set; }

    public int IntentosFallidos { get; set; }

    [InverseProperty("IdUsuarioNavigation")]
    public virtual Administrador? Administrador { get; set; }

    [InverseProperty("IdUsuarioNavigation")]
    public virtual Cliente? Cliente { get; set; }

    [InverseProperty("IdUsuarioNavigation")]
    public virtual ICollection<NotificacionEmail> NotificacionEmails { get; set; } = new List<NotificacionEmail>();

    [InverseProperty("IdUsuarioNavigation")]
    public virtual ICollection<NotificacionWhatsApp> NotificacionWhatsApps { get; set; } = new List<NotificacionWhatsApp>();

    [InverseProperty("IdUsuarioNavigation")]
    public virtual ICollection<Reporte> Reportes { get; set; } = new List<Reporte>();

    [InverseProperty("IdUsuarioNavigation")]
    public virtual ICollection<Sancion> Sancions { get; set; } = new List<Sancion>();
}
