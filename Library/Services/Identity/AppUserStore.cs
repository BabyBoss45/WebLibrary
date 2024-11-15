using System.Data;
using Dapper;
using Library.Data;
using Microsoft.AspNetCore.Identity;

namespace Library.Services.Identity
{
    public class AppUserStore : IUserStore<AppUser>, IUserEmailStore<AppUser>, IUserPhoneNumberStore<AppUser>,
        IUserTwoFactorStore<AppUser>, IUserPasswordStore<AppUser>, IUserRoleStore<AppUser>, IUserSecurityStampStore<AppUser>, IUserAuthenticatorKeyStore<AppUser>
    {
        private readonly IDb _db;

        public AppUserStore(IDb db)
        {
            _db = db;
        }
        // надо заменить все appUsers или
        public async Task<IdentityResult> CreateAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var con = await _db.ConnectAsync(cancellationToken))
            {
                var param = new DynamicParameters(user);

                param.Add(nameof(user.EmailConfirmed), user.EmailConfirmed ? 1 : 0);
                param.Add(nameof(user.PhoneNumberConfirmed), user.PhoneNumberConfirmed ? 1 : 0);
                param.Add(nameof(user.TwoFactorEnabled), user.TwoFactorEnabled ? 1 : 0);

                param.Add(name: nameof(user.Id), dbType: DbType.Int64, direction: ParameterDirection.Output);

                await con.ExecuteAsync($@"INSERT INTO AppUsers (UserName, NormalizedUserName, Name, Email,
                    NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled)
                    VALUES ({_db.Param(nameof(user.UserName))}, {_db.Param(nameof(user.NormalizedUserName))}, {_db.Param(nameof(user.Name))}, {_db.Param(nameof(user.Email))},
                    {_db.Param(nameof(user.NormalizedEmail))}, {_db.Param(nameof(user.EmailConfirmed))}, {_db.Param(nameof(user.PasswordHash))}, {_db.Param(nameof(user.SecurityStamp))},
                    {_db.Param(nameof(user.PhoneNumber))}, {_db.Param(nameof(user.PhoneNumberConfirmed))}, {_db.Param(nameof(user.TwoFactorEnabled))})
                    {_db.Returning(nameof(user.Id))}", param);

                user.Id = param.Get<long>(nameof(user.Id));

            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var con = await _db.ConnectAsync(cancellationToken))
            {
                await con.ExecuteAsync($"DELETE FROM AppUsers WHERE Id = {_db.Param(nameof(AppUser.Id))}", user);
            }

            return IdentityResult.Success;
        }

        public async Task<AppUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var con = await _db.ConnectAsync(cancellationToken))
            {
                return await con.QuerySingleOrDefaultAsync<AppUser>($@"SELECT * FROM AppUsers
                    WHERE Id = {_db.Param(nameof(userId))}", new { userId = long.Parse(userId) });
            }
        }

        public async Task<AppUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var con = await _db.ConnectAsync(cancellationToken))
            {
                return await con.QuerySingleOrDefaultAsync<AppUser>($@"SELECT * FROM AppUsers
                    WHERE NormalizedUserName = {_db.Param(nameof(normalizedUserName))}", new { normalizedUserName });
            }
        }

        public Task<string> GetNormalizedUserNameAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(AppUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.FromResult(0);
        }

        public Task SetUserNameAsync(AppUser user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.FromResult(0);
        }

        public async Task<IdentityResult> UpdateAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var con = await _db.ConnectAsync(cancellationToken))
            {
                var param = new DynamicParameters(user);

                param.Add(nameof(user.Name), user.Name);
                param.Add(nameof(user.EmailConfirmed), user.EmailConfirmed ? 1 : 0);
                param.Add(nameof(user.PhoneNumberConfirmed), user.PhoneNumberConfirmed ? 1 : 0);
                param.Add(nameof(user.TwoFactorEnabled), user.TwoFactorEnabled ? 1 : 0);

                await con.ExecuteAsync($@"UPDATE AppUsers SET
                    Name = {_db.Param(nameof(user.Name))},
                    UserName = {_db.Param(nameof(user.UserName))},
                    NormalizedUserName = {_db.Param(nameof(user.NormalizedUserName))},
                    Email = {_db.Param(nameof(user.Email))},
                    NormalizedEmail = {_db.Param(nameof(user.NormalizedEmail))},
                    EmailConfirmed = {_db.Param(nameof(user.EmailConfirmed))},
                    PasswordHash = {_db.Param(nameof(user.PasswordHash))},
                    SecurityStamp = {_db.Param(nameof(user.SecurityStamp))},
                    PhoneNumber = {_db.Param(nameof(user.PhoneNumber))},
                    PhoneNumberConfirmed = {_db.Param(nameof(user.PhoneNumberConfirmed))},
                    TwoFactorEnabled = {_db.Param(nameof(user.TwoFactorEnabled))}
                    WHERE Id = {_db.Param(nameof(user.Id))}", param);
            }

            return IdentityResult.Success;
        }

        public Task SetEmailAsync(AppUser user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.FromResult(0);
        }

        public Task<string> GetEmailAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(AppUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = confirmed;
            return Task.FromResult(0);
        }

        public async Task<AppUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var con = await _db.ConnectAsync(cancellationToken))
            {
                return await con.QuerySingleOrDefaultAsync<AppUser>($@"SELECT * FROM AppUsers
                    WHERE NormalizedEmail = {_db.Param(nameof(normalizedEmail))}", new { normalizedEmail });
            }
        }

        public Task<string> GetNormalizedEmailAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetNormalizedEmailAsync(AppUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail;
            return Task.FromResult(0);
        }

        public Task SetPhoneNumberAsync(AppUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            user.PhoneNumber = phoneNumber;
            return Task.FromResult(0);
        }

        public Task<string> GetPhoneNumberAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(AppUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.PhoneNumberConfirmed = confirmed;
            return Task.FromResult(0);
        }

        public Task SetTwoFactorEnabledAsync(AppUser user, bool enabled, CancellationToken cancellationToken)
        {
            user.TwoFactorEnabled = enabled;
            return Task.FromResult(0);
        }

        public Task<bool> GetTwoFactorEnabledAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task SetPasswordHashAsync(AppUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash != null);
        }


        public Task SetSecurityStampAsync(AppUser user, string stamp, CancellationToken cancellationToken)
        {
            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        public Task<string> GetSecurityStampAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.SecurityStamp);
        }

        public async Task AddToRoleAsync(AppUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var con = await _db.ConnectAsync(cancellationToken))
            {
                var normalizedName = roleName.ToUpper();
                var roleId = await con.ExecuteScalarAsync<int?>($"SELECT Id FROM AppRoles WHERE NormalizedName = {_db.Param(nameof(normalizedName))}", new { normalizedName });
                if (!roleId.HasValue)
                    roleId = await con.ExecuteAsync($"INSERT INTO AppRoles(Name, NormalizedName) VALUES({_db.Param(nameof(roleName))}, {_db.Param(nameof(normalizedName))})",
                        new { roleName, normalizedName });

                if ((await con.QueryFirstAsync<int>($"select count(*) from AppUserRoles where UserId = {_db.Param("userId")} and RoleId = {_db.Param("roleId")}", new
                {
                    userId = user.Id,
                    roleId
                })) == 0)
                {
                    await con.QueryAsync($"insert into AppUserRoles(UserId, RoleId) values({_db.Param("userId")}, {_db.Param("roleId")})", new
                    {
                        userId = user.Id,
                        roleId
                    });
                }
            }
        }

        public async Task RemoveFromRoleAsync(AppUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var con = await _db.ConnectAsync(cancellationToken))
            {
                var roleId = await con.ExecuteScalarAsync<int?>($"SELECT Id FROM AppRoles WHERE NormalizedName = {_db.Param("normalizedName")}", new { normalizedName = roleName.ToUpper() });
                if (!roleId.HasValue)
                    await con.ExecuteAsync($"DELETE FROM AppUserRoles WHERE UserId = {_db.Param("userId")} AND RoleId = {_db.Param(nameof(roleId))}", new { userId = user.Id, roleId });
            }
        }

        public async Task<IList<string>> GetRolesAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var con = await _db.ConnectAsync(cancellationToken))
            {
                var queryResults = await con.QueryAsync<string>("SELECT r.Name FROM AppRoles r INNER JOIN AppUserRoles ur ON ur.RoleId = r.Id " +
                    $"WHERE ur.UserId = {_db.Param("userId")}", new { userId = user.Id });

                return queryResults.AsList();
            }
        }

        public async Task<bool> IsInRoleAsync(AppUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var con = await _db.ConnectAsync(cancellationToken))
            {
                var roleId = await con.ExecuteScalarAsync<int?>($"SELECT Id FROM AppRoles WHERE NormalizedName = {_db.Param("normalizedName")}", new { normalizedName = roleName.ToUpper() });
                if (roleId == default(int)) return false;
                var matchingRoles = await con.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM AppUserRoles WHERE UserId = {_db.Param("userId")} AND RoleId = {_db.Param(nameof(roleId))}",
                    new { userId = user.Id, roleId });

                return matchingRoles > 0;
            }
        }

        public async Task<IList<AppUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var con = await _db.ConnectAsync(cancellationToken))
            {
                var queryResults = await con.QueryAsync<AppUser>("SELECT u.* FROM AppUsers u " +
                    $"INNER JOIN AppUserRoles ur ON ur.UserId = u.Id INNER JOIN AppRoles r ON r.Id = ur.RoleId WHERE r.NormalizedName = {_db.Param("normalizedName")}",
                    new { normalizedName = roleName.ToUpper() });

                return queryResults.AsList();
            }
        }

        protected async Task<IdentityUserToken<long>> FindTokenAsync(AppUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var con = await _db.ConnectAsync(cancellationToken))
            {
                return await con.QuerySingleOrDefaultAsync<IdentityUserToken<long>>($@"SELECT * FROM AppUserTokens
                    WHERE UserId = {_db.Param("UserId")} AND LoginProvider = {_db.Param(nameof(loginProvider))} AND Name = {_db.Param(nameof(name))}", new { UserId = user.Id, loginProvider, name });
            }
        }

        protected async Task AddUserTokenAsync(IdentityUserToken<long> token)
        {
            using (var con = await _db.ConnectAsync())
            {
                var param = new DynamicParameters(token);

                await con.ExecuteAsync($@"INSERT INTO AppUserTokens (UserId, LoginProvider, Name, Value)
                    VALUES ({_db.Param(nameof(IdentityUserToken<long>.UserId))}, {_db.Param(nameof(IdentityUserToken<long>.LoginProvider))}, 
                    {_db.Param(nameof(IdentityUserToken<long>.Name))}, {_db.Param(nameof(IdentityUserToken<long>.Value))})", param);
            }
        }

        protected IdentityUserToken<long> CreateUserToken(AppUser user, string loginProvider, string name, string value)
        {
            return new IdentityUserToken<long>
            {
                UserId = user.Id,
                LoginProvider = loginProvider,
                Name = name,
                Value = value
            };
        }

        public async Task SetTokenAsync(AppUser user, string loginProvider, string name, string value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var token = await FindTokenAsync(user, loginProvider, name, cancellationToken).ConfigureAwait(false);
            if (token == null)
            {
                await AddUserTokenAsync(CreateUserToken(user, loginProvider, name, value)).ConfigureAwait(false);
            }
            else
            {
                token.Value = value;
            }
        }

        public async Task<string> GetTokenAsync(AppUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var entry = await FindTokenAsync(user, loginProvider, name, cancellationToken).ConfigureAwait(false);
            return entry?.Value;
        }

        private const string InternalLoginProvider = "[AspNetUserStore]";
        private const string AuthenticatorKeyTokenName = "AuthenticatorKey";

        public Task SetAuthenticatorKeyAsync(AppUser user, string key, CancellationToken cancellationToken)
            => SetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, key, cancellationToken);

        public Task<string> GetAuthenticatorKeyAsync(AppUser user, CancellationToken cancellationToken)
            => GetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, cancellationToken);

        public void Dispose()
        {
            // Nothing to dispose.
        }
    }
}