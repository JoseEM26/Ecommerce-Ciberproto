namespace appCoreAPI.Models
{
    public class Producto
    {
        public int IdProducto { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }

        public int IdMarca { get; set; }
        public int IdCategoria { get; set; }

        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string? UrlImagen { get; set; }
        public bool Activo { get; set; }

        public string? NombreMarca { get; set; }
        public string? NombreCategoria { get; set; }
    }
}
