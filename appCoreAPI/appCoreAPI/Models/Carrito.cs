namespace appCoreAPI.Models
{
    public class Carrito
    {
        public int IdCarrito { get; set; }
        public int IdUsuario { get; set; }
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }


        /// JOINS

        public string? NombreProducto { get; set; }
        public string DescripcionProducto { get; set; }
        public decimal? PrecioProducto { get; set; }
        public int Stock { get; set; }
        public string? ImagenProducto { get; set; }
        public string? NombreMarca { get; set; }
        public string? NombreCategoria { get; set; }
        public bool? ActivoProducto { get; set; }

    }
}
