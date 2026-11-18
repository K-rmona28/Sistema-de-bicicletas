using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiCycleOn.Models;

[Table("NotificacionWhatsApp")]
[Index("IdUsuario", Name = "IX_NotWa_Usuario")]
public partial class NotificacionWhatsApp
{
    [Key]
    [Column("idNotificacion")]
    public int IdNotificacion { get; set; }

    [Column("idUsuario")]
    public int IdUsuario { get; set; }

    [StringLength(1000)]
    public string Mensaje { get; set; } = null!;

    [Precision(0)]
    public DateTime FechaHora { get; set; }

    [StringLength(20)]
    public string Estado { get; set; } = null!;

    [ForeignKey("IdUsuario")]
    [InverseProperty("NotificacionWhatsApps")]
    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
