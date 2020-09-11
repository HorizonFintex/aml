using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AMLAPIExample
{
    public class RequestAccessTokenResult
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
    }
    public class AMLAccount
    {
        private string accessToken = null;
        private HttpClient _client;
        public AMLAccount()
        {
            _client = new HttpClient();
        }

        /// <summary>
        /// Request access token with provided refresh token
        /// </summary>
        /// <returns>true if access token successfully aquired, false otherwise.</returns>
        public async Task<bool> RequestAccessToken()
        {
            try
            {
                Uri u = new Uri(Globals.b2cRequestEndpoint);
                var dict = new Dictionary<string, string>
                {
                    { "client_id", Globals.b2cClientId },
                    { "grant_type", Globals.b2cGrantType },
                    { "scope", Globals.b2cScope },
                    { "redirect_uri", Globals.b2cRedirectUrl },
                    { "client_secret", Globals.b2cClientSecret },
                    { "refresh_token", Globals.b2cRefreshToken }
                };
                _client.DefaultRequestHeaders.Clear();
                HttpResponseMessage result = await _client.PostAsync(u, new FormUrlEncodedContent(dict));
                if (result.IsSuccessStatusCode)
                {
                    var content = await result.Content.ReadAsStringAsync();
                    var json = JsonSerializer.Deserialize<RequestAccessTokenResult>(content);
                    accessToken = json.AccessToken;

                    return true;
                }
                else
                {
                    accessToken = string.Empty;
                    return false;
                }
            }
            catch (Exception)
            {
                accessToken = string.Empty;
                return false;
            }
        }

        /// <summary>
        /// Do aml search against AML API.
        /// </summary>
        /// <returns></returns>
        public async Task<ValueTuple<bool, string>> Search(string fname, string lname, string country, string wallet)
        {
            try
            {
                var uriBuilder = new StringBuilder();
                uriBuilder.Append(Globals.amlAPIEndpoint + "/api/aml/searchRaw");

                // append firstname
                uriBuilder.Append($"/firstname/{fname}");
                if(!string.IsNullOrEmpty(lname))
                {
                    uriBuilder.Append($"/lastname/{lname}");
                    if(!string.IsNullOrEmpty(country))
                    {
                        uriBuilder.Append($"/country/{country}");
                        if (!string.IsNullOrEmpty(wallet))
                        {
                            uriBuilder.Append($"/wallet/{wallet}");
                        }
                    }
                }

                Uri u = new Uri(uriBuilder.ToString());
                _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                HttpResponseMessage result = await _client.GetAsync(u);
                if (result.IsSuccessStatusCode)
                {
                    var content = await result.Content.ReadAsStringAsync();
                    return (true, content);
                }
                else
                {
                    if (result.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    {
                        // update access token
                        await RequestAccessToken();
                    }
                    return (false, null);
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
