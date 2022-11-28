using Google.Cloud.Functions.Framework;
using Google.Cloud.Functions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.SqlClient;

namespace MyFunc01
{
    [FunctionsStartup(typeof(Startup))]
    public class Function : IHttpFunction
    {
        private readonly SqlConnection _conn;
        private readonly ILogger<Function> _logger;

        public Function(SqlConnection conn, ILogger<Function> logger)
        {
            _conn = conn;
            _logger = logger;
        }

        public async Task HandleAsync(HttpContext context)
        {
            _logger.LogInformation("Iniciando execução da função...");
            var payload = await context.Request.ReadFromJsonAsync<WebhookResponse>();

            if (payload == null)
            {
                _logger.LogInformation("Payload não recuperado");
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Payload não recuperado");
                return;
            }

            var validationToken = Environment.GetEnvironmentVariable("VALIDATION_TOKEN");

            if (string.IsNullOrEmpty(payload.ValidationToken) || !payload.ValidationToken.Equals(validationToken))
            {
                _logger.LogInformation("Validation Token não validado");
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Token inválido");
                return;
            }
            else
                _logger.LogInformation("Token validado");

            try
            {
                if (_conn.State != ConnectionState.Open)
                    await _conn.OpenAsync();

                using SqlCommand cmd = _conn.CreateCommand();

                cmd.CommandText = "SELECT @@VERSION;";

                var result = await cmd.ExecuteScalarAsync();

                if (result == null)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("Resultado não encontrado");
                    return;
                }

                await context.Response.WriteAsync(result.ToString());
                _logger.LogInformation("Resposta Ok!");
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Falha: {ex.Message}");
                throw;
            }
        }
    }
}