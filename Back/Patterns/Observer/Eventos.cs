namespace ApiCycleOn.Patterns.Observer;

// Eventos de dominio: "algo ocurrió". Quien lo origina no sabe quién reacciona.
public interface IEventoDominio { }

public sealed record UsuarioRegistradoEvento(int IdUsuario, string Nombre, string Rol) : IEventoDominio;
public sealed record UsuarioBloqueadoEvento(int IdUsuario, string Motivo) : IEventoDominio;
public sealed record RestablecimientoSolicitadoEvento(int IdUsuario, string Token) : IEventoDominio;
public sealed record AccesoRestablecidoEvento(int IdUsuario) : IEventoDominio;
// En los siguientes bloques se agregan: PrestamoFinalizadoEvento, PagoCompletadoEvento,
// SancionAplicadaEvento, ReporteRegistradoEvento, MantenimientoFinalizadoEvento...
