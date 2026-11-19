using System.ComponentModel.DataAnnotations;

namespace ApiCycleOn.DTOs;

#region Requests

// ExportarFactura() - idFactura va en la ruta; la respuesta es un archivo, no JSON
public record ExportarFacturaRequest(
    [property: RegularExpression("^(PDF)$", ErrorMessage = "Formato no soportado.")] string Formato = "PDF");

// ConsultarFactura() usa la ruta; listados con PagedRequest

#endregion

#region Responses

public record GetFacturaResponse(
    int IdFactura,
    int IdUsuario,
    string NombreCliente,
    int IdPago,
    int IdPrestamo,
    DateTime FechaEmision,
    decimal MontoTotal,
    string CodigoFactura);

#endregion