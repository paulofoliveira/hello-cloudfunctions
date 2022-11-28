using Google.Cloud.Functions.Framework;
using Google.Cloud.Functions.Hosting;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Data.SqlClient;

namespace MyFunc01
{
    [FunctionsStartup(typeof(Startup))]
    public class Function : IHttpFunction
    {
        private readonly SqlConnection _conn;

        public Function(SqlConnection conn)
        {
            _conn = conn;
        }

        public async Task HandleAsync(HttpContext context)
        {
            var payload = await context.Request.ReadFromJsonAsync<WebhookResponse>();

            if (payload == null)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Payload não recuperado");
                return;
            }

            var validationToken = Environment.GetEnvironmentVariable("VALIDATION_TOKEN");

            if (!payload.ValidationToken.Equals(validationToken))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Token inválido");
                return;
            }

            if (_conn.State != ConnectionState.Open)
                await _conn.OpenAsync();

            using SqlCommand cmd = _conn.CreateCommand();

            cmd.CommandText = "SELECT @@VERSION;";

            var result = cmd.ExecuteScalarAsync();

            if (result == null)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Resultado não encontrado");
                return;
            }

            await context.Response.WriteAsync(result.ToString());
        }
    }
}