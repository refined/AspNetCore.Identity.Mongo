using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Mongo.Model
{
	public class IdentityUserToken : IdentityUserToken<string>
    {
        /// <summary>
        /// Gets or sets the primary key of the user that the token belongs to.
        /// </summary>
        public override string UserId { get; set; }

        /// <summary>Gets or sets the LoginProvider this token is from.</summary>
        public override string LoginProvider { get; set; }

        /// <summary>Gets or sets the name of the token.</summary>
        public override string Name { get; set; }

        /// <summary>Gets or sets the token value.</summary>
        [ProtectedPersonalData]
        public override string Value { get; set; }
    }
}