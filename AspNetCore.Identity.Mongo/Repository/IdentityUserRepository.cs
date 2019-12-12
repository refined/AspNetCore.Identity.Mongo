using AspNetCore.Identity.Mongo.Settings;
using Microsoft.Extensions.Options;

namespace AspNetCore.Identity.Mongo.Repository
{
    public class IdentityUserRepository : IdentityUserEntityRepositoryBase
    {
        public IdentityUserRepository(IOptions<MongoDbSettings> options) : this(options.Value) { }

        public IdentityUserRepository(MongoDbSettings options) : base(options, "IdentityUserEntity")
        {
        }
    }
}
