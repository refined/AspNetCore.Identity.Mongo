using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Mongo.Model
{
    public class MongoUser : IdentityUser
	{
        public MongoUser()
		{
			Roles = new List<string>();
			//Claims = new List<IdentityUserClaim>();
		    //Logins = new List<IdentityUserLogin>();
			//Tokens = new List<IdentityUserToken>();
			RecoveryCodes = new List<TwoFactorRecoveryCode>();
		}

		public string AuthenticatorKey { get; set; }

		public List<string> Roles { get; set; }

		//public List<IdentityUserClaim> Claims { get; set; }

		//public List<IdentityUserLogin> Logins { get; set; }

		//public List<IdentityUserToken> Tokens { get; set; }

		public List<TwoFactorRecoveryCode> RecoveryCodes { get; set; }
	}
}