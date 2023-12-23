using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text.Json.Serialization;
using WebPageUnderTheHood.Authorization;
using WebPageUnderTheHood.DTO;
using WebPageUnderTheHood.Pages.Account;

namespace WebPageUnderTheHood.Pages
{
    [Authorize(Policy = "HRManagerOnly")]
    public class HRManagerModel : PageModel
    {
        [BindProperty]
        public List<WeatherForecastDTO> WeatherForecastItems { get; set; } = new List<WeatherForecastDTO>();

        public IHttpClientFactory httpClientFactory;
        public HRManagerModel(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }
        public async Task OnGetAsync()
        {
            JwtToken token = null;

            var strToken = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(strToken))
            {
                token = await Authenticate();
            }
            else
            {
                token = JsonConvert.DeserializeObject<JwtToken>(strToken) ?? new JwtToken();
            }

            
            if (token == null || string.IsNullOrWhiteSpace(token.AccessToken) || token.ExpiresAt <= DateTime.Now)
            {
                token = await Authenticate();
            }
            var httpClient = httpClientFactory.CreateClient("OurWebApi");
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token?.AccessToken ?? string.Empty);
            WeatherForecastItems = await httpClient.GetFromJsonAsync<List<WeatherForecastDTO>>("WeatherForecast") ?? new List<WeatherForecastDTO>();
        }

        private async Task<JwtToken> Authenticate()
        {
            var httpClient = httpClientFactory.CreateClient("OurWebApi");
            var res = await httpClient.PostAsJsonAsync("auth", new Credential { Username = "admin", Password = "admin" });
            res.EnsureSuccessStatusCode();
            string strJwt = await res.Content.ReadAsStringAsync();
            HttpContext.Session.SetString("access_token", strJwt);
            return JsonConvert.DeserializeObject<JwtToken>(strJwt) ?? new JwtToken();
        }
    }
}
