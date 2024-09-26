using Microsoft.Data.SqlClient;
using System.Data;

namespace Library.Data
{
    /// <summary>
    /// Inereface of Db
    /// </summary>
    public class MsDb : IDb
    {
        private Dictionary<string, string> _connectionStrings { get; set; }

        public MsDb(IConfiguration config)
        {
            _connectionStrings = new Dictionary<string, string>();

            foreach (DbConnectionName item in Enum.GetValues(typeof(DbConnectionName)))
            {
                string connection = config.GetConnectionString(item.ToString());
                if (!string.IsNullOrEmpty(connection))
                {
                    SqlConnectionStringBuilder ms = new SqlConnectionStringBuilder(connection);
                    _connectionStrings.Add(item.ToString(), ms.ConnectionString);
                }
            }
        }

        public RDBMS DbKind => RDBMS.MS;

        public async Task<IDbConnection> ConnectAsync(Enum name = null)
        {
            var c = new SqlConnection(_connectionStrings.GetValueOrDefault((name ?? DbConnectionName.DefaultConnection).ToString()));

            await c.OpenAsync();

            return c;
        }

        public async Task<IDbConnection> ConnectAsync(CancellationToken cancellationToken, Enum name = null)
        {
            var c = new SqlConnection(_connectionStrings.GetValueOrDefault((name ?? DbConnectionName.DefaultConnection).ToString()));

            await c.OpenAsync(cancellationToken);

            return c;
        }

        public async Task<IDbConnection> ConnectAsync(string name)
        {
            var c = new SqlConnection(_connectionStrings.GetValueOrDefault(!string.IsNullOrEmpty(name) ? name : DbConnectionName.DefaultConnection.ToString()));

            await c.OpenAsync();

            return c;
        }

        public async Task<IDbConnection> ConnectAsync(CancellationToken cancellationToken, string name)
        {
            var c = new SqlConnection(_connectionStrings.GetValueOrDefault(!string.IsNullOrEmpty(name) ? name : DbConnectionName.DefaultConnection.ToString()));

            await c.OpenAsync(cancellationToken);

            return c;
        }
    }
}
