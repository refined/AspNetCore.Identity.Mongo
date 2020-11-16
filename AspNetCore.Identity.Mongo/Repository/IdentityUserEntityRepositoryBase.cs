using AspNetCore.Identity.Mongo.Entities;
using AspNetCore.Identity.Mongo.Settings;
using MongoDB.Driver;

namespace AspNetCore.Identity.Mongo.Repository
{
    public abstract class IdentityUserEntityRepositoryBase : MongoRepository<IdentityUserEntity, string>, IIdentityRepository<IdentityUserEntity>
    {
        protected IdentityUserEntityRepositoryBase(MongoDbSettings options, string collectionName = null) : base(options, collectionName)
        {
            Collection.Indexes.CreateMany(new[]
            {
                new CreateIndexModel<IdentityUserEntity>(
                    Builders<IdentityUserEntity>.IndexKeys.Ascending(a => a.UserName),
                    new CreateIndexOptions {Background = true, Unique = true}),
                new CreateIndexModel<IdentityUserEntity>(
                    Builders<IdentityUserEntity>.IndexKeys.Ascending(a => a.NormalizedUserName),
                    new CreateIndexOptions {Background = true, Unique = true}),
                new CreateIndexModel<IdentityUserEntity>(
                    Builders<IdentityUserEntity>.IndexKeys.Ascending(a => a.Email),
                    new CreateIndexOptions {Background = true, Unique = true}),
                new CreateIndexModel<IdentityUserEntity>(
                    Builders<IdentityUserEntity>.IndexKeys.Ascending(a => a.NormalizedEmail),
                    new CreateIndexOptions {Background = true, Unique = true}),
                new CreateIndexModel<IdentityUserEntity>(
                    Builders<IdentityUserEntity>.IndexKeys.Ascending(a => a.PhoneNumber),
                    new CreateIndexOptions {Background = true, Unique = false, Version = 1}),
            });
        }
    }
}