using GalileuszSchool.External;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GalileuszSchool.Services.Facebook
{
    public class FacebookAuthService : IFacebookAuthService
    {
        private const string TokenValidationUrl = "https://graph.facebook.com/debug_token?input_token={0}&access_token={1}|{2}";
        private const string UserInfoUrl = "https://graph.facebook.com/me?fields=first_name,last_name,email,picture&access_token={0}";
        private readonly FacebookAuthSettings _facebookAuthSettings;
        private readonly IHttpClientFactory _httpClientFactory;
        public FacebookAuthService(FacebookAuthSettings facebookAuthSettings, IHttpClientFactory httpClientFactory)
        {
            _facebookAuthSettings = facebookAuthSettings;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<FacebookTokenValidationResult> ValidateAccessTokenAsync(string accessToken)
        {
            var formattedUrl = string.Format(TokenValidationUrl, accessToken, _facebookAuthSettings.AppId,
                _facebookAuthSettings.AppSecret);
            var result = await _httpClientFactory.CreateClient().GetAsync(formattedUrl);
            result.EnsureSuccessStatusCode();//it will throw an exception if not successfull/ better exception handling needed

            var responseAsString = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<FacebookTokenValidationResult>(responseAsString);
        }
        public async Task<FacebookUserInfoResult> GetUserInfoAsync(string accessToken)
        {
            var formattedUrl = string.Format(UserInfoUrl, accessToken);
            var result = await _httpClientFactory.CreateClient().GetAsync(formattedUrl);
            result.EnsureSuccessStatusCode();//it will throw an exception if not successfull/ better exception handling needed

            var responseAsString = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<FacebookUserInfoResult>(responseAsString);
        }
    }
}
