using MercadoVerde.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MercadoVerde.Application.Services;

public class ReporteService
{
    private readonly ITiendaDbContext _db;

    public ReporteService(ITiendaDbContext db)
    {
        _db = db;
    }

    public class FilaReporte
    {
        public int PedidoId { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public int CantidadArticulos { get; set; }
        public decimal Total { get; set; }
    }

    // Genera el reporte de ventas de un rango de fechas.
    // En producción la tabla Pedidos tiene cientos de miles de filas.
    public List<FilaReporte> GenerarReporteVentas(DateTime desdeUtc, DateTime hastaUtc)
    {
        return _db.Pedidos //TICK-204 - DANIEL PEÑA
        .AsNoTracking()
        .Where(p => p.FechaUtc >= desdeUtc && p.FechaUtc <= hastaUtc)
        .Select(pedido => new FilaReporte
        {
            PedidoId = pedido.Id,
            Cliente = _db.Clientes
                .Where(c => c.Id == pedido.ClienteId)
                .Select(c => c.Nombre)
                .FirstOrDefault() ?? "(desconocido)",
            CantidadArticulos = _db.LineasPedido
                .Where(l => l.PedidoId == pedido.Id)
                .Sum(l => l.Cantidad),
            Total = pedido.Total
        })
        .ToList();
    }
}
