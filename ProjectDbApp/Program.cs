using System;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;


class Program
{
    static void Main(string[] args)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        IConfigurationRoot configuration = builder.Build();
        string connectionString = configuration.GetConnectionString("SourceDatabase");

        SalesApplication salesApp = new SalesApplication(connectionString);
        salesApp.Run();
    }
}