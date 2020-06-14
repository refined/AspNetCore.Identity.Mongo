using Microsoft.Extensions.Options;
using Novikov.MongoRepository;

namespace AspNetCore.Identity.Mongo.Repository
{
    public class IdentityAdminRepository : IdentityUserEntityRepositoryBase
    {
        public IdentityAdminRepository(IOptions<MongoDbSettings> options) : this(options.Value) { }

        public IdentityAdminRepository(MongoDbSettings options) : base(options, "IdentityAdminEntity")
        {
        }
    }
}