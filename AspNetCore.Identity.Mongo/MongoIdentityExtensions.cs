using System;
using AspNetCore.Identity.Mongo.Collections;
using AspNetCore.Identity.Mongo.Model;
using AspNetCore.Identity.Mongo.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Identity.Mongo
{
	public static class MongoIdentityExtensions
	{
        public static IdentityBuilder AddIdentityMongoDbProvider<TUser, TRole>(this IServiceCollection services,
			Action<MongoIdentityOptions> setupDatabaseAction)
            where TUser : MongoUser
			where TRole : MongoRole
        {
            return AddIdentityMongoDbProvider<TUser, TRole>(services, x => { }, setupDatabaseAction);
        }


	    public static IdentityBuilder AddIdentityMongoDbProvider<TUser, TRole>(this IServiceCollection services,
	        Action<IdentityOptions> setupIdentityAction, Action<MongoIdentityOptions> setupDatabaseAction) 
	        where TUser : MongoUser
	        where TRole : MongoRole
	    {
            var dbOptions = new MongoIdentityOptions();
	        setupDatabaseAction(dbOptions);

	        var builder = services.AddIdentity<TUser, TRole>(setupIdentityAction ?? (x => { }));

	        builder.AddRoleStore<RoleStore<TRole>>()
	            .AddUserStore<UserStore<TUser, TRole>>()
	            .AddUserManager<UserManager<TUser>>()
	            .AddRoleManager<RoleManager<TRole>>()
	            .AddDefaultTokenProviders();


	        var userCollection = new IdentityUserCollection<TUser>(dbOptions.ConnectionString, dbOptions.UsersCollection);
	        var roleCollection = new IdentityRoleCollection<TRole>(dbOptions.ConnectionString, dbOptions.RolesCollection);

	        services.AddTransient<IIdentityUserCollection<TUser>>(x => userCollection);
	        services.AddTransient<IIdentityRoleCollection<TRole>>(x => roleCollection);

	        // Identity Services
	        services.AddTransient<IUserStore<TUser>>(x => new UserStore<TUser, TRole>(userCollection, roleCollection, x.GetService<ILookupNormalizer>()));
	        services.AddTransient<IRoleStore<TRole>>(x => new RoleStore<TRole>(roleCollection));
	       
	        
	        return builder;
	    }
	}
}