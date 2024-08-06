using Npgsql;

namespace manto_stock_system_API.Utils
{

    public static class DbConnectionUtils
    {
        public static string BuildPostgresConnectionString(this string url)
        {
            var databaseUri = new Uri(url);
            var userInfo = databaseUri.UserInfo.Split(':');

            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
            };

            return builder.ToString();
        }
    }
}
