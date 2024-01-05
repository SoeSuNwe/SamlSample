using Microsoft.Data.SqlClient;
namespace SamlSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Build the configuration
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")  // Make sure this file exists in your project
                .Build();

            // Check SQL connection before building and running the host
            CheckSqlConnection(configuration);
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>(); // Use the Startup class here
                });
        private static void CheckSqlConnection(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection"); // Replace with your actual database connection string

            using SqlConnection connection = new(connectionString);
            try
            {
                // Open the connection
                connection.Open();

                // Check the connection state
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    Console.WriteLine("SQL Connection is open and successful.");
                }
                else
                {
                    Console.WriteLine("SQL Connection is not open.");
                }

                // Close the connection (this will be done automatically when exiting the using block)
                // connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}