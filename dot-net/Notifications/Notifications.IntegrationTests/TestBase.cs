using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Notifications.IntegrationTests
{
    public class TestBase
    {
        private const string ApplicationJson = "application/json";
        public static Uri BaseUri;
        public static HttpClient Client;

        static TestBase()
        {
            BaseUri = new Uri("http://localhost");
            Client = OneTimeTestSetup.Client;
        }

        protected async Task<HttpResponseMessage> Get(string url)
        {
            return await OneTimeTestSetup.Client.GetAsync(url);
        }

        protected async Task<string> GetJson(string url)
        {
            var response = await Client.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();
            return body;
        }

        protected async Task<HttpResponseMessage> PostJson(string url, dynamic jToken)
        {
            var message = new StringContent(jToken.ToString(), Encoding.UTF8, ApplicationJson);
            var response = await Client.PostAsync(url, message);
            return response;
        }

        protected async Task<HttpResponseMessage> PostJson(string url)
        {
            var response = await Client.PostAsync(url, new StringContent(string.Empty));
            return response;
        }

        protected async Task<HttpResponseMessage> PutJson(string url, dynamic jToken,
            Dictionary<string, string> headers)
        {
            string data = JsonConvert.SerializeObject(jToken);
            var message = new StringContent(data, Encoding.UTF8, ApplicationJson);

            if (headers == null || !headers.Any()) return await Client.PutAsync(url, message);

            foreach (var (key, value) in headers)
            {
                message.Headers.Add(key, value);
            }

            return await OneTimeTestSetup.Client.PutAsync(url, message);
        }

        protected static string BuildJsonApiPayload<T>(T content)
        {
            var jsonSettings = new JsonSerializerSettings();
            jsonSettings.Converters.Add(new StringEnumConverter());
            var payload = JsonConvert.SerializeObject(content, jsonSettings);
            return payload;
        }
    }
}