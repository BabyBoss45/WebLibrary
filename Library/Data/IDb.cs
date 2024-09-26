using Dapper;
using System.Data;
using System.Xml.Linq;

namespace Library.Data
{
    public enum RDBMS
    {
        NONE = 0,
        FB = 1,
        ORA = 2,
        MS = 3,
        PG = 4,
        ORA19 = 5
    }

    public interface IDb
    {
        RDBMS DbKind { get; }
        bool IsDbOracle => DbKind == RDBMS.ORA || DbKind == RDBMS.ORA19;
        Task<IDbConnection> ConnectAsync(Enum name = null);
        Task<IDbConnection> ConnectAsync(CancellationToken cancellationToken, Enum name = null);
        Task<IDbConnection> ConnectAsync(string name);
        Task<IDbConnection> ConnectAsync(CancellationToken cancellationToken, string name);
    }

    public static class DbExtension
    {
        public static string Param(this IDb db, string name)
        {
            return (db.DbKind == RDBMS.MS ? "@" : ":") + name;
        }

        public static string Returning(this IDb db, string param)
        {
            if (db.DbKind == RDBMS.MS)
            {
                return $"; SET {db.Param(param)} = SCOPE_IDENTITY();";
            }
            else
            {
                return $" RETURNING {param}" + (db.IsDbOracle ? $" INTO {db.Param(param)}" : "");
            }
        }
    }
}
