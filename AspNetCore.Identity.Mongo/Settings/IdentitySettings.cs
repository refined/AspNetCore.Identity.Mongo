namespace AspNetCore.Identity.Mongo.Settings
{
    public class IdentitySettings
    {
        public bool UseJwtTokens { get; set; } = false;
        public bool RequireConfirmedPhoneNumber { get; set; } = false;
    }
}