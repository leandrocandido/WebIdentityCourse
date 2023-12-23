using Newtonsoft.Json;
using System.Text.Json;

namespace WebPageUnderTheHood.Authorization
{
    public class JwtToken
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; } = String.Empty;

        [JsonProperty("expires_at")]
        public DateTime ExpiresAt { get; set; }
    }
}
