using Npgsql;
namespace TemplateAPI.Classes;

public class DatabaseConnector {
    private string _connectionString;
    private int _maxRetries = 5;
    private int _retryDelayMs = 1000;

    public DatabaseConnector(string connectionString) {
        _connectionString = connectionString;
    }

    public NpgsqlConnection ConnectWithRetry() {
        int retryCount = 0;
        while (retryCount < _maxRetries) {
            try {
                var connection = new NpgsqlConnection(_connectionString);
                connection.Open();
                return connection;
            } catch (NpgsqlException ex) when (ex.InnerException is System.Net.Sockets.SocketException) {
                retryCount++;
                Console.WriteLine($"Connection failed, retrying... ({retryCount}/{_maxRetries})");
                Thread.Sleep(_retryDelayMs);
            }
        }
        throw new Exception("Failed to connect to the database after multiple retries.");
    }
}
