using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Mongo.Model
{
	public class IdentityUserClaim : IdentityUserClaim<string>
    {
		public IdentityUserClaim()
		{
		}

        /// <summary>Gets or sets the identifier for this user claim.</summary>
        public override int Id { get; set; }

        /// <summary>
        /// Gets or sets the primary key of the user associated with this claim.
        /// </summary>
        public override string UserId { get; set; }

        /// <summary>Gets or sets the claim type for this claim.</summary>
        public override string ClaimType { get; set; }

        /// <summary>Gets or sets the claim value for this claim.</summary>
        public override string ClaimValue { get; set; }


    }
}