using Microsoft.Extensions.Options;
using Novikov.MongoRepository;

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
