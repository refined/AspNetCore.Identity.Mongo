using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Identity.Mongo.Model;

namespace AspNetCore.Identity.Mongo.Collections
{
	public interface IIdentityUserCollection<TUser> where TUser : MongoUser
	{
	    Task<TUser> FindByEmailAsync(string normalizedEmail);
	    Task<TUser> FindByUserNameAsync(string username);
	    Task<TUser> FindByNormalizedUserNameAsync(string normalizedUserName);
	    Task<IEnumerable<TUser>> FindUsersInRoleAsync(string roleName);
	    IQueryable<TUser> GetAsQueryable();
	    Task<TUser> CreateAsync(TUser obj);
	    Task UpdateAsync(TUser obj);
	    Task DeleteAsync(TUser obj);
	    Task<TUser> FindByIdAsync(string itemId);
    }
}