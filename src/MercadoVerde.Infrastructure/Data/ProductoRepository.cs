using System.Linq;
using Microsoft.EntityFrameworkCore;
using MercadoVerde.Application.Abstractions;
using MercadoVerde.Domain;

namespace MercadoVerde.Infrastructure.Data;

public class ProductoRepository : IProductoRepository
{
    private readonly TiendaDbContext _db;

    public ProductoRepository(TiendaDbContext db)
    {
        _db = db;
    }

    // Búsqueda de productos por nombre para el catálogo público.
    // El término de búsqueda llega directamente desde la query string del usuario.
    public List<Producto> BuscarPorNombre(string termino)
    {
        if(string.IsNullOrWhiteSpace(termino))
            return new List<Producto>();
        // Se arma la consulta SQL concatenando el texto recibido del usuario.
        // (Búsqueda sin distinguir mayúsculas/minúsculas, como el catálogo público.)
        return _db.Productos.AsNoTracking().Where(p => p.Activo && p.Nombre.ToLower().Contains(termino.ToLower())).ToList(); //TICK-206 - DANIEL PEÑA
    }

    public Producto? ObtenerPorId(int id) => _db.Productos.FirstOrDefault(p => p.Id == id);
}
