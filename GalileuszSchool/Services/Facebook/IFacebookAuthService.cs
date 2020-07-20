using GalileuszSchool.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GalileuszSchool.Services.Facebook
{
    public interface IFacebookAuthService
    {
        Task<FacebookTokenValidationResult> ValidateAccessTokenAsync(string accessToken);

        Task<FacebookUserInfoResult> GetUserInfoAsync(string accessToken);
    }
}
