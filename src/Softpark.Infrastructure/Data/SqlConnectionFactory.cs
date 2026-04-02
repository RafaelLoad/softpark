using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Softpark.Infrastructure.Data;

public class SqlConnectionFactory(IOptions<ConnectionStrings> settings) : IDbConnectionFactory
{
    private readonly string _connectionString = settings.Value.DefaultConnection;

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
