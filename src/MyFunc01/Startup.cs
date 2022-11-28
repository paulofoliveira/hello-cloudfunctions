using Google.Cloud.Functions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Data.SqlClient;

namespace MyFunc01
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
        {
            services.AddScoped(c => new SqlConnection(Environment.GetEnvironmentVariable("CONNECTION_STRING")));
        }
    }
}
