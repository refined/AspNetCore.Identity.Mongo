namespace AspNetCore.Identity.Mongo.Settings
{
    public class IdentitySettings
    {
        public bool RequireConfirmedPhoneNumber { get; set; } = false;
        public bool RequireConfirmedEmail { get; set; } = true;
    }
}