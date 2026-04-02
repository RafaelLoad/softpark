using System.Data;
using Microsoft.Data.Sqlite;

namespace Softpark.Infrastructure.Data;

public class SqliteConnectionFactory : IDbConnectionFactory, IDisposable
{
    private readonly SqliteConnection _keepAliveConnection;

    public SqliteConnectionFactory()
    {
        _keepAliveConnection = new SqliteConnection("Data Source=SoftparkDev;Mode=Memory;Cache=Shared");
        _keepAliveConnection.Open();
    }

    public IDbConnection CreateConnection()
    {
        return new SqliteConnection("Data Source=SoftparkDev;Mode=Memory;Cache=Shared");
    }

    public void Dispose()
    {
        _keepAliveConnection.Dispose();
    }
}
