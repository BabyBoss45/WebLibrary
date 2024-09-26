using System.Data;
using Dapper;
using Library.Data;
using Microsoft.AspNetCore.Identity;

namespace Library.Services.Identity
{
    public class AppRoleStore : IRoleStore<AppRole>
    {
        private readonly IDb _db;

        public AppRoleStore(IDb db)
        {
            _db = db;
        }

        public async Task<IdentityResult> CreateAsync(AppRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var con = await _db.ConnectAsync())
            {
                role.NormalizedName = role.Name.ToUpper();

                var param = new DynamicParameters(role);

                param.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await con.ExecuteAsync($@"INSERT INTO AppRoles (Name, NormalizedName, ConcurrencyStamp)
                    VALUES ({_db.Param(nameof(AppRole.Name))}, {_db.Param(nameof(AppRole.NormalizedName))}, {_db.Param(nameof(AppRole.ConcurrencyStamp))})
                    {_db.Returning(nameof(AppRole.Id))}", param);

                role.Id = param.Get<int>("Id");
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(AppRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var con = await _db.ConnectAsync(cancellationToken))
            {
                role.NormalizedName = role.Name.ToUpper();

                await con.ExecuteAsync($@"UPDATE AppRoles SET
                    Name = {_db.Param(nameof(AppRole.Name))},
                    NormalizedName = {_db.Param(nameof(AppRole.NormalizedName))},
                    ConcurrencyStamp = {_db.Param(nameof(AppRole.ConcurrencyStamp))}
                    WHERE Id = {_db.Param(nameof(AppRole.Id))}", role);
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(AppRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var con = await _db.ConnectAsync(cancellationToken))
            {   
                await con.ExecuteAsync($"DELETE FROM AppRoles WHERE Id = {_db.Param(nameof(AppRole.Id))}", role);
            }

            return IdentityResult.Success;
        }

        public Task<string> GetRoleIdAsync(AppRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Id.ToString());
        }

        public Task<string> GetRoleNameAsync(AppRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        public Task SetRoleNameAsync(AppRole role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return Task.FromResult(0);
        }

        public Task<string> GetNormalizedRoleNameAsync(AppRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.NormalizedName);
        }

        public Task SetNormalizedRoleNameAsync(AppRole role, string normalizedName, CancellationToken cancellationToken)
        {
            role.NormalizedName = normalizedName;
            return Task.FromResult(0);
        }

        public async Task<AppRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var con = await _db.ConnectAsync(cancellationToken))
            {   
                return await con.QuerySingleOrDefaultAsync<AppRole>($@"SELECT * FROM AppRoles
                    WHERE Id = {_db.Param(nameof(roleId))}", new { roleId = int.Parse(roleId) });
            }
        }

        public async Task<AppRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var con = await _db.ConnectAsync(cancellationToken))
            {   
                return await con.QuerySingleOrDefaultAsync<AppRole>($@"SELECT * FROM AppRoles
                    WHERE NormalizedName = {_db.Param(nameof(normalizedRoleName))}", new { normalizedRoleName });
            }
        }

        public void Dispose()
        {
            // Nothing to dispose.
        }
    }
}