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
            services.AddScoped(c =>
            {
                var host = Environment.GetEnvironmentVariable("DB_HOST");
                var port = Environment.GetEnvironmentVariable("DB_PORT");
                var databaseName = Environment.GetEnvironmentVariable("DB_NAME");
                var user = Environment.GetEnvironmentVariable("DB_USER");
                var password = Environment.GetEnvironmentVariable("DB_PASSWD");

                var connectionString = $"Data Source={host},{port};Initial Catalog={databaseName};Persist Security Info=True;User Id={user};Password={password};MultipleActiveResultSets=True;App=MyFunc01;";
                return new SqlConnection(Environment.GetEnvironmentVariable(connectionString));
            });
        }


    }
}
