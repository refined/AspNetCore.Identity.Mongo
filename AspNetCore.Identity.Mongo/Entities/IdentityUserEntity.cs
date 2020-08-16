using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Mongo.Entities
{
    public class IdentityUserEntity : IdentityUser, IEntity<string>
    {
        public bool IsTransient()
        {
            return CreatedDate == default(DateTime);
        }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public List<IdentityUserClaim<string>> Claims { get; set; }
        public List<IdentityUserLogin<string>> Logins { get; set; }
        public List<IdentityUserToken<string>> Tokens { get; set; }
    }
}