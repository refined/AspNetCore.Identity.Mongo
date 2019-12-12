using System;

namespace AspNetCore.Identity.Mongo.Entities
{
    [Serializable]
    public abstract class Entity<TIdentifier> : IEntity<TIdentifier>
    {
        public TIdentifier Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public virtual bool IsTransient()
        {
            return (object)this.Id == null || this.Id.Equals((object)default(TIdentifier));
        }
    }
}