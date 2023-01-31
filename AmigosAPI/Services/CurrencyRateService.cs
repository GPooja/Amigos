using Newtonsoft.Json.Linq;

namespace AmigosAPI.Services
{
    public class CurrencyRateService
    {
        private IConfiguration configuration;
        public CurrencyRateService(IConfiguration iConfig) 
        {
            configuration= iConfig;
        }
        public async Task<double> GetConversionRateAsync(string fromCurrencyCode, string toCurrencyCode, DateTime date)
        {
            HttpClient client = new HttpClient();
            var dateStr = date.ToString("yyyy-MM-dd");
            string URL = configuration.GetValue<string>("CurrencyRateAPIUrl")+dateStr+"?base="+fromCurrencyCode+"&symbols="+toCurrencyCode;
            HttpResponseMessage response = await client.GetAsync(URL);
            string result = "";
            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsStringAsync();
            }
            var parsedJSON = JObject.Parse(result);            
            var rate = parsedJSON != null ? (double) parsedJSON["rates"][toCurrencyCode] : 0;
            return rate;
        }
    }
}
