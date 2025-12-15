namespace appCoreAPI.DTO
{
    public class CambiarClaveDTO
    {
        public string NuevaClave { get; set; }
        public string ConfirmarClave { get; set; }
    }
    public class AuthResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public ClienteDTO Cliente { get; set; }
        public bool DebeReestablecerClave { get; set; } // Campo importante
        public string Token { get; set; } // Para JWT si lo implementas
    }
}
