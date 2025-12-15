namespace appCoreAPI.DTO
{
    public class ClienteDTO
    {
        public int IdCliente { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Correo { get; set; }
    }
    public class ClienteRegistroDto
    {
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Correo { get; set; }
    }

    public class ClienteLoginDto
    {
        public string Correo { get; set; }
        public string Clave { get; set; }
    }
}
