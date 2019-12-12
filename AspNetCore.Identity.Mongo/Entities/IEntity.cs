using System;

namespace AspNetCore.Identity.Mongo.Entities
{
    public interface IEntity<TIdentifier>
    {
        TIdentifier Id { get; set; }

        DateTime CreatedDate { get; set; }

        DateTime UpdatedDate { get; set; }

        bool IsTransient();
    }
}