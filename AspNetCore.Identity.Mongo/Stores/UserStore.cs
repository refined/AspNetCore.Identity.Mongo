using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Identity.Mongo.Collections;
using AspNetCore.Identity.Mongo.Model;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Mongo.Stores
{
	public class UserStore<TUser, TRole> :
		//IUserClaimStore<TUser>,
		//IUserLoginStore<TUser>,
		IUserRoleStore<TUser>,
		IUserPasswordStore<TUser>,
		IUserSecurityStampStore<TUser>,
		IUserEmailStore<TUser>,
		IUserPhoneNumberStore<TUser>,
		IQueryableUserStore<TUser>,
		IUserTwoFactorStore<TUser>,
		IUserLockoutStore<TUser>,
		//IUserAuthenticatorKeyStore<TUser>,
		//IUserAuthenticationTokenStore<TUser>,
		IUserTwoFactorRecoveryCodeStore<TUser> where TUser : MongoUser
		where TRole : MongoRole
	{
		private readonly IIdentityRoleCollection<TRole> _roleCollection;

		private readonly IIdentityUserCollection<TUser> _userCollection;

	    private readonly ILookupNormalizer _normalizer;

        public UserStore(IIdentityUserCollection<TUser> userCollection, IIdentityRoleCollection<TRole> roleCollection, ILookupNormalizer normalizer)
		{
			_userCollection = userCollection;
			_roleCollection = roleCollection;
		    _normalizer = normalizer;
		}

		public IQueryable<TUser> Users => _userCollection.GetAsQueryable();

		public async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
		{
		    cancellationToken.ThrowIfCancellationRequested();

			var u = await _userCollection.FindByUserNameAsync(user.UserName);
			if (u != null) return IdentityResult.Failed(new IdentityError {Code = "Username already in use"});

			await _userCollection.CreateAsync(user);

		    if (user.Email != null)
		    {
                await SetEmailAsync(user, user.Email, cancellationToken);
		    }

		    await _userCollection.UpdateAsync(user);
            return IdentityResult.Success;
		}

		public async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
		{
		    cancellationToken.ThrowIfCancellationRequested();

			await _userCollection.DeleteAsync(user);
			return IdentityResult.Success;
		}

		public Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
		{
		    cancellationToken.ThrowIfCancellationRequested();

			return _userCollection.FindByIdAsync(userId);
		}

		public Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
		{
		    cancellationToken.ThrowIfCancellationRequested();

			return _userCollection.FindByNormalizedUserNameAsync(normalizedUserName);
		}

		public async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
		{
		    cancellationToken.ThrowIfCancellationRequested();

		    await SetEmailAsync(user, user.Email, cancellationToken);
			await _userCollection.UpdateAsync(user);
			return IdentityResult.Success;
		}

		public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
		{
		    cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(user.NormalizedUserName);
		}

		public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
		{
		    cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user?.Id);
		}

		public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
		{
		    cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(user.UserName);
		}

		public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
		{
			user.NormalizedUserName = normalizedName;
		    return _userCollection.UpdateAsync(user);
		}

		public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
		{
		    cancellationToken.ThrowIfCancellationRequested();

			user.UserName = userName;
		    return _userCollection.UpdateAsync(user);
		}

		void IDisposable.Dispose()
		{
		}

		public async Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken)
		{
		    cancellationToken.ThrowIfCancellationRequested();

		    return (await _userCollection.FindByIdAsync(user.Id))?.Email ?? user.Email;
		}

		public async Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken)
		{
		    cancellationToken.ThrowIfCancellationRequested();

			return (await _userCollection.FindByIdAsync(user.Id))?.EmailConfirmed ?? user.EmailConfirmed;
		}

		public async Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
		{
		    cancellationToken.ThrowIfCancellationRequested();

			return await _userCollection.FindByEmailAsync(normalizedEmail);
		}

		public async Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken)
		{
		    cancellationToken.ThrowIfCancellationRequested();

			return (await _userCollection.FindByIdAsync(user.Id))?.NormalizedEmail ?? user.NormalizedEmail;
		}

		public Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
		{
			user.EmailConfirmed = confirmed;
		    return _userCollection.UpdateAsync(user);
        }

        public Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken)
		{
		    cancellationToken.ThrowIfCancellationRequested();

			user.NormalizedEmail = normalizedEmail;
		    return _userCollection.UpdateAsync(user);
		}

		public async Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken)
		{
		    cancellationToken.ThrowIfCancellationRequested();

		    await SetNormalizedEmailAsync(user, _normalizer.Normalize(user.Email), cancellationToken);
            user.Email = email;
		    
		    await _userCollection.UpdateAsync(user);
		}

		public async Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
		{
		    cancellationToken.ThrowIfCancellationRequested();

			return (await _userCollection.FindByIdAsync(user.Id))?.AccessFailedCount ?? user.AccessFailedCount;
		}

		public async Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken)
		{
		    cancellationToken.ThrowIfCancellationRequested();

			return (await _userCollection.FindByIdAsync(user.Id))?.LockoutEnabled ?? user.LockoutEnabled;
		}

		public async Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
		{
		    cancellationToken.ThrowIfCancellationRequested();

			user.AccessFailedCount++;
			await _userCollection.UpdateAsync(user);
			return user.AccessFailedCount;
		}

		public Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
		{
		    cancellationToken.ThrowIfCancellationRequested();

			user.AccessFailedCount = 0;
		    return _userCollection.UpdateAsync(user);
		}

		public async Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken)
		{
		    cancellationToken.ThrowIfCancellationRequested();

			return (await _userCollection.FindByIdAsync(user.Id))?.LockoutEnd ?? user.LockoutEnd;
		}

		public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
		{
		    cancellationToken.ThrowIfCancellationRequested();

			user.LockoutEnd = lockoutEnd;
		    return _userCollection.UpdateAsync(user);
		}

        public Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
		{
		    cancellationToken.ThrowIfCancellationRequested();

			user.LockoutEnabled = enabled;
		    return _userCollection.UpdateAsync(user);
		}

		public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
		{
		    cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(user.PasswordHash);
		}

		public async Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
		{
		    cancellationToken.ThrowIfCancellationRequested();

			return (await _userCollection.FindByIdAsync(user.Id))?.PasswordHash != null;
		}

		public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
		{
		    cancellationToken.ThrowIfCancellationRequested();

			user.PasswordHash = passwordHash;
		    return _userCollection.UpdateAsync(user);
		}

		public async Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken)
		{
		    cancellationToken.ThrowIfCancellationRequested();

			return (await _userCollection.FindByIdAsync(user.Id))?.PhoneNumber;
		}

		public async Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken)
		{
		    cancellationToken.ThrowIfCancellationRequested();

			return (await _userCollection.FindByIdAsync(user.Id))?.PhoneNumberConfirmed ?? false;
		}

		public Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken)
		{
		    cancellationToken.ThrowIfCancellationRequested();

			user.PhoneNumber = phoneNumber;
		    return _userCollection.UpdateAsync(user);
		}

		public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
		{
		    cancellationToken.ThrowIfCancellationRequested();

			user.PhoneNumberConfirmed = confirmed;
		    return _userCollection.UpdateAsync(user);
		}

		public Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
		{
			if (user.Roles == null) user.Roles = new List<string>();
			user.Roles.Add(roleName);

		    return _userCollection.UpdateAsync(user);
		}

        public Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
		{
		    cancellationToken.ThrowIfCancellationRequested();

		    user.Roles.Remove(roleName);

		    return _userCollection.UpdateAsync(user);
		}


        public async Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
		{
		    cancellationToken.ThrowIfCancellationRequested();

			return (await _userCollection.FindUsersInRoleAsync(roleName)).ToList();
		}

		public async Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
		{
		    cancellationToken.ThrowIfCancellationRequested();

			return (await _userCollection.FindByIdAsync(user.Id))?.Roles
			       ?.Select(roleId => _roleCollection.FindByNameAsync(roleId).Result)
			       .Where(x => x != null)
			       .Select(x => x.Name)
			       .ToList() ?? new List<string>();
		}

		public async Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
		{
		    cancellationToken.ThrowIfCancellationRequested();

			var dbUser = await _userCollection.FindByIdAsync(user.Id);
			return dbUser?.Roles.Contains(roleName) ?? false;
		}

		public Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken)
		{
		    cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(user.SecurityStamp);
		}

		public Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken)
		{
			user.SecurityStamp = stamp;
		    return _userCollection.UpdateAsync(user);
		}

		public Task ReplaceCodesAsync(TUser user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken)
		{
		    cancellationToken.ThrowIfCancellationRequested();

			user.RecoveryCodes = recoveryCodes.Select(x => new TwoFactorRecoveryCode {Code = x, Redeemed = false})
				.ToList();

		    return _userCollection.UpdateAsync(user);
		}

		public async Task<bool> RedeemCodeAsync(TUser user, string code, CancellationToken cancellationToken)
		{
		    cancellationToken.ThrowIfCancellationRequested();

			var dbUser = await _userCollection.FindByIdAsync(user.Id);
			if (dbUser == null) return false;

			var c = user.RecoveryCodes.FirstOrDefault(x => x.Code == code);

			if (c == null || c.Redeemed) return false;

			c.Redeemed = true;

		    await _userCollection.UpdateAsync(user);

			return true;
		}

		public async Task<int> CountCodesAsync(TUser user, CancellationToken cancellationToken)
		{
		    cancellationToken.ThrowIfCancellationRequested();

			var dbUser = await _userCollection.FindByIdAsync(user.Id);
			return dbUser?.RecoveryCodes.Count ?? 0;
		}

		public async Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken cancellationToken)
		{
		    cancellationToken.ThrowIfCancellationRequested();

			return (await _userCollection.FindByIdAsync(user.Id))?.TwoFactorEnabled ?? user.TwoFactorEnabled;
		}

		public Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
		{
		    cancellationToken.ThrowIfCancellationRequested();

			user.TwoFactorEnabled = enabled;
		    return _userCollection.UpdateAsync(user);
		}
	}
}
