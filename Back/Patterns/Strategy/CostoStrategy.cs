using ApiCycleOn.Exceptions;

namespace ApiCycleOn.Patterns.Strategy;

// PATRÓN STRATEGY (1/2): algoritmo de cálculo del costo del préstamo, intercambiable sin tocar el flujo.
public interface ICostoStrategy
{
    string Nombre { get; }
    decimal Calcular(decimal valorHora, int minutos);
}

// Se cobra exactamente lo usado: valorHora/60 por minuto
public sealed class CostoProporcionalStrategy : ICostoStrategy
{
    public string Nombre => "Proporcional";

    public decimal Calcular(decimal valorHora, int minutos) =>
        minutos <= 0 ? 0m : Math.Round(valorHora * minutos / 60m, 2, MidpointRounding.AwayFromZero);
}

// Toda hora iniciada se cobra completa
public sealed class CostoHoraCompletaStrategy : ICostoStrategy
{
    public string Nombre => "HoraCompleta";

    public decimal Calcular(decimal valorHora, int minutos) =>
        minutos <= 0 ? 0m : (decimal)Math.Ceiling(minutos / 60m) * valorHora;
}

// Se cobra por bloques de 15 minutos
public sealed class CostoFraccion15Strategy : ICostoStrategy
{
    public string Nombre => "Fraccion15";

    public decimal Calcular(decimal valorHora, int minutos) =>
        minutos <= 0 ? 0m : Math.Round((decimal)Math.Ceiling(minutos / 15m) * valorHora / 4m, 2, MidpointRounding.AwayFromZero);
}

// Elige la estrategia por nombre o por configuración (Tarifas:Estrategia en appsettings)
public sealed class CostoStrategyResolver(IEnumerable<ICostoStrategy> estrategias, IConfiguration config)
{
    private readonly Dictionary<string, ICostoStrategy> _mapa =
        estrategias.ToDictionary(e => e.Nombre, StringComparer.OrdinalIgnoreCase);

    public ICostoStrategy Obtener(string? nombre = null)
    {
        nombre ??= config["Tarifas:Estrategia"] ?? "Proporcional";
        return _mapa.TryGetValue(nombre, out var estrategia)
            ? estrategia
            : throw new ReglaNegocioException($"Estrategia de costo desconocida: {nombre}");
    }
}
