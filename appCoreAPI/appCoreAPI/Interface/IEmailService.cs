namespace appCoreAPI.Interface
{
    public interface IEmailService
    {
        Task<bool> EnviarCorreoAsync(string destinatario, string asunto, string cuerpo);
        Task<bool> EnviarContrasenaTemporalAsync(string destinatario, string nombreCompleto, string contrasenaTemporal);
    }
}
