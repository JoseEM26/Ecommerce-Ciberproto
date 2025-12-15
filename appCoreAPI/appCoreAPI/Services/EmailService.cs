using appCoreAPI.Interface;
using System.Net;
using System.Net.Mail;

namespace appCoreAPI.Services
{
    public class EmailService : IEmailService
    {

        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> EnviarCorreoAsync(string destinatario, string asunto, string cuerpo)
        {
            try
            {
                var smtpHost = _configuration["Email:SmtpHost"];
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"]);
                var smtpUser = _configuration["Email:SmtpUser"];
                var smtpPass = _configuration["Email:SmtpPass"];
                var fromEmail = _configuration["Email:FromEmail"];
                var fromName = _configuration["Email:FromName"];

                using (var client = new SmtpClient(smtpHost, smtpPort))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(smtpUser, smtpPass);

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(fromEmail, fromName),
                        Subject = asunto,
                        Body = cuerpo,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(destinatario);

                    await client.SendMailAsync(mailMessage);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar correo: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> EnviarContrasenaTemporalAsync(string destinatario, string nombreCompleto, string contrasenaTemporal)
        {
            var asunto = "Contraseña Temporal - E-Commerce";
            var cuerpo = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; }}
                        .content {{ padding: 20px; background-color: #f9f9f9; }}
                        .password-box {{ 
                            background-color: #fff; 
                            border: 2px solid #4CAF50; 
                            padding: 15px; 
                            margin: 20px 0;
                            text-align: center;
                            font-size: 24px;
                            font-weight: bold;
                            letter-spacing: 2px;
                        }}
                        .warning {{ color: #d32f2f; font-weight: bold; }}
                        .footer {{ text-align: center; padding: 20px; color: #666; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>Bienvenido a E-Commerce</h1>
                        </div>
                        <div class='content'>
                            <h2>Hola {nombreCompleto},</h2>
                            <p>Tu cuenta ha sido creada exitosamente. Esta es tu contraseña temporal:</p>
                            
                            <div class='password-box'>
                                {contrasenaTemporal}
                            </div>
                            
                            <p class='warning'>⚠️ IMPORTANTE:</p>
                            <ul>
                                <li>Debes cambiar esta contraseña en tu primer inicio de sesión</li>
                                <li>No compartas esta contraseña con nadie</li>
                                <li>Por seguridad, esta contraseña es de un solo uso</li>
                            </ul>
                            
                            <p>Para iniciar sesión, visita nuestro sitio web y usa tu correo electrónico junto con esta contraseña temporal.</p>
                        </div>
                        <div class='footer'>
                            <p>© 2024 E-Commerce. Todos los derechos reservados.</p>
                            <p>Si no solicitaste esta cuenta, por favor ignora este correo.</p>
                        </div>
                    </div>
                </body>
                </html>
            ";

            return await EnviarCorreoAsync(destinatario, asunto, cuerpo);      
        }
    }
}
