using ApiCycleOn.Data;
using ApiCycleOn.Enums;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace ApiCycleOn.Common;

public static class SancionValidator
{
    public static int ExtraerDias(string? tipoPenalizacion)
    {
        if (string.IsNullOrWhiteSpace(tipoPenalizacion)) return 1;
        var match = Regex.Match(tipoPenalizacion, @"(\d+)\s*d[ií]a", RegexOptions.IgnoreCase);
        return match.Success && int.TryParse(match.Groups[1].Value, out var dias) && dias > 0
            ? dias
            : 1;
    }

    public static async Task VerificarYActualizarSancionesAsync(CycleOnContext context, int? idUsuario = null)
    {
        var query = context.Sancions
            .Where(s => s.Estado == EstadoSancionEnum.ACTIVA_TEMPORAL);

        if (idUsuario.HasValue)
            query = query.Where(s => s.IdUsuario == idUsuario.Value);

        var temporales = await query.ToListAsync();
        if (temporales.Count == 0) return;

        var ahora = DateTime.Now;
        var huboCambios = false;

        foreach (var s in temporales)
        {
            var dias = ExtraerDias(s.TipoDePenalizacion);
            if (ahora >= s.FechaHora.AddDays(dias))
            {
                s.Estado = EstadoSancionEnum.CUMPLIDA;
                huboCambios = true;
            }
        }

        if (huboCambios)
            await context.SaveChangesAsync();
    }

    public static async Task<string?> ValidarUsuarioHabilitadoAsync(CycleOnContext context, int idUsuario)
    {
        await VerificarYActualizarSancionesAsync(context, idUsuario);

        var usuario = await context.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);

        if (usuario is null)
            return "Usuario no encontrado.";

        if (string.Equals(usuario.Estado, "BLOQUEADO", StringComparison.OrdinalIgnoreCase))
            return "Tu cuenta se encuentra bloqueada. Comunícate con un administrador.";

        var sancionActiva = await context.Sancions
            .AsNoTracking()
            .Where(s => s.IdUsuario == idUsuario &&
                (s.Estado == EstadoSancionEnum.ACTIVA_TEMPORAL ||
                 s.Estado == EstadoSancionEnum.ACTIVA_PERMANENTE ||
                 s.Estado == EstadoSancionEnum.PENDIENTE_PAGO))
            .OrderByDescending(s => s.FechaHora)
            .FirstOrDefaultAsync();

        if (sancionActiva is null)
            return null;

        if (sancionActiva.Estado == EstadoSancionEnum.ACTIVA_TEMPORAL)
        {
            var dias = ExtraerDias(sancionActiva.TipoDePenalizacion);
            var fechaFin = sancionActiva.FechaHora.AddDays(dias);
            return $"Tienes una sanción activa hasta el {fechaFin:dd/MM/yyyy HH:mm}. Motivo: {sancionActiva.Motivo}.";
        }

        if (sancionActiva.Estado == EstadoSancionEnum.ACTIVA_PERMANENTE)
            return $"Tienes una sanción permanente activa. Motivo: {sancionActiva.Motivo}.";

        return $"Tienes una sanción pendiente de pago ({sancionActiva.Motivo}). Regularízala para usar el servicio.";
    }
}