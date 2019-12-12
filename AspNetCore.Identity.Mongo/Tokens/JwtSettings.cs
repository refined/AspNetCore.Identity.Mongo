namespace AspNetCore.Identity.Mongo.Tokens
{
    public class JwtSettings
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public string SecretKey { get; set; }
        public int? TokenExpirationMinutes { get; set; }
    }
}