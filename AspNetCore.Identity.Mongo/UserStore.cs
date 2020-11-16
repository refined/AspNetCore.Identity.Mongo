using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Identity.Mongo.Entities;
using AspNetCore.Identity.Mongo.Repository;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;

namespace AspNetCore.Identity.Mongo
{
    public class UserStore :
        IUserSecurityStampStore<IdentityUserEntity>,
        IUserLoginStore<IdentityUserEntity>,
        IUserPasswordStore<IdentityUserEntity>,
        IUserEmailStore<IdentityUserEntity>,
        IUserPhoneNumberStore<IdentityUserEntity>,
        IUserLockoutStore<IdentityUserEntity>,
        IUserAuthenticationTokenStore<IdentityUserEntity>
    {
        private readonly IIdentityRepository<IdentityUserEntity> _userRepository;
        private readonly ILookupNormalizer _normalizer;

        public UserStore(
            IIdentityRepository<IdentityUserEntity> userRepository, 
            ILookupNormalizer normalizer)
        {
            _userRepository = userRepository;
            _normalizer = normalizer;
        }
        public async Task<IdentityResult> CreateAsync(IdentityUserEntity user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var u = await _userRepository.FirstOrDefaultAsync(_ => _.UserName == user.UserName, cancellationToken: cancellationToken);
            if (u != null) return IdentityResult.Failed(new IdentityError { Code = "Username already in use" });
            u = await _userRepository.FirstOrDefaultAsync(_ => _.NormalizedEmail == user.NormalizedEmail, cancellationToken: cancellationToken);
            if (u != null) return IdentityResult.Failed(new IdentityError { Code = "Email already in use" });

            user.Id = ObjectId.GenerateNewId().ToString(); // replace base Microsoft.AspNetCore.Identity, because we need valid MongoID
            await _userRepository.SaveAsync(user, cancellationToken);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(IdentityUserEntity user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _userRepository.DeleteAsync(user.Id, cancellationToken);
            return IdentityResult.Success;
        }

        public Task<IdentityUserEntity> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return _userRepository.GetAsync(userId, cancellationToken);
        }

        public Task<IdentityUserEntity> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return _userRepository.FirstOrDefaultAsync(_ => _.NormalizedUserName == normalizedUserName, cancellationToken: cancellationToken);
        }

        public async Task<IdentityResult> UpdateAsync(IdentityUserEntity user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _userRepository.SaveAsync(user, cancellationToken);
            return IdentityResult.Success;
        }
        
        public Task<string> GetNormalizedUserNameAsync(IdentityUserEntity user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(IdentityUserEntity user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user?.Id);
        }

        public Task<string> GetUserNameAsync(IdentityUserEntity user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.UserName);
        }

        public async Task SetNormalizedUserNameAsync(IdentityUserEntity user, string normalizedName, CancellationToken cancellationToken)
        {
            var updatedUser = await _userRepository.UpdateAsync(_ => _.Id == user.Id, entity => entity.NormalizedUserName, normalizedName, cancellationToken);
            user.NormalizedUserName = updatedUser?.NormalizedUserName ?? normalizedName;
        }

        public async Task SetUserNameAsync(IdentityUserEntity user, string userName, CancellationToken cancellationToken)
        {
            var updatedUser = await _userRepository.UpdateAsync(_ => _.Id == user.Id, entity => entity.UserName, userName, cancellationToken);
            user.UserName = updatedUser?.UserName ?? userName;
        }

        void IDisposable.Dispose()
        {
        }

        public async Task<string> GetEmailAsync(IdentityUserEntity user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return (await _userRepository.GetAsync(user.Id, cancellationToken))?.Email ?? user.Email;
        }

        public async Task<bool> GetEmailConfirmedAsync(IdentityUserEntity user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return (await _userRepository.GetAsync(user.Id, cancellationToken))?.EmailConfirmed ?? user.EmailConfirmed;
        }

        public Task<IdentityUserEntity> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            return _userRepository.FirstOrDefaultAsync(_ => _.NormalizedEmail == normalizedEmail, cancellationToken);
        }

        public async Task<string> GetNormalizedEmailAsync(IdentityUserEntity user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return (await _userRepository.GetAsync(user.Id, cancellationToken))?.NormalizedEmail ?? user.NormalizedEmail;
        }

        public async Task SetEmailConfirmedAsync(IdentityUserEntity user, bool confirmed, CancellationToken cancellationToken)
        {
            var updatedUser = await _userRepository.UpdateAsync(_ => _.Id == user.Id, entity => entity.EmailConfirmed, confirmed, cancellationToken);
            user.EmailConfirmed = updatedUser?.EmailConfirmed ?? confirmed;
        }

        public async Task SetNormalizedEmailAsync(IdentityUserEntity user, string normalizedEmail, CancellationToken cancellationToken)
        {
            var updatedUser = await _userRepository.UpdateAsync(_ => _.Id == user.Id, entity => entity.NormalizedEmail, normalizedEmail, cancellationToken);
            user.NormalizedEmail = updatedUser?.NormalizedEmail ?? normalizedEmail;
        }

        public async Task SetEmailAsync(IdentityUserEntity user, string email, CancellationToken cancellationToken)
        {
            await SetNormalizedEmailAsync(user, _normalizer.NormalizeEmail(user.Email), cancellationToken);

            var updatedUser = await _userRepository.UpdateAsync(_ => _.Id == user.Id, entity => entity.Email, email, cancellationToken);
            user.Email = updatedUser?.Email ?? email;
        }

        public async Task<int> GetAccessFailedCountAsync(IdentityUserEntity user, CancellationToken cancellationToken)
        {
            return (await _userRepository.GetAsync(user.Id, cancellationToken))?.AccessFailedCount ?? user.AccessFailedCount;
        }

        public async Task<bool> GetLockoutEnabledAsync(IdentityUserEntity user, CancellationToken cancellationToken)
        {
            return (await _userRepository.GetAsync(user.Id, cancellationToken))?.LockoutEnabled ?? user.LockoutEnabled;
        }

        public async Task<int> IncrementAccessFailedCountAsync(IdentityUserEntity user, CancellationToken cancellationToken)
        {
            var updatedUser = await _userRepository.UpdateAsync(_ => _.Id == user.Id, entity => entity.AccessFailedCount, user.AccessFailedCount + 1, cancellationToken);
            user.AccessFailedCount = updatedUser?.AccessFailedCount ?? (user.AccessFailedCount + 1);
            return user.AccessFailedCount;
        }

        public async Task ResetAccessFailedCountAsync(IdentityUserEntity user, CancellationToken cancellationToken)
        {
            var updatedUser = await _userRepository.UpdateAsync(_ => _.Id == user.Id, entity => entity.AccessFailedCount, 0, cancellationToken);
            user.AccessFailedCount = updatedUser?.AccessFailedCount ?? 0;
        }

        public async Task<DateTimeOffset?> GetLockoutEndDateAsync(IdentityUserEntity user, CancellationToken cancellationToken)
        {
            return (await _userRepository.GetAsync(user.Id, cancellationToken))?.LockoutEnd ?? user.LockoutEnd;
        }

        public async Task SetLockoutEndDateAsync(IdentityUserEntity user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            var updatedUser = await _userRepository.UpdateAsync(_ => _.Id == user.Id, entity => entity.LockoutEnd, lockoutEnd, cancellationToken);
            user.LockoutEnd = updatedUser?.LockoutEnd ?? lockoutEnd;
        }

        public async Task SetLockoutEnabledAsync(IdentityUserEntity user, bool enabled, CancellationToken cancellationToken)
        {
            var updatedUser = await _userRepository.UpdateAsync(_ => _.Id == user.Id, entity => entity.LockoutEnabled, enabled, cancellationToken);
            user.LockoutEnabled = updatedUser?.LockoutEnabled ?? enabled;
        }

        public async Task AddLoginAsync(IdentityUserEntity user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            if (user.Logins == null) user.Logins = new List<IdentityUserLogin<string>>();

            user.Logins.Add(new IdentityUserLogin<string>
            {
                UserId = user.Id,
                LoginProvider = login.LoginProvider,
                ProviderDisplayName = login.ProviderDisplayName,
                ProviderKey = login.ProviderKey
            });
            
            var updatedUser = await _userRepository.UpdateAsync(_ => _.Id == user.Id, entity => entity.Logins, user.Logins, cancellationToken);
            user.Logins = updatedUser?.Logins ?? user.Logins;
        }

        public async Task RemoveLoginAsync(IdentityUserEntity user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var dbUser = await _userRepository.GetAsync(user.Id, cancellationToken);
            user.Logins.RemoveAll(x => x.LoginProvider == loginProvider && x.ProviderKey == providerKey);
            dbUser.Logins.RemoveAll(x => x.LoginProvider == loginProvider && x.ProviderKey == providerKey);

            var updatedUser = await _userRepository.UpdateAsync(_ => _.Id == user.Id, entity => entity.Logins, user.Logins, cancellationToken);
            user.Logins = updatedUser?.Logins ?? user.Logins;
        }

        public Task<IdentityUserEntity> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return _userRepository.FirstOrDefaultAsync(_ => _.Logins.Exists(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey), cancellationToken: cancellationToken);
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(IdentityUserEntity user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var dbUser = await _userRepository.GetAsync(user.Id, cancellationToken);
            return dbUser?.Logins?.Select(x => new UserLoginInfo(x.LoginProvider, x.ProviderKey, x.ProviderDisplayName))?.ToList() ?? new List<UserLoginInfo>();
        }

        public Task<string> GetPasswordHashAsync(IdentityUserEntity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public async Task<bool> HasPasswordAsync(IdentityUserEntity user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return (await _userRepository.GetAsync(user.Id, cancellationToken))?.PasswordHash != null;
        }

        public async Task SetPasswordHashAsync(IdentityUserEntity user, string passwordHash, CancellationToken cancellationToken)
        {
            var updatedUser = await _userRepository.UpdateAsync(_ => _.Id == user.Id, entity => entity.PasswordHash, passwordHash, cancellationToken);
            user.PasswordHash = updatedUser?.PasswordHash ?? passwordHash;
        }

        public async Task<string> GetPhoneNumberAsync(IdentityUserEntity user, CancellationToken cancellationToken)
        {
            return (await _userRepository.GetAsync(user.Id, cancellationToken))?.PhoneNumber ?? user.PhoneNumber;
        }

        public async Task<bool> GetPhoneNumberConfirmedAsync(IdentityUserEntity user, CancellationToken cancellationToken)
        {
            return (await _userRepository.GetAsync(user.Id, cancellationToken))?.PhoneNumberConfirmed ?? user.PhoneNumberConfirmed;
        }

        public async Task SetPhoneNumberAsync(IdentityUserEntity user, string phoneNumber, CancellationToken cancellationToken)
        {
            var updatedUser = await _userRepository.UpdateAsync(_ => _.Id == user.Id, entity => entity.PhoneNumber, phoneNumber, cancellationToken);
            user.PhoneNumber = updatedUser.PhoneNumber;
        }

        public async Task SetPhoneNumberConfirmedAsync(IdentityUserEntity user, bool confirmed, CancellationToken cancellationToken)
        {
            var updatedUser = await _userRepository.UpdateAsync(_ => _.Id == user.Id, entity => entity.PhoneNumberConfirmed, confirmed, cancellationToken);
            user.PhoneNumberConfirmed = updatedUser.PhoneNumberConfirmed;
        }

        public async Task SetTokenAsync(IdentityUserEntity user, string loginProvider, string name, string value, CancellationToken cancellationToken)
        {
            if (user.Tokens == null) user.Tokens = new List<IdentityUserToken<string>>();

            var token = user.Tokens.FirstOrDefault(x => x.LoginProvider == loginProvider && x.Name == name);

            if (token == null)
            {
                token = new IdentityUserToken<string> { LoginProvider = loginProvider, Name = name, Value = value };
                user.Tokens.Add(token);
            }
            else
            {
                token.Value = value;
            }

            var updatedUser = await _userRepository.UpdateAsync(_ => _.Id == user.Id, entity => entity.Tokens, user.Tokens, cancellationToken);
            user.Tokens = updatedUser?.Tokens ?? user.Tokens;
        }

        public async Task RemoveTokenAsync(IdentityUserEntity user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user?.Tokens == null) return;

            user.Tokens.RemoveAll(x => x.LoginProvider == loginProvider && x.Name == name);

            var updatedUser = await _userRepository.UpdateAsync(_ => _.Id == user.Id, entity => entity.Tokens, user.Tokens, cancellationToken);
            user.Tokens = updatedUser?.Tokens ?? user.Tokens;
        }

        public Task<string> GetTokenAsync(IdentityUserEntity user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user?.Tokens?.FirstOrDefault(x => x.LoginProvider == loginProvider && x.Name == name)?.Value);
        }
        public async Task<string> GetSecurityStampAsync(IdentityUserEntity user, CancellationToken cancellationToken)
        {
            return (await _userRepository.GetAsync(user.Id, cancellationToken))?.SecurityStamp ?? user.SecurityStamp;
        }

        public async Task SetSecurityStampAsync(IdentityUserEntity user, string stamp, CancellationToken cancellationToken)
        {
            var updatedUser = await _userRepository.UpdateAsync(_ => _.Id == user.Id, entity => entity.SecurityStamp, stamp, cancellationToken);
            user.SecurityStamp = updatedUser?.SecurityStamp ?? stamp;
        }
    }
}